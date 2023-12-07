using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// This class is responsible for managing the bullet pool in the game. It pre-instantiates a number of bullets
    /// to be used in the game and provides methods to get and return bullets to the pool. It utilizes a singleton
    /// design pattern to ensure that only one instance of the bullet pool exists in the game.
    /// </summary>
    public class BulletPool : MonoBehaviour
    {
        // An Instance variable to enforce a singleton pattern
        public static BulletPool Instance;

        // A reference to the bullet prefab
        public GameObject bulletPrefab;
        
        // The number of bullets to create in the pool
        private const int PoolSize = 100;

        // A queue to store the bullets in the pool
        private readonly Queue<GameObject> bullets = new();

        // A dictionary to cache the rigidbodies of the bullets in the pool
        private readonly Dictionary<GameObject, Rigidbody> bulletRigidbodies = new();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;

            // Populate the pool
            for (var i = 0; i < PoolSize; i++)
            {
                var bullet = Instantiate(bulletPrefab);
                bullet.SetActive(false);
                bullets.Enqueue(bullet);
                
                bulletRigidbodies.Add(bullet, bullet.GetComponent<Rigidbody>());
            }
        }

        
        /// <summary>
        /// Returns a bullet from the pool. If the pool is empty, it will expand the pool by instantiating a new bullet.
        /// </summary>
        /// <returns> A bullet from the pool </returns>
        public GameObject GetBullet()
        {
            if (bullets.Count > 0)
            {
                var bullet = bullets.Dequeue();
                bullet.SetActive(true);
                return bullet;
            }
            else
            {
                // Expand the pool if it is empty
                var bullet = Instantiate(bulletPrefab);
                return bullet;
            }
        }

        
        /// <summary>
        /// Returns a bullet to the pool.
        /// </summary>
        /// <param name="bullet"> The bullet to return to the pool. </param>
        public void ReturnBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            bullets.Enqueue(bullet);
        }
        
        
        /// <summary>
        /// Returns the rigidbody of a given bullet.
        /// </summary>
        /// <param name="bullet"> The bullet to get the rigidbody of. </param>
        /// <returns> The rigidbody of the given bullet. </returns>
        public Rigidbody GetBulletRigidbody(GameObject bullet)
        {
            return bulletRigidbodies[bullet];
        }
    }
}