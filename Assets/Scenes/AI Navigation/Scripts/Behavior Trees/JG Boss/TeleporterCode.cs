using UnityEngine;
using UnityEngine.UI;

public class TeleporterCode : MonoBehaviour
{
    public Image interactUI;
    Image uiUse;
    
    void Awake()
    {
        //interactUI = Instantiate(interactUI, );
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Display ui");
        }
    }
}
