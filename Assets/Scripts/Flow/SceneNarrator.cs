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
        [SerializeField] private AudioClip annePastaniBegendinMi;

        [Header("Masaya geçiþ")]
        [SerializeField] private OgretmenGorevSistemi.UI.SceneTransitionOverlay transitionOverlay;
        [SerializeField] private float fadeDuration = 2f;
        [Tooltip("Ekran tam karardýktan sonra, karakterler ýþýnlanýp otururken beklenecek süre — bu süre boyunca hiçbir þey görünmüyor.")]
        [SerializeField] private float blackHoldDuration = 2f;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform playerSeat;
        [SerializeField] private Transform anneSeat;
        [SerializeField] private Transform ablaSeat;

        [Header("Anne'nin dönüþleri")]
        [SerializeField] private TurnToFaceOnCue anneTurn;
        [SerializeField] private Transform anneTransform;
        [SerializeField] private Transform childTransform;
        [SerializeField] private Transform ablaTransform;
        [Tooltip("Anne 'þuna bak' derken kolunu kaldýrýp Abla'yý iþaret ettiði IK bileþeni.")]
        [SerializeField] private HandPointAt anneArmPoint;

        [Header("Abla'nýn yürüyüþü")]
        [SerializeField] private WalkToTargetOnCue ablaWalk;
        [Tooltip("Oyuncunun kendi sýrasýnda Abla'nýn önce yürüyeceði YAKIN nokta — oraya varýnca kendiliðinden durur, bekler. 'Dur iþareti' tamamlanýnca gerçek hedefe (WalkToTargetOnCue'daki asýl Target'a) yönlendirilir.")]
        [SerializeField] private Transform ablaIntermediateTarget;

        [Header("Kamera")]
        [SerializeField] private OgretmenGorevSistemi.CameraSystem.DemoCameraDirector cameraDirector;
        [SerializeField] private OgretmenGorevSistemi.Tasks.TaskManager taskManager;
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
        private Vector3 _anneStartPosition;
        private Quaternion _anneStartRotation;
        private Vector3 _ablaStartPosition;
        private Quaternion _ablaStartRotation;
        private Transform _ablaFinalTarget;

        private void Awake()
        {
            if (anneTransform != null)
            {
                _anneStartPosition = anneTransform.position;
                _anneStartRotation = anneTransform.rotation;
            }
            if (ablaTransform != null)
            {
                _ablaStartPosition = ablaTransform.position;
                _ablaStartRotation = ablaTransform.rotation;
            }
            if (ablaWalk != null) _ablaFinalTarget = ablaWalk.Target;
        }

        private void OnEnable()
        {
            GameEvents.OnDemoSequenceStarted += HandleDemoStarted;
            GameEvents.OnPlayerConfirmedReady += HandlePlayerAttemptStarted;
            GameEvents.OnTaskStepCompleted += HandleStepCompleted;
            GameEvents.OnHintStarted += HandleHintStarted;
            GameEvents.OnHintFinished += HandleHintFinished;
        }

        private void OnDisable()
        {
            GameEvents.OnDemoSequenceStarted -= HandleDemoStarted;
            GameEvents.OnPlayerConfirmedReady -= HandlePlayerAttemptStarted;
            GameEvents.OnTaskStepCompleted -= HandleStepCompleted;
            GameEvents.OnHintStarted -= HandleHintStarted;
            GameEvents.OnHintFinished -= HandleHintFinished;
        }

        private void HandleHintStarted()
        {
            if (cameraDirector == null || taskManager == null) return;

            int currentIndex = taskManager.CurrentStepIndex;
            Transform target = taskManager.CurrentStep?.target;
            if (target == null) return;

            if (currentIndex == durIsaretiStepIndex + 1)
            {
                cameraDirector.ShowTable();
                return;
            }

            if (ablaTransform != null && target.IsChildOf(ablaTransform))
            {
                cameraDirector.ShowAblaChild();
                if (currentIndex == durIsaretiStepIndex)
                    StartCoroutine(ResetAblaToIntermediateAndWalk());
            }
            else if (anneTransform != null && target.IsChildOf(anneTransform))
            {
                cameraDirector.ShowAnneChild();
            }
        }

        private void HandleHintFinished()
        {
            if (taskManager == null || taskManager.CurrentStepIndex != durIsaretiStepIndex) return;
            StartCoroutine(ResetAblaToIntermediateAndWalk());
        }
        private IEnumerator ResetAblaToIntermediateAndWalk()
        {
            if (_isDemoMode) yield break;
            if (ablaWalk == null || ablaIntermediateTarget == null || _ablaFinalTarget == null || ablaTransform == null) yield break;

            ablaTransform.position = ablaIntermediateTarget.position;
            ablaTransform.rotation = ablaIntermediateTarget.rotation;

            yield return new WaitForSeconds(0.6f);

            ablaWalk.SetTarget(_ablaFinalTarget);
            ablaWalk.StartWalking();
        }

        private void HandleDemoStarted()
        {
            _isDemoMode = true;
            StartCoroutine(BlockedRoutine(OpeningRoutine()));
        }

        private void HandlePlayerAttemptStarted()
        {
            _isDemoMode = false;
            ResetToStartPositions();
            ResetAnimatorToIdle(playerTransform);
            if (cameraDirector != null) cameraDirector.ShowWide();

            StartCoroutine(BlockedRoutine(OpeningRoutine()));
        }

        private void ResetToStartPositions()
        {
            if (anneTransform != null)
            {
                anneTransform.position = _anneStartPosition;
                anneTransform.rotation = _anneStartRotation;
                ResetAnimatorToIdle(anneTransform);
            }
            if (ablaTransform != null)
            {
                ablaTransform.position = _ablaStartPosition;
                ablaTransform.rotation = _ablaStartRotation;
                ResetAnimatorToIdle(ablaTransform);
            }
        }

        private void ResetAnimatorToIdle(Transform character)
        {
            if (character == null) return;
            Animator animator = character.GetComponentInChildren<Animator>();
            if (animator == null) return;

            if (HasParameter(animator, "IsSitting")) animator.SetBool("IsSitting", false);
            if (HasParameter(animator, "IsMoving")) animator.SetBool("IsMoving", false);
        }

        private bool HasParameter(Animator animator, string paramName)
        {
            foreach (var p in animator.parameters)
                if (p.name == paramName) return true;
            return false;
        }

        // stepIndex, TAMAMLANAN adýmýn index'i.
        private void HandleStepCompleted(int stepIndex)
        {

            if (stepIndex == 0) // Annene bak (1.) bitti — Anne Abla'ya dönüp "þuna bak" desin,
                                // Abla henüz yürümesin, sadece iþaret edilsin
            {
                StartCoroutine(BlockedRoutine(SunaBakRoutine()));
            }
            else if (stepIndex == 1) // Ablaya bak (1.) bitti — TAM BU ANDA Abla
                                     // yakýn ara noktaya yürümeye BAÞLASIN, Anne çocuða dönüp anlatsýn
            {
                StartCoroutine(BlockedRoutine(PastayiYiyecekRoutine()));
            }
            else if (stepIndex == durIsaretiStepIndex - 1) // Ablaya bak (2.) bitti — Dur Ýþareti
                                                           // görevi BAÞLIYOR, Abla TAM BU ANDA
                                                           // gerçek hedefe doðru yeniden yürümeye
                                                           // baþlasýn (yetiþemezse son noktada
                                                           // kendiliðinden dursun, hint gelince
                                                           // zaten sýfýrlanacak)
            {
                if (!_isDemoMode && ablaWalk != null && _ablaFinalTarget != null)
                {
                    ablaWalk.SetTarget(_ablaFinalTarget);
                    ablaWalk.StartWalking();
                }
            }
            else if (stepIndex == durIsaretiStepIndex) // Dur iþareti bitti — Abla TAM BU ANDA,
                                                       // gerçekten hareket halindeyken dursun
            {
                if (ablaWalk != null) ablaWalk.StopWalking();
                StartCoroutine(BlockedRoutine(TableTransitionRoutine()));
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
            if (anneArmPoint != null && ablaTransform != null)
                anneArmPoint.SetTarget(ablaTransform);

            if (anneVoice != null)
                yield return anneVoice.PlayAndWaitRoutine(anneSunaBak);

            
            if (anneArmPoint != null)
                anneArmPoint.SetTarget(null);
            yield return CutAndWait(cameraDirector != null ? (System.Action)cameraDirector.ShowAblaChild : null);

            yield return new WaitForSeconds(pauseBetweenBeats);
        }

        private IEnumerator PastayiYiyecekRoutine()
        {
            if (ablaWalk != null)
            {
                if (_isDemoMode)
                {
                    ablaWalk.StartWalking();
                }
                else if (ablaIntermediateTarget != null)
                {
                    ablaWalk.SetTarget(ablaIntermediateTarget);
                    ablaWalk.StartWalking();
                }
            }

            yield return CutAndWait(cameraDirector != null ? (System.Action)cameraDirector.ShowAnneChild : null);
            if (anneTurn != null && childTransform != null)
                yield return anneTurn.TurnToFaceRoutine(childTransform);
            if (anneVoice != null)
                yield return anneVoice.PlayAndWaitRoutine(annePastayiYiyecek);
            yield return new WaitForSeconds(pauseBetweenBeats);
        }

        private IEnumerator TableTransitionRoutine()
        {
            if (transitionOverlay != null)
                yield return transitionOverlay.FadeToBlackRoutine(fadeDuration);

            TeleportTo(playerTransform, playerSeat);
            TeleportTo(anneTransform, anneSeat);
            TeleportTo(ablaTransform, ablaSeat);

            if (cameraDirector != null) cameraDirector.ShowTable();

            yield return new WaitForSeconds(blackHoldDuration);

            if (transitionOverlay != null)
                yield return transitionOverlay.FadeFromBlackRoutine(fadeDuration);

            yield return new WaitForSeconds(pauseBetweenBeats);

            if (anneTurn != null && childTransform != null)
                yield return anneTurn.TurnToFaceRoutine(childTransform);

            if (anneVoice != null)
                yield return anneVoice.PlayAndWaitRoutine(annePastaniBegendinMi);

            yield return new WaitForSeconds(pauseBetweenBeats);
        }

        private void TeleportTo(Transform character, Transform seat)
        {
            if (character == null || seat == null) return;

            CharacterController cc = character.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            character.position = seat.position;
            character.rotation = seat.rotation;

            if (cc != null) cc.enabled = true;

            Animator animator = character.GetComponentInChildren<Animator>();
            if (animator != null) animator.SetBool("IsSitting", true);
        }

        private IEnumerator CutAndWait(System.Action cut)
        {
            cut?.Invoke();
            if (cut != null)
                yield return new WaitForSeconds(cameraBlendDuration);
        }
    }
}