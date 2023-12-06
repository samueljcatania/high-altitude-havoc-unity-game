using Game;
using UnityEngine;

namespace StealthBomber
{
    /// <summary>
    /// This class implements the IInteractable interface to allow the player to interact with the joystick.
    /// </summary>
    public class JoystickInteractable : MonoBehaviour, IInteractable
    {
        // Reference to the camera manager
        public CameraManager cameraManager;
        
        
        /// <summary>
        /// When the player interacts with the joystick, switch to the follow camera.
        /// </summary>
        public void PerformAction()
        {
            cameraManager.UpdateGameState(GameState.Flying);
        }
    }
}

