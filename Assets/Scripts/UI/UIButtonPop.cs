using UnityEngine;
using UnityEngine.EventSystems; 

public class UIButtonPop : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.07f;
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float speed = 14f;

    private float target = 1f; 
    private bool hovering;

    void OnEnable()
    {
        transform.localScale = Vector3.one; 
        target = 1f; 
    }

    void Update()
    {
        float s = Mathf.Lerp(transform.localScale.x, target, 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime)); 
        transform.localScale = Vector3.one * s; 
    }

    public void OnPointerEnter(PointerEventData e)
    {
        hovering = true; 
        target = hoverScale; 
    }
    public void OnPointerExit(PointerEventData e)
    {
        hovering = false; 
        target = 1f; 
    }
    public void OnPointerDown(PointerEventData e)
    {
        target = pressScale; 
    }
    public void OnPointerUp(PointerEventData e)
    {
        target = hovering ? hoverScale : 1f; 
    }
}
