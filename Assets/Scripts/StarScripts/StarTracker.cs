using System.Collections.Generic;
using UnityEngine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.Tasks
{
    public class StarTracker : MonoBehaviour
    {
        private readonly Dictionary<int, int> _hintCounts = new Dictionary<int, int>();
        private bool _isPlayerAttempt;

        private void OnEnable()
        {
            GameEvents.OnDemoSequenceStarted += HandleDemoStarted;
            GameEvents.OnPlayerConfirmedReady += HandlePlayerAttemptStarted;
            GameEvents.OnTaskStepFailed += HandleStepFailed;
            GameEvents.OnTaskStepCompleted += HandleStepCompleted;
        }

        private void OnDisable()
        {
            GameEvents.OnDemoSequenceStarted -= HandleDemoStarted;
            GameEvents.OnPlayerConfirmedReady -= HandlePlayerAttemptStarted;
            GameEvents.OnTaskStepFailed -= HandleStepFailed;
            GameEvents.OnTaskStepCompleted -= HandleStepCompleted;
        }

        private void HandleDemoStarted() => _isPlayerAttempt = false;
        private void HandlePlayerAttemptStarted() => _isPlayerAttempt = true;

        private void HandleStepFailed(int stepIndex)
        {
            if (!_isPlayerAttempt) return;
            _hintCounts.TryGetValue(stepIndex, out int count);
            _hintCounts[stepIndex] = count + 1;
        }

        private void HandleStepCompleted(int stepIndex)
        {
            if (!_isPlayerAttempt) return;

            _hintCounts.TryGetValue(stepIndex, out int hints);
            int stars = Mathf.Max(0, 3 - hints);
            GameEvents.RaiseStarsEarned(stars);
        }
    }
}