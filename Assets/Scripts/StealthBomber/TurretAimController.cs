using System.Collections;
using Game;
using UnityEngine;

namespace StealthBomber
{
    public class TurretAimController : MonoBehaviour
    {
        // Reference to the barrel of the turret to control its spin
        public Transform barrel;

        public Light muzzleFlashLight;
        public ParticleSystem muzzleFlashParticles;
        public float muzzleFlashDuration = 0.05f;

        // References to the audio clips for the turret
        public AudioClip barrelSpinUpSound;
        public AudioClip firingInitialSound;
        public AudioClip firingLoopSound;
        public AudioClip firingSpinDownSound;
        public AudioClip barrelSpinDownSound;

        // The speed at which the turret will spin up, spin down, and rotate when firing
        public float maxSpinSpeed = 2000f;
        public float spinUpSpeed = 1200f;
        public float spinDownSpeed = 1000f;

        // The speed at which the turret will fire
        public float fireRate = 0.01f;

        // References to the audio sources for the turret to make for each audio clip
        private AudioSource _barrelSpinUpAudioSource;
        private AudioSource _firingInitialAudioSource;
        private AudioSource _firingLoopAudioSource;
        private AudioSource _firingSpinDownAudioSource;
        private AudioSource _barrelSpinDownAudioSource;

        // The sensitivity of the mouse when aiming the turret
        public float sensitivity = 100.0f;

        // The smoothing applied to the movement of the turret when aiming
        public float aimSmoothing = 10.0f;

        public Transform firePoint;

        // The maximum and minimum angles that the gun can aim at
        private const float MaxYAngle = 20f;
        private const float MinYAngle = -10f;
        private const float MaxXAngle = 50f;
        private const float MinXAngle = -50f;

        public float bulletSpeed = 2000f;


        private float _currentSpinSpeed = 0.0f;
        private bool _isFiring = false;

        private Vector2 _currentRotation;
        private Vector2 _targetRotation;

        private float _fireTimer;


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
            _targetRotation = new Vector2(0, 0);
            _currentRotation = _targetRotation;
            transform.localEulerAngles = new Vector3(_currentRotation.x, _targetRotation.y, 0);
        }


        /// <summary>
        /// Sets up the audio sources with corresponding audio clips and properties for the turret.
        /// </summary>
        private void SetupAudio()
        {
            // Create audio sources
            _barrelSpinUpAudioSource = gameObject.AddComponent<AudioSource>();
            _firingInitialAudioSource = gameObject.AddComponent<AudioSource>();
            _firingLoopAudioSource = gameObject.AddComponent<AudioSource>();
            _firingSpinDownAudioSource = gameObject.AddComponent<AudioSource>();
            _barrelSpinDownAudioSource = gameObject.AddComponent<AudioSource>();

            // Set audio source clips
            _barrelSpinUpAudioSource.clip = barrelSpinUpSound;
            _firingInitialAudioSource.clip = firingInitialSound;
            _firingLoopAudioSource.clip = firingLoopSound;
            _firingSpinDownAudioSource.clip = firingSpinDownSound;
            _barrelSpinDownAudioSource.clip = barrelSpinDownSound;

            // Set audio source properties
            _barrelSpinUpAudioSource.loop = false;
            _firingInitialAudioSource.loop = false;
            _firingLoopAudioSource.loop = true;
            _firingSpinDownAudioSource.loop = false;
            _barrelSpinDownAudioSource.loop = false;

            _barrelSpinUpAudioSource.playOnAwake = false;
            _firingInitialAudioSource.playOnAwake = false;
            _firingLoopAudioSource.playOnAwake = false;
            _firingSpinDownAudioSource.playOnAwake = false;
            _barrelSpinDownAudioSource.playOnAwake = false;

            _barrelSpinUpAudioSource.volume = 0.1f;
            _firingInitialAudioSource.volume = 0.1f;
            _firingLoopAudioSource.volume = 0.1f;
            _firingSpinDownAudioSource.volume = 0.1f;
            _barrelSpinDownAudioSource.volume = 0.1f;
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

            _targetRotation.y += mouseX;
            _targetRotation.x += mouseY;

            // Clamp the rotation
            _targetRotation.x = Mathf.Clamp(_targetRotation.x, MinYAngle, MaxYAngle);
            _targetRotation.y = Mathf.Clamp(_targetRotation.y, MinXAngle, MaxXAngle);

            // Smoothly interpolate towards the target rotation
            _currentRotation = Vector2.Lerp(_currentRotation, _targetRotation, aimSmoothing * Time.deltaTime);

            // Apply the rotation
            transform.localEulerAngles = new Vector3(_currentRotation.x, _currentRotation.y, 0);
        }


        /// <summary>
        /// Handles the spinning animation of the barrel of the turret and the audio for the spinning.
        /// </summary>
        private void HandleSpin()
        {
            if (Input.GetMouseButton(0))
            {
                if (!_isFiring && !_barrelSpinUpAudioSource.isPlaying)
                {
                    // // Get the percentage of the current spin speed to the max spin speed and use that to set the playback position
                    // var time = currentSpinSpeed / maxSpinSpeed * _barrelSpinUpAudioSource.clip.length - 0.01f;
                    // _barrelSpinUpAudioSource.time = time;
                    // Debug.Log("Playing barrel spin up" + time);
                    _barrelSpinUpAudioSource.Play();

                    if (_barrelSpinDownAudioSource.isPlaying)
                    {
                        _barrelSpinDownAudioSource.Stop();
                    }
                }

                // Spin up
                _currentSpinSpeed += spinUpSpeed * Time.deltaTime;
                if (_currentSpinSpeed >= maxSpinSpeed)
                {
                    _currentSpinSpeed = maxSpinSpeed;
                    //_firingInitialAudioSource.Play();

                    if (!_firingLoopAudioSource.isPlaying)
                    {
                        _firingLoopAudioSource.Play();
                    }

                    _isFiring = true;
                }
            }
            else
            {
                // Spin down
                _currentSpinSpeed -= spinDownSpeed * Time.deltaTime;
                if (_currentSpinSpeed <= 0.0f)
                {
                    _currentSpinSpeed = 0.0f;
                }

                if (_isFiring)
                {
                    if (!_barrelSpinDownAudioSource.isPlaying && _currentSpinSpeed > 0f)
                    {
                        _barrelSpinDownAudioSource.Play();
                    }

                    if (_firingLoopAudioSource.isPlaying)
                    {
                        _firingLoopAudioSource.Stop();
                    }

                    // float playbackPosition =
                    //     Mathf.Clamp(currentSpinSpeed / 2.0f, 0, _firingSpinDownAudioSource.clip.length);
                    // _firingSpinDownAudioSource.time = playbackPosition;
                    // _firingSpinDownAudioSource.Play();
                }

                else
                {
                    if (_barrelSpinUpAudioSource.isPlaying)
                    {
                        _barrelSpinUpAudioSource.Stop();
                    }
                }

                _isFiring = false;
            }

            barrel.Rotate(Vector3.forward, _currentSpinSpeed * Time.deltaTime);
        }

        void HandleFiring()
        {
            if (_isFiring)
            {
                if (_fireTimer <= 0f)
                {
                    Fire();
                    _fireTimer = fireRate;
                }

                _fireTimer -= Time.deltaTime;
            }
        }


        void Fire()
        {
            muzzleFlashParticles.Emit(1);
            StartCoroutine(TurnOnLight());
            GameObject bullet = BulletPool.Instance.GetBullet();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = -firePoint.forward * bulletSpeed;
        }

        private IEnumerator TurnOnLight()
        {
            muzzleFlashLight.enabled = true;
            yield return new WaitForSeconds(muzzleFlashDuration);
            muzzleFlashLight.enabled = false;
        }
    }
}