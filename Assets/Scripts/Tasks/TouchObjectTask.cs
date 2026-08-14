using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Tasks
{
    [CreateAssetMenu(menuName = "Görevler/Objeye Dokun", fileName = "YeniDokunGorevi")]
    public class TouchObjectTask : TaskDefinition
    {
        [SerializeField] private float touchDistance = 1.0f;
        [SerializeField] private float demoSpeed = 2f;
        [SerializeField] private float hintSpeed = 4f;

        public override IEnumerator ExecuteRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] {character.name}, {target.name} objesine dokunuyor.");
            yield return MoveTowards(character, target, demoSpeed, touchDistance);
        }

        public override bool Validate(Transform character, Transform target)
        {
            return Vector3.Distance(character.position, target.position) <= touchDistance;
        }

        public override IEnumerator PlayHintRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] Hatýrlatma: {target.name} objesine dokunman gerekiyordu.");
            yield return MoveTowards(character, target, hintSpeed, touchDistance);
            yield return new WaitForSeconds(0.3f);
        }
    }
}