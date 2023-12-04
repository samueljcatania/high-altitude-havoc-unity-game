using UnityEngine;

namespace StealthBomber
{
    public class CockpitLookController : MonoBehaviour
    {
        public Camera mainCamera;
        
        public float sensitivity = 100.0f;
        public float aimSmoothing = 10.0f;
        
        // The maximum and minimum angles that the gun can aim at
        private const float MaxYAngle = 50f;
        private const float MinYAngle = -15f;
        private const float MaxXAngle = 220f;
        private const float MinXAngle = 140f;

        private Vector2 currentRotation;
        private Vector2 targetRotation;
        
        private void Start()
        {
            OnEnable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentRotation = transform.localEulerAngles;
            targetRotation = transform.localEulerAngles;
        }
        
        private void OnEnable()
        {
            targetRotation = new Vector2(25, 180);
            currentRotation = targetRotation;
            transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0);
        }

        private void Update()
        {
            // Get mouse movement
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            
            targetRotation.y += mouseX;
            targetRotation.x -= mouseY;

            // Clamp the rotation
            targetRotation.x = Mathf.Clamp(targetRotation.x, MinYAngle, MaxYAngle);
            targetRotation.y = Mathf.Clamp(targetRotation.y, MinXAngle, MaxXAngle);

            // Smoothly interpolate towards the target rotation
            currentRotation = Vector2.Lerp(currentRotation, targetRotation, aimSmoothing * Time.deltaTime);

            // Apply the rotation
            transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0);
            
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                // Check if the ray hits an interactable object
                if (hit.collider.CompareTag($"Interactable")) {
                    // Implement interaction logic here, e.g., if the player clicks
                    if (Input.GetMouseButtonDown(0)) { // 0 is the left mouse button
                        // Call a method on the hit object to interact with it
                        //2hit.collider.GetComponent<YourInteractableClass>().Interact();
                    }
                }
            }

        }
    }
}