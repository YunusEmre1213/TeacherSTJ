using System.Collections;
using UnityEngine;
using OgretmenGorevSistemi.Character;

namespace OgretmenGorevSistemi.Tasks
{

    [CreateAssetMenu(menuName = "Görevler/Kelime Söyle", fileName = "YeniKelimeSoyleGorevi")]
    public class SayWordTask : TaskDefinition
    {
        [Tooltip("Oyuncunun söylemesi gereken kelime küçük harf")]
        [SerializeField] private string targetWord = "merhaba";

        [Tooltip(" karakterin söylediði ses klibi")]
        [SerializeField] private AudioClip voiceClip;

        public override IEnumerator ExecuteRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] {character.name}, \"{targetWord}\" diyor.");
            yield return PlayVoiceClip(character);
        }

        public override bool Validate(Transform character, Transform target)
        {
            var voice = character.GetComponent<VoiceRecognizer>();
            if (voice == null) return false;
            return voice.ConsumeKeyword(targetWord);
        }

        public override IEnumerator PlayHintRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] Hatýrlatma: \"{targetWord}\" demen gerekiyordu.");
            yield return PlayVoiceClip(character);
        }

        private IEnumerator PlayVoiceClip(Transform character)
        {
            AudioSource source = character.GetComponentInChildren<AudioSource>();
            if (source != null && voiceClip != null)
            {
                source.PlayOneShot(voiceClip);
                yield return new WaitForSeconds(voiceClip.length);
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
            }
        }
    }
}