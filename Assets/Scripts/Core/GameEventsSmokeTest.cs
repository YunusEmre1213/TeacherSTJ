using UnityEngine;

namespace OgretmenGorevSistemi.Core
{

    public class GameEventsSmokeTest : MonoBehaviour
    {
        private void OnEnable()
        {
            GameEvents.OnTeacherGreetingFinished += HandleGreetingFinished;
        }

        private void OnDisable()
        {
            GameEvents.OnTeacherGreetingFinished -= HandleGreetingFinished;
        }

        private void Start()
        {
            Debug.Log("Start eventi tetikleniyor");
            GameEvents.RaiseTeacherGreetingFinished();
        }

        private void HandleGreetingFinished()
        {
            Debug.Log("Event dinleyici çalýþýyor");
        }
    }
}