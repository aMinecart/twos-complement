using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI; 

//will see how the crosshair should be drawn based on the weapon type
public enum CrosshairStyle
{
    None, //NO Crosshair 
    Dot, //small dot: FOR: Melee weapons (the dot will pulse w/each swing!)
    Cross, //Four Bars: FOR: guns; gaps show spread -> pisol tigher & shotgun wide 
    Ring //Ring & center dot; FOR: projectile weapons 
}

//will build the crosshair for whichever weapon is currently active 
//ONLY VISISBLE WHEN WEAPON IS ACTIVE!/ when combat is occuring 
//shape of crosshair depends on weapon that is active 
//will be red when along the weapon's aim line has an enemy in proximity 
//fades when the weapon can't fire (when reloading, empty/out of bullets, cooldown for melee)
[RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
public class CrosshairController : MonoBehaviour
{
    [Header("Reference (auto-found if left empty)")] //if player's weapon manager is epty -> found through PlayerReference.Instance in start 
    [SerializeField] private WeaponManager weaponManager; 
    [SerializeField] private Camera aimCamera; //ONLY to be used if weapon has no firing origin also to read the field of view when converting spread angle -> screen size

    [Header("Targeting")] //layers that there is an enemy under crosshair ray that can be ehit 
    [SerializeField] private LayerMask aimMask = ~0; 
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal; 

    [Header("Cross-hair Shape")]
    [SerializeField] private float lineLength = 10f; //length of each bar in cross style
    [SerializeField] private float lineThickness = 2f; 
    [SerializeField] private float dotSize = 5f; //dot diameter
    [SerializeField] private float baseGap = 6f; 

    [Header("Spread")]
    [SerializeField] private float maxGap = 120f; //caps extreme spread angles so it doesn't fill the screen!
    [SerializeField] private float kickRecoverySpeed = 60f; //pixels per sec. the fire kick will decay 
    [SerializeField] private float gapSmoothing = 18f; 

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white; 
    [SerializeField] private Color enemyColor = new Color (1f, 0.25f, 0.25f, 1f); 
    [SerializeField, Range(0f, 1f)] private float dimmedAlpha = 0.35f; 

    private Canvas canvas; //convert pixels -> canvas units
    private RectTransform root; //parent to every crosshair peice; toggles on/off
    private readonly Image[] bars = new Image[4]; //top, bottom, left, right 
    private Image dotImage; //center dot
    private Image ringImage; //ring outline
    private Image[] allImages; 
    private Sprite circleSprite; 
    private Sprite ringSprite; 

    private readonly RaycastHit[] hits = new RaycastHit[16]; //reusable buffer for aim raycast 
    private Transform ownerRoot; 
    private Weapon lastWeapon; //last weapon we drew for; 
    private float currentGap; 
    private float kick; 

    //runs in editor when component is first added; ONLY CHANGES WHEN COMPONENT IS ADDED!
    private void Reset()
    {
        if(TryGetComponent(out Canvas canvasComponent))
        {
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay; //draws crosshair on top of game view
        }

        //scalres UI w/the screen 
        if(TryGetComponent(out CanvasScaler scaler))
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; 
            scaler.referenceResolution = new Vector2(1920f, 1080f); 
            scaler.matchWidthOrHeight = 0.5f; 
        }
    }

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        BuildUI(); //creates all UI images to front 
    }

    private void Start()
    {   
        //finds player's weapon manager 
        if(weaponManager == null && PlayerReference.Instance != null)
        {
            weaponManager = PlayerReference.Instance.GetComponentInChildren<WeaponManager>();
        } 

        if(aimCamera == null)
        {
            aimCamera = Camera.main; 
        }

        if(weaponManager != null)
        {
            ownerRoot = weaponManager.transform.root; 
            Subscribe();
        }
        currentGap = baseGap; 
    } 

    private void OnEnable() => Subscribe(); 
    private void OnDisable()
    {
        if(weaponManager != null)
        {
            weaponManager.WeaponFired -= OnWeaponFired; 
        }
    }

    private void OnDestroy()
    {
        //frees the generated sprites and textures manually 
        if(circleSprite != null)
        {
            Destroy(circleSprite.texture); 
            Destroy(circleSprite);
        }
        if(ringSprite != null)
        {
            Destroy(ringSprite.texture); 
            Destroy(ringSprite); 
        }
    }

    private void Subscribe()
    {
        if(weaponManager == null)
        {
            return;
        }
        weaponManager.WeaponFired -= OnWeaponFired; 
        weaponManager.WeaponFired += OnWeaponFired; 
    }

    private void OnWeaponFired(Weapon weapon)
    {
        kick = weapon.CrosshairFireKick; 
    }

    private void LateUpdate()
    {
        //crosshair will reflect this frame's weapon state after all other scripts have run 
        
        if(weaponManager == null)
        {
            return;
        }

        Weapon weapon = weaponManager.ActiveWeapon; 
        bool playing = GameManager.instance != null && GameManager.instance.CurrentState == GameManager.GameState.Playing; 

        //no weapon, a disabled weapon, or a puased/menu state means no crosshair 
        CrosshairStyle style = (weapon != null && weapon.isActiveAndEnabled && playing) ? weapon.Style : CrosshairStyle.None;

        //show/hide the whole crosshair 
        bool visible = style != CrosshairStyle.None; 
        if(root.gameObject.activeSelf != visible)
        {
            root.gameObject.SetActive(visible);
        }

        if (!visible)
        {
            lastWeapon = null; 
            return; 
        }

        float restingGap = Mathf.Clamp(Mathf.Max(baseGap, AngleToCanvasUnits(weapon.CrosshairSpreadAngle)), baseGap, maxGap); 

        if(weapon != lastWeapon)
        {
            lastWeapon = weapon; 
            currentGap = restingGap; 
            kick = 0f; 
        }

        kick = Mathf.MoveTowards(kick, 0f, kickRecoverySpeed * Time.unscaledDeltaTime); 
        currentGap = Mathf.Lerp(currentGap, restingGap + kick, 1f - Mathf.Exp(-gapSmoothing * Time.unscaledDeltaTime)); 
        ApplyShape(style); 
        ApplyColor(weapon); 
    }

    private float AngleToCanvasUnits(float angleDegrees)
    {
        //converts the spread half-angle -> distance from screen center in canvas units
        //where a shot at that angle would appear 
        
        //perfectly accurate weapon 
        if(angleDegrees <= 0f || aimCamera == null)
        {
            return 0f; 
        }

        //perspective projection/ offset on screen
        float tanAngle = Mathf.Tan(Mathf.Clamp(angleDegrees, 0f, 89f) * Mathf.Deg2Rad);
        float tanHalfFov = Mathf.Tan(aimCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float screenPixels = tanAngle / tanHalfFov * (Screen.height * 0.5f);

        //screen pixels -> canvas units 
        float scale= canvas != null && canvas.scaleFactor > 0f ? canvas.scaleFactor : 1f; 
        return screenPixels / scale; 
    }

    //shows only the pieces that are needed for current style then positions  them according to currentGap
    private void ApplyShape(CrosshairStyle style)
    {
        bool cross = style == CrosshairStyle.Cross; 
        bool ring = style == CrosshairStyle.Ring; 
        bool dot = style == CrosshairStyle.Dot || ring; //ring will also have a center dot

        foreach(Image bar in bars)
        {
            SetActive(bar, cross);
        }
        SetActive(ringImage, ring); 
        SetActive(dotImage, dot);

        if (cross)
        {
             float offset = currentGap + lineLength * 0.5f;
            bars[0].rectTransform.anchoredPosition = new Vector2(0f, offset); //top
            bars[1].rectTransform.anchoredPosition = new Vector2(0f, -offset); //bottom
            bars[2].rectTransform.anchoredPosition = new Vector2(-offset, 0f); //left
            bars[3].rectTransform.anchoredPosition = new Vector2(offset, 0f); //right
        }
        if (ring)
        {
            float diameter = currentGap * 2f + lineLength; 
            ringImage.rectTransform.sizeDelta = new Vector2(diameter, diameter); 
        }
        if (dot)
        {
            float size = style == CrosshairStyle.Dot ? dotSize + kick * 0.5f : dotSize; 
            dotImage.rectTransform.sizeDelta = new Vector2(size, size);
        }
    }

    private void ApplyColor(Weapon weapon)
    {
        Color color = IsEnemeyUnderCrosshair(weapon) ? enemyColor: normalColor;

        if (weapon.IsCrosshairDimmed)
        {
            color.a *= dimmedAlpha; 
        }
        foreach(Image image in allImages)
        {
            image.color = color; 
        }
    }

    private bool IsEnemeyUnderCrosshair(Weapon weapon)
    {
        //true if -> weaepon's attack will hit an enemy currently
        Vector3 origin; 
        Vector3 direction; 

        if(weapon.FiringOrigin != null)
        {
            origin = weapon.FiringOrigin.position; 
            direction = weapon.FiringOrigin.forward; 
        } else if(aimCamera != null)
        {
            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            origin = ray.origin;
            direction = ray.direction;
        }
        else
        {
            return false; 
        }

        //hits stored in 'hits' array
        int count = Physics.RaycastNonAlloc(origin, direction, hits, weapon.Range, aimMask, triggerInteraction);
        float nearest = float.MaxValue;
        Collider nearestCollider = null;

        for(int i = 0; i < count; i++)
        {
            //skip players own colliders
            if(ownerRoot != null && hits[i].transform.IsChildOf(ownerRoot))
            {
                continue;
            }

            if(hits[i].distance < nearest)
            {
                nearest = hits[i].distance; 
                nearestCollider = hits[i].collider; 
            }
        }

        return nearestCollider != null && nearestCollider.GetComponentInParent<HealthTracker>() !=null;
    }

    //will construct the crosshair hiearchy at runtime
    //canvas -> crosshair -> top/bottom/left/right; rign; dot 
    private void BuildUI()
    {
        //generates sprites
        circleSprite = MakeCircleSprite(64, 0f); //filled circle 
        ringSprite = MakeCircleSprite(128, 0.92f); 

        root = NewRect("Crosshair", transform); 

        bars[0] = NewImage("Top", root, null, new Vector2(lineThickness, lineLength));
        bars[1] = NewImage("Bottom", root, null, new Vector2(lineThickness, lineLength));
        bars[2] = NewImage("Left", root, null, new Vector2(lineLength, lineThickness));
        bars[3] = NewImage("Right", root, null, new Vector2(lineLength, lineThickness));
        ringImage = NewImage("Ring", root, ringSprite, Vector2.zero); // Sized every frame in ApplyShape
        dotImage = NewImage("Dot", root, circleSprite, new Vector2(dotSize, dotSize));
 
        allImages = new[] { bars[0], bars[1], bars[2], bars[3], ringImage, dotImage };

        root.gameObject.SetActive(false); 
    }

    private static RectTransform NewRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform)); 
        RectTransform rect = (RectTransform)go.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
        return rect;
    }

    //creates UI image w/given size 
    private static Image NewImage(string name, RectTransform parent, Sprite sprite, Vector2 size)
    {
        RectTransform rect = NewRect(name, parent);
        rect.sizeDelta = size;
 
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.raycastTarget = false;
        return image; 
    }

    private static void SetActive(Image image, bool active)
    {
        if(image.gameObject.activeSelf != active)
        {
            image.gameObject.SetActive(active); 
        }
    }


     private static Sprite MakeCircleSprite(int size, float innerFraction)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };
 
        float radius = size * 0.5f;
        Vector2 center = new Vector2(radius, radius);
        Color[] pixels = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                //how far this pixel's center is from the circle's center
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
 
                //opacity is 1 inside the outer edge and fades over 1 pixel at the edge (smooth edge)
                float outer = Mathf.Clamp01(radius - distance);
 
                //for rings, cut out the middle with the same 1-pixel fade; filled circles skip this
                float inner = innerFraction > 0f ? Mathf.Clamp01(distance - radius * innerFraction) : 1f;
 
                //white w/variable alpha; the Image's color tints it at runtime
                pixels[y * size + x] = new Color(1f, 1f, 1f, outer * inner);
            }
        }
 
        texture.SetPixels(pixels);
        texture.Apply();
 
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
