using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OgretmenGorevSistemi.Character;

namespace OgretmenGorevSistemi.Tasks
{
    [CreateAssetMenu(menuName = "Görevler/El Ýþareti Yap", fileName = "YeniElIsaretiGorevi")]
    public class PlayGestureTask : TaskDefinition
    {
        [Tooltip("Karakterdeki BonePoseController'lardan hangisi çalýþacak (Pose Name alanýyla eþleþmeli).")]
        [SerializeField] private string poseName = "DurIsareti";

        [Tooltip("Demo/hatýrlatma sýrasýnda pozun tutulma süresi (saniye).")]
        [SerializeField] private float gestureDuration = 1.5f;

        public override IEnumerator ExecuteRoutine(Transform character, Transform target)
        {
            BonePoseController controller = FindPoseController(character);
            Debug.Log($"[PlayGestureTask] Pose Name: \"{poseName}\" için controller bulundu mu: {controller != null}. Karakterde toplam {character.GetComponentsInChildren<BonePoseController>().Length} adet BonePoseController var.");
            if (controller != null)
                yield return controller.PlayPose(gestureDuration);
            else
                yield return new WaitForSeconds(gestureDuration);
        }

        public override bool Validate(Transform character, Transform target)
        {
            var input = character.GetComponent<PlayerInputHandler>();
            if (input == null) return false;
            return input.ConsumeInteractPressed();
        }

        public override IEnumerator PlayCompletionRoutine(Transform character, Transform target)
        {
            yield return ExecuteRoutine(character, target);
        }

        public override IEnumerator PlayHintRoutine(Transform character, Transform target)
        {

            yield return FaceTargetRoutine(character, target);
            yield return ExecuteRoutine(character, target);
        }

        private IEnumerator FaceTargetRoutine(Transform character, Transform target)
        {
            if (target == null) yield break;

            var fpsController = character.GetComponent<OgretmenGorevSistemi.Character.FPSPlayerController>();
            if (fpsController != null) fpsController.ResetToHomeOrientation();


            var headLookAt = character.GetComponentInChildren<HeadLookAt>();
            if (headLookAt != null) headLookAt.SetTarget(target);

            Vector3 direction = target.position - character.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f) yield break;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            const float turnSpeed = 180f;
            while (Quaternion.Angle(character.rotation, targetRotation) > 1f)
            {
                character.rotation = Quaternion.RotateTowards(character.rotation, targetRotation, turnSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private BonePoseController FindPoseController(Transform character)
        {
            var controllers = character.GetComponentsInChildren<BonePoseController>();
            foreach (var c in controllers)
            {
                if (c.PoseName == poseName) return c;
            }
            return null;
        }
    }
}