using System.Collections.Generic;
using UnityEngine;

namespace StealthBomber
{
    public class CockpitLookController : MonoBehaviour
    {
        // Reference to the main camera in the scene
        public Camera mainCamera;

        // Reference to the material used to highlight interactable objects
        public Material highlightMaterial;

        // The sensitivity of the mouse when looking around the cockpit
        public float sensitivity = 150f;

        // The smoothing applied to the movement when looking around the cockpit
        public float aimSmoothing = 10f;

        // The maximum and minimum angles that the gun can aim at
        private const float MaxYAngle = 60f;
        private const float MinYAngle = -15f;
        private const float MaxXAngle = 220f;
        private const float MinXAngle = 140f;

        // The current and target rotation used for smoothing
        private Vector2 _currentRotation;
        private Vector2 _targetRotation;

        // Dictionary used to cache the renderers of game objects that are interactable in the cockpit
        private readonly Dictionary<GameObject, Renderer> _objectRenderers = new Dictionary<GameObject, Renderer>();
        private readonly Dictionary<Renderer, Material> _originalMaterials = new Dictionary<Renderer, Material>();

        // A reference to the currently highlighted object
        private GameObject _currentlyHighlightedObject;

        // The delay between raycasts that check for interactable objects to improve performance
        private const float RaycastDelay = 0.1f;

        // The time of the last raycast
        private float _lastRaycastTime = 0f;
        
        // Reference to the layer that contains the interactable objects in the cockpit
        private LayerMask _interactableLayer;


        /// <summary>
        /// Provides initial setup for the cockpit camera.
        /// </summary>
        private void Start()
        {
            _interactableLayer =  LayerMask.NameToLayer("Interactable");
            OnEnable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CacheObjectRenderers();
        }


        /// <summary>
        /// Called every time the object is enabled, resets the rotation variables to a default value.
        /// </summary>
        private void OnEnable()
        {
            _targetRotation = new Vector2(25, 180);
            _currentRotation = _targetRotation;
            transform.localEulerAngles = new Vector3(_currentRotation.x, _currentRotation.y, 0);
        }


        /// <summary>
        /// Called every frame, handles the aiming and interaction raycast of the cockpit camera.
        /// </summary>
        private void Update()
        {
            HandleAiming();

            // If the time since the last raycast is less than the delay, return
            if (!(Time.time - _lastRaycastTime >= RaycastDelay)) return;

            HandleInteraction();
            _lastRaycastTime = Time.time;
        }


        /// <summary>
        /// Caches the renderers of the interactable objects in the cockpit.
        /// </summary>
        private void CacheObjectRenderers()
        {
            foreach (var interactableObject in GameObject.FindGameObjectsWithTag("Interactable"))
            {
                var objectRenderer = interactableObject.GetComponent<Renderer>();
                if (objectRenderer != null)
                {
                    _objectRenderers[interactableObject] = objectRenderer;
                }
            }
        }


        /// <summary>
        /// Applies a highlight material to a given game object, storing the original material if it hasn't been stored
        /// yet to later revert revert back to.
        /// </summary>
        /// <param name="interactableObject"> The interactable game object to highlight. </param>
        private void HighlightObject(GameObject interactableObject)
        {
            // If the game object is null, return
            if (!_objectRenderers.TryGetValue(interactableObject, out var objectRenderer)) return;

            // Store the original material if it hasn't been stored yet
            _originalMaterials.TryAdd(objectRenderer, objectRenderer.material);

            // Set the highlight material
            objectRenderer.material = highlightMaterial;
        }

        /// <summary>
        /// Resets the highlight material of the currently highlighted object to its original material.
        /// </summary>
        private void ResetHighlight()
        {
            // If the currently highlighted object is null or doesn't exist in the dictionary, return
            if (_currentlyHighlightedObject == null ||
                !_objectRenderers.TryGetValue(_currentlyHighlightedObject, out var objectRenderer)) return;

            // Revert to the original material
            if (_originalMaterials.TryGetValue(objectRenderer, out var originalMaterial))
            {
                objectRenderer.material = originalMaterial;
            }

            _currentlyHighlightedObject = null;
        }


        /// <summary>
        /// Handles the aiming of the turret.
        /// </summary>
        private void HandleAiming()
        {
            // Get mouse movement
            var mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            _targetRotation.y += mouseX;
            _targetRotation.x -= mouseY;

            // Clamp the rotation to the minimum and maximum angles
            _targetRotation.x = Mathf.Clamp(_targetRotation.x, MinYAngle, MaxYAngle);
            _targetRotation.y = Mathf.Clamp(_targetRotation.y, MinXAngle, MaxXAngle);

            // Smoothly interpolate towards the target rotation
            _currentRotation = Vector2.Lerp(_currentRotation, _targetRotation, aimSmoothing * Time.deltaTime);

            // Apply the rotation
            transform.localEulerAngles = new Vector3(_currentRotation.x, _currentRotation.y, 0);
        }


        /// <summary>
        /// Handles the interaction of the cockpit camera with the environment of the cockpit.
        /// </summary>
        private void HandleInteraction()
        {
            var ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _interactableLayer))
            {
                var hitObject = hit.collider.gameObject;

                // If the object hit is not interactable or is already highlighted, return
                if (!hitObject.CompareTag("Interactable") || hitObject == _currentlyHighlightedObject) return;
                
                // Reset the highlight of the currently highlighted object and highlight the new object
                ResetHighlight();
                _currentlyHighlightedObject = hitObject;
                HighlightObject(_currentlyHighlightedObject);
                    
                // If the player clicks the left mouse button when looking at an interactable object
                if (Input.GetMouseButtonDown(0))
                {
                    //2hit.collider.GetComponent<YourInteractableClass>().Interact();
                }
            }
            else
            {
                ResetHighlight();
            }
        }
    }
}