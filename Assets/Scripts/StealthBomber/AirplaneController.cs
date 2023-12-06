using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public float maneuverSpeed = 10f;
    public float tiltAmount = 20f;

    public float bankAmount = 20f;

    // public GameObject bulletPrefab;
    // public Transform bulletSpawnPoint;
    // public float shootingRate = 0.5f;
    // private float nextShootTime = 0f;
    public Vector2 boundaryX = new Vector2(-100f, 100f);
    public Vector2 boundaryY = new Vector2(-100f, 100f);

    void Update()
    {
        // Lateral Movement (using localPosition for relative movement)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, vertical, 0) * maneuverSpeed;
        transform.localPosition += direction * Time.deltaTime;

        // Tilt and Bank (using localRotation)
        transform.localRotation = Quaternion.Euler(vertical * tiltAmount, 0, -horizontal * bankAmount);

        // Boundaries (using localPosition) 
        Vector3 localPos = transform.localPosition;
        localPos.x = Mathf.Clamp(localPos.x, boundaryX.x, boundaryX.y);
        localPos.y = Mathf.Clamp(localPos.y, boundaryY.x, boundaryY.y);
        transform.localPosition = localPos;
    }
}


// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.HighDefinition;
//
// namespace StealthBomber
// {
//     public class AirplaneController : MonoBehaviour
//     {
//         public float rollSpeed = 10f;
//         public float pitchSpeed = 10f;
//         public float yawSpeed = 5f;
//         public float baseSpeed = 10000f;
//         public float boostedSpeed = 20000f;
//
//         public float boostDuration = 5f; // Duration of the speed boost
//         public float speedIncreaseTime = 1f; // Time to reach boosted speed
//
//         private float currentSpeed;
//         private float boostTimer;
//         private bool isBoosting;
//
//         private float rollInput;
//         private float pitchInput;
//         private float yawInput;
//         private bool thrustInput;
//     
//         public Volume volume;
//         private VisualEnvironment _visualEnvironment;
//
//
//         void Start()
//         {
//             volume.profile.TryGet(out _visualEnvironment);
//         
//             currentSpeed = baseSpeed;
//             boostTimer = 0f;
//             isBoosting = false;
//         }
//
//         private void FixedUpdate()
//         {
//             if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
//             {
//                 isBoosting = true;
//                 boostTimer = boostDuration;
//             }
//
//             if (isBoosting)
//             {
//                 // Gradually increase the speed to boosted speed over speedIncreaseTime
//                 currentSpeed = Mathf.Lerp(baseSpeed, boostedSpeed, 1 - (boostTimer / speedIncreaseTime));
//                 boostTimer -= Time.deltaTime;
//
//                 if (boostTimer <= 0)
//                 {
//                     isBoosting = false;
//                     currentSpeed = baseSpeed;
//                 }
//             }
//
//             // Get keyboard inputs
//             rollInput = Input.GetAxis("Horizontal"); // A and D keys for roll
//             pitchInput = Input.GetAxis("Vertical"); // W and S keys for pitch
//             //yawInput = Input.GetAxis("Yaw"); // Q and E keys for yaw
//             thrustInput = Input.GetKeyDown(KeyCode.LeftShift); // Up and Down Arrow keys for thrust
//
//             // Apply rotations to the plane
//             transform.Rotate(pitchInput * pitchSpeed * Time.deltaTime,
//                 yawInput * yawSpeed * Time.deltaTime,
//                 -rollInput * rollSpeed * Time.deltaTime,
//                 Space.Self);
//
//             // Apply the current speed to the clouds to give the illusion of movement
//             _visualEnvironment.windSpeed.value = currentSpeed;
//         }
//     }
// }