using System.Collections.Generic;
using UnityEngine;

namespace StealthBomber
{
    public class CockpitLookController : MonoBehaviour
    {
        // Reference to the main camera in the scene
        public Camera mainCamera;
        
        // Reference to the cockpit game object
        public GameObject cockpit;

        // Reference to the layer that contains the interactable objects in the cockpit
        public LayerMask interactableLayer;

        // Reference to the material used to highlight interactable objects
        public Material highlightMaterial;

        // The sensitivity of the mouse when looking around the cockpit
        public float sensitivity = 150f;

        // The smoothing applied to the movement when looking around the cockpit
        public float aimSmoothing = 10f;

        // The maximum and minimum angles that the gun can aim at
        private const float MaxYAngle = 60f;
        private const float MinYAngle = -15f;
        private const float MaxXAngle = 50f;
        private const float MinXAngle = -50f;

        // The current and target rotation used for smoothing
        private Vector2 _currentRotation;
        private Vector2 _targetRotation;

        // Dictionary used to cache the renderers of game objects that are interactable in the cockpit
        private readonly Dictionary<GameObject, Renderer> _objectRenderers = new();

        // Dictionary used to cache the original materials of the interactable objects in the cockpit
        private readonly Dictionary<Renderer, Material> _originalMaterials = new();

        // Dictionary used to cache the interactable scripts of each interactable object in the cockpit
        private readonly Dictionary<GameObject, IInteractable> _objectInteractables = new();

        // A reference to the currently highlighted object
        public GameObject _currentlyHighlightedObject;


        /// <summary>
        /// Provides initial setup for the cockpit camera.
        /// </summary>
        private void Start()
        {
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
            _targetRotation = new Vector2(25, 0);
            _currentRotation = _targetRotation;
            transform.localEulerAngles = new Vector3(_currentRotation.x, _currentRotation.y, 0);
        }


        /// <summary>
        /// Called every frame, handles the aiming and interaction raycast of the cockpit camera.
        /// </summary>
        private void FixedUpdate()
        {
            // Only handle aiming and interaction if the game state is Cockpit
            if (GameStateManager.CurrentGameState != GameState.Cockpit) return;
            
            HandleAiming();
            HandleInteraction();
        }


        /// <summary>
        /// Caches the renderers of the interactable objects in the cockpit.
        /// </summary>
        private void CacheObjectRenderers()
        {
            cockpit.SetActive(true);
            
            foreach (var interactableObject in GameObject.FindGameObjectsWithTag("Interactable"))
            {
                var objectRenderer = interactableObject.GetComponent<Renderer>();
                _objectRenderers[interactableObject] = objectRenderer;
                _objectInteractables[interactableObject] = interactableObject.GetComponent<IInteractable>();
            }
            
            cockpit.SetActive(false);
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
            if (ReferenceEquals(_currentlyHighlightedObject, null) ||
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

            if (Physics.Raycast(ray, out var hit, 10f, interactableLayer))
            {
                var hitObject = hit.collider.gameObject;

                // If the new object is the same as the currently highlighted object, don't reset the highlight
                if (hitObject == !_currentlyHighlightedObject)
                {
                    ResetHighlight();
                    _currentlyHighlightedObject = hitObject;
                    HighlightObject(_currentlyHighlightedObject);
                }

                // If the player clicks the left mouse button when looking at an interactable object
                if (Input.GetMouseButtonDown(0))
                {
                    _objectInteractables[hitObject].PerformAction();
                }
            }
            else
            {
                ResetHighlight();
            }
        }
    }
}