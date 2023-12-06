using UnityEngine;

namespace StealthBomber
{
    public class JoystickInteractable : MonoBehaviour, IInteractable
    {
        public CameraManager cameraManager;
        
        public void PerformAction()
        {
            cameraManager.SwitchToFollowCamera();
        }
    }
}

