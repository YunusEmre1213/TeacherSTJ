using UnityEngine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.Character
{

    public class NpcVisibility : MonoBehaviour
    {
        [Tooltip("Gizlenip gösterilecek görsel kök — script'in kendi objesi DEÐÝL, bir çocuk obje olmalý.")]
        [SerializeField] private GameObject visualRoot;

        private void Awake()
        {
            visualRoot.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnDemoSequenceStarted += Show;
            GameEvents.OnDemoSequenceFinished += Hide;
            GameEvents.OnPlayerConfirmedReady += Show;
            GameEvents.OnAllStepsCompleted += Hide;
        }

        private void OnDisable()
        {
            GameEvents.OnDemoSequenceStarted -= Show;
            GameEvents.OnDemoSequenceFinished -= Hide;
            GameEvents.OnPlayerConfirmedReady -= Show;
            GameEvents.OnAllStepsCompleted -= Hide;
        }

        private void Show() => visualRoot.SetActive(true);
        private void Hide() => visualRoot.SetActive(false);
    }
}