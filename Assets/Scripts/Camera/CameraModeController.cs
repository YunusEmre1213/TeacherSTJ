using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.CameraSystem
{
    public class CameraModeController : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera fpsCamera;

        [Tooltip("Oyuncu kontrolündeyken FPS kamerasýnýn önceliði — DemoCameraDirector'ýn en yüksek (aktif) önceliðinden kesinlikle daha büyük olmalý.")]
        [SerializeField] private int playerModePriority = 30;

        [Tooltip("Demo/hint sýrasýnda FPS kamerasýnýn önceliði — DemoCameraDirector'ýn en düþük (pasif) önceliðinden kesinlikle daha küçük olmalý, ki hiçbir demo kamerasýyla yarýþmasýn.")]
        [SerializeField] private int demoModePriority = 5;

        [Tooltip("Gerçek kamera geçiþinin ertelenme süresi — DemoFrame'in karartma fade süresine yakýn tutulmalý.")]
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
            fpsCamera.Priority = demoModePriority;
        }

        [ContextMenu("Oyuncu Görünümüne Geç")]
        public void SwitchToPlayerView()
        {
            fpsCamera.Priority = playerModePriority;
        }
    }
}