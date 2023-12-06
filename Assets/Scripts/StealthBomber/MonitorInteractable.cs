using UnityEngine;

namespace StealthBomber
{
    public class MonitorInteractable : MonoBehaviour, IInteractable
    {
        public CameraManager cameraManager;
        
        public void PerformAction()
        {
            cameraManager.SwitchToMinigunCamera();
        }
    }
}
