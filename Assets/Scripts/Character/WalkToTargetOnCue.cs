using System.Collections;
using UnityEngine;

namespace OgretmenGorevSistemi.Character
{
    public class WalkToTargetOnCue : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float speed = 1.5f;
        [SerializeField] private float stopDistance = 0.5f;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator == null)
                Debug.LogWarning("[WalkToTargetOnCue] Alt objelerde Animator bulunamadý — yürüme animasyonu tetiklenmeyecek.", this);
        }
        public void WalkToTarget()
        {
            StartCoroutine(WalkToTargetRoutine());
        }

        public IEnumerator WalkToTargetRoutine()
        {
            if (target == null) yield break;

            if (_animator != null) _animator.SetBool("IsMoving", true);

            while (Vector3.Distance(transform.position, target.position) > stopDistance)
            {
                Vector3 direction = target.position - transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.0001f)
                {
                    transform.rotation = Quaternion.LookRotation(direction.normalized);
                    transform.position += direction.normalized * speed * Time.deltaTime;
                }
                yield return null;
            }

            if (_animator != null) _animator.SetBool("IsMoving", false);
        }

        private bool _isWalkingIndefinitely;

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
                    transform.rotation = Quaternion.LookRotation(direction.normalized);
                    transform.position += direction.normalized * speed * Time.deltaTime;
                }
                yield return null;
            }

            if (_animator != null) _animator.SetBool("IsMoving", false);
        }
    }
}