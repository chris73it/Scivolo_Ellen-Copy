using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace HeroicArcade.CC
{
    [Serializable]
    public class MoveInputEvent : UnityEvent<Vector2>
    {
    }

    public sealed class InputController : MonoBehaviour
    {
        [SerializeField] MoveInputEvent moveInputEvent;

        Controls controls;
        private void Awake()
        {
            controls = new Controls();

            //isWalkingHash = Animator.StringToHash("isWalking");
            //isRunningHash = Animator.StringToHash("isRunning");
            //isJumpingHash = Animator.StringToHash("isJumping");

            controls.Gameplay.Move.started += OnMove;
            controls.Gameplay.Move.canceled += OnMove;
            controls.Gameplay.Move.performed += OnMove;

            controls.Gameplay.Jump.started += OnJump;
            controls.Gameplay.Jump.canceled += OnJump;

            controls.Gameplay.Shoot.started += OnShoot;
            controls.Gameplay.Shoot.canceled += OnShoot;
            //controls.Gameplay.Shoot.performed += OnShoot;

            controls.Gameplay.Sprint.started += OnSprint;
            controls.Gameplay.Sprint.canceled += OnSprint;
            //controls.Gameplay.Sprint.performed += OnSprint;

            controls.Gameplay.Aim.started += OnAim;
            controls.Gameplay.Aim.canceled += OnAim;
            //controls.Gameplay.Aim.performed += OnAim;

            controls.Gameplay.AimSwitch.started += OnAimSwitch;
            //controls.Gameplay.AimSwitch.canceled += OnAimSwitch;
            //controls.Gameplay.AimSwitch.performed += OnAimSwitch;

            //controls.Gameplay.WeaponSelection.started += OnWeaponSelection;
            //controls.Gameplay.WeaponSelection.canceled += OnWeaponSelection;
            controls.Gameplay.WeaponSelection.performed += OnWeaponSelection;
        }

        private Vector2 moveInput;
        [HideInInspector] public bool IsMovePressed;
        private void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
            IsMovePressed = moveInput != Vector2.zero;
            //Debug.Log($"IsMovePressed {IsMovePressed}");
            moveInputEvent.Invoke(moveInput);
        }

        [HideInInspector] public bool IsJumpPressed;
        private void OnJump(InputAction.CallbackContext context)
        {
            IsJumpPressed = context.ReadValueAsButton();
        }

        [HideInInspector] public bool IsShootPressed;
        private void OnShoot(InputAction.CallbackContext context)
        {
            //Debug.Log("OnShoot");
            //IsShootPressed = !IsShootPressed;
            IsShootPressed = context.ReadValueAsButton();
            //IsShootPressed = context.ReadValueAsButton();
        }

        [HideInInspector] public bool IsSprintPressed;
        private void OnSprint(InputAction.CallbackContext context)
        {
            IsSprintPressed = context.ReadValueAsButton();
        }

        [HideInInspector] public bool IsAimingPressed;
        private void OnAim(InputAction.CallbackContext context)
        {
            //Debug.Log("OnAim");
            IsAimingPressed = context.ReadValueAsButton();
        }

        [HideInInspector] public bool IsAimSwitchingPressed;
        private void OnAimSwitch(InputAction.CallbackContext context)
        {
            IsAimSwitchingPressed = context.ReadValueAsButton();
            Debug.Log("IsAimSwitchingPressed: " + IsAimSwitchingPressed);
        }

        [HideInInspector] public float WeaponSelected;
        private void OnWeaponSelection(InputAction.CallbackContext context)
        {
            WeaponSelected = context.ReadValue<float>();
        }

        private void OnEnable()
        {
            controls.Gameplay.Enable();
        }

        private void OnDisable()
        {
            controls.Gameplay.Disable();
        }
    }
}