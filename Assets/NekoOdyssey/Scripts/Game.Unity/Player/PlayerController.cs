﻿using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player
{
    public class PlayerController : MonoBehaviour
    {
        //Input Action Reference
        private PlayerInputActions playerControls;

        //Value movement
        public float moveSpeed = 1.5f;
        public float boostMultiplier = 1.5f;

        private float _gravity = -9.81f;
        [SerializeField] private float gravityMultiplier = 1; // 3.0f;

        private float _velocity;

        //turn around timing
        public float turningTimerSet;
        private float turningTimerCounter;

        private Vector2 moveInput;
        private Vector2 lastInput;
        private Vector3 moveDirection;

        private SpriteRenderer sprite;

        // private Rigidbody rb;
        private CharacterController cc;

        private bool isMoving;
        private bool stopMove;
        private bool isTurnAround;
        private bool stillRunning = false;

        private Animator animator;

        private float? currentSpeed;
        private Vector2 inertiaMoveInput = Vector2.zero;

        private void Awake()
        {
            playerControls = new PlayerInputActions();
            turningTimerCounter = turningTimerSet;
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void Start()
        {
            // rb = GetComponent<Rigidbody>();
            cc = GetComponent<CharacterController>();
            sprite = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            Debug.Log($">>character_controller<< {cc}");
        }

        private void Update()
        {
            RotateSprite();
            RunMode();
            AnimationsUpdate();
        }


        private void FixedUpdate()
        {
            ApplyMovement();
        }

        public void RunMode()
        {
            if (playerControls.Player.Run.triggered)
            {
                stillRunning = !stillRunning;
                Debug.Log("Run : " + stillRunning);
            }
        }

        private void ApplyMovement()
        {
            // Input
            moveInput = playerControls.Player.Movement.ReadValue<Vector2>();
            var directionChanged = (int)lastInput.x == (int)-moveInput.x;

            //Turn Around
            if (!isTurnAround && moveInput.x != 0 && directionChanged)
            {
                Debug.Log(">>turn_around<<");
                turningTimerCounter = stillRunning ? 0.6f : 0.4f;
                stopMove = true;
                isTurnAround = true;
                animator.SetTrigger($"Move Reverse");
            }

            //Set last direction for Animation Idle
            if (
                (moveInput.x != 0 || moveInput.y != 0) &&
                (moveDirection.x == 0 || moveDirection.z == 0) || isTurnAround
            )
            {
                lastInput = moveInput;
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
            if (cc.isGrounded && moveDirection.y < 0.0f)
            {
                moveDirection.y = -1.0f;
            }
            else
            {
                moveDirection.y += _gravity * gravityMultiplier * Time.deltaTime;
            }

            var multiplier = 0f;

            // Stop pressing controller
            if (!isTurnAround && (moveInput.x != 0 || moveInput.y != 0))
            {
                isMoving = true;
                currentSpeed = null;
                inertiaMoveInput = moveInput;
                multiplier = !stillRunning ? moveSpeed : moveSpeed * boostMultiplier;
            }
            else
            {
                switch (currentSpeed)
                {
                    case null when inertiaMoveInput != Vector2.zero:
                        currentSpeed = !stillRunning ? moveSpeed : moveSpeed * boostMultiplier;
                        Debug.Log($">>current_speed<< {currentSpeed} {inertiaMoveInput}");
                        break;
                    case <= 0:
                        multiplier = 0;
                        if (!isTurnAround)
                        {
                            isMoving = false;
                            inertiaMoveInput = Vector2.zero;
                            lastInput = moveInput;
                        }

                        break;
                    case > 0 when currentSpeed != null:
                        currentSpeed -= !stillRunning ? 0.005f : 0.002f;
                        Debug.Log($">>current_speed<< {currentSpeed}");
                        multiplier = currentSpeed.Value;
                        break;
                }
                // multiplier = 0;
            }


            var targetMove = new Vector3(
                moveDirection.x * multiplier,
                moveDirection.y,
                moveDirection.z * multiplier
            );
            cc.Move(targetMove);

            FlipSprite();
        }

        private void AnimationsUpdate()
        {
            animator.SetFloat($"Input X", inertiaMoveInput != Vector2.zero ? inertiaMoveInput.x : moveInput.x);
            animator.SetFloat($"Input Y", inertiaMoveInput != Vector2.zero ? inertiaMoveInput.y : moveInput.y);
            animator.SetFloat($"Last Input X", lastInput.x);
            animator.SetFloat($"Last Input Y", lastInput.y);
            animator.SetBool($"Move", isMoving || inertiaMoveInput != Vector2.zero);
            animator.SetBool($"Run", stillRunning);
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
        
        void OnGUI()
        {
            if (Application.isEditor)  // or check the app debug flag
            {
                GUI.Label(new Rect(0, 0, 300, 300), $"turn_around={isTurnAround}\nmove_input={moveInput}\ninertia={inertiaMoveInput}\naa={aa}");
            }
        }

        private string aa = "";

        private void ResetTurnAround()
        {
            aa = "yo";
            Debug.Log($">>reset_turn_around<<");
            isTurnAround = false;
            inertiaMoveInput = Vector2.zero;
            lastInput = moveInput;
            AnimationsUpdate();
            FlipSprite();
            // animator.ResetTrigger($"Move Reverse");
        }
    }
}