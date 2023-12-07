using Game;
using UnityEngine;

namespace StealthBomber
{
    /// <summary>
    /// This class implements the ICockpitInteractable interface to allow the player to interact with the monitor.
    /// </summary>
    public class MonitorInteractable : MonoBehaviour, ICockpitInteractable
    {
        // Reference to the camera manager
        public CameraManager cameraManager;


        /// <summary>
        /// When the player interacts with the monitor, switch to the minigun camera.
        /// </summary>
        public void PerformAction()
        {
            cameraManager.UpdateGameState(GameState.Minigun);
        }
    }
}