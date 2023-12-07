using UnityEngine;

namespace Menu
{
    /// <summary>
    /// This class is responsible for orbiting the camera around the stealth bomber.
    /// </summary>
    public class OrbitCamera : MonoBehaviour
    {
        // The target to orbit around
        public Transform stealthBomber;
    
        // The speed at which the camera will orbit
        private const float OrbitSpeed = 1;

        
        /// <summary>
        /// Orbits the camera around the stealth bomber.
        /// </summary>
        private void FixedUpdate()
        {
            transform.LookAt(stealthBomber);
            transform.Translate(Vector3.right * (OrbitSpeed * Time.fixedDeltaTime));
        }
    }
}

