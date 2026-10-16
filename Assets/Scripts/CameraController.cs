using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    private MovementController playerRef;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRef = PlayerReference.Instance.GetComponent<MovementController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = playerRef.transform.position + offset;
        transform.eulerAngles = new Vector3(-playerRef.lookDir.y, playerRef.lookDir.x, 0.0f);
    }
}
