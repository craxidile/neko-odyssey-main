using System;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UniRx.Triggers;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        private bool _active;

        private Vector2 moveInput;
        private Vector2 lastInput;
        private Vector2 standInput;
        private Vector3 moveDirection;

        private SpriteRenderer sprite;

        // private Rigidbody rb;
        private CharacterController _characterController;

        private bool isMoving;
        private bool stopMove;
        private bool isTurnAround;

        private bool Running => GameRunner.Instance.GameCore.Player.Running;

        private Animator animator;

        private float? currentSpeed;
        private Vector2 inertiaMoveInput = Vector2.zero;

        private Animator _animator;
        private float _gravity;
        private float _gravityMultiplier;
        private float _moveSpeed;
        private float _boostMultiplier;

        private void SetActive(PlayerMode mode)
        {
            _active = mode == PlayerMode.Move;

            if (!_active) return;
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Phone"), 0f);
        }

        private void Awake()
        {
            var playerController = GameRunner.Instance.GameCore.Player.GameObject.GetComponent<PlayerController>();
            _gravity = playerController.gravity;
            _gravityMultiplier = playerController.gravityMultiplier;
            _moveSpeed = playerController.moveSpeed;
            _boostMultiplier = playerController.boostMultiplier;
            _animator = playerController.GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            sprite = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            GameRunner.Instance.GameCore.Player.OnMove.Subscribe(input => { moveInput = input; });
            GameRunner.Instance.GameCore.Player.OnChangeMode.Subscribe(SetActive);
        }

        private void Update()
        {
            RotateSprite();
            AnimationsUpdate();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            // Input
            // moveInput = playerControls.Player.Movement.ReadValue<Vector2>();
            var directionChanged =
                (int)moveInput.x != 0 && (int)lastInput.x != 0 && (int)lastInput.x == (int)-moveInput.x;

            // Turn Around
            if (!isTurnAround && Running && directionChanged)
            {
                // stopMove = true;
                isTurnAround = true;
                animator.SetTrigger($"Move Reverse");
            }

            // Set last direction for Animation Idle
            if (moveInput.x != 0 || moveInput.y != 0)
            {
                lastInput = moveInput;
            }

            if (moveInput.x != 0 || moveInput.y != 0)
            {
                standInput = moveInput;
            }

            if (Camera.main == null) return;
            var cameraTransform = Camera.main.transform;

            // Set move direction towards camera
            if (inertiaMoveInput != Vector2.zero)
            {
                moveDirection = new Vector3(inertiaMoveInput.x, 0f, inertiaMoveInput.y).normalized;
            }
            else
            {
                moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            }

            moveDirection = cameraTransform.forward * moveDirection.z + cameraTransform.right * moveDirection.x;
            if (_characterController.isGrounded && moveDirection.y < 0.0f)
            {
                moveDirection.y = -1.0f;
            }
            else
            {
                moveDirection.y += _gravity * _gravityMultiplier * Time.deltaTime;
            }

            var multiplier = 0f;

            // Stop pressing controller
            if (!isTurnAround && (moveInput.x != 0 || moveInput.y != 0))
            {
                isMoving = true;
                currentSpeed = null;
                inertiaMoveInput = moveInput;
                multiplier = !Running ? _moveSpeed : _moveSpeed * _boostMultiplier;
            }
            else
            {
                switch (currentSpeed)
                {
                    case null when inertiaMoveInput != Vector2.zero:
                        currentSpeed = !Running ? _moveSpeed : _moveSpeed * _boostMultiplier;
                        break;
                    case <= 0:
                        multiplier = 0;
                        if (isTurnAround) return;
                        isMoving = false;
                        inertiaMoveInput = Vector2.zero;
                        lastInput = moveInput;
                        break;
                    case > 0 when currentSpeed != null:
                        currentSpeed -= !Running ? 0.005f : 0.002f;
                        multiplier = currentSpeed.Value;
                        break;
                }
                // multiplier = 0;
            }


            var targetMove = new Vector3(moveDirection.x * multiplier, moveDirection.y, moveDirection.z * multiplier);
            _characterController.Move(targetMove);

            FlipSprite();
        }

        private void AnimationsUpdate()
        {
            animator.SetFloat($"Input X", inertiaMoveInput != Vector2.zero ? inertiaMoveInput.x : moveInput.x);
            animator.SetFloat($"Input Y", inertiaMoveInput != Vector2.zero ? inertiaMoveInput.y : moveInput.y);
            animator.SetFloat($"Last Input X", lastInput.x);
            animator.SetFloat($"Last Input Y", lastInput.y);
            animator.SetFloat($"Stand Input X", standInput.x);
            animator.SetFloat($"Stand Input Y", standInput.y);
            animator.SetBool($"Move", isMoving || inertiaMoveInput != Vector2.zero);
            animator.SetBool($"Run", Running);
            // animator.SetBool($"Stop Move", stopMove);
            animator.SetBool($"Stop Move", isTurnAround);
            animator.SetBool($"Turn Around", isTurnAround);
            // if (isTurnAround)
            // {
            //     animator.SetTrigger($"Move Reverse");
            // }
        }


        private void RotateSprite()
        {
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }

        private void FlipSprite()
        {
            switch (moveInput.x)
            {
                case > 0:
                    sprite.flipX = isTurnAround;
                    break;
                case < 0:
                    sprite.flipX = !isTurnAround;
                    break;
                default:
                {
                    if (moveInput.y != 0 && inertiaMoveInput == Vector2.zero)
                    {
                        sprite.flipX = false;
                    }

                    break;
                }
            }
        }

        public void ResetTurnAround()
        {
            isTurnAround = false;
            inertiaMoveInput = Vector2.zero;
            lastInput = moveInput;
            AnimationsUpdate();
            FlipSprite();
        }
    }
}