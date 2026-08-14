using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Tasks
{
    [CreateAssetMenu(menuName = "Görevler/Noktaya Yürü", fileName = "YeniNoktayaYuruGorevi")]
    public class WalkToPointTask : TaskDefinition
    {
        [SerializeField] private float arrivalDistance = 0.5f;
        [SerializeField] private float demoSpeed = 2f;
        [SerializeField] private float hintSpeed = 4f;

        public override IEnumerator ExecuteRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}]  {character.name}, {target.name} noktasýna yürüyor.");
            yield return MoveTowards(character, target, demoSpeed, arrivalDistance);
        }

        public override bool Validate(Transform character, Transform target)
        {
            return Vector3.Distance(character.position, target.position) <= arrivalDistance;
        }

        public override IEnumerator PlayHintRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] Hatýrlatma: {target.name} noktasýna gitmen gerekiyordu.");
            yield return MoveTowards(character, target, hintSpeed, arrivalDistance);
            yield return new WaitForSeconds(0.3f);
        }
    }
}