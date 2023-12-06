using StealthBomber;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// This class is responsible for playing the initial cutscene when the game starts.
    /// </summary>
    public class InitialCutScene : MonoBehaviour
    {
        // Reference to the camera manager
        public CameraManager cameraManager;
        
        //Starting altitude of the stealth bomber
        private const float StartingAltitude = 10000;

        // The altitude at which the stealth bomber will stop ascending
        private const float EndingAltitude = 15000;

        // The speed at which the stealth bomber will ascend
        private float _ascendSpeed = 200f;

        // Flag to indicate if the camera should switch to the follow camera
        private bool _switchToFollowCamera;
        
        
        /// <summary>
        /// Set the initial position of the stealth bomber to the starting altitude.
        /// </summary>
        private void Start()
        {
            transform.position = new Vector3(0, StartingAltitude, 0);
        }

        
        /// <summary>
        /// Move the stealth bomber up until it reaches the ending altitude, then disable this script.
        /// </summary>
        private void FixedUpdate()
        {
            // When the stealth bomber reaches the ending altitude, set the game state to Flying and disable this script
            if (!(transform.position.y < EndingAltitude))
            {
                GameStateManager.CurrentGameState = GameState.Flying;
                enabled = false;
            }
            
            transform.Translate(Vector3.up * (_ascendSpeed * Time.fixedDeltaTime));

            // If the stealth bomber is within 3000 units of the ending altitude, switch to the follow camera
            if (!(transform.position.y > EndingAltitude - 3000f) || _switchToFollowCamera) return;

            _ascendSpeed = 400f;
            cameraManager.UpdateGameState(GameState.CutsceneFollow);
            _switchToFollowCamera = true;
        }
    }
}