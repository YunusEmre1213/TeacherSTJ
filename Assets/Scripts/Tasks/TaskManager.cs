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

        [Tooltip("Kamera Demo görünümüne geçtikten sonra hareketin baþlamasý için beklenecek süre ")]
        [SerializeField] private float cameraSwitchDelay = 1f;

        private int _currentIndex = -1;
        private bool _suppressValidation;
        private bool _autoPlay;
        private float _holdTimer;

        [Header("Test/Debug")]
        [Tooltip("'Adýmý Test Et' komutunun hangi adýmdan baþlayacaðý")]
        [SerializeField] private int debugStartIndex = 0;

        public TaskStep CurrentStep =>
            (_currentIndex >= 0 && _currentIndex < steps.Count) ? steps[_currentIndex] : null;

        private void Start()
        {
            GameEvents.RaiseTotalStepsKnown(steps.Count);
        }

        private void Update()
        {
            if (_suppressValidation || CurrentStep == null || _autoPlay) return;

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
        private void TeleportCharacter(Vector3 position)
        {
            CharacterController cc = character.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            character.position = position;
            if (cc != null) cc.enabled = true;
        }

        [ContextMenu("Demo Sýrasýný Baþlat")]
        public void BeginDemoSequence()
        {
            TeleportCharacter(startPoint.position);
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
            TeleportCharacter(startPoint.position);
            _autoPlay = false;
            _currentIndex = 0;
            RunCurrentStep();
        }

        [ContextMenu("Adýmý Test Et (Debug Start Index)")]
        public void BeginPlayerAttemptFromDebugIndex()
        {
            TeleportCharacter(startPoint.position);
            _autoPlay = false;
            _currentIndex = Mathf.Clamp(debugStartIndex, 0, steps.Count - 1);
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
                    TeleportCharacter(startPoint.position);
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

            GameEvents.RaiseHintFinished();
            _suppressValidation = false;
        }
    }
}