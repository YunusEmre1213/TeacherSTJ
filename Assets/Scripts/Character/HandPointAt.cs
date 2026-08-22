using UnityEngine;

namespace OgretmenGorevSistemi.Character
{
    [RequireComponent(typeof(Animator))]
    public class HandPointAt : MonoBehaviour
    {
        [SerializeField] private AvatarIKGoal hand = AvatarIKGoal.RightHand;

        [Tooltip("Kolun uzanma baþlangýç noktasý (omuz civarý) — boþ býrakýlýrsa Animator'dan otomatik bulunur.")]
        [SerializeField] private Transform shoulderReference;

        [Tooltip("Elin hedefe doðru ne kadar uzatýlacaðý (metre).")]
        [SerializeField] private float reachDistance = 0.6f;

        [SerializeField] private float weight = 1f;
        [SerializeField] private float weightSmoothTime = 0.3f;

        private Animator _animator;
        private Transform _pointTarget;
        private float _currentWeight;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.LogWarning("[HandPointAt] Bu objede Animator yok — script'in Animator ile AYNI objede olmasý gerekiyor.", this);
                return;
            }

            if (shoulderReference == null)
                shoulderReference = _animator.GetBoneTransform(
                    hand == AvatarIKGoal.RightHand ? HumanBodyBones.RightUpperArm : HumanBodyBones.LeftUpperArm);
        }
        public void SetTarget(Transform newTarget) => _pointTarget = newTarget;

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null) return;

            float targetWeight = _pointTarget != null ? weight : 0f;
            float smoothing = weightSmoothTime > 0f ? Time.deltaTime / weightSmoothTime : 1f;
            _currentWeight = Mathf.Lerp(_currentWeight, targetWeight, smoothing);

            _animator.SetIKPositionWeight(hand, _currentWeight);

            if (_pointTarget != null)
            {
                Vector3 origin = shoulderReference != null ? shoulderReference.position : transform.position;
                Vector3 direction = (_pointTarget.position - origin).normalized;
                _animator.SetIKPosition(hand, origin + direction * reachDistance);
            }
        }
    }
}