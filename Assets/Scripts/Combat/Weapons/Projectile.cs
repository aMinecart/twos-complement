using UnityEngine;

// Handles movement and collision for projectile weapons 
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime;

    private HealthTracker ownerHealth;
    private float damage;
    private float speed;
    private float range;
    private float distanceTraveled = 0f;

    private Rigidbody rb;
    private bool initialized = false;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

    }

    public void Initialize(HealthTracker owner, float projectileDamage, float projectileSpeed, float projectileRange)
    {
        ownerHealth = owner;
        damage = projectileDamage;
        speed = projectileSpeed;
        range = projectileRange;

        // Ignore collision with character firing 
        if (ownerHealth != null)
        {
            Collider[] ownerColliders = ownerHealth.GetComponentsInChildren<Collider>();
            Collider[] projectileColliders = GetComponentsInChildren<Collider>();

            foreach (Collider projectileCollider in projectileColliders)
            {
                foreach (Collider ownerCollider in ownerColliders)
                {
                    Physics.IgnoreCollision(projectileCollider, ownerCollider);
                }
            }
        }

        // Launch projectile forward
        rb.linearVelocity = transform.forward * speed;
        initialized = true;

        // Remove projectile if it doesn't hit anything 
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (!initialized || hasHit)
            return;
        
        // Track distance travaled
        distanceTraveled += rb.linearVelocity.magnitude * Time.fixedDeltaTime;

        // Destroy project if max range reached 
        if (distanceTraveled >= range)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!initialized || hasHit)
            return;
        
        // Check if has health component 
        HealthTracker targetHealth = other.GetComponentInParent<HealthTracker>();

        // Ignore character using weapon 
        if (ownerHealth != null && targetHealth == ownerHealth)
            return;
        
        hasHit = true;

        // Damage target only 
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damage);
            Debug.Log("Projectile hit: " + other.name);
        }

        // Destroy projectile on impact 
        Destroy(gameObject);
    }
}
