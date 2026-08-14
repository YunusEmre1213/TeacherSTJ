using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Tasks
{
  
    [CreateAssetMenu(menuName = "Görevler/Bir Yere Bak", fileName = "YeniBakGorevi")]
    public class LookAtTask : TaskDefinition
    {
        [SerializeField] private float viewAngleThreshold = 20f;
        [SerializeField] private float demoRotateSpeed = 90f;
        [SerializeField] private float hintRotateSpeed = 180f;

        public override IEnumerator ExecuteRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] {character.name}, {target.name} yönüne dönüyor.");
            yield return RotateTowards(character, target, demoRotateSpeed);
        }

        public override bool Validate(Transform character, Transform target)
        {
            Transform lookOrigin = GetLookOrigin(character);
            Vector3 direction = (target.position - lookOrigin.position).normalized;
            return Vector3.Angle(lookOrigin.forward, direction) <= viewAngleThreshold;
        }

        public override IEnumerator PlayHintRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] Hatýrlatma: {target.name} yönüne bakman gerekiyordu.");
            yield return RotateTowards(character, target, hintRotateSpeed);
            yield return new WaitForSeconds(0.3f);
        }

       
        private IEnumerator RotateTowards(Transform character, Transform target, float speed)
        {
            Transform lookOrigin = GetLookOrigin(character);

            while (true)
            {
                Vector3 direction = (target.position - lookOrigin.position).normalized;
                if (Vector3.Angle(lookOrigin.forward, direction) <= viewAngleThreshold) yield break;

                
                Vector3 flatDirection = direction;
                flatDirection.y = 0f;
                if (flatDirection.sqrMagnitude > 0.0001f)
                {
                    Quaternion bodyTarget = Quaternion.LookRotation(flatDirection.normalized);
                    character.rotation = Quaternion.RotateTowards(character.rotation, bodyTarget, speed * Time.deltaTime);
                }

                if (lookOrigin != character)
                {
                    Quaternion desiredWorld = Quaternion.LookRotation(direction);
                    Quaternion desiredLocal = Quaternion.Inverse(character.rotation) * desiredWorld;
                    lookOrigin.localRotation = Quaternion.RotateTowards(lookOrigin.localRotation, desiredLocal, speed * Time.deltaTime);
                }

                yield return null;
            }
        }
    }
}