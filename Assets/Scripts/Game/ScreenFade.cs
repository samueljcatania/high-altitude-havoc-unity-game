using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    /// <summary>
    /// This class is used to fade the screen to any color over a specified duration.
    /// </summary>
    public class ScreenFade : MonoBehaviour
    {
        // Reference to the RawImage component
        public RawImage fadeImage;

        // A constant that represents the duration of the fade in seconds only for the start of the game
        private const float FadeDuration = 13f;


        /// <summary>
        /// Call the starting fade from white to clear when the game starts.
        /// </summary>
        private void Start()
        {
            StartCoroutine(FadeScreen(FadeDuration, Color.clear, 3f));
        }


        /// <summary>
        /// Fades the screen to the specified color over the specified duration.
        /// </summary>
        /// <param name="duration"> The duration of the fade in seconds. </param>
        /// <param name="targetColor"> The color to fade to. </param>
        /// <param name="timeToWait"> The time to wait before starting the fade. </param>
        /// <returns> An IEnumerator that can be used to start the fade. </returns>
        public IEnumerator FadeScreen(float duration, Color targetColor, float timeToWait = 0f)
        {
            float timeElapsed = 0;
            var startColor = fadeImage.color;

            yield return new WaitForSeconds(timeToWait);

            while (timeElapsed < duration)
            {
                fadeImage.color = Color.Lerp(startColor, targetColor, timeElapsed / duration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            fadeImage.color = targetColor;
        }
    }
}