using UnityEngine;

namespace Game
{
    /// <summary>
    /// This class is used to manage the bullet game objects that make up the bullet pool. It is responsible for returning
    /// bullets to the pool when they collide with an object or when they exceed their lifetime.
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        // The amount of time that has passed since the bullet was fired
        private float timeSinceFired;

        // The amount of health to take away from an object when they collide with a bullet
        private const float Damage = 1f;

        // The amount of time a bullet can exist before it is returned to the pool
        private const float Lifetime = 4f;


        /// <summary>
        /// When the bullet is enabled, reset the time since it was fired.
        /// </summary>
        private void OnEnable()
        {
            timeSinceFired = 0f;
        }


        /// <summary>
        /// When the bullet is disabled, return it to the pool.
        /// </summary>
        private void Update()
        {
            timeSinceFired += Time.deltaTime;
            if (timeSinceFired >= Lifetime)
            {
                ReturnToPool();
            }
        }


        /// <summary>
        /// Returns the bullet to the pool.
        /// </summary>
        private void ReturnToPool()
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }


        /// <summary>
        /// When the bullet collides with an object, take away health from the object and return the bullet to the pool.
        /// </summary>
        /// <param name="other"> The object that the bullet collided with. </param>
        private void OnTriggerEnter(Collider other)
        {
            var health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(Damage);
            }

            ReturnToPool();
        }
    }
}