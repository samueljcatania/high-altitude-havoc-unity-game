using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class MovementController : MonoBehaviour
{
    public float rollSpeed = 10f;
    public float pitchSpeed = 10f;
    public float yawSpeed = 5f;
    public float baseSpeed = 10000f;
    public float boostedSpeed = 20000f;

    public float boostDuration = 5f; // Duration of the speed boost
    public float speedIncreaseTime = 1f; // Time to reach boosted speed

    private float currentSpeed;
    private float boostTimer;
    private bool isBoosting;

    private float rollInput;
    private float pitchInput;
    private float yawInput;
    private bool thrustInput;
    
    public Volume volume;
    private VisualEnvironment _visualEnvironment;


    void Start()
    {
        volume.profile.TryGet(out _visualEnvironment);
        
        currentSpeed = baseSpeed;
        boostTimer = 0f;
        isBoosting = false;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isBoosting = true;
            boostTimer = boostDuration;
        }

        if (isBoosting)
        {
            // Gradually increase the speed to boosted speed over speedIncreaseTime
            currentSpeed = Mathf.Lerp(baseSpeed, boostedSpeed, 1 - (boostTimer / speedIncreaseTime));
            boostTimer -= Time.deltaTime;

            if (boostTimer <= 0)
            {
                isBoosting = false;
                currentSpeed = baseSpeed;
            }
        }

        // Get keyboard inputs
        rollInput = Input.GetAxis("Horizontal"); // A and D keys for roll
        pitchInput = Input.GetAxis("Vertical"); // W and S keys for pitch
        yawInput = Input.GetAxis("Yaw"); // Q and E keys for yaw
        thrustInput = Input.GetKeyDown(KeyCode.LeftShift); // Up and Down Arrow keys for thrust

        // Apply rotations to the plane
        transform.Rotate(pitchInput * pitchSpeed * Time.deltaTime,
            yawInput * yawSpeed * Time.deltaTime,
            -rollInput * rollSpeed * Time.deltaTime,
            Space.Self);

        // Apply the current speed to the clouds to give the illusion of movement
        _visualEnvironment.windSpeed.value = currentSpeed;
    }
}