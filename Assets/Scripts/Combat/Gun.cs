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

    private bool CanUseWeapon()
    {
        return GameManager.instance != null && GameManager.instance.CurrentState == GameManager.GameState.Playing;
    }
    void Start()
    {
        currentAmmo = magazineSize;
    }

    private void OnAttack(InputValue value)
    {
        if(value.isPressed)
        {
            TryShoot();
        }
    }

    private void OnReload(InputValue value)
    {
        if(value.isPressed)
        {
            TryReload();
        }
    }

    private void TryShoot()
    {
        if(!CanUseWeapon())
            return;
        if(isReloading || Time.time < nextFireTime)
            return;
        
        if(currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
        else
        {
            Debug.Log("Out of ammo! Press R to reload");
        }
    }

    private void TryReload()
    {
        if (!CanUseWeapon())
            return;
        if(isReloading || currentAmmo == magazineSize)
            return;

        StartCoroutine(Reload());
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
        float elapsedTime = 0f;

        while (elapsedTime < reloadTime)
        {
            if(!CanUseWeapon())
            {
                isReloading = false;
                Debug.Log("Reload canceled: Player died.");
                yield break;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!CanUseWeapon())
        {
            isReloading = false;
            Debug.Log("Reload canceled: Player died.");
            yield break;
        }
        
        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Reload complete. Ammo: " + currentAmmo + "/" + magazineSize);
    }
}
