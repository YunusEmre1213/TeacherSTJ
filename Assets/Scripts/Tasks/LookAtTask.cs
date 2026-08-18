using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Tasks
{

    [CreateAssetMenu(menuName = "Görevler/Bir Yere Bak", fileName = "YeniBakGorevi")]
    public class LookAtTask : TaskDefinition
    {
        [Tooltip("Iþýnýn hedefe ulaþabileceði maksimum mesafe.")]
        [SerializeField] private float maxLookDistance = 10f;

        [Tooltip("Iþýnýn kalýnlýðý (SphereCast yarýçapý) — küçük sapmalara tolerans saðlar. Hedef objenin kendi Collider'ýndan baðýmsýz, ray'in kalýnlýðý.")]
        [SerializeField] private float rayRadius = 0.25f;

        [Tooltip("Raycast hedefe deðse bile, doðal görünmesi için açý bu deðerin altýnda olmalý (kalýn SphereCast'in çok erken/gevþek isabet saymasýný önler).")]
        [SerializeField] private float maxCenteringAngle = 15f;

        [SerializeField] private float demoRotateSpeed = 90f;
        [SerializeField] private float hintRotateSpeed = 180f;

        [Tooltip("Yeni bir hedefe geçmeden önce, görsel bakýþýn serbest býrakýlýp bir an nötr pozda bekleyeceði süre — 'gözlerin ani sýçramasý' hissini önler.")]
        [SerializeField] private float lookResetPause = 0.4f;

        public override IEnumerator ExecuteRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] Demo: {character.name}, {target.name} yönüne dönüyor.");

            ClearVisualHeadTarget(character);
            yield return new WaitForSeconds(lookResetPause);

            UpdateVisualHeadTarget(character, target);
            yield return RotateTowards(character, target, demoRotateSpeed);
        }

        public override bool Validate(Transform character, Transform target)
        {
            Transform lookOrigin = GetLookOrigin(character);
            return IsLookingAt(lookOrigin, target);
        }

        public override IEnumerator PlayHintRoutine(Transform character, Transform target)
        {
            Debug.Log($"[{TaskName}] Hatýrlatma: {target.name} yönüne bakman gerekiyordu.");
            UpdateVisualHeadTarget(character, target);
            yield return RotateTowards(character, target, hintRotateSpeed);
            yield return new WaitForSeconds(0.3f);
        }
        private void UpdateVisualHeadTarget(Transform character, Transform target)
        {
            var headLookAt = character.GetComponentInChildren<OgretmenGorevSistemi.Character.HeadLookAt>();
            if (headLookAt != null) headLookAt.SetTarget(target);
        }

        private void ClearVisualHeadTarget(Transform character)
        {
            var headLookAt = character.GetComponentInChildren<OgretmenGorevSistemi.Character.HeadLookAt>();
            if (headLookAt != null) headLookAt.SetTarget(null);
        }
        private bool IsLookingAt(Transform lookOrigin, Transform target)
        {
            Ray ray = new Ray(lookOrigin.position, lookOrigin.forward);
            if (!Physics.SphereCast(ray, rayRadius, out RaycastHit hit, maxLookDistance))
                return false;

            if (hit.transform != target && !hit.transform.IsChildOf(target))
                return false;

            Vector3 direction = (target.position - lookOrigin.position).normalized;
            float angle = Vector3.Angle(lookOrigin.forward, direction);
            return angle <= maxCenteringAngle;
        }

        private IEnumerator RotateTowards(Transform character, Transform target, float speed)
        {
            Transform lookOrigin = GetLookOrigin(character);

            if (lookOrigin != character)
            {
                lookOrigin.localRotation = Quaternion.identity;
            }

            while (!IsLookingAt(lookOrigin, target))
            {
                Vector3 direction = (target.position - lookOrigin.position).normalized;

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