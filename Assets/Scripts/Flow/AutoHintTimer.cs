using UnityEngine;
using OgretmenGorevSistemi.Core;
using OgretmenGorevSistemi.Dialogue;
using OgretmenGorevSistemi.Tasks;

namespace OgretmenGorevSistemi.Flow
{
    public class AutoHintTimer : MonoBehaviour
    {
        [SerializeField] private TaskManager taskManager;
        [SerializeField] private TeacherVoice teacherVoice;

        [Tooltip("Oyuncuya, bir sonraki otomatik hatýrlatmadan önce tanýnan süre (saniye).")]
        [SerializeField] private float timeoutSeconds = 11f;

        private float _secondsElapsed;
        private int _autoHintCount;
        private bool _isPlayerAttempt;
        private bool _isCounting;

        private void OnEnable()
        {
            GameEvents.OnPlayerConfirmedReady += HandlePlayerAttemptStarted;
            GameEvents.OnDemoSequenceStarted += HandleDemoStarted;
            GameEvents.OnCurrentStepChanged += HandleCurrentStepChanged;
            GameEvents.OnHintStarted += HandleHintStarted;
            GameEvents.OnHintFinished += HandleHintFinished;
            GameEvents.OnDemoBlocked += HandleDemoBlocked;
            GameEvents.OnDemoUnblocked += HandleDemoUnblocked;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerConfirmedReady -= HandlePlayerAttemptStarted;
            GameEvents.OnDemoSequenceStarted -= HandleDemoStarted;
            GameEvents.OnCurrentStepChanged -= HandleCurrentStepChanged;
            GameEvents.OnHintStarted -= HandleHintStarted;
            GameEvents.OnHintFinished -= HandleHintFinished;
            GameEvents.OnDemoBlocked -= HandleDemoBlocked;
            GameEvents.OnDemoUnblocked -= HandleDemoUnblocked;
        }

        private int _demoBlockCount;

        private void Update()
        {
            if (!_isCounting || _demoBlockCount > 0) return;

            _secondsElapsed += Time.deltaTime;
            if (_secondsElapsed >= timeoutSeconds)
            {
                TriggerAutoHint();
            }
        }

        private void HandleDemoBlocked() => _demoBlockCount++;
        private void HandleDemoUnblocked() => _demoBlockCount = Mathf.Max(0, _demoBlockCount - 1);

        private void HandlePlayerAttemptStarted() => _isPlayerAttempt = true;
        private void HandleDemoStarted() => _isPlayerAttempt = false;

        private void HandleCurrentStepChanged(string taskName)
        {
            if (!_isPlayerAttempt || string.IsNullOrEmpty(taskName))
            {
                _isCounting = false;
                return;
            }

            _secondsElapsed = 0f;
            _autoHintCount = 0;
            _isCounting = true;
        }

        private void HandleHintStarted() => _isCounting = false;

        private void HandleHintFinished()
        {
            if (!_isPlayerAttempt) return;
            _secondsElapsed = 0f;
            _isCounting = true;
        }

        private void TriggerAutoHint()
        {
            _isCounting = false;
            _autoHintCount++;

            if (_autoHintCount > 3)
            {
                taskManager.SkipCurrentStep();
                return;
            }

            if (_autoHintCount == 3)
            {
                AudioClip clip = taskManager.CurrentStep?.definition?.InstructionVoiceClip;
                if (clip != null) teacherVoice.Play(clip, null);
            }

            taskManager.ReplayHintForCurrentStep();
        }
    }
}