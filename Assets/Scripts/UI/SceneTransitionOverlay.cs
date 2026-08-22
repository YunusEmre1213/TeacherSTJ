using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.UI
{
    public class SceneTransitionOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup overlayGroup;

        private void Awake()
        {
            overlayGroup.alpha = 0f;
            overlayGroup.blocksRaycasts = false;
        }

        public IEnumerator FadeToBlackRoutine(float duration)
        {
            yield return Fade(overlayGroup.alpha, 1f, duration);
        }

        public IEnumerator FadeFromBlackRoutine(float duration)
        {
            yield return Fade(overlayGroup.alpha, 0f, duration);
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            float elapsed = 0f;
            overlayGroup.alpha = from;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                overlayGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            overlayGroup.alpha = to;
        }
    }
}