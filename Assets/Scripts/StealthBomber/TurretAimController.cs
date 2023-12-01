using UnityEngine;

namespace StealthBomber
{
    public class TurretAimController : MonoBehaviour
    {
        public Camera gameCamera;
        public float sensitivity = 100.0f;
        public float aimSmoothing = 10.0f;

        public AudioClip barrelSpinUpSound;
        public AudioClip firingInitialSound;
        public AudioClip firingLoopSound;
        public AudioClip barrelSpinningLoopSound;
        public AudioClip firingSpinDownSound;
        public AudioClip barrelSpinDownSound;

        private AudioSource _barrelSpinUpAudioSource;
        private AudioSource _firingInitialAudioSource;
        private AudioSource _firingLoopAudioSource;
        private AudioSource _barrelSpinningLoopAudioSource;
        private AudioSource _firingSpinDownAudioSource;
        private AudioSource _barrelSpinDownAudioSource;

        private float currentSpinSpeed = 0.0f;
        private bool isFiring = false;

        private bool isHoldingFire = false;

        // The maximum and minimum angles that the gun can aim at
        private const float MaxYAngle = 30f;
        private const float MinYAngle = -10f;
        private const float MaxXAngle = 50f;
        private const float MinXAngle = -50f;

        private Vector2 currentRotation;
        private Vector2 targetRotation;

        public float maxSpinSpeed = 2000f;
        public float spinUpSpeed = 1200f;
        public float spinDownSpeed = 1000f;

        public float fireRate = 0.1f;
        private float fireTimer;

        public Transform barrel;


        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentRotation = transform.localEulerAngles;
            targetRotation = transform.localEulerAngles;

            // Create audio sources
            _barrelSpinUpAudioSource = gameObject.AddComponent<AudioSource>();
            _firingInitialAudioSource = gameObject.AddComponent<AudioSource>();
            _firingLoopAudioSource = gameObject.AddComponent<AudioSource>();
            _barrelSpinningLoopAudioSource = gameObject.AddComponent<AudioSource>();
            _firingSpinDownAudioSource = gameObject.AddComponent<AudioSource>();
            _barrelSpinDownAudioSource = gameObject.AddComponent<AudioSource>();

            // Set audio source clips
            _barrelSpinUpAudioSource.clip = barrelSpinUpSound;
            _firingInitialAudioSource.clip = firingInitialSound;
            _firingLoopAudioSource.clip = firingLoopSound;
            _barrelSpinningLoopAudioSource.clip = barrelSpinningLoopSound;
            _firingSpinDownAudioSource.clip = firingSpinDownSound;
            _barrelSpinDownAudioSource.clip = barrelSpinDownSound;

            // Set audio source properties
            _barrelSpinUpAudioSource.loop = false;
            _firingInitialAudioSource.loop = false;
            _firingLoopAudioSource.loop = true;
            _barrelSpinningLoopAudioSource.loop = true;
            _firingSpinDownAudioSource.loop = false;
            _barrelSpinDownAudioSource.loop = false;

            _barrelSpinUpAudioSource.playOnAwake = false;
            _firingInitialAudioSource.playOnAwake = false;
            _firingLoopAudioSource.playOnAwake = false;
            _barrelSpinningLoopAudioSource.playOnAwake = false;
            _firingSpinDownAudioSource.playOnAwake = false;
            _barrelSpinDownAudioSource.playOnAwake = false;

            _barrelSpinUpAudioSource.volume = 0.1f;
            _firingInitialAudioSource.volume = 0.1f;
            _firingLoopAudioSource.volume = 0.0f;
            _barrelSpinningLoopAudioSource.volume = 0.1f;
            _firingSpinDownAudioSource.volume = 0.1f;
            _barrelSpinDownAudioSource.volume = 0.1f;
        }

        private void Update()
        {
            AimTurret();
            HandleSpin();
            HandleFiring();
        }


        void Fire()
        {
            // Instantiate bullet and add force, or implement your firing logic here
            //Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }

        private void AimTurret()
        {
            // Get mouse movement
            float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

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

        private void HandleSpin()
        {
            if (Input.GetMouseButton(0))
            {
                if (!isFiring && !_barrelSpinUpAudioSource.isPlaying && !isHoldingFire)
                {
                    // Get the percentage of the current spin speed to the max spin speed and use that to set the playback position
                    var time = Mathf.Clamp01(1.0f - currentSpinSpeed / maxSpinSpeed) *
                        _barrelSpinUpAudioSource.clip.length - 0.01f;
                    _barrelSpinUpAudioSource.time = time;
                    _barrelSpinUpAudioSource.Play();

                    if (_barrelSpinDownAudioSource.isPlaying)
                    {
                        _barrelSpinDownAudioSource.Stop();
                    }
                }

                // Spin up
                currentSpinSpeed += spinUpSpeed * Time.deltaTime;
                if (currentSpinSpeed >= maxSpinSpeed)
                {
                    currentSpinSpeed = maxSpinSpeed;
                    //_firingInitialAudioSource.Play();

                    if (!_firingLoopAudioSource.isPlaying)
                    {
                        _firingLoopAudioSource.Play();
                    }

                    if (!_barrelSpinningLoopAudioSource.isPlaying)
                    {
                        Debug.Log("Playing barrel spinning loop");
                        _barrelSpinningLoopAudioSource.Play();
                    }

                    isFiring = true;
                }

                isHoldingFire = true;
            }
            else
            {
                // Spin down
                currentSpinSpeed -= spinDownSpeed * Time.deltaTime;
                if (currentSpinSpeed <= 0.0f)
                {
                    currentSpinSpeed = 0.0f;
                }

                if (isFiring)
                {
                    if (_firingLoopAudioSource.isPlaying)
                    {
                        _firingLoopAudioSource.Stop();
                    }

                    if (_barrelSpinningLoopAudioSource.isPlaying)
                    {
                        _barrelSpinningLoopAudioSource.Stop();
                    }
                    //
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

                    if (!_barrelSpinDownAudioSource.isPlaying && !_firingSpinDownAudioSource.isPlaying &&
                        currentSpinSpeed > 0.0f && isHoldingFire)
                    {
                        // Get the percentage of the current spin speed to the max spin speed and use that to set the playback position
                        var time = Mathf.Clamp01(1.0f - currentSpinSpeed / maxSpinSpeed) *
                            _barrelSpinDownAudioSource.clip.length - 0.01f;
                        _barrelSpinDownAudioSource.time = time;
                        _barrelSpinDownAudioSource.Play();
                    }
                }

                isHoldingFire = false;
                isFiring = false;
            }

            barrel.Rotate(Vector3.forward, currentSpinSpeed * Time.deltaTime);
        }

        void HandleFiring()
        {
            if (isFiring)
            {
                if (fireTimer <= 0f)
                {
                    Fire();
                    fireTimer = fireRate;
                }

                fireTimer -= Time.deltaTime;
            }
        }
    }
}