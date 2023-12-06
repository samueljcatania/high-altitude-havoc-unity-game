using UnityEngine;

namespace Game
{
    public class Bullet : MonoBehaviour
    {
        public float damage = 1f;
        public float lifetime = 5f;

        private float timeSinceFired;

        
        void OnEnable()
        {
            timeSinceFired = 0f;
        }
        
        void Update()
        {
            timeSinceFired += Time.deltaTime;
            if (timeSinceFired >= lifetime)
            {
                ReturnToPool();
            }
        }

        void ReturnToPool()
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }

        void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            ReturnToPool();
        }
    }
}