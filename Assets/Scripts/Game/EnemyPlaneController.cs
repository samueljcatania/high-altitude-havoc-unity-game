using UnityEngine;

public class EnemyPlaneController : MonoBehaviour
{
    public Transform bomber;
    public Transform enemyPlane;
    public float followDistance = 20f;
    public float lateralMovementAmplitude = 5f;
    public float lateralMovementFrequency = 0.5f;
    public float verticalMovementAmplitude = 3f;
    public float verticalMovementInterval = 5f;
    public float verticalMovementTransitionTime = 2f;
    public float baseVerticalOffset = 5f; // Base vertical offset of the enemy plane
    public float maxRollAngle = 45f; // Maximum roll angle
    public float maxPitchAngle = 30f; // Maximum pitch angle
    public float rotationSensitivity = 10f;
    public float rotationSmoothness = 2f;

    private float lateralOffset;
    private float targetVerticalOffset;
    private float currentVerticalOffset;
    private float verticalChangeTimer;
    private float verticalTransitionTimer;
    private Vector3 previousPosition;

    // Start is called before the first frame update
    void Start()
    {
        targetVerticalOffset = verticalMovementAmplitude + baseVerticalOffset;
        currentVerticalOffset = verticalMovementAmplitude + baseVerticalOffset;
        verticalChangeTimer = verticalMovementInterval;
        previousPosition = enemyPlane.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyPosition();
        UpdateEnemyRotation();
        previousPosition = enemyPlane.position;
    }

    void UpdateEnemyPosition()
    {
        // Lateral Movement
        lateralOffset = lateralMovementAmplitude * Mathf.Sin(Time.time * lateralMovementFrequency);

        // Vertical Movement
        verticalChangeTimer -= Time.deltaTime;
        if (verticalChangeTimer <= 0)
        {
            targetVerticalOffset = Random.Range(-verticalMovementAmplitude, verticalMovementAmplitude) + baseVerticalOffset;
            verticalChangeTimer = verticalMovementInterval;
            verticalTransitionTimer = 0; // Reset transition timer
        }

        if (verticalTransitionTimer < verticalMovementTransitionTime)
        {
            verticalTransitionTimer += Time.deltaTime;
            currentVerticalOffset = Mathf.Lerp(currentVerticalOffset, targetVerticalOffset, verticalTransitionTimer / verticalMovementTransitionTime);
        }

        // Calculate and set new position
        Vector3 newPosition = bomber.position - bomber.forward * followDistance;
        newPosition += bomber.right * lateralOffset;
        newPosition += bomber.up * currentVerticalOffset;

        enemyPlane.position = newPosition;
    }

    void UpdateEnemyRotation()
    {
        Vector3 movementDirection = (enemyPlane.position - previousPosition) / Time.deltaTime;
        Vector3 directionToBomber = bomber.position - enemyPlane.position;

        // Calculate roll and pitch based on movement speed
        float rollAngle = Mathf.Clamp(movementDirection.x * rotationSensitivity, -maxRollAngle, maxRollAngle);
        float pitchAngle = Mathf.Clamp(movementDirection.y * rotationSensitivity, -maxPitchAngle, maxPitchAngle);

        Quaternion targetRotation = Quaternion.LookRotation(directionToBomber);
        Quaternion rollAndPitch = Quaternion.Euler(pitchAngle, targetRotation.eulerAngles.y, rollAngle);

        // Smoothly interpolate the enemy's rotation
        enemyPlane.rotation = Quaternion.Slerp(enemyPlane.rotation, rollAndPitch, rotationSmoothness * Time.deltaTime);
    }
}





//
// // Update is called once per frame
// void Update()
// {
//     foreach (Transform enemy in enemyPlanes)
//     {
//         // Move each enemy plane towards the player plane
//         PositionBehindBomber(enemy);
//
//         // Check if the enemy is too far and respawn if necessary
//         if (Vector3.Distance(enemy.position, stealthBomber.position) > maxDistance)
//         {
//             RespawnEnemy(enemy);
//         }
//     }
// }
//
// void PositionBehindBomber(Transform enemy)
// {
//     // Calculate the position directly behind the bomber
//     Vector3 behindPosition = stealthBomber.position - stealthBomber.forward * followDistance;
//     
//     // Add some random lateral and vertical movement
//     behindPosition += stealthBomber.right * Random.Range(-lateralMovementSpeed, lateralMovementSpeed) * Time.deltaTime;
//     behindPosition += stealthBomber.up * Random.Range(-verticalMovementRange, verticalMovementRange);
//
//     // Update the enemy's position
//     enemy.position = behindPosition;
// }
//
// void RespawnEnemy(Transform enemy)
// {
//     Vector3 randomPosition = Random.insideUnitSphere * spawnDistance;
//     randomPosition += stealthBomber.position;
//     randomPosition.y = stealthBomber.position.y; // Keep them at the same height
//
//     enemy.position = randomPosition;
// }