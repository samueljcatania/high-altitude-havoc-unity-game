using System.Globalization;
using TMPro;
using UnityEngine;

namespace Menu
{
    /// <summary>
    /// This class is used to display the FPS counter in the top left corner of the screen, only if the player has enabled it.
    /// </summary>
    public class FPSCounter : MonoBehaviour
    {
        // Reference to the TextMeshProUGUI text component
        public TextMeshProUGUI fpsDisplay;

        // Tracks the average FPS, which is a smoother value than the current FPS
        private float _averageFPS = 0f;

        /*
         * Update the FPS counter every frame if it is enabled
         */
        private void Update()
        {
            // Check if the player has enabled the FPS counter
            if (PlayerPrefs.GetInt("FPS") != 1)
            {
                fpsDisplay.text = "";
                return;
            }

            // Calculate the average FPS and display it
            _averageFPS += (Time.unscaledDeltaTime - _averageFPS) * 0.1f;
            fpsDisplay.text = Mathf.Ceil(1f / _averageFPS).ToString(CultureInfo.InvariantCulture);
        }
    
    }
}