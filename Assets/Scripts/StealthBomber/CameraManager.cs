using System.Collections;
using Cinemachine;
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
        
        // Reference to cockpit game object
        public GameObject cockpit;
        
        // Reference to the raw image to fade the screen to a colour
        public RawImage fadeImage;
        public float fadeDuration = 3f;

        /*
     * Make the follow stealth bomber camera active and the cockpit camera inactive when the game starts
     */
        private void Start()
        {
            cinemachineFollowStealthBomberCamera.gameObject.SetActive(true);
            cinemachineCockpitCamera.gameObject.SetActive(false);
            
            cockpit.SetActive(false);
        }

        public void SwitchToCockpitCamera()
        {
            cockpit.SetActive(true);
            
            fadeImage.gameObject.SetActive(true);
            
            cinemachineFollowStealthBomberCamera.gameObject.SetActive(false);
            cinemachineCockpitCamera.gameObject.SetActive(true);
            
            StartCoroutine(EnableCockpit());
        }
        
        public void SwitchToFollowCamera()
        {
            cinemachineWidePanCamera.gameObject.SetActive(false);
            cinemachineFollowStealthBomberCamera.gameObject.SetActive(true);
        }
        
        
        private IEnumerator EnableCockpit()
        {
            float elapsedTime = 0;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = new Color(0, 0, 0, alpha);
            }
            
            yield return new WaitForSeconds(1f);

            fadeImage.gameObject.SetActive(false);
        }
    }
}