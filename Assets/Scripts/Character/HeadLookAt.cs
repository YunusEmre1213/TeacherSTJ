using UnityEngine;

namespace OgretmenGorevSistemi.Character
{
    [RequireComponent(typeof(Animator))]
    public class HeadLookAt : MonoBehaviour
    {
        [Tooltip("Bakýþýn ne kadar güçlü uygulanacaðý ")]
        [SerializeField] private float lookWeight = 0.7f;

        [Tooltip("Aðýrlýðýn hedefe/sýfýra yumuþakça geçme süresi")]
        [SerializeField] private float weightSmoothTime = 0.3f;

        private Animator _animator;
        private Transform _lookTarget;
        private float _currentWeight;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetTarget(Transform newTarget) => _lookTarget = newTarget;

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null) return;

            float targetWeight = _lookTarget != null ? lookWeight : 0f;
            float smoothing = weightSmoothTime > 0f ? Time.deltaTime / weightSmoothTime : 1f;
            _currentWeight = Mathf.Lerp(_currentWeight, targetWeight, smoothing);

            _animator.SetLookAtWeight(_currentWeight);

            if (_lookTarget != null)
            {
                _animator.SetLookAtPosition(_lookTarget.position);
            }
        }
    }
}