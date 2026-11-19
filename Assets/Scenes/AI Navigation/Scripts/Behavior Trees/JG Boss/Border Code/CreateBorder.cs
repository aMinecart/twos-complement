using UnityEngine;

public class CreateBorder : MonoBehaviour
{
    GameObject player;
    public GameObject wall;
    public GameObject wallCollider;
    public GameObject boss;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = PlayerReference.Instance.gameObject;
    }

    //If player enters, spawn walls
    void OnCollisionEnter(Collision other)
    {
        GameObject obj = other.gameObject;
        if(obj == player)
        {
            //Wall stuff
            wall.SetActive(true);
            wallCollider.SetActive(true);

            //Disable collider
            gameObject.SetActive(false);

            //Activate boss scripts
            boss.GetComponent<JungleBossTree>().enabled = true;
        }
    }
}
