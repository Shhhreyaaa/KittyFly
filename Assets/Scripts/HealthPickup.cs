using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healAmount = 25f;
    [SerializeField] float rotateSpeed = 60f;
    [SerializeField] float bobSpeed = 3f;
    [SerializeField] float bobHeight = 0.25f;

    Vector3 initialPos;
    bool collected = false;

    private void Start()
    {
        initialPos = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);
        float newY = initialPos.y + (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
        transform.position = new Vector3(initialPos.x, newY, initialPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        RocketHealth health = other.GetComponentInParent<RocketHealth>();
        if (health != null)
        {
            collected = true;
            health.Heal(healAmount);
            Debug.Log($"[HealthPickup] Player collected Nano-Repair! +{healAmount} HP (Current: {health.CurrentHealth}/{health.MaxHealth})");
            gameObject.SetActive(false);
        }
    }
}
