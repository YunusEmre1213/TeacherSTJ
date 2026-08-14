using UnityEngine;
using OgretmenGorevSistemi.Core;

namespace OgretmenGorevSistemi.Character
{

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInputHandler))]
    public class FPSPlayerController : MonoBehaviour
    {
        
        [SerializeField] private Transform cameraPivot;

     
        public Transform CameraPivot => cameraPivot;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private LayerMask interactableMask = ~0;

        [SerializeField] private Animator animator;

        private CharacterController _controller;
        private PlayerInputHandler _input;
        private float _verticalVelocity;
        private float _pitch;
        private bool _controlsEnabled = true;

        public event System.Action<Transform> Interacted;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputHandler>();
        }

        private void OnEnable()
        {
            _input.InteractPressed += HandleInteractPressed;
            _input.HelpRequested += HandleHelpRequested;
            GameEvents.OnDialogueShown += DisableControl;
            GameEvents.OnDialogueHidden += EnableControl;
            GameEvents.OnHintStarted += DisableControl;
            GameEvents.OnHintFinished += EnableControl;
            GameEvents.OnDemoSequenceStarted += DisableControl;
            GameEvents.OnDemoSequenceFinished += EnableControl;
        }

        private void OnDisable()
        {
            _input.InteractPressed -= HandleInteractPressed;
            _input.HelpRequested -= HandleHelpRequested;
            GameEvents.OnDialogueShown -= DisableControl;
            GameEvents.OnDialogueHidden -= EnableControl;
            GameEvents.OnHintStarted -= DisableControl;
            GameEvents.OnHintFinished -= EnableControl;
            GameEvents.OnDemoSequenceStarted -= DisableControl;
            GameEvents.OnDemoSequenceFinished -= EnableControl;
        }

        private void Update()
        {
            if (!_controlsEnabled) return;
            HandleLook();
            HandleMove();
        }

        private void DisableControl()
        {
            _controlsEnabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _pitch = 0f;
            cameraPivot.localRotation = Quaternion.identity;
        }

        private void EnableControl()
        {
            _controlsEnabled = true;
            _verticalVelocity = 0f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void HandleLook()
        {
            transform.Rotate(Vector3.up, _input.LookInput.x * mouseSensitivity);

            _pitch = Mathf.Clamp(_pitch - _input.LookInput.y * mouseSensitivity, -80f, 80f);
            cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void HandleMove()
        {
            Vector3 move = transform.right * _input.MoveInput.x + transform.forward * _input.MoveInput.y;
            move *= moveSpeed;

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -1f;
            _verticalVelocity += gravity * Time.deltaTime;
            move.y = _verticalVelocity;

            _controller.Move(move * Time.deltaTime);

            if (animator != null)
                animator.SetBool("IsMoving", _input.MoveInput.sqrMagnitude > 0.01f);
        }

        private void HandleInteractPressed()
        {
            if (!_controlsEnabled) return;

            if (Physics.Raycast(cameraPivot.position, cameraPivot.forward, out RaycastHit hit, interactRange, interactableMask))
            {
                Interacted?.Invoke(hit.transform);
            }
        }

        private void HandleHelpRequested()
        {
            if (!_controlsEnabled) return;
            GameEvents.RaiseHelpRequested();
        }
    }
}