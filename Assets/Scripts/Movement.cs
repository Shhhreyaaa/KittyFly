using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [SerializeField] InputAction thrust;
    [SerializeField] InputAction rotation;
    [SerializeField] float thrustStrength = 1200f;
    [SerializeField] float rotationStrength = 120f;

    [Header("Thruster Particle Effects")]
    [SerializeField] ParticleSystem mainThrusterParticles;
    [SerializeField] ParticleSystem leftThrusterParticles;
    [SerializeField] ParticleSystem rightThrusterParticles;
    [SerializeField] Light thrusterLight;

    Rigidbody rb;

    private void Awake()
    {
        FindThrusterReferences();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                             RigidbodyConstraints.FreezeRotationY | 
                             RigidbodyConstraints.FreezePositionZ;
        }

        StopThrusting();
        if (leftThrusterParticles != null) leftThrusterParticles.Stop();
        if (rightThrusterParticles != null) rightThrusterParticles.Stop();
    }

    private void FindThrusterReferences()
    {
        if (mainThrusterParticles == null)
        {
            Transform t = transform.Find("Thruster_FX");
            if (t != null) mainThrusterParticles = t.GetComponent<ParticleSystem>();
        }

        if (leftThrusterParticles == null)
        {
            Transform t = transform.Find("RCS_Left");
            if (t != null) leftThrusterParticles = t.GetComponent<ParticleSystem>();
        }

        if (rightThrusterParticles == null)
        {
            Transform t = transform.Find("RCS_Right");
            if (t != null) rightThrusterParticles = t.GetComponent<ParticleSystem>();
        }

        if (thrusterLight == null)
        {
            thrusterLight = GetComponentInChildren<Light>();
        }
    }

    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    private void OnDisable()
    {
        thrust.Disable();
        rotation.Disable();
        StopThrusting();
        if (leftThrusterParticles != null && leftThrusterParticles.isPlaying) leftThrusterParticles.Stop();
        if (rightThrusterParticles != null && rightThrusterParticles.isPlaying) rightThrusterParticles.Stop();
    }

    private void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
    }

    private void ProcessThrust()
    {
        bool isThrusting = thrust.IsPressed();
        if (!isThrusting && Keyboard.current != null)
        {
            isThrusting = Keyboard.current.spaceKey.isPressed || 
                          Keyboard.current.wKey.isPressed || 
                          Keyboard.current.upArrowKey.isPressed;
        }

        if (isThrusting)
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void StartThrusting()
    {
        if (rb != null)
        {
            rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
        }

        if (mainThrusterParticles != null && !mainThrusterParticles.isPlaying)
        {
            mainThrusterParticles.Play();
        }

        if (thrusterLight != null && !thrusterLight.enabled)
        {
            thrusterLight.enabled = true;
        }
    }

    private void StopThrusting()
    {
        if (mainThrusterParticles != null && mainThrusterParticles.isPlaying)
        {
            mainThrusterParticles.Stop();
        }

        if (thrusterLight != null && thrusterLight.enabled)
        {
            thrusterLight.enabled = false;
        }
    }

    private void ProcessRotation()
    {
        float rotationInput = rotation.ReadValue<float>();

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                rotationInput = -1f;
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                rotationInput = 1f;
            }
        }

        if (rotationInput < -0.01f)
        {
            // Turning left: fire right RCS thruster
            ApplyRotation(-rotationInput);
            if (rightThrusterParticles != null && !rightThrusterParticles.isPlaying) rightThrusterParticles.Play();
            if (leftThrusterParticles != null && leftThrusterParticles.isPlaying) leftThrusterParticles.Stop();
        }
        else if (rotationInput > 0.01f)
        {
            // Turning right: fire left RCS thruster
            ApplyRotation(-rotationInput);
            if (leftThrusterParticles != null && !leftThrusterParticles.isPlaying) leftThrusterParticles.Play();
            if (rightThrusterParticles != null && rightThrusterParticles.isPlaying) rightThrusterParticles.Stop();
        }
        else
        {
            if (leftThrusterParticles != null && leftThrusterParticles.isPlaying) leftThrusterParticles.Stop();
            if (rightThrusterParticles != null && rightThrusterParticles.isPlaying) rightThrusterParticles.Stop();
        }
    }

    private void ApplyRotation(float rotationAmount)
    {
        float rotationThisFrame = rotationAmount * rotationStrength * Time.fixedDeltaTime;
        transform.Rotate(Vector3.forward * rotationThisFrame);
    }
}
