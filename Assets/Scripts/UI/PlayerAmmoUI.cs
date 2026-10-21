using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; 
//will show the amt of ammo the player has for their weapon 
public class PlayerAmmoUI : MonoBehaviour
{
    [Header("Current Weapon")]
    [Tooltip("The whole ammo panel. It is hidden when you hold a melee weapon or nothing.")]
    [SerializeField] private GameObject root; 
    [SerializeField] private TMP_Text ammoText; 
    [SerializeField] private TMP_Text weaponNameText; 

    [Header("Hotbar list")] //optional for now 
    [SerializeField] private TMP_Text[] hotbarSlotTexts;
    [SerializeField] private Color selectedSlotColor = Color.white;
    [SerializeField] private Color otherSlotColor = new Color32(150, 158, 176, 255);

    [Header("Colours")]
    [SerializeField] private Color normalColor = Color.white; 
     [SerializeField] private Color lowColor = new Color32(249, 115, 22, 255);
    [SerializeField] private Color emptyColor = new Color32(239, 68, 68, 255);
    [SerializeField] private int lowAmmoThreshold = 3;

    private WeaponManager weaponManager; 
    private float searchTimer; 

    //temporary testing 
    [Header("Temporary Ammo UI Testing")]
    [SerializeField] private bool enableAmmoUITest = true;
    [SerializeField] private int testMagazineSize = 12;

    private int testAmmo;
    private bool testModeActive = false;


    //text only rewritten when there is a change!
    private Weapon shownWeapon; 
    private int shownAmmo = int.MinValue; 
    private int shownMaxAmmo = int.MinValue; 
    private Weapon[] slotWeapon; 
    private int[] slotAmmo; 
    private bool[] slotSelected; 

    private void Awake()
    {
        int n = hotbarSlotTexts !=null ? hotbarSlotTexts.Length : 0; 
        slotWeapon = new Weapon[n]; 
        slotAmmo = new int[n]; 
        slotSelected = new bool[n]; 
        for(int i = 0; i < n; i++)
        {
            slotAmmo[i] = int.MinValue; 
        }
    }

    private void Show(bool visible)
    {
        if(root!= null && root.activeSelf != visible)
        {
            root.SetActive(visible); 
        }
    }

    private void UpdateAmmoUITest()
    {
        if (!enableAmmoUITest)
            return;

        if (Keyboard.current == null)
        {
            Debug.LogWarning("No keyboard detected by the Input System.");
            return;
        }

        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            Debug.Log("F6 detected!");

            testModeActive = !testModeActive;

            if (testModeActive)
            {
                testMagazineSize = Mathf.Max(1, testMagazineSize);
                testAmmo = testMagazineSize;
                Show(true);
            }
            else
            {
                shownWeapon = null;
                shownAmmo = int.MinValue;
                shownMaxAmmo = int.MinValue;
            }
        }

        if (!testModeActive)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame ||
            Keyboard.current.numpad1Key.wasPressedThisFrame)
        {
            testAmmo = Mathf.Max(0, testAmmo - 1);
        }

        // Press R to refill the test magazine
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            testAmmo = Mathf.Max(0, testMagazineSize);
        }

        // Press 0 to empty the test magazine
        if (Keyboard.current.digit0Key.wasPressedThisFrame ||
            Keyboard.current.numpad0Key.wasPressedThisFrame)
        {
            testAmmo = 0;
        }

        if (ammoText != null)
        {
            ammoText.text = $"{testAmmo}/{testMagazineSize}";

            ammoText.color = testAmmo <= 0
                ? emptyColor
                : (testAmmo <= lowAmmoThreshold
                    ? lowColor
                    : normalColor);
        }

        if (weaponNameText != null)
        {
            weaponNameText.text = "TEST WEAPON";
        }

        Show(true);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateAmmoUITest();
        if (testModeActive)
        {
            return;
        }
        //1)find the player's weaponManager 
        if(weaponManager == null)
        {
            Show(false); 
            searchTimer -= Time.unscaledDeltaTime;
            if (searchTimer > 0f)
            {
                return;
            }
            searchTimer = 0.5f; 
            GameObject player = GameObject.FindGameObjectWithTag("Player"); 
            
            if(player != null)
            {
                weaponManager = player.GetComponent<WeaponManager>();
            }

            if (weaponManager == null)
            {
                return;
            }
        }
        UpdateCurrentWeapon();
        UpdateHotbar();
    }

        private void UpdateCurrentWeapon()
        {
            Weapon current = weaponManager.ActiveWeapon; //null when players hands are empty
            if(current is RangedWeapon ranged)
            {
                Show(true); 
                int ammo = ranged.CurrentAmmo; 
                int maxAmmo = ranged.MagazineSize; 

                if(current != shownWeapon || ammo != shownAmmo || maxAmmo != shownMaxAmmo)
                {
                    shownWeapon = current; 
                    shownAmmo = ammo; 
                    shownMaxAmmo = maxAmmo; 

                    if(ammoText != null)
                    {
                        ammoText.text = $"{ammo}/{maxAmmo}";
                        ammoText.color = ammo <= 0 ? emptyColor : (ammo <= lowAmmoThreshold ? lowColor : normalColor);
                    }

                    if(weaponNameText != null)
                    {
                        weaponNameText.text = current.gameObject.name; 
                    }
                }
            }
            else
            {
                Show(false); 
                shownWeapon = null; 
                shownAmmo = int.MinValue; 
                shownMaxAmmo = int.MinValue; 
            }
        }

        private void UpdateHotbar()
        {
            if(slotWeapon == null || slotWeapon.Length == 0)
            {
                return; 
            }
            var equipped = weaponManager.EquippedWeapons; 
            int selected = weaponManager.SelectedWeaponIndex; 

            for (int i = 0; i < hotbarSlotTexts.Length; i++)
            {
                TMP_Text label = hotbarSlotTexts[i];
                if (label == null)
                {
                    continue;
                }
    
                Weapon w = i < equipped.Count ? equipped[i] : null;
                int ammo = w is RangedWeapon r ? r.CurrentAmmo : -1; // -1 = melee w/no ammo
                bool isSelected = i == selected;
    
                if (w == slotWeapon[i] && ammo == slotAmmo[i] && isSelected == slotSelected[i]){
                    continue;
                }

                slotWeapon[i] = w;
                slotAmmo[i] = ammo;
                slotSelected[i] = isSelected;
    
                label.text = w == null ? "" : (i + 1) + "  " + w.gameObject.name + "  " + (ammo >= 0 ? ammo.ToString() : "-");
                label.color = isSelected ? selectedSlotColor : otherSlotColor;
            }
        }
}
