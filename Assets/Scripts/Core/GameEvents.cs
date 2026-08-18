using System;

namespace OgretmenGorevSistemi.Core
{
    public static class GameEvents
    {
        // ---- Diyalog  ----
        public static event Action OnTeacherGreetingFinished;
        public static event Action OnPlayerConfirmedReady;
        public static event Action OnDialogueShown;
        public static event Action OnDialogueHidden;
        public static event Action OnHelpRequested;
        public static event Action OnHintStarted;
        public static event Action OnHintFinished;
        public static event Action<string> OnCurrentStepChanged;
        public static event Action<int> OnTotalStepsKnown;
        public static event Action<int> OnStarsEarned;
        public static event Action OnDemoBlocked;
        public static event Action OnDemoUnblocked;

        // ---- Görev Akýþý ----
        public static event Action OnDemoSequenceStarted;
        public static event Action OnDemoSequenceFinished;
        public static event Action<int> OnTaskStepCompleted;
        public static event Action<int> OnTaskStepFailed;
        public static event Action OnAllStepsCompleted;

        // ---- Tetikleyiciler ----
        public static void RaiseTeacherGreetingFinished() => OnTeacherGreetingFinished?.Invoke();
        public static void RaisePlayerConfirmedReady() => OnPlayerConfirmedReady?.Invoke();
        public static void RaiseDialogueShown() => OnDialogueShown?.Invoke();
        public static void RaiseDialogueHidden() => OnDialogueHidden?.Invoke();
        public static void RaiseHelpRequested() => OnHelpRequested?.Invoke();
        public static void RaiseHintStarted() => OnHintStarted?.Invoke();
        public static void RaiseHintFinished() => OnHintFinished?.Invoke();
        public static void RaiseCurrentStepChanged(string taskName) => OnCurrentStepChanged?.Invoke(taskName);
        public static void RaiseTotalStepsKnown(int totalSteps) => OnTotalStepsKnown?.Invoke(totalSteps);
        public static void RaiseStarsEarned(int stars) => OnStarsEarned?.Invoke(stars);
        public static void RaiseDemoBlocked() => OnDemoBlocked?.Invoke();
        public static void RaiseDemoUnblocked() => OnDemoUnblocked?.Invoke();
        public static void RaiseDemoSequenceStarted() => OnDemoSequenceStarted?.Invoke();
        public static void RaiseDemoSequenceFinished() => OnDemoSequenceFinished?.Invoke();
        public static void RaiseTaskStepCompleted(int stepIndex) => OnTaskStepCompleted?.Invoke(stepIndex);
        public static void RaiseTaskStepFailed(int stepIndex) => OnTaskStepFailed?.Invoke(stepIndex);
        public static void RaiseAllStepsCompleted() => OnAllStepsCompleted?.Invoke();
    }
}