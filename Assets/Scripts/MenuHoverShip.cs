using UnityEngine;

public class MenuHoverShip : MonoBehaviour
{
    [Header("Hover Dynamics")]
    [SerializeField] public float bobSpeed = 1.8f;
    [SerializeField] public float bobHeight = 0.15f;
    [SerializeField] public float yawSpeed = 15f;
    [SerializeField] public float rollTilt = 3.5f;

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        float newY = initialPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);

        // Gentle yaw rotation and subtle roll sway
        float roll = Mathf.Sin(Time.time * (bobSpeed * 0.8f)) * rollTilt;
        transform.Rotate(Vector3.up, yawSpeed * Time.deltaTime, Space.World);
    }
}
