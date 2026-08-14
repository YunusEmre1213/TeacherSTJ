using UnityEngine;

namespace OgretmenGorevSistemi.Character
{
    [RequireComponent(typeof(Animator))]
    public class HeadLookAt : MonoBehaviour
    {
        [SerializeField] private Transform lookTarget;

        [Range(0f, 1f)]
        [SerializeField] private float lookWeight = 1f;

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

     
        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null || lookTarget == null) return;

            _animator.SetLookAtWeight(lookWeight, 0.2f, 0.9f, 1.0f, 0.5f);

           
            _animator.SetLookAtPosition(lookTarget.position);
        }
    }
}