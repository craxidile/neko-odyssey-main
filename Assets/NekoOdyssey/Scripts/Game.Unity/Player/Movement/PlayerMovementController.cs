using Assets.NekoOdyssey.Scripts.Audio;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;
using FMOD.Studio;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Movement
{
    public class PlayerMovementController : MonoBehaviour
    {
        private bool _active;
        private SpriteRenderer _sprite;
        private Animator _animator;
        private Vector2 _moveInput;
        private Vector2 _lastInput;
        private Vector2 _standInput;
        private Vector3 _moveDirection;
        private Vector2 _inertiaMoveInput = Vector2.zero;
        private float _gravity;
        private float _gravityMultiplier;
        private float _moveSpeed;
        private float _boostMultiplier;
        private bool _forcePosition;
        private bool _isMoving;
        private bool _stopMove;
        private bool _isTurnAround;
        private float? _currentSpeed;
        
        //audio
        private EventInstance playerFootSteps;

        private CharacterController _characterController;
        private bool Running => GameRunner.Instance.Core.Player.Running;

        private void SetActive(PlayerMode mode)
        {
            _active = mode == PlayerMode.Move;

            if (!_active || !_animator) return;
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Phone"), 0f);
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Bag"), 0f);
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Capture"), 0f);
        }
        
        private void Awake()
        {
            var playerController = GameRunner.Instance.Core.Player.GameObject.GetComponent<PlayerController>();
            _gravity = playerController.gravity;
            _gravityMultiplier = playerController.gravityMultiplier;
            _moveSpeed = playerController.moveSpeed;
            _boostMultiplier = playerController.boostMultiplier;
            _animator = playerController.GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            _sprite = GetComponent<SpriteRenderer>();
        }

        public void ForceSetPosition(Vector3 position)
        {
            _characterController.enabled = false;
            transform.position = position;
            _characterController.enabled = true;
        }

        public void ForceSetRotation(Vector3 rotation)
        {
            _characterController.enabled = false;
            transform.eulerAngles = rotation;
            _characterController.enabled = true;
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.OnMove.Subscribe(input => { _moveInput = input; });
            GameRunner.Instance.Core.Player.OnChangeMode.Subscribe(SetActive);
            //FMOD add by Ping
            playerFootSteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootSteps);
        }

        private void Update()
        {
            if (GameRunner.Instance.Core.Player.Mode == PlayerMode.Capture) return;
            AnimationsUpdate();
        }

        private void FixedUpdate()
        {
            if (GameRunner.Instance.Core.Player.Mode == PlayerMode.Capture) return;
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            // Input
            // moveInput = playerControls.Player.Movement.ReadValue<Vector2>();
            var directionChanged =
                (int)_moveInput.x != 0 && (int)_lastInput.x != 0 && (int)_lastInput.x == (int)-_moveInput.x;

            // Turn Around
            if (!_isTurnAround && Running && directionChanged)
            {
                // stopMove = true;
                _isTurnAround = true;
                _animator.SetTrigger($"Move Reverse");
            }

            // Set last direction for Animation Idle
            if (_moveInput.x != 0 || _moveInput.y != 0)
            {
                _lastInput = _moveInput;
            }

            if (_moveInput.x != 0 || _moveInput.y != 0)
            {
                _standInput = _moveInput;
            }

            var mainCamera = GameRunner.Instance.cameras.mainCamera;
            if (!mainCamera) return;
            var cameraTransform = mainCamera.transform;

            // Set move direction towards camera
            _moveDirection = _inertiaMoveInput != Vector2.zero
                ? new Vector3(_inertiaMoveInput.x, 0f, _inertiaMoveInput.y).normalized
                : new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;

            _moveDirection = cameraTransform.forward * _moveDirection.z + cameraTransform.right * _moveDirection.x;
            if (_characterController.isGrounded && _moveDirection.y < 0.0f)
                _moveDirection.y = -1.0f;
            else
                _moveDirection.y += _gravity * _gravityMultiplier * Time.deltaTime;

            var multiplier = 0f;

            // Stop pressing controller
            if (!_isTurnAround && (_moveInput.x != 0 || _moveInput.y != 0))
            {
                _isMoving = true;
                _currentSpeed = null;
                _inertiaMoveInput = _moveInput;
                multiplier = !Running ? _moveSpeed : _moveSpeed * _boostMultiplier;
            }
            else
            {
                switch (_currentSpeed)
                {
                    case null when _inertiaMoveInput != Vector2.zero:
                        _currentSpeed = !Running ? _moveSpeed : _moveSpeed * _boostMultiplier;
                        break;
                    case <= 0:
                        multiplier = 0;
                        if (_isTurnAround) return;
                        _isMoving = false;
                        _inertiaMoveInput = Vector2.zero;
                        _lastInput = _moveInput;
                        break;
                    case > 0 when _currentSpeed != null:
                        _currentSpeed -= !Running ? 0.005f : 0.002f;
                        multiplier = _currentSpeed.Value;
                        break;
                }
                // multiplier = 0;
            }


            var targetMove = new Vector3(_moveDirection.x * multiplier, _moveDirection.y,
                _moveDirection.z * multiplier);
            _characterController.Move(targetMove);

            FlipSprite();
        }

        private void AnimationsUpdate()
        {
            _animator.SetFloat($"Input X", _inertiaMoveInput != Vector2.zero ? _inertiaMoveInput.x : _moveInput.x);
            _animator.SetFloat($"Input Y", _inertiaMoveInput != Vector2.zero ? _inertiaMoveInput.y : _moveInput.y);
            _animator.SetFloat($"Last Input X", _lastInput.x);
            _animator.SetFloat($"Last Input Y", _lastInput.y);
            _animator.SetFloat($"Stand Input X", _standInput.x);
            _animator.SetFloat($"Stand Input Y", _standInput.y);
            _animator.SetBool($"Move", _isMoving || _inertiaMoveInput != Vector2.zero);
            _animator.SetBool($"Run", Running);
            // animator.SetBool($"Stop Move", stopMove);
            _animator.SetBool($"Stop Move", _isTurnAround);
            _animator.SetBool($"Turn Around", _isTurnAround);
        }

        private void FlipSprite()
        {
            switch (_moveInput.x)
            {
                case > 0:
                    _sprite.flipX = _isTurnAround;
                    break;
                case < 0:
                    _sprite.flipX = !_isTurnAround;
                    break;
                default:
                {
                    if (_moveInput.y != 0 && _inertiaMoveInput == Vector2.zero)
                    {
                        _sprite.flipX = false;
                    }

                    break;
                }
            }
        }

        private void UpdateSound()
        {
            //Start footsteps event if the player has an x or y movement
            if (_moveInput.x != 0 || _moveInput.y != 0)
            {
                //get the playback state
                PLAYBACK_STATE playbackState;
                playerFootSteps.getPlaybackState(out playbackState);
                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                {
                    playerFootSteps.start();
                }
            }
            //Otherwise, stop the footsteps event
            else
            {
                playerFootSteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }

        public void ResetTurnAround()
        {
            _isTurnAround = false;
            _inertiaMoveInput = Vector2.zero;
            _lastInput = _moveInput;
            AnimationsUpdate();
            FlipSprite();
        }
    }
}