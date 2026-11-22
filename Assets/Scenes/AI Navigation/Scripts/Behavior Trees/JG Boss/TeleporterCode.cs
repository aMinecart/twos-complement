using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TeleporterCode : MonoBehaviour
{
    public GameObject interactUI;
    public String nextSceneName;

    private bool canSwapScene = false;

    void OnTriggerEnter(Collider other)
    {
        if(!enabled) return;

        if(other.gameObject.tag == "Player")
        {
            Debug.Log("here");
            interactUI.SetActive(true);
            canSwapScene = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(!enabled) return;
        
        if (other.gameObject.tag == "Player")
        {
            interactUI.SetActive(false);
            canSwapScene = false;
        }
    }

    public void OnInteract()
    {
        if(!enabled) return;

        if (canSwapScene)
        {
            Debug.Log("swap scene");
            try
            {
                SceneManager.LoadScene(nextSceneName);
            }
            catch(Exception e)
            {
                Debug.Log("Scene does not exist. Please check boss code scene name grahh");
            }
        }
    }
}
