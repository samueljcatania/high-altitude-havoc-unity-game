using Unity.VisualScripting;
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


        /*
         * Called when the game starts, gets all the afterburner emitters and particle systems in the stealth bomber
         */
        private void Start()
        {
            // Get all the afterburner emitters in the children of the stealth bomber
            _afterburners = GameObject.FindGameObjectsWithTag($"Afterburner");

            // Get all the particle systems in the children of the stealth bomber
            _engines = GetComponentsInChildren<ParticleSystem>();

            // Initially disable all the particle systems
            foreach (var engine in _engines)
            {
                engine.Stop();
            }
        }


        /*
         * Called when the players enters the start menu, enables the afterburner emitters and particle systems
         */
        public void StartEngines()
        {
            // Change the material of the afterburner emitters to the afterburner material
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


        /*
         * Called when the player exits the start menu, disables the afterburner emitters and particle systems
         */
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