using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private int magazineSize = 8;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private Camera playerCamera;
    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = magazineSize;
    }

    void Update()
    {
        if (isReloading)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame &&
            currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Mouse.current != null && 
            Mouse.current.leftButton.wasPressedThisFrame &&
            Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            } else {
                Debug.Log("Out of ammo! Press R to reload.");
            }
        }
    }
    public void Shoot()
    {
        currentAmmo--;
        Debug.Log("Ammo: " + currentAmmo + "/" + magazineSize);

        RaycastHit hit; 
        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            range))
        {
            Debug.Log("Hit: " + hit.transform.name);
            Health targetHealth = hit.transform.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        
        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Reload complete. Ammo: " + currentAmmo + "/" + magazineSize);
    }
}
