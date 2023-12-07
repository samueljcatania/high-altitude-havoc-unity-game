using System.Collections;
using Game;
using UnityEngine;

namespace StealthBomber
{
    /// <summary>
    /// This class is responsible for controlling the turret on the stealth bomber. It allows the player to aim the
    /// turret and fire bullets from it. It also handles the audio for the turret.
    /// </summary>
    public class TurretController : MonoBehaviour
    {
        // Reference to the barrel of the turret to control its spin
        public Transform barrel;
        
        // Reference to the point in space where the bullets will fire from
        public Transform firePoint;

        // Reference to the muzzle flash light
        public Light muzzleFlashLight;
        
        // Reference to the muzzle flash particles system
        public ParticleSystem muzzleFlashParticles;
        
        // The duration of the muzzle flash
        public float muzzleFlashDuration = 0.05f;

        // References to the audio clips for the turret
        public AudioClip barrelSpinUpSound;
        public AudioClip firingInitialSound;
        public AudioClip firingLoopSound;
        public AudioClip firingSpinDownSound;
        public AudioClip barrelSpinDownSound;

        // The sensitivity of the mouse when aiming the turret
        public float sensitivity = 100.0f;

        // The smoothing applied to the movement of the turret when aiming
        public float aimSmoothing = 10.0f;

        // The current and target rotation of the minigun on its pivot stand
        private Vector2 currentRotation;
        private Vector2 targetRotation;
        
        // The current spin speed of the barrel of the turret
        private float currentSpinSpeed;
        
        // A timer to control the firing rate of the turret
        private float fireTimer;
        
        // A flag to indicate if the turret is currently firing
        private bool isFiring;
        
        // References to the audio sources for the turret to make for each audio clip
        private AudioSource barrelSpinUpAudioSource;
        private AudioSource firingInitialAudioSource;
        private AudioSource firingLoopAudioSource;
        private AudioSource firingSpinDownAudioSource;
        private AudioSource barrelSpinDownAudioSource;
        
        // The speed at which the turret will spin up, spin down, and rotate when firing
        private const float MaxSpinSpeed = 2000f;
        private const float SpinUpSpeed = 3000f;
        private const float SpinDownSpeed = 1000f;
        
        // The speed at which the turret will fire
        private const float FireRate = 0.01f;
        
        // The maximum and minimum angles that the gun can aim at
        private const float MaxYAngle = 20f;
        private const float MinYAngle = -10f;
        private const float MaxXAngle = 50f;
        private const float MinXAngle = -50f;

        // The speed at which the bullets will fire
        private const float BulletSpeed = 500f;


        /// <summary>
        /// Sets the initial rotation of the turret, hides the cursor and locks it to the center of the screen,
        /// and sets up the audio. 
        /// </summary>
        private void Start()
        {
            OnEnable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetupAudio();
        }


        /// <summary>
        /// Called every time the object is enabled, resets the rotation variables to a default value.
        /// </summary>
        private void OnEnable()
        {
            targetRotation = new Vector2(0, 0);
            currentRotation = targetRotation;
            transform.localEulerAngles = new Vector3(currentRotation.x, targetRotation.y, 0);
        }


        /// <summary>
        /// Sets up the audio sources with corresponding audio clips and properties for the turret.
        /// </summary>
        private void SetupAudio()
        {
            // Create audio sources
            barrelSpinUpAudioSource = gameObject.AddComponent<AudioSource>();
            firingInitialAudioSource = gameObject.AddComponent<AudioSource>();
            firingLoopAudioSource = gameObject.AddComponent<AudioSource>();
            firingSpinDownAudioSource = gameObject.AddComponent<AudioSource>();
            barrelSpinDownAudioSource = gameObject.AddComponent<AudioSource>();

            // Set audio source clips
            barrelSpinUpAudioSource.clip = barrelSpinUpSound;
            firingInitialAudioSource.clip = firingInitialSound;
            firingLoopAudioSource.clip = firingLoopSound;
            firingSpinDownAudioSource.clip = firingSpinDownSound;
            barrelSpinDownAudioSource.clip = barrelSpinDownSound;

            // Set audio source properties
            barrelSpinUpAudioSource.loop = false;
            firingInitialAudioSource.loop = false;
            firingLoopAudioSource.loop = true;
            firingSpinDownAudioSource.loop = false;
            barrelSpinDownAudioSource.loop = false;

            barrelSpinUpAudioSource.playOnAwake = false;
            firingInitialAudioSource.playOnAwake = false;
            firingLoopAudioSource.playOnAwake = false;
            firingSpinDownAudioSource.playOnAwake = false;
            barrelSpinDownAudioSource.playOnAwake = false;

            barrelSpinUpAudioSource.volume = 0.1f;
            firingInitialAudioSource.volume = 0.1f;
            firingLoopAudioSource.volume = 0.1f;
            firingSpinDownAudioSource.volume = 0.1f;
            barrelSpinDownAudioSource.volume = 0.1f;
        }


        /// <summary>
        /// Called every frame, handles the aiming and firing of the turret.
        /// </summary>
        private void Update()
        {
            // Only handle aiming and firing if the game state is Minigun
            if (GameStateManager.CurrentGameState != GameState.Minigun) return;

            AimTurret();
            HandleSpin();
            HandleFiring();
        }


        /// <summary>
        /// Handles the aiming of the turret.
        /// </summary>
        private void AimTurret()
        {
            // Get mouse movement
            var mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            var mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            targetRotation.y += mouseX;
            targetRotation.x += mouseY;

            // Clamp the rotation
            targetRotation.x = Mathf.Clamp(targetRotation.x, MinYAngle, MaxYAngle);
            targetRotation.y = Mathf.Clamp(targetRotation.y, MinXAngle, MaxXAngle);

            // Smoothly interpolate towards the target rotation
            currentRotation = Vector2.Lerp(currentRotation, targetRotation, aimSmoothing * Time.deltaTime);

            // Apply the rotation
            transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, 0);
        }


        /// <summary>
        /// Handles the spinning animation of the barrel of the turret and the audio for the spinning.
        /// </summary>
        private void HandleSpin()
        {
            // If the player is holding down the left mouse button, spin up the barrel
            if (Input.GetMouseButton(0))
            {
                // Play the barrel spin up audio if it's not already playing
                if (!isFiring && !barrelSpinUpAudioSource.isPlaying)
                {
                    barrelSpinUpAudioSource.Play();

                    // Stop the barrel spin down audio if it's playing
                    if (barrelSpinDownAudioSource.isPlaying)
                    {
                        barrelSpinDownAudioSource.Stop();
                    }
                }

                // Spin up the barrel of the turret and play the firing loop audio
                currentSpinSpeed += SpinUpSpeed * Time.deltaTime;
                if (currentSpinSpeed >= MaxSpinSpeed)
                {
                    currentSpinSpeed = MaxSpinSpeed;

                    if (!firingLoopAudioSource.isPlaying)
                    {
                        firingLoopAudioSource.Play();
                    }

                    isFiring = true;
                }
            }
            else
            {
                // Spin down the barrel of the turret
                currentSpinSpeed -= SpinDownSpeed * Time.deltaTime;
                if (currentSpinSpeed <= 0.0f)
                {
                    currentSpinSpeed = 0.0f;
                }

                // If the gun was firing, play the barrel spin down audio and stop the firing loop audio
                if (isFiring)
                {
                    if (!barrelSpinDownAudioSource.isPlaying && currentSpinSpeed > 0f)
                    {
                        barrelSpinDownAudioSource.Play();
                    }

                    if (firingLoopAudioSource.isPlaying)
                    {
                        firingLoopAudioSource.Stop();
                    }
                }

                // If the gun is not firing and the barrel is not spinning down, stop the barrel spin up audio
                else
                {
                    if (barrelSpinUpAudioSource.isPlaying)
                    {
                        barrelSpinUpAudioSource.Stop();
                    }
                }

                isFiring = false;
            }

            barrel.Rotate(Vector3.forward, currentSpinSpeed * Time.deltaTime);
        }


        /// <summary>
        /// Handles the firing of the turret.
        /// </summary>
        private void HandleFiring()
        {
            // If the turret is not firing, return
            if (!isFiring) return;

            if (fireTimer <= 0f)
            {
                Fire();
                fireTimer = FireRate;
            }

            fireTimer -= Time.deltaTime;
        }


        /// <summary>
        /// Fires a bullet from the turret and plays the muzzle flash particles and light.
        /// </summary>
        private void Fire()
        {
            // Play muzzle flash particles and light
            muzzleFlashParticles.Emit(1);
            StartCoroutine(TurnOnLight());

            // Get a bullet from the pool and set its initial position and rotation
            var bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;

            // Apply a force to the bullet to fire it
            BulletPool.Instance.GetBulletRigidbody(bullet).velocity = -firePoint.forward * BulletSpeed;
        }


        /// <summary>
        /// Subroutine to turn on the muzzle flash light for a short duration.
        /// </summary>
        /// <returns> An IEnumerator to be used as a coroutine. </returns>
        private IEnumerator TurnOnLight()
        {
            muzzleFlashLight.enabled = true;
            yield return new WaitForSeconds(muzzleFlashDuration);
            muzzleFlashLight.enabled = false;
        }
    }
}