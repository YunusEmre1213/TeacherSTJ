using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OgretmenGorevSistemi.Character
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerControls _controls;
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public event Action InteractPressed;
        public event Action HelpRequested;

        private bool _interactPressedFlag;

        private void Awake()
        {
            _controls = new PlayerControls();

            _controls.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            _controls.Player.Move.canceled += _ => MoveInput = Vector2.zero;

            _controls.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
            _controls.Player.Look.canceled += _ => LookInput = Vector2.zero;

            _controls.Player.Interact.performed += _ =>
            {
                InteractPressed?.Invoke();
                _interactPressedFlag = true;
            };
            _controls.Player.Help.performed += _ => HelpRequested?.Invoke();
        }

        private void OnEnable() => _controls.Player.Enable();
        private void OnDisable() => _controls.Player.Disable();

        public bool ConsumeInteractPressed()
        {
            if (_interactPressedFlag)
            {
                _interactPressedFlag = false;
                return true;
            }
            return false;
        }
    }
}