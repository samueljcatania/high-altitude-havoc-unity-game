using UnityEngine;

namespace Game
{
    public class Health : MonoBehaviour
    {
        public float maxHealth = 100f;
        // public RectTransform healthBarCanvas; // For the enemy plane
        // public UnityEngine.UI.Image healthBar; // For the enemy plane

        private float currentHealth;

        void Start()
        {
            currentHealth = maxHealth;
        }

        void OnParticleCollision(GameObject other)
        {
            if (other.CompareTag("Bullet")) // Ensure your particle system has the "Bullet" tag
            {
                TakeDamage(10); // Arbitrary damage value, adjust as needed
            }
        }

        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            // UpdateHealthBar();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // private void UpdateHealthBar()
        // {
        //     if (healthBar != null)
        //     {
        //         healthBar.fillAmount = currentHealth / maxHealth;
        //     }
        // }

        private void Die()
        {
            // Handle the death of the plane
            Destroy(gameObject);
        }

        void Update()
        {
            // if (healthBarCanvas != null)
            // {
            //     healthBarCanvas.LookAt(Camera.main.transform);
            // }
        }
    }
}