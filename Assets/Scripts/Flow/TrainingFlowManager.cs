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

        [Header("Öðretmen Sesleri")]
        [SerializeField] private TeacherVoice teacherVoice;
        [SerializeField] private AudioClip greetingVoice;
        [SerializeField] private AudioClip readyVoice;

        [SerializeField] private float completionDelay = 2f;

        [SerializeField] private float initialDelay = 1.2f;

        private void Awake()
        {
            helpButton.onClick.AddListener(taskManager.ReplayHintForCurrentStep);
            helpButton.gameObject.SetActive(false);
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
            GameEvents.RaiseTeacherGreetingFinished();
            taskManager.BeginDemoSequence();
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
            GameEvents.RaisePlayerConfirmedReady();
            helpButton.gameObject.SetActive(true);
            taskManager.BeginPlayerAttempt();
        }

        private void HandleAllStepsCompleted()
        {
            StartCoroutine(ShowCompletionAfterDelay());
        }

        private IEnumerator ShowCompletionAfterDelay()
        {
            yield return new WaitForSeconds(completionDelay);
            helpButton.gameObject.SetActive(false);
            dialogue.ShowMessage("Harika, görevi tamamladýn!");
        }

        private void HandleHelpRequested()
        {
            taskManager.ReplayHintForCurrentStep();
        }
    }
}