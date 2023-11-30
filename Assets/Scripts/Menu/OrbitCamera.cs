using UnityEngine;

namespace Menu
{
    /// <summary>
    /// This class is responsible for orbiting the camera around the stealth bomber.
    /// </summary>
    public class OrbitCamera : MonoBehaviour
    {
        // The target to orbit around
        public Transform target;
    
        // The speed at which the camera will orbit
        public float orbitSpeed = 0.5f;

        private void Update()
        {
            transform.LookAt(target);
            transform.Translate(Vector3.right * (orbitSpeed * Time.fixedDeltaTime));
        }
    }
}

