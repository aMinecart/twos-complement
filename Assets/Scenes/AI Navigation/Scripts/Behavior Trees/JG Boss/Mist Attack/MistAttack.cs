using System;
using UnityEngine;

public class MistAttack : MonoBehaviour
{
    GameObject player;
    float timeBetweenDamage = 1f;
    float timeSinceAttack = 0f;
    float time = 3f;
    bool canDamage = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = PlayerReference.Instance.gameObject;        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(time <= 0)
        {
            time = 3f;
            gameObject.SetActive(false);
        }
        time -= Time.deltaTime;

        if(timeSinceAttack <= 0)
        {
            timeSinceAttack = timeBetweenDamage;
            canDamage = true;
        }
        else
        {
            canDamage = false;
            timeSinceAttack -= Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("here");
        if(other.gameObject.tag == "Player" && canDamage)
        {
            player.GetComponent<HealthTracker>().TakeDamage(10);
        }
    }
}
