using Unity.VisualScripting;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; 
#endif

[RequireComponent(typeof(RectTransform))]
public class UIDepthLayer : MonoBehaviour
{
    [SerializeField] private float parallax = 20f; 
    [SerializeField] private float bobAmount = 8f; 
    [SerializeField] private float bobSpeed = 1.2f; 
    [SerializeField] private float smoothing = 6f; 

    private RectTransform rt; 
    private Vector2 basePos; 
    private Vector2 current; 

    void Awake()
    {
        rt = (RectTransform)transform; 
        basePos = rt.anchoredPosition; 
    }

    Vector2 MousePos()
    {
        #if ENABLE_INPUT_SYSTEM
        return Mouse.current!= null ? Mouse.current.position.ReadValue() : new Vector2(Screen.width / 2f, Screen.height / 2f);
        #else 
        return Input.mousePosition; 
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 m = MousePos(); 
        Vector2 n = new Vector2(m.x / Screen.width - 0.5f, m.y / Screen.height - 0.5f)*2f; 
        Vector2 target = n * parallax; 
        target.y += Mathf.Sin(Time.unscaledTime * bobSpeed) * bobAmount; 
        current = Vector2.Lerp(current, target, 1f - Mathf.Exp(-smoothing * Time.unscaledDeltaTime)); 
        rt.anchoredPosition = basePos + current; 
    }
}
