using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// This class is responsible for managing the cameras in the game. It allows for the player to switch between
    /// different cameras and provides a transition effect when switching between cameras, depending on the pair
    /// of game states being switched to and from.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        [System.Serializable]
        // Struct to map pairs of game states to their corresponding camera and transition
        public struct StateTransitionPair
        {
            public GameState fromState;
            public GameState toState;
            public Transition transition;
        }

        [System.Serializable]
        // Struct to store transition information
        public struct Transition
        {
            public CinemachineVirtualCamera camera;
            public Color transitionColor;
            public float transitionDuration;
            private int _cameraIndex;

            public int CameraIndex
            {
                get => _cameraIndex;
                set => _cameraIndex = value;
            }
        }

        [SerializeField]
        // Array of structs to map pairs of game states to their corresponding camera and transition
        private StateTransitionPair[] stateTransitionPairs;

        // Reference to cockpit game object
        public GameObject cockpit;

        // Reference to the stealth bomber game object
        public GameObject stealthBomber;

        // Reference to the screen fade game script object
        public ScreenFade screenFade;

        // Reference to all the cinemachine virtual cameras
        private List<CinemachineVirtualCamera> virtualCameras;

        // Map all the game states to their corresponding transition
        private Dictionary<(GameState, GameState), Transition> statePairTransitionMap;

        // The priority of the active camera
        private const int ActivePriority = 10;


        /// <summary>
        /// Create and initialize the list of virtual cameras and the map of game state pairs.
        /// </summary>
        private void Awake()
        {
            virtualCameras = new List<CinemachineVirtualCamera>();
            statePairTransitionMap = new Dictionary<(GameState, GameState), Transition>();

            // Fetch and add virtual cameras
            foreach (var obj in GameObject.FindGameObjectsWithTag("VirtualCamera"))
            {
                var virtualCam = obj.GetComponent<CinemachineVirtualCamera>();
                virtualCameras.Add(virtualCam);
            }
            
            // Initialize the map of game state pairs to their corresponding transitions
            foreach (var pair in stateTransitionPairs)
            {
                var cameraIndex = virtualCameras.IndexOf(pair.transition.camera);
                var modifiedTransition = pair.transition;
                modifiedTransition.CameraIndex = cameraIndex;
                statePairTransitionMap.Add((pair.fromState, pair.toState), modifiedTransition);
            }
        }


        /// <summary>
        /// Disable the cockpit and all cameras but the follow stealth bomber camera when the game starts.
        /// </summary>
        private void Start()
        {
            cockpit.SetActive(false);
            UpdateGameState(GameStateManager.CurrentGameState);
        }


        /// <summary>
        /// Check if the player has pressed the C key and the game state is Flying. If so, switch to the cockpit camera.
        /// </summary>
        private void Update()
        {
            // Return if the player hasn't pressed the C key
            if (!Input.GetKeyDown(KeyCode.C)) return;

            if (GameStateManager.CurrentGameState == GameState.Minigun ||
                GameStateManager.CurrentGameState == GameState.Flying)
            {
                UpdateGameState(GameState.Cockpit);
            }
        }


        /// <summary>
        /// Update the game state and switch to the appropriate camera that corresponds to the new game state.
        /// </summary>
        /// <param name="newState"> The new game state to switch to. </param>
        public void UpdateGameState(GameState newState)
        {
            var previousState = GameStateManager.CurrentGameState;
            var secondCameraIndex = -1;
            
            // Check if the transition exists in the map
            if (!statePairTransitionMap.TryGetValue((previousState, newState), out var transition)) return;

            // Transition logic for the pair
            if (newState == GameState.Minigun || (previousState == GameState.Minigun && newState == GameState.Cockpit))
            {
                GameStateManager.CurrentGameState = GameState.Monitor;
                SwitchCamera(statePairTransitionMap[(GameState.Monitor, GameState.Monitor)].CameraIndex);
                secondCameraIndex = statePairTransitionMap[(previousState, newState)].CameraIndex;
            }
            else
            {
                SwitchCamera(statePairTransitionMap[(previousState, newState)].CameraIndex);
            }
            
            // If the transition duration is -1, don't fade
            if (Math.Abs(statePairTransitionMap[(previousState, newState)].transitionDuration - (-1f)) < 0.01) return;

            StartCoroutine(FadeSequence(screenFade.FadeScreen(
                transition.transitionDuration,
                transition.transitionColor), secondCameraIndex));
            
            GameStateManager.CurrentGameState = newState;
        }


        /// <summary>
        /// Switches the active camera to the camera at the specified index.
        /// </summary>
        /// <param name="cameraIndex"> The index of the camera to switch to. </param>
        private void SwitchCamera(int cameraIndex)
        {
            // Set the priority of the active camera to ActivePriority and the priority of all other cameras to 0
            foreach (var virtualCamera in virtualCameras)
            {
                virtualCamera.Priority = virtualCamera == virtualCameras[cameraIndex] ? ActivePriority : 0;
            }
        }


        /// <summary>
        /// Allows for a two coroutines to be executed one after the other such that the second coroutine is executed
        /// after the first coroutine has finished. Allows for game objects to be enabled and disabled between the two.
        /// </summary>
        /// <param name="coroutine"> The first coroutine to execute. </param>
        /// <param name="additionalCameraIndex"> The index of the camera to switch to after the first coroutine has finished. </param>
        /// <returns> An IEnumerator that can be used to start the sequence. </returns>
        private IEnumerator FadeSequence(IEnumerator coroutine, int additionalCameraIndex)
        {
            yield return StartCoroutine(coroutine);

            stealthBomber.SetActive(!stealthBomber.activeSelf);
            cockpit.SetActive(!cockpit.activeSelf);

            if (additionalCameraIndex != -1) SwitchCamera(additionalCameraIndex);

            yield return StartCoroutine(screenFade.FadeScreen(1f, Color.clear, 1f));
        }
    }
}