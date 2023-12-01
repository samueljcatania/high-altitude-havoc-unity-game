using System;
using UnityEngine;

namespace StealthBomber
{
    public class TurretFiringController : MonoBehaviour
    {
        // The speed at which the turret will spin up, spin down, and rotate when firing
        public float maxSpinSpeed = 2000f;
        public float spinUpSpeed = 500f;
        public float spinDownSpeed = 1000f;
        
        //public GameObject bulletPrefab;
        //public Transform bulletSpawnPoint;
        public float fireRate = 0.1f;

        private float currentSpinSpeed = 0.0f;
        private float fireTimer;
        private bool isFiring = false;


        void Update()
        {
            HandleSpin();
            HandleFiring();
        }

        void HandleSpin()
        {
            if (Input.GetMouseButton(0))
            {
                // Spin up
                currentSpinSpeed += spinUpSpeed * Time.deltaTime;
                if (currentSpinSpeed >= maxSpinSpeed)
                {
                    currentSpinSpeed = maxSpinSpeed;
                    isFiring = true;
                }
            }
            else
            {
                // Spin down
                currentSpinSpeed -= spinDownSpeed * Time.deltaTime;
                if (currentSpinSpeed <= 0.0f)
                {
                    currentSpinSpeed = 0.0f;
                    isFiring = false;
                }
            }

            transform.Rotate(Vector3.forward, currentSpinSpeed * Time.deltaTime);
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

        void Fire()
        {
            // Instantiate bullet and add force, or implement your firing logic here
            //Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        }
    }
}