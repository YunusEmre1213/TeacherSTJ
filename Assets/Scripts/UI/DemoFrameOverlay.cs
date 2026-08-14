using System.Collections;
using UnityEngine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.UI
{
 
    public class DemoFrameOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup frameGroup;
        [SerializeField] private float fadeDuration = 0.25f;

        private Coroutine _fadeRoutine;

        private void Awake()
        {
            frameGroup.alpha = 0f;
            frameGroup.blocksRaycasts = false;
            frameGroup.interactable = false;
        }

        private void OnEnable()
        {
            GameEvents.OnDemoSequenceStarted += ShowFrame;
            GameEvents.OnDemoSequenceFinished += HideFrame;
            GameEvents.OnHintStarted += ShowFrame;
            GameEvents.OnHintFinished += HideFrame;
        }

        private void OnDisable()
        {
            GameEvents.OnDemoSequenceStarted -= ShowFrame;
            GameEvents.OnDemoSequenceFinished -= HideFrame;
            GameEvents.OnHintStarted -= ShowFrame;
            GameEvents.OnHintFinished -= HideFrame;
        }

        private void ShowFrame() => StartFade(1f);
        private void HideFrame() => StartFade(0f);

        private void StartFade(float target)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeRoutine(target));
        }

        private IEnumerator FadeRoutine(float target)
        {
            float start = frameGroup.alpha;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                frameGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
                yield return null;
            }
            frameGroup.alpha = target;
        }
    }
}