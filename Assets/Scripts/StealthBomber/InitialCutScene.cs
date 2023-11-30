using UnityEngine;
using UnityEngine.Serialization;

namespace StealthBomber
{
    /// <summary>
    /// This class is responsible for the movement of the stealth bomber.
    /// </summary>
    public class StealthBomberMovement : MonoBehaviour
    {
        //Starting altitude of the stealth bomber
        public float startingAltitude = 5000f;
        
        // The altitude at which the stealth bomber will stop ascending
        public float endingAltitude = 8900f;

        // The speed at which the stealth bomber will ascend
        public float ascendSpeed = 200f;

        // Reference to the camera manager
        public CameraManager cameraManager;

        private bool _switchToCockpitCamera = false;
        private bool _switchToFollowCamera = false;
        
        /*
         * Called when the game starts, sets the starting position of the stealth bomber
         */
        private void Start()
        {
            transform.position = new Vector3(0, startingAltitude, 0);
        }

        private void FixedUpdate()
        {
            if (transform.position.y < endingAltitude)
            {
                transform.Translate(Vector3.up * (ascendSpeed * Time.fixedDeltaTime));
                
                if (transform.position.y > endingAltitude - 3000f && !_switchToFollowCamera)
                {
                    cameraManager.SwitchToFollowCamera();
                    _switchToFollowCamera = true;
                }
            }
            else if (!_switchToCockpitCamera)
            {
                // cameraManager.SwitchToCockpitCamera();
                // _switchToCockpitCamera = true;
            }
        }
    }
}