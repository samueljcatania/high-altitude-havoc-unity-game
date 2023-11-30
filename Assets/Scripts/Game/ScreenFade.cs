using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class ScreenFade : MonoBehaviour
    {
        public RawImage fadeImage;
        public float fadeDuration = 10f;

        private void Start()
        {
            if (fadeImage != null)
            {
                StartCoroutine(FadeScreen());
            }
        }

        private IEnumerator FadeScreen()
        {
            float elapsedTime = 0;
            
            yield return new WaitForSeconds(2f);

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = 1.0f - (elapsedTime / fadeDuration);
                fadeImage.color = new Color(1, 1, 1, alpha);
                yield return null;
            }

            fadeImage.gameObject.SetActive(false);
        }
    }
}