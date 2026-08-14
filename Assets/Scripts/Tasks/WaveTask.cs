using System.Collections;
using UnityEngine;
using OgretmenGorevSistemi.Character;

namespace OgretmenGorevSistemi.Tasks
{

    [CreateAssetMenu(menuName = "Görevler/El Salla", fileName = "YeniElSallaGorevi")]
    public class WaveTask : TaskDefinition
    {
        [SerializeField] private float waveDuration = 1.7f; //El sallama anim uzunluk

        public override IEnumerator ExecuteRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}]  {character.name}, {target.name}'e el sallýyor.");
            TriggerWaveAnimation(character);
            yield return new WaitForSeconds(waveDuration);
        }

        public override bool Validate(Transform character, Transform target)
        {
            var input = character.GetComponent<PlayerInputHandler>();
            if (input == null)
            {
                Debug.LogWarning($"[{TaskName}] PlayerInputHandler bulunamadý");
                return false;
            }

            if (!input.ConsumeInteractPressed()) return false;

            TriggerWaveAnimation(character);
            return true;
        }

        public override IEnumerator PlayHintRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] Hatýrlatma: {target.name} E'ye basarak el sallaman gerekiyordu.");
            TriggerWaveAnimation(character);
            yield return new WaitForSeconds(waveDuration);
        }

        private void TriggerWaveAnimation(Transform character)
        {
            Animator animator = character.GetComponentInChildren<Animator>();
            if (animator != null) animator.SetTrigger("Wave");
        }
    }
}