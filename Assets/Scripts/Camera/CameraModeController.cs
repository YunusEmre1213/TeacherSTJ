using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.CameraSystem
{
    public class CameraModeController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera fpsCamera;
        [SerializeField] private CinemachineCamera demoCamera;

        [SerializeField] private int activePriority = 20;
        [SerializeField] private int inactivePriority = 10;

        [SerializeField] private float switchDelay = 0.2f;

        private void Awake()
        {
            SwitchToPlayerView();
        }

        private void OnEnable()
        {
            GameEvents.OnDemoSequenceStarted += RequestDemoView;
            GameEvents.OnDemoSequenceFinished += RequestPlayerView;
            GameEvents.OnHintStarted += RequestDemoView;
            GameEvents.OnHintFinished += RequestPlayerView;
        }

        private void OnDisable()
        {
            GameEvents.OnDemoSequenceStarted -= RequestDemoView;
            GameEvents.OnDemoSequenceFinished -= RequestPlayerView;
            GameEvents.OnHintStarted -= RequestDemoView;
            GameEvents.OnHintFinished -= RequestPlayerView;
        }

        private void RequestDemoView() => StartCoroutine(SwitchAfterDelay(SwitchToDemoView));
        private void RequestPlayerView() => StartCoroutine(SwitchAfterDelay(SwitchToPlayerView));

        private IEnumerator SwitchAfterDelay(System.Action switchAction)
        {
            yield return new WaitForSeconds(switchDelay);
            switchAction();
        }

        [ContextMenu("Demo Görünümüne Geç")]
        public void SwitchToDemoView()
        {
            demoCamera.Priority = activePriority;
            fpsCamera.Priority = inactivePriority;
        }

        [ContextMenu("Oyuncu Görünümüne Geç")]
        public void SwitchToPlayerView()
        {
            fpsCamera.Priority = activePriority;
            demoCamera.Priority = inactivePriority;
        }
    }
}