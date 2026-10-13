using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected Transform firingOriginTransform;

    [SerializeField] protected float damage;
    [SerializeField] protected float fireRate;
    [SerializeField] protected int magazineSize;
    [SerializeField] protected float reloadTime;
    
    protected int currentAmmo;
    protected bool reloading = false;

    protected bool CanUseWeapon()
    {
        return GameManager.instance != null && GameManager.instance.CurrentState == GameManager.GameState.Playing;
    }

    protected virtual void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryShoot();
        }
    }

    protected virtual void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TryReload();
        }
    }

    protected abstract void OnEnable();
    protected abstract void OnDisable();

    protected abstract bool TryShoot();
    protected abstract void Shoot();

    protected abstract bool TryReload();
    protected abstract IEnumerator Reload();
}