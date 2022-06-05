using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace UnityTutorial.Manager
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput PlayerInput;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool Jump { get; private set; }
        public bool Crouch { get; private set; }
        public bool Weapon1 { get; private set; }
        public bool Weapon2 { get; private set; }
        public bool Weapon3 { get; private set; }

        private InputActionMap _currentMap;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _runAction;
        private InputAction _jumpAction;
        private InputAction _crouchAction;
        private InputAction _weapon1Action;
        private InputAction _weapon2Action;
        private InputAction _weapon3Action;


        private void Awake()
        {
            HideCursor();
            _currentMap = PlayerInput.currentActionMap;
            _moveAction = _currentMap.FindAction("Move");
            _lookAction = _currentMap.FindAction("Look");
            _runAction = _currentMap.FindAction("Run");
            _jumpAction = _currentMap.FindAction("Jump");
            _crouchAction = _currentMap.FindAction("Crouch");
            _weapon1Action = _currentMap.FindAction("Weapon1");
            _weapon2Action = _currentMap.FindAction("Weapon2");
            _weapon3Action = _currentMap.FindAction("Weapon3");


            _moveAction.performed += onMove;
            _lookAction.performed += onLook;
            _runAction.performed += onRun;
            _jumpAction.performed += onJump;
            _crouchAction.started += onCrouch;
            _weapon1Action.performed += onWeapon1;
            _weapon2Action.performed += onWeapon2;
            _weapon3Action.performed += onWeapon3;

            _moveAction.canceled += onMove;
            _lookAction.canceled += onLook;
            _runAction.canceled += onRun;
            _jumpAction.canceled += onJump;
            _crouchAction.canceled += onCrouch;
            _weapon1Action.canceled += onWeapon1;
            _weapon2Action.canceled += onWeapon2;
            _weapon3Action.canceled += onWeapon3;
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void onMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }
        private void onLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }
        private void onRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
            //Debug.Log(Run);
        }
        private void onJump(InputAction.CallbackContext context)
        {
            Jump = context.ReadValueAsButton();
            //Debug.Log(Jump);

        }
        private void onCrouch(InputAction.CallbackContext context)
        {
            Crouch = context.ReadValueAsButton();
        }
        private void onWeapon1(InputAction.CallbackContext context)
        {
            Weapon1 = context.ReadValueAsButton();
        }
        private void onWeapon2(InputAction.CallbackContext context)
        {
            Weapon2 = context.ReadValueAsButton();
        }
        private void onWeapon3(InputAction.CallbackContext context)
        {
            Weapon3 = context.ReadValueAsButton();
        }
        private void OnEnable()
        {
            _currentMap.Enable();
        }

        private void OnDisable()
        {
            _currentMap.Disable();
        }

        private void Update()
        {
            //Debug.Log(Jump);
        }

    }
}