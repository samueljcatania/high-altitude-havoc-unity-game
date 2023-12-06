using StealthBomber;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// This class is responsible for the movement of the stealth bomber.
    /// </summary>
    public class StealthBomberMovement : MonoBehaviour
    {
        // Reference to the camera manager
        public CameraManager cameraManager;
        
        //Starting altitude of the stealth bomber
        private float startingAltitude = 10000;
        
        // The altitude at which the stealth bomber will stop ascending
        private float endingAltitude = 16000;

        // The speed at which the stealth bomber will ascend
        private float ascendSpeed = 200f;

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
                    cameraManager.TransitionFromWidePan();
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