using System.Collections;
using UnityEngine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.CameraSystem
{
    public class DemoRenderTarget : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private RenderTexture renderTexture;

        [SerializeField] private float switchDelay = 0.2f;

        private void OnEnable()
        {
            GameEvents.OnDemoSequenceStarted += RequestRenderTexture;
            GameEvents.OnDemoSequenceFinished += RequestScreen;
            GameEvents.OnHintStarted += RequestRenderTexture;
            GameEvents.OnHintFinished += RequestScreen;
        }

        private void OnDisable()
        {
            GameEvents.OnDemoSequenceStarted -= RequestRenderTexture;
            GameEvents.OnDemoSequenceFinished -= RequestScreen;
            GameEvents.OnHintStarted -= RequestRenderTexture;
            GameEvents.OnHintFinished -= RequestScreen;
        }

        private void RequestRenderTexture() => StartCoroutine(SwitchAfterDelay(true));
        private void RequestScreen() => StartCoroutine(SwitchAfterDelay(false));

        private IEnumerator SwitchAfterDelay(bool useTexture)
        {
            yield return new WaitForSeconds(switchDelay);
            mainCamera.targetTexture = useTexture ? renderTexture : null;
        }
    }
}