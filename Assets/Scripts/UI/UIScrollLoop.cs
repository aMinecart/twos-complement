using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

//puts up full-screen UI image; 
/*
-scrolls forever in one direction
-one copy on itself above/below so there's never a gap
-will work when the game is still paused 
*/

[RequireComponent(typeof(RectTransform))]
public class UIScrollLoop : MonoBehaviour
{
    [SerializeField] private float speed = 40f; //pos = up neg = down; (pixels/sec)
    private static bool cloning; 
    private RectTransform rt, cloneRt; 
    private Vector2 basePos; 
    private float offset; 

    void Awake()
    {
        if (cloning)
        {
            return; 
        }
        rt = (RectTransform)transform; 
        basePos = rt.anchoredPosition; 
        cloning = true; 
        GameObject clone = Instantiate(gameObject, transform.parent); 
        cloning = false; 
        clone.name = name + " (Loop Copy)"; 
        Destroy(clone.GetComponent<UIScrollLoop>());
        cloneRt = (RectTransform)clone.transform;
    }

    void Update()
    {
       if(rt == null)
        {
            return;
        } 
        float h = rt.rect.height;
        if (h <= 0f)
        {
            return; 
        }
        offset = Mathf.Repeat(offset + speed * Time.unscaledDeltaTime, h); 
        rt.anchoredPosition = basePos + new Vector2 (0f, offset); 
        cloneRt.anchoredPosition = basePos + new Vector2(0f, offset-h);
    }
}
