using System;
using System.Collections.Generic;
using UnityEngine;

namespace OgretmenGorevSistemi.Character
{

    public class VoiceRecognizer : MonoBehaviour
    {
        [SerializeField] private string[] keywords = { "evet", "hayýr", "merhaba" };
        [SerializeField] private VoskSpeechToText voskSpeechToText;

        public event Action<string> KeywordRecognized;
        public bool IsSupported { get; private set; }

        private readonly HashSet<string> _pendingKeywords = new HashSet<string>();

        private void Awake()
        {
            if (voskSpeechToText == null)
            {
                Debug.LogWarning("[VoiceRecognizer] VoskSpeechToText atanmamýþ, ses tanýma devre dýþý.");
                IsSupported = false;
                return;
            }

            voskSpeechToText.OnTranscriptionResult += HandleTranscriptionResult;
            voskSpeechToText.StartVoskStt(new List<string>(keywords), maxAlternatives: 0);
            IsSupported = true;
        }

        private void OnDestroy()
        {
            if (voskSpeechToText != null)
                voskSpeechToText.OnTranscriptionResult -= HandleTranscriptionResult;
        }

        private void HandleTranscriptionResult(string json)
        {
            string text = ExtractText(json);
            if (string.IsNullOrEmpty(text)) return;

            text = text.ToLower().Trim();
            _pendingKeywords.Add(text);
            KeywordRecognized?.Invoke(text);
        }

        public bool ConsumeKeyword(string keyword)
        {
            return _pendingKeywords.Remove(keyword.ToLower());
        }

        private string ExtractText(string json)
        {
            if (string.IsNullOrEmpty(json)) return null;

            int idx = json.IndexOf("\"text\"", StringComparison.Ordinal);
            if (idx < 0) return null;

            int colon = json.IndexOf(':', idx);
            if (colon < 0) return null;

            int firstQuote = json.IndexOf('"', colon + 1);
            if (firstQuote < 0) return null;

            int secondQuote = json.IndexOf('"', firstQuote + 1);
            if (secondQuote < 0) return null;

            return json.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
        }
    }
}