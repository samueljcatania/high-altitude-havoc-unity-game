using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance;

        public GameObject bulletPrefab;
        public int poolSize = 20;

        private Queue<GameObject> bullets = new Queue<GameObject>();

        void Awake()
        {
            Instance = this;

            // Populate the pool
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab);
                bullet.SetActive(false);
                bullets.Enqueue(bullet);
            }
        }

        public GameObject GetBullet()
        {
            if (bullets.Count > 0)
            {
                GameObject bullet = bullets.Dequeue();
                bullet.SetActive(true);
                return bullet;
            }
            else
            {
                // Optionally expand the pool if empty
                GameObject bullet = Instantiate(bulletPrefab);
                return bullet;
            }
        }

        public void ReturnBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            bullets.Enqueue(bullet);
        }
    }
}