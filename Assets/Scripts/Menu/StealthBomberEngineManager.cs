using UnityEngine;

namespace Menu
{
    /// <summary>
    /// This class is responsible for managing the afterburner emitters and particle systems in the stealth bomber.
    /// </summary>
    public class StealthBomberEngineManager : MonoBehaviour
    {
        // References to the afterburner emitters in the stealth bomber
        private GameObject[] _afterburners;

        // References to the particle systems in the stealth bomber
        private ParticleSystem[] _engines;

        // The material to change the afterburner emitters to when they are throttled down
        public Material idleMaterial;

        // The material to change the afterburner emitters to when they are throttled up
        public Material glowingMaterial;

        
        /// <summary>
        /// Called when the game starts, finds all the afterburner emitters and particle systems in the stealth bomber.
        /// </summary>
        private void Start()
        {
            _afterburners = GameObject.FindGameObjectsWithTag($"Afterburner");
            _engines = GetComponentsInChildren<ParticleSystem>();

            // Initially disable all the particle systems
            foreach (var engine in _engines)
            {
                engine.Stop();
            }
        }


        /// <summary>
        /// Called when the player enters the start menu, enables the afterburner emitters and particle systems.
        /// </summary>
        public void StartEngines()
        {
            // Change the material of the afterburner emitters to the glowing material
            foreach (var afterburner in _afterburners)
            {
                afterburner.GetComponent<Renderer>().material = glowingMaterial;
            }

            // Enable all the particle systems
            foreach (var engine in _engines)
            {
                engine.Play();
            }
        }


        /// <summary>
        /// Called when the player exits the start menu, disables the afterburner emitters and particle systems.
        /// </summary>
        public void StopEngines()
        {
            // Change the material of the afterburner emitters to the default material
            foreach (var afterburner in _afterburners)
            {
                afterburner.GetComponent<Renderer>().material = idleMaterial;
            }

            // Disable all the particle systems
            foreach (var engine in _engines)
            {
                engine.Stop();
            }
        }
    }
}