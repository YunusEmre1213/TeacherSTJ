using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.Tasks
{
    public class TaskManager : MonoBehaviour
    {
        [SerializeField] private Transform character;
        [SerializeField] private Transform startPoint;
        [SerializeField] private List<TaskStep> steps = new List<TaskStep>();

        [Tooltip("Kamera Demo görünümüne geçtikten sonra hareketin baþlamasý için beklenecek süre")]
        [SerializeField] private float cameraSwitchDelay = 1f;

        private int _currentIndex = -1;
        private bool _suppressValidation;
        private bool _autoPlay;
        private float _holdTimer;
        private int _demoBlockCount;

        [Header("Test/Debug")]
        [SerializeField] private int debugStartIndex = 0;

        public TaskStep CurrentStep =>
            (_currentIndex >= 0 && _currentIndex < steps.Count) ? steps[_currentIndex] : null;

        public int CurrentStepIndex => _currentIndex;

        private void OnEnable()
        {
            GameEvents.OnDemoBlocked += HandleDemoBlocked;
            GameEvents.OnDemoUnblocked += HandleDemoUnblocked;
        }

        private void OnDisable()
        {
            GameEvents.OnDemoBlocked -= HandleDemoBlocked;
            GameEvents.OnDemoUnblocked -= HandleDemoUnblocked;
        }

        private void HandleDemoBlocked() => _demoBlockCount++;
        private void HandleDemoUnblocked() => _demoBlockCount = Mathf.Max(0, _demoBlockCount - 1);

        private void Start()
        {
            GameEvents.RaiseTotalStepsKnown(steps.Count);
        }

        private void Update()
        {
            if (_suppressValidation || CurrentStep == null || _autoPlay || _demoBlockCount > 0) return;

            if (!CurrentStep.definition.Validate(character, CurrentStep.target))
            {
                _holdTimer = 0f;
                return;
            }

            float requiredHold = CurrentStep.definition.RequiredHoldDuration;
            if (requiredHold > 0f)
            {
                _holdTimer += Time.deltaTime;
                if (_holdTimer < requiredHold) return;
            }

            _holdTimer = 0f;
            StartCoroutine(CompleteAfterRoutine());
        }
        private IEnumerator CompleteAfterRoutine()
        {
            _suppressValidation = true;
            yield return CurrentStep.definition.PlayCompletionRoutine(character, CurrentStep.target);
            _suppressValidation = false;
            CompleteCurrentStep();
        }
        private void TeleportCharacter(Vector3 position, Quaternion? rotation = null)
        {
            CharacterController cc = character.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            character.position = position;
            if (rotation.HasValue) character.rotation = rotation.Value;
            if (cc != null) cc.enabled = true;

            var fps = character.GetComponent<Character.FPSPlayerController>();
            if (fps != null && rotation.HasValue)
            {
                fps.SetHomeOrientation(rotation.Value);
                fps.ResetToHomeOrientation();
            }
        }

        [Header("Öðretmen odasý")]
        [SerializeField] private Transform teacherRoomPoint;

        public void TeleportToTeacherRoom()
        {
            if (teacherRoomPoint != null) TeleportCharacter(teacherRoomPoint.position, teacherRoomPoint.rotation);
        }

        [ContextMenu("Demo Sýrasýný Baþlat")]
        public void BeginDemoSequence()
        {
            TeleportCharacter(startPoint.position, startPoint.rotation);
            GameEvents.RaiseDemoSequenceStarted();
            _autoPlay = true;
            _currentIndex = 0;
            StartCoroutine(BeginDemoAfterCameraSwitch());
        }

        private IEnumerator BeginDemoAfterCameraSwitch()
        {
            yield return new WaitForSeconds(cameraSwitchDelay);
            RunCurrentStep();
        }

        [ContextMenu("Oyuncu Denemesini Baþlat")]
        public void BeginPlayerAttempt()
        {
            TeleportCharacter(startPoint.position, startPoint.rotation);
            _autoPlay = false;
            _currentIndex = 0;
            RunCurrentStep();
        }

        [ContextMenu("Adýmý Test Et (Debug Start Index)")]
        public void BeginPlayerAttemptFromDebugIndex()
        {
            TeleportCharacter(startPoint.position, startPoint.rotation);
            _autoPlay = false;
            _currentIndex = Mathf.Clamp(debugStartIndex, 0, steps.Count - 1);
            Debug.Log($"[TaskManager] Debug baþlatma — steps.Count: {steps.Count}, debugStartIndex: {debugStartIndex}, hesaplanan _currentIndex: {_currentIndex}, o index'teki görev: {(steps.Count > 0 ? steps[_currentIndex].definition.TaskName : "YOK")}");
            GameEvents.RaisePlayerConfirmedReady();
            RunCurrentStep();
        }

        private void RunCurrentStep()
        {
            _holdTimer = 0f;

            if (CurrentStep == null)
            {
                GameEvents.RaiseCurrentStepChanged(null);

                if (_autoPlay)
                {
                    TeleportToTeacherRoom();
                    GameEvents.RaiseDemoSequenceFinished();
                }
                else GameEvents.RaiseAllStepsCompleted();
                return;
            }

            GameEvents.RaiseCurrentStepChanged(CurrentStep.definition.TaskName);

            if (_autoPlay)
            {
                StartCoroutine(RunDemoStep());
            }
        }

        private IEnumerator RunDemoStep()
        {
            if (_demoBlockCount > 0)
                yield return new WaitUntil(() => _demoBlockCount <= 0);

            yield return CurrentStep.definition.ExecuteRoutine(character, CurrentStep.target);

            float hold = CurrentStep.definition.RequiredHoldDuration;
            if (hold > 0f) yield return new WaitForSeconds(hold);

            CompleteCurrentStep();
        }

        private void CompleteCurrentStep()
        {
            GameEvents.RaiseTaskStepCompleted(_currentIndex);
            _currentIndex++;
            RunCurrentStep();
        }
        public void SkipCurrentStep()
        {
            if (CurrentStep == null) return;
            CompleteCurrentStep();
        }

        [ContextMenu("Hint Oynat (Mevcut Adým)")]
        public void ReplayHintForCurrentStep()
        {
            Debug.Log($"[TaskManager] ReplayHintForCurrentStep çaðrýldý. CurrentStep null mu: {CurrentStep == null}, index: {_currentIndex}");
            if (CurrentStep == null) return;
            StartCoroutine(HintRoutine());
        }

        private IEnumerator HintRoutine()
        {
            _suppressValidation = true;
            GameEvents.RaiseTaskStepFailed(_currentIndex);
            GameEvents.RaiseHintStarted();

            yield return new WaitForSeconds(cameraSwitchDelay);

            Vector3 originalPosition = character.position;
            yield return CurrentStep.definition.PlayHintRoutine(character, CurrentStep.target);
            TeleportCharacter(originalPosition);

            var fps = character.GetComponent<Character.FPSPlayerController>();
            if (fps != null) fps.ResetToHomeOrientation();

            GameEvents.RaiseHintFinished();
            _suppressValidation = false;
        }
    }
}