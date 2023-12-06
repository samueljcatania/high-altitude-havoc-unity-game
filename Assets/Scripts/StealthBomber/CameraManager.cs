using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace StealthBomber
{
    public class CameraManager : MonoBehaviour
    {
        // References to all the cinemachine virtual cameras
        public CinemachineVirtualCamera cinemachineFollowStealthBomberCamera;
        public CinemachineVirtualCamera cinemachineCockpitCamera;
        public CinemachineVirtualCamera cinemachineWidePanCamera;
        public CinemachineVirtualCamera cinemachineMinigunCamera;

        // Reference to cockpit game object
        public GameObject cockpit;

        // Reference to the stealth bomber game object
        public GameObject stealthBomber;

        // Reference to the screen fade game script object
        public ScreenFade screenFade;

        /*
         * Make the follow stealth bomber camera active and the cockpit camera inactive when the game starts
         */
        private void Start()
        {
            cinemachineFollowStealthBomberCamera.gameObject.SetActive(true);
            cinemachineCockpitCamera.gameObject.SetActive(false);
            cinemachineMinigunCamera.gameObject.SetActive(false);
            cockpit.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C) && !cinemachineCockpitCamera.gameObject.activeSelf)
            {
                SwitchToCockpitCamera();
            }
        }

        private void SwitchToCockpitCamera()
        {
            // Check if either the follow stealth bomber camera or the minigun camera is active and disable it
            if (cinemachineFollowStealthBomberCamera.gameObject.activeSelf)
            {
                cinemachineFollowStealthBomberCamera.gameObject.SetActive(false);
            }
            else if (cinemachineMinigunCamera.gameObject.activeSelf)
            {
                cinemachineCockpitCamera.gameObject.SetActive(false);
            }

            cinemachineCockpitCamera.gameObject.SetActive(true);

            StartCoroutine(FadeSequence(
                screenFade.FadeScreen(1f, Color.black),
                screenFade.FadeScreen(1f, Color.clear, 1f),
                stealthBomber,
                cockpit));
        }

        public void TransitionFromWidePan()
        {
            cinemachineWidePanCamera.gameObject.SetActive(false);
            cinemachineFollowStealthBomberCamera.gameObject.SetActive(true);
        }

        public void SwitchToFollowCamera()
        {
            cinemachineFollowStealthBomberCamera.gameObject.SetActive(false);
            cinemachineCockpitCamera.gameObject.SetActive(true);
        }

        public void SwitchToMinigunCamera()
        {
            cinemachineMinigunCamera.gameObject.SetActive(true);
            cinemachineCockpitCamera.gameObject.SetActive(false);
        }


        /// <summary>
        /// Allows for a two coroutines to be executed one after the other such that the second coroutine is executed
        /// after the first coroutine has finished. Allows for game objects to be enabled and disabled between the two.
        /// </summary>
        /// <param name="coroutineOne"> The first coroutine to execute. </param>
        /// <param name="coroutineTwo"> The second coroutine to execute. </param>
        /// <param name="gameObjectToDisable"> The game object to disable after the first coroutine has finished. </param>
        /// <param name="gameObjectToEnable"> The game object to enable after the first coroutine has finished. </param>
        /// <returns> An IEnumerator that can be used to start the sequence. </returns>
        private IEnumerator FadeSequence(IEnumerator coroutineOne, IEnumerator coroutineTwo,
            GameObject gameObjectToDisable, GameObject gameObjectToEnable)
        {
            yield return StartCoroutine(coroutineOne);
            gameObjectToDisable.SetActive(false);
            gameObjectToEnable.SetActive(true);
            yield return StartCoroutine(coroutineTwo);
        }
    }
}