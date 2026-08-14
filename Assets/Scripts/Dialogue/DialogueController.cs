using System;
using UnityEngine;
using TMPro;
using OgretmenGorevSistemi.Core;
using OgretmenGorevSistemi.Character;

namespace OgretmenGorevSistemi.Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI lineText;
        [SerializeField] private VoiceRecognizer voiceRecognizer;

        private Action _onYes;

        private void Awake()
        {
            Hide();
        }

        private void OnEnable()
        {
            if (voiceRecognizer != null)
                voiceRecognizer.KeywordRecognized += HandleKeyword;
        }

        private void OnDisable()
        {
            if (voiceRecognizer != null)
                voiceRecognizer.KeywordRecognized -= HandleKeyword;
        }

        public void ShowConfirm(string text, Action onYes)
        {
            _onYes = onYes;
            lineText.text = text;
            panel.SetActive(true);
            GameEvents.RaiseDialogueShown();
        }

        public void ShowMessage(string text)
        {
            _onYes = null;
            lineText.text = text;
            panel.SetActive(true);
            GameEvents.RaiseDialogueShown();
        }

        public void Hide()
        {
            panel.SetActive(false);
            GameEvents.RaiseDialogueHidden();
        }

        private void HandleKeyword(string keyword)
        {
            if (panel == null || !panel.activeSelf) return;

            if (keyword == "evet") HandleYes();
            else if (keyword == "hayýr") HandleNo();
        }

        [ContextMenu("Evet (Test)")]
        private void HandleYes()
        {
            Hide();
            _onYes?.Invoke();
        }

        private void HandleNo()
        {
           
        }
    }
}