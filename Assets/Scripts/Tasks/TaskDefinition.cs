using System.Collections;
using UnityEngine;
using OgretmenGorevSistemi.Character;

namespace OgretmenGorevSistemi.Tasks
{
    public abstract class TaskDefinition : ScriptableObject, ITask
    {
        [SerializeField] private string taskName;
        public string TaskName => taskName;

        [Tooltip("Otomatik hatýrlatma 3. kez tetiklendiðinde, demoyla birlikte çalýnacak öðretmen sesi ")]
        [SerializeField] private AudioClip instructionVoiceClip;
        public AudioClip InstructionVoiceClip => instructionVoiceClip;

        [Tooltip("Validate() oyuncu denemesinde kaç saniye kesintisiz true dönmeli (0 = anlýk tamamlanýr).")]
        [SerializeField] private float requiredHoldDuration = 0f;
        public float RequiredHoldDuration => requiredHoldDuration;

        public abstract IEnumerator ExecuteRoutine(Transform character, Transform target);
        public abstract bool Validate(Transform character, Transform target);
        public abstract IEnumerator PlayHintRoutine(Transform character, Transform target);

        public virtual IEnumerator PlayCompletionRoutine(Transform character, Transform target)
        {
            yield break;
        }

        protected IEnumerator MoveTowards(Transform character, Transform target, float speed, float stopDistance)
        {
            Animator animator = character.GetComponentInChildren<Animator>();
            if (animator != null) animator.SetBool("IsMoving", true);

            while (Vector3.Distance(character.position, target.position) > stopDistance)
            {
                Vector3 direction = target.position - character.position;
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.0001f)
                {
                    Quaternion faceRotation = Quaternion.LookRotation(direction.normalized);
                    character.rotation = Quaternion.RotateTowards(character.rotation, faceRotation, 360f * Time.deltaTime);
                }

                character.position = Vector3.MoveTowards(character.position, target.position, speed * Time.deltaTime);
                yield return null;
            }

            if (animator != null) animator.SetBool("IsMoving", false);
        }

        protected Transform GetLookOrigin(Transform character)
        {
            var fps = character.GetComponent<FPSPlayerController>();
            return fps != null ? fps.CameraPivot : character;
        }
    }
}