using UnityEngine;
using UnityEngine.UI;

public class TeleporterCode : MonoBehaviour
{
    public GameObject interactUI;
    
    void Awake()
    {
        //interactUI = Instantiate(interactUI, );
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("here");
            interactUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            interactUI.SetActive(false);
        }
    }
}
