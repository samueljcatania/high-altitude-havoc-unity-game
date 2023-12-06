using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StealthBomber;
using UnityEngine;

namespace Game
{
    public class CameraManager : MonoBehaviour
    {
        // Reference to all the cinemachine virtual cameras
        public CinemachineVirtualCamera[] virtualCameras;

        // Map all the game states to their corresponding camera index
        private static readonly Dictionary<GameState, int> GameStateToCameraIndex = new()
        {
            { GameState.CutsceneWidePan, 0 },
            { GameState.CutsceneFollow, 1 },
            { GameState.Flying, 1 },
            { GameState.Cockpit, 2 },
            { GameState.Minigun, 4 }
        };

        // Reference to cockpit game object
        public GameObject cockpit;

        // Reference to the stealth bomber game object
        public GameObject stealthBomber;

        // Reference to the screen fade game script object
        public ScreenFade screenFade;

        // The priority of the active camera
        private const int ActivePriority = 10;


        /// <summary>
        /// Disable the cockpit and all cameras but the follow stealth bomber camera when the game starts.
        /// </summary>
        private void Start()
        {
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
            GameStateManager.CurrentGameState = newState;

            switch (newState)
            {
                case GameState.Cockpit:
                    if (previousState == GameState.Minigun)
                    {
                        SwitchCamera(4);
                        StartCoroutine(FadeSequence(
                            screenFade.FadeScreen(1f, Color.white),
                            screenFade.FadeScreen(1f, Color.clear, 1f),
                            GameStateToCameraIndex[newState]));
                    }
                    else
                    {
                        SwitchCamera(GameStateToCameraIndex[newState]);
                        StartCoroutine(FadeSequence(
                            screenFade.FadeScreen(1f, Color.black),
                            screenFade.FadeScreen(1f, Color.clear, 1f)));
                    }

                    break;

                case GameState.Flying:
                    SwitchCamera(GameStateToCameraIndex[newState]);
                    StartCoroutine(FadeSequence(
                        screenFade.FadeScreen(1f, Color.black),
                        screenFade.FadeScreen(1f, Color.clear, 1f)));
                    break;

                case GameState.Minigun:
                    SwitchCamera(GameStateToCameraIndex[newState]);
                    StartCoroutine(FadeSequence(
                        screenFade.FadeScreen(1f, Color.white),
                        screenFade.FadeScreen(1f, Color.clear, 1f), 3));
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
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
        /// <param name="coroutineOne"> The first coroutine to execute. </param>
        /// <param name="coroutineTwo"> The second coroutine to execute. </param>
        /// <param name="additionalCameraIndex"> The index of the camera to switch to after the first coroutine has finished. </param>
        /// <returns> An IEnumerator that can be used to start the sequence. </returns>
        private IEnumerator FadeSequence(IEnumerator coroutineOne, IEnumerator coroutineTwo,
            int additionalCameraIndex = -1)
        {
            yield return StartCoroutine(coroutineOne);

            stealthBomber.SetActive(!stealthBomber.activeSelf);
            cockpit.SetActive(!cockpit.activeSelf);

            if (additionalCameraIndex != -1) SwitchCamera(additionalCameraIndex);

            yield return StartCoroutine(coroutineTwo);
        }
    }
}