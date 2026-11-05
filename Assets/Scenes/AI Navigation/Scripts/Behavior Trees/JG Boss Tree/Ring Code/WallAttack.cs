using UnityEngine;

public class WallAttack : MonoBehaviour
{

    float scaleAmount = .5f;
    float speed = 8f;
    float removeTime = 6f;

    // Update is called once per frame
    void Update()
    {
        Transform playerTransform = PlayerReference.Instance.transform;

        //Make larger
        transform.localScale += transform.localScale * scaleAmount * Time.deltaTime;

        //Move Forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        //If far, destroy
        removeTime -= Time.deltaTime;
        if(removeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<HealthTracker>().TakeDamage(25);
        }
    }
}
