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

        public TaskStep CurrentStep =>
            (_currentIndex >= 0 && _currentIndex < steps.Count) ? steps[_currentIndex] : null;

        private void Start()
        {
            GameEvents.RaiseTotalStepsKnown(steps.Count);
        }

        private void Update()
        {
            if (_suppressValidation || CurrentStep == null || _autoPlay) return;

            if (CurrentStep.definition.Validate(character, CurrentStep.target))
            {
                CompleteCurrentStep();
            }
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

        private void RunCurrentStep()
        {
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