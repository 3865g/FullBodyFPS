using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTutorial.Manager;

namespace UnityTutorial.PlayerControl
{
    public class PlayerController : MonoBehaviour
    {       

        [SerializeField] private float AnimBlendSpeed = 8.9f;
        [SerializeField] private Transform CameraRoot;
        [SerializeField] private Transform Camera;
        [SerializeField] private float UpperLimit = -40f;
        [SerializeField] private float BottomLimit = 70f;
        [SerializeField] private float MouseSensitivity = 21.9f;
        [SerializeField] private float JumpFactor = 1;
        [SerializeField] private float Dis2Ground = 0.9f;
        [SerializeField] private LayerMask GroundCheck;
        [SerializeField] private float AirResistance = 0.8f;

        private Rigidbody _playerRigidbody;
        private InputManager _inputManager;
        private Animator _animator;
        private bool _hasAnimator;
        private bool _grounded = false;
        //private bool _freeHandHash;
        //private bool _pistolHandHash;
       // private bool _rifleHandHash;
        private int _xVelHash;
        private int _yVelHash;
        //(z for blend after jump if u run or walk)
        private int _zVelHash;
        private int _jumpHash;
        private int _groundHash;
        private int _fallingHash;
        private int _crouchHash;        
        private float _xRotation;
        

        private const float _walkSpeed = 2f;
        private const float _runSpeed = 6f;
        private Vector2 _currentVelocity;
        


        private void Start() {
            _hasAnimator = TryGetComponent<Animator>(out _animator);
            _playerRigidbody = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputManager>();


            _xVelHash = Animator.StringToHash("xVelocity");
            _yVelHash = Animator.StringToHash("yVelocity");
            _jumpHash = Animator.StringToHash("Jump");
            _groundHash = Animator.StringToHash("Grounded");
            _fallingHash = Animator.StringToHash("Falling");
            _crouchHash = Animator.StringToHash("Crouch");
            

        }

        private void FixedUpdate() 
        {
            SampleGround();
            Move();
            HandleJump();
            HandleCrouch();
            Weapon1State();
            Weapon2State();
            Weapon3State();
            



        }
        private void LateUpdate() {
            CamMovements();
        }

        private void Move()
        {
            if (!_hasAnimator) return;

            float targetSpeed = _inputManager.Run ? _runSpeed : _walkSpeed;
            if (_inputManager.Crouch) targetSpeed = 1.5f;
            if (_inputManager.Move == Vector2.zero) targetSpeed = 0;

            if (_grounded)
            {

                _currentVelocity.x = Mathf.Lerp(_currentVelocity.x, _inputManager.Move.x * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);
                _currentVelocity.y = Mathf.Lerp(_currentVelocity.y, _inputManager.Move.y * targetSpeed, AnimBlendSpeed * Time.fixedDeltaTime);

                var xVelDifference = _currentVelocity.x - _playerRigidbody.velocity.x;
                var zVelDifference = _currentVelocity.y - _playerRigidbody.velocity.z;

                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(xVelDifference, 0, zVelDifference)), ForceMode.VelocityChange);
            }
            else
            {
                _playerRigidbody.AddForce(transform.TransformVector(new Vector3(_currentVelocity.x * AirResistance, 0, _currentVelocity.y * AirResistance)), ForceMode.VelocityChange);
            }


            _animator.SetFloat(_xVelHash, _currentVelocity.x);
            _animator.SetFloat(_yVelHash, _currentVelocity.y);
        }

        private void CamMovements()
        {
            if (!_hasAnimator) return;

            var Mouse_X = _inputManager.Look.x;
            var Mouse_Y = _inputManager.Look.y;
            Camera.position = CameraRoot.position;

            _xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            Camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            _playerRigidbody.MoveRotation(_playerRigidbody.rotation * Quaternion.Euler(0, Mouse_X * MouseSensitivity * Time.smoothDeltaTime, 0));
        }

        private void HandleCrouch() => _animator.SetBool(_crouchHash, _inputManager.Crouch);
        private void HandleJump()
        {
            if (!_hasAnimator) return;
            if (!_inputManager.Jump) return;
            //if (!_grounded) return;

            _animator.SetTrigger(_jumpHash);            
        }

        public void JumpAddForce()
        {
            _playerRigidbody.AddForce(-_playerRigidbody.velocity.y * Vector3.up, ForceMode.VelocityChange);
            _playerRigidbody.AddForce(Vector3.up * JumpFactor, ForceMode.Impulse);
            _animator.ResetTrigger(_jumpHash);
        }

        private void SampleGround()
        {
            if (!_hasAnimator) return;

            RaycastHit hitInfo;
            if (Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out hitInfo, Dis2Ground + 0.1f, GroundCheck))
            {
                //Grounded
                _grounded = true;
                SetAnimationGrounding();
                return;
            }
            //Falling
            _grounded = false;
            _animator.SetFloat(_zVelHash, _playerRigidbody.velocity.y);
            SetAnimationGrounding();
            return;
        }

        private void SetAnimationGrounding()
        {
            _animator.SetBool(_fallingHash, !_grounded);
            _animator.SetBool(_groundHash, _grounded);
        }

        private void SetAnimaLayer()
        {
            bool freeHand = _animator.GetBool("FreeHand");
            if(freeHand)
                _animator.SetLayerWeight(1, 0);
        }

        private void Weapon1State()
        {
            if (!_hasAnimator) return;
            if (_inputManager.Weapon1)
            {
                _animator.SetBool("FreeHand", true);
                _animator.SetBool("PistolHand", false);
                _animator.SetBool("RifleHand", false);
                //_animator.SetLayerWeight(1, 0);
            }
        }
        private void Weapon2State()
        {
            if (!_hasAnimator) return;
            if (_inputManager.Weapon2)
            {
                _animator.SetBool("FreeHand", false);
                _animator.SetBool("PistolHand", true);
                _animator.SetBool("RifleHand", false);
                _animator.SetLayerWeight(1, 1);
            }
        }
        private void Weapon3State()
        {
            if (!_hasAnimator) return;
            if (_inputManager.Weapon3)
            {
                _animator.SetBool("FreeHand", false);
                _animator.SetBool("PistolHand", false);
                _animator.SetBool("RifleHand", true);
                _animator.SetLayerWeight(1, 1);
                
            }
        }



        private void Update()
        {

            //RaycastHit hitInfo;
            //if (Physics.Raycast(_playerRigidbody.worldCenterOfMass, Vector3.down, out hitInfo, Dis2Ground + 0.1f, GroundCheck))
            //    Debug.Log("Raycast work!!!");
            //Debug.Log(_inputManager.Jump);
            //Debug.Log("FreeHand", _animator.GetBool("FreeHand");
            // Debug.DrawRay(_playerRigidbody.worldCenterOfMass, Vector3.down * 100, Color.red);
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.DrawRay(groundCheckActor.transform.position, groundCheckActor.transform.TransformDirection(Vector3.down), Color.red);
        }




    }
}
