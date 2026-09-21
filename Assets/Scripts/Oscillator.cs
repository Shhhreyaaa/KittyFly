using UnityEngine;

public class Oscillator : MonoBehaviour
{
    Vector3 startingPosition;
    [SerializeField] Vector3 movementVector = new Vector3(0f, 10f, 0f);
    [SerializeField] float period = 4f;
    [SerializeField] float phaseOffset = 0f;

    float cycles;
    const float tau = Mathf.PI * 2; // 6.283
    float rawSinWave;
    float movementFactor;

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        if (period <= Mathf.Epsilon) return;

        cycles = (Time.time / period) + phaseOffset;
        rawSinWave = Mathf.Sin(cycles * tau); // -1 to 1
        movementFactor = (rawSinWave + 1f) / 2f; // 0 to 1

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}
