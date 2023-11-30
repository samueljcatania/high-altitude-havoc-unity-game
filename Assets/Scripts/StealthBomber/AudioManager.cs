using UnityEngine;

namespace StealthBomber
{
    
    /// <summary>
    /// This class is responsible for managing the audio in the stealth bomber.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        // Audio clips to play when the Stealth Bomber is flying
        public AudioClip engineSound;
        public AudioClip intenseRadioChatter;
        
        // The audio sources used to play the audio clips
        private AudioSource _engineAudioSource;
        private AudioSource _intenseRadioChatterAudioSource;
        
        /*
         * Called when the game starts, creates the audio source for the engine sound and sets the audio clip
         */
        private void Start()
        {
            // Create a new audio sources
            _engineAudioSource = gameObject.AddComponent<AudioSource>();
            _intenseRadioChatterAudioSource = gameObject.AddComponent<AudioSource>();
            
            // Set the audio clips for each audio source
            _engineAudioSource.clip = engineSound;
            _intenseRadioChatterAudioSource.clip = intenseRadioChatter;
            
            // Set the volume for both audio source
            _engineAudioSource.volume = 0.1f;
            _intenseRadioChatterAudioSource.volume = 0.5f;
            
            // Play the sounds
            _engineAudioSource.Play();
            _intenseRadioChatterAudioSource.Play();
        }
    }
}
