using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using OgretmenGorevSistemi.Core;
using OgretmenGorevSistemi.Tasks;
using OgretmenGorevSistemi.Dialogue;

namespace OgretmenGorevSistemi.Flow
{
    public class TrainingFlowManager : MonoBehaviour
    {
        [SerializeField] private DialogueController dialogue;
        [SerializeField] private TaskManager taskManager;
        [SerializeField] private Button helpButton;

        [Header("Geçiþ öðretmen odasýndan oyun odasýna")]
        [SerializeField] private OgretmenGorevSistemi.UI.SceneTransitionOverlay transitionOverlay;
        [SerializeField] private float transitionFadeDuration = 1.5f;

        [Header("Öðretmen Sesleri")]
        [SerializeField] private TeacherVoice teacherVoice;
        [SerializeField] private AudioClip greetingVoice;
        [SerializeField] private AudioClip readyVoice;
        [SerializeField] private AudioClip completionVoice;

        [Tooltip("Son adým tamamlanýnca Tebrikler mesajýndan önce beklenecek süre ")]
        [SerializeField] private float completionDelay = 2f;

        [Tooltip("Oyun baþladýðýnda ilk sesin çalmasý için beklenecek süre ")]
        [SerializeField] private float initialDelay = 1.2f;

        private void Awake()
        {
            if (helpButton != null)
            {
                helpButton.onClick.AddListener(taskManager.ReplayHintForCurrentStep);
                helpButton.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            GameEvents.OnDemoSequenceFinished += HandleDemoFinished;
            GameEvents.OnAllStepsCompleted += HandleAllStepsCompleted;
            GameEvents.OnHelpRequested += HandleHelpRequested;
        }

        private void OnDisable()
        {
            GameEvents.OnDemoSequenceFinished -= HandleDemoFinished;
            GameEvents.OnAllStepsCompleted -= HandleAllStepsCompleted;
            GameEvents.OnHelpRequested -= HandleHelpRequested;
        }

        private void Start()
        {
            StartCoroutine(StartGreetingAfterDelay());
        }

        private IEnumerator StartGreetingAfterDelay()
        {
            yield return new WaitForSeconds(initialDelay);
            StartGreeting();
        }

        [ContextMenu("Baþtan Baþlat")]
        public void StartGreeting()
        {
            teacherVoice.Play(greetingVoice, ShowGreetingPanel);
        }

        private void ShowGreetingPanel()
        {
            dialogue.ShowConfirm("", OnGreetingConfirmed);
        }

        private void OnGreetingConfirmed()
        {
            StartCoroutine(TransitionToDemoRoutine());
        }
        private IEnumerator TransitionToDemoRoutine()
        {
            if (transitionOverlay != null)
                yield return transitionOverlay.FadeToBlackRoutine(transitionFadeDuration);

            GameEvents.RaiseTeacherGreetingFinished();
            taskManager.BeginDemoSequence();

            if (transitionOverlay != null)
            {
                yield return new WaitForSeconds(0.5f);
                yield return transitionOverlay.FadeFromBlackRoutine(transitionFadeDuration);
            }
        }

        private void HandleDemoFinished()
        {
            teacherVoice.Play(readyVoice, ShowReadyPanel);
        }

        private void ShowReadyPanel()
        {
            dialogue.ShowConfirm("", OnPlayerReadyConfirmed);
        }

        private void OnPlayerReadyConfirmed()
        {
            StartCoroutine(TransitionToPlayerAttemptRoutine());
        }
        private IEnumerator TransitionToPlayerAttemptRoutine()
        {
            if (transitionOverlay != null)
                yield return transitionOverlay.FadeToBlackRoutine(transitionFadeDuration);

            GameEvents.RaisePlayerConfirmedReady();
            if (helpButton != null) helpButton.gameObject.SetActive(true);
            taskManager.BeginPlayerAttempt();

            if (transitionOverlay != null)
            {
                yield return new WaitForSeconds(0.5f);
                yield return transitionOverlay.FadeFromBlackRoutine(transitionFadeDuration);
            }
        }

        private void HandleAllStepsCompleted()
        {
            StartCoroutine(CompletionRoutine());
        }
        private IEnumerator CompletionRoutine()
        {
            yield return new WaitForSeconds(completionDelay);
            if (helpButton != null) helpButton.gameObject.SetActive(false);

            if (transitionOverlay != null)
                yield return transitionOverlay.FadeToBlackRoutine(transitionFadeDuration);

            taskManager.TeleportToTeacherRoom();

            if (transitionOverlay != null)
            {
                yield return new WaitForSeconds(0.5f);
                yield return transitionOverlay.FadeFromBlackRoutine(transitionFadeDuration);
            }

            if (teacherVoice != null && completionVoice != null)
                teacherVoice.Play(completionVoice, null);
            dialogue.ShowMessage("Harika, görevi tamamladýn!");
        }

        private void HandleHelpRequested()
        {
            taskManager.ReplayHintForCurrentStep();
        }
    }
}