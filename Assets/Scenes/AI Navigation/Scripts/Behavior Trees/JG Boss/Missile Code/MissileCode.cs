using UnityEngine;

public class MissileCode : MonoBehaviour
{
    public float rocketSpeed = 5f;
    public float rocketLiveTime = 15f;
    GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = PlayerReference.Instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        rocketLiveTime -= Time.deltaTime;

        transform.position += (player.transform.position - gameObject.transform.position).normalized * rocketSpeed * Time.deltaTime;
        gameObject.transform.LookAt(player.transform);

        if(rocketLiveTime <= 0) Destroy(gameObject);
        if(Vector3.Distance(gameObject.transform.position, player.transform.position) < 1f)
        {
            player.GetComponent<HealthTracker>().TakeDamage(25);
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
