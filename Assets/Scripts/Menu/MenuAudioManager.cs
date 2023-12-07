using UnityEngine;

namespace Menu
{
    /// <summary>
    /// This class is responsible for managing the audio in the main menu.
    /// </summary>
    public class MenuAudioManager : MonoBehaviour
    {
        // References to the audio sources to play in the menu
        public AudioClip jetStartup;
        public AudioClip jetIdlingLoop;
        public AudioClip radioChatterLoop;

        // References to the audio sources for the audio clips
        private AudioSource _jetStartupAudioSource;
        private AudioSource _jetIdlingLoopAudioSource;
        private AudioSource _radioChatterAudioSource;

        // Tracks whether the player is currently in the start menu
        private bool _inStartMenu;

        /// <summary>
        /// When the game starts, create the audio sources and play the audio clips.
        /// </summary>
        private void Awake()
        {
            // Create a new audio source for each audio clip
            _jetStartupAudioSource = gameObject.AddComponent<AudioSource>();
            _jetIdlingLoopAudioSource = gameObject.AddComponent<AudioSource>();
            _radioChatterAudioSource = gameObject.AddComponent<AudioSource>();

            // Set the audio clips for each audio source
            _jetStartupAudioSource.clip = jetStartup;
            _jetIdlingLoopAudioSource.clip = jetIdlingLoop;
            _radioChatterAudioSource.clip = radioChatterLoop;

            // Set the volume for each audio source
            _jetStartupAudioSource.volume = 0.2f;
            _jetIdlingLoopAudioSource.volume = 0.2f;
            _radioChatterAudioSource.volume = 0.01f;

            // Toggle audio sources from playing on awake
            _jetStartupAudioSource.playOnAwake = false;
            _jetIdlingLoopAudioSource.playOnAwake = false;
            _radioChatterAudioSource.playOnAwake = true;

            // Set the loop audio source to loop
            _jetIdlingLoopAudioSource.loop = true;
            _radioChatterAudioSource.loop = true;

            // Add reverb filter for both audio sources
            gameObject.AddComponent<AudioReverbFilter>();

            // Set the reverb preset to "hangar"
            gameObject.GetComponent<AudioReverbFilter>().reverbPreset =
                AudioReverbPreset.Hangar;

            // Play the radio chatter sound
            _radioChatterAudioSource.Play();
        }


        /// <summary>
        /// If the player is in the start menu and the jet startup sound has finished playing, play the jet idling loop
        /// </summary>
        private void Update()
        {
            if (_inStartMenu && !_jetStartupAudioSource.isPlaying && !_jetIdlingLoopAudioSource.isPlaying)
            {
                _jetIdlingLoopAudioSource.Play();
            }
        }


        /// <summary>
        /// Called when the player enters the start menu, plays the jet startup sound.
        /// </summary>
        public void PlayJetStartupSound()
        {
            _inStartMenu = true;
            _jetStartupAudioSource.Play();
        }


        /// <summary>
        /// Called when the player exits the start menu, stops the jet startup and idle sound.
        /// </summary>
        public void StopJetStartupSound()
        {
            _inStartMenu = false;

            if (_jetStartupAudioSource.isPlaying)
            {
                _jetStartupAudioSource.Stop();
            }

            if (_jetIdlingLoopAudioSource.isPlaying)
            {
                _jetIdlingLoopAudioSource.Stop();
            }
        }
    }
}