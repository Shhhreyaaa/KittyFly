using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] Vector3 rotationSpeed = new Vector3(0f, 0f, 25f);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}
