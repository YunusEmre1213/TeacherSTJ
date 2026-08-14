using UnityEngine;
using TMPro;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.UI
{
    public class TaskLabelDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        private void Awake()
        {
            label.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameEvents.OnCurrentStepChanged += UpdateLabel;
        }

        private void OnDisable()
        {
            GameEvents.OnCurrentStepChanged -= UpdateLabel;
        }

        private void UpdateLabel(string taskName)
        {
            bool hasTask = !string.IsNullOrEmpty(taskName);
            label.gameObject.SetActive(hasTask);
            label.text = hasTask ? taskName : "";
        }
    }
}