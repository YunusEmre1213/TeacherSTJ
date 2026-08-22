using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Character
{

    public class WalkToTargetOnCue : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float speed = 1.5f;
        [SerializeField] private float stopDistance = 0.5f;
        [SerializeField] private float facingOffsetY = 0f;

        public Transform Target => target;

        public void SetTarget(Transform newTarget) => target = newTarget;

        private Animator _animator;
        private bool _isWalkingIndefinitely;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null)
                Debug.LogWarning("[WalkToTargetOnCue] Alt objelerde Animator bulunamadý — yürüme animasyonu tetiklenmeyecek.", this);
        }

        private Quaternion GetFacingRotation(Vector3 direction)
        {
            return Quaternion.LookRotation(direction) * Quaternion.Euler(0f, facingOffsetY, 0f);
        }
        public void StartWalking()
        {
            _isWalkingIndefinitely = true;
            StartCoroutine(WalkIndefinitelyRoutine());
        }
        public void StopWalking()
        {
            _isWalkingIndefinitely = false;
        }

        private IEnumerator WalkIndefinitelyRoutine()
        {
            if (target == null) yield break;

            while (_isWalkingIndefinitely)
            {
                Vector3 direction = target.position - transform.position;
                direction.y = 0f;
                bool reachedTarget = direction.magnitude <= stopDistance;

                if (_animator != null) _animator.SetBool("IsMoving", !reachedTarget);

                if (!reachedTarget)
                {
                    transform.rotation = GetFacingRotation(direction.normalized);
                    transform.position += direction.normalized * speed * Time.deltaTime;
                }
                yield return null;
            }

            if (_animator != null) _animator.SetBool("IsMoving", false);
        }
    }
}