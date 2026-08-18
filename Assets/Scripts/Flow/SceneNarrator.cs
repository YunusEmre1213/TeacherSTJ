using System.Collections;
using UnityEngine;
using OgretmenGorevSistemi.Core;
using OgretmenGorevSistemi.Character;
using OgretmenGorevSistemi.Dialogue;

namespace OgretmenGorevSistemi.Flow
{
    public class SceneNarrator : MonoBehaviour
    {
        [Header("Karakterlerin sesleri")]
        [SerializeField] private TeacherVoice ablaVoice;
        [SerializeField] private TeacherVoice anneVoice;

        [Header("Replikler")]
        [SerializeField] private AudioClip ablaPastaYiyecegim;
        [SerializeField] private AudioClip anneAhmet;
        [SerializeField] private AudioClip anneSunaBak;
        [SerializeField] private AudioClip annePastayiYiyecek;
        [SerializeField] private AudioClip annePastaniBegendinMi; // þimdilik kullanýlmýyor

        [Header("Anne'nin dönüþleri")]
        [SerializeField] private TurnToFaceOnCue anneTurn;
        [SerializeField] private Transform childTransform;
        [SerializeField] private Transform ablaTransform;

        [Header("Abla'nýn yürüyüþü")]
        [SerializeField] private WalkToTargetOnCue ablaWalk;

        [Header("Kamera")]
        [SerializeField] private OgretmenGorevSistemi.CameraSystem.DemoCameraDirector cameraDirector;
        [Tooltip("Kamera kesmesinden sonra, sýradaki eylem (ses/dönüþ/yürüyüþ) baþlamadan önce beklenecek süre — Cinemachine Custom Blends'teki Time deðeriyle ayný olmalý.")]
        [SerializeField] private float cameraBlendDuration = 1.2f;

        [Header("Zamanlama")]
        [Tooltip("Demo baþlar baþlamaz, Abla konuþmadan önce beklenecek süre (kamera odaklanmasý için yer tutucu).")]
        [SerializeField] private float introPause = 2f;
        [Tooltip("Her replik/dönüþ biriminin bitiminden sonra beklenecek süre.")]
        [SerializeField] private float pauseBetweenBeats = 2f;

        [Tooltip("Steps listesinde 'Dur Ýþareti' adýmýnýn sýrasý (0'dan sayarak) — Abla bu adým bitince duracak.")]
        [SerializeField] private int durIsaretiStepIndex = 4;

        private bool _isDemoMode;

        private void OnEnable()
        {
            GameEvents.OnDemoSequenceStarted += HandleDemoStarted;
            GameEvents.OnPlayerConfirmedReady += HandlePlayerAttemptStarted;
            GameEvents.OnTaskStepCompleted += HandleStepCompleted;
        }

        private void OnDisable()
        {
            GameEvents.OnDemoSequenceStarted -= HandleDemoStarted;
            GameEvents.OnPlayerConfirmedReady -= HandlePlayerAttemptStarted;
            GameEvents.OnTaskStepCompleted -= HandleStepCompleted;
        }

        private void HandleDemoStarted()
        {
            _isDemoMode = true;
            StartCoroutine(BlockedRoutine(OpeningRoutine()));
        }

        private void HandlePlayerAttemptStarted()
        {
            _isDemoMode = false;
        }

        private void HandleStepCompleted(int stepIndex)
        {
            if (!_isDemoMode) return;

            if (stepIndex == 0) // Annene bak (1.) bitti — Anne Abla'ya dönüp "þuna bak" desin,
                                // Abla yürümeyi bunun ÝÇÝNDE, sözle senkron baþlatýyor
            {
                StartCoroutine(BlockedRoutine(SunaBakRoutine()));
            }
            else if (stepIndex == 1) // Ablaya bak (1.) bitti — Anne çocuða dönüp anlatsýn
            {
                StartCoroutine(BlockedRoutine(PastayiYiyecekRoutine()));
            }
            else if (stepIndex == durIsaretiStepIndex) // Dur iþareti bitti — Abla dursun
            {
                if (ablaWalk != null) ablaWalk.StopWalking();
            }

        }
        private IEnumerator BlockedRoutine(IEnumerator inner)
        {
            GameEvents.RaiseDemoBlocked();
            yield return inner;
            GameEvents.RaiseDemoUnblocked();
        }

        private IEnumerator OpeningRoutine()
        {
            if (cameraDirector != null) cameraDirector.ShowWide();
            yield return new WaitForSeconds(introPause);

            yield return CutAndWait(cameraDirector != null ? (System.Action)cameraDirector.ShowAbla : null);
            if (ablaVoice != null)
                yield return ablaVoice.PlayAndWaitRoutine(ablaPastaYiyecegim);
            yield return new WaitForSeconds(pauseBetweenBeats);

            yield return CutAndWait(cameraDirector != null ? (System.Action)cameraDirector.ShowAnneChild : null);
            if (anneTurn != null && childTransform != null)
                yield return anneTurn.TurnToFaceRoutine(childTransform);
            if (anneVoice != null)
                yield return anneVoice.PlayAndWaitRoutine(anneAhmet);
            yield return new WaitForSeconds(pauseBetweenBeats);
        }

        private IEnumerator SunaBakRoutine()
        {

            if (anneTurn != null && ablaTransform != null)
                yield return anneTurn.TurnToFaceRoutine(ablaTransform);

            if (anneVoice != null)
                yield return anneVoice.PlayAndWaitRoutine(anneSunaBak);

            yield return CutAndWait(cameraDirector != null ? (System.Action)cameraDirector.ShowAbla : null);
            if (ablaWalk != null) ablaWalk.StartWalking();

            yield return new WaitForSeconds(pauseBetweenBeats);
        }

        private IEnumerator PastayiYiyecekRoutine()
        {
            yield return CutAndWait(cameraDirector != null ? (System.Action)cameraDirector.ShowAnneChild : null);
            if (anneTurn != null && childTransform != null)
                yield return anneTurn.TurnToFaceRoutine(childTransform);
            if (anneVoice != null)
                yield return anneVoice.PlayAndWaitRoutine(annePastayiYiyecek);
            yield return new WaitForSeconds(pauseBetweenBeats);
        }
        private IEnumerator CutAndWait(System.Action cut)
        {
            cut?.Invoke();
            if (cut != null)
                yield return new WaitForSeconds(cameraBlendDuration);
        }
    }
}