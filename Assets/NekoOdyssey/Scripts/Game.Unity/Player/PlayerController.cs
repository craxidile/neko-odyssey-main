using System;
using Cinemachine;
using NekoOdyssey.Scripts.Database.Commons.Models;
using NekoOdyssey.Scripts.Game.Unity.Player.Bag;
using NekoOdyssey.Scripts.Game.Unity.Player.Cameras;
using NekoOdyssey.Scripts.Game.Unity.Player.Capture;
using NekoOdyssey.Scripts.Game.Unity.Player.Conversations;
using NekoOdyssey.Scripts.Game.Unity.Player.Movement;
using NekoOdyssey.Scripts.Game.Unity.Player.Petting;
using NekoOdyssey.Scripts.Game.Unity.Player.Phone;
using NekoOdyssey.Scripts.Game.Unity.Player.EndDay;
using NekoOdyssey.Scripts.Game.Unity.Player.Feed;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace NekoOdyssey.Scripts.Game.Unity.Player
{
    public class PlayerController : MonoBehaviour
    {
        //Start here
        //public static Vector3 MainPlayerAnchor = new(75, -1.6f, -9.5f); 

        //Next
        // public static Vector3 MainPlayerAnchor = new(60, -1.6f, -11f);       


        //BackUp Location
        public static Vector3 MainPlayerAnchor = new(25, -1.662279f, -25.688f);

        // public static Vector3 MainPlayerAnchor = new(28, -1.662279f, -41.5f);
        private PlayerMovementController _movementController;
        private ZoomCameraController _zoomCameraController;
        private PlayerPhoneController _phoneController;
        private PlayerBagController _bagController;
        private PlayerCaptureController _captureController;
        private PlayerConversationController _conversationController;
        private PlayerPettingController _pettingController;
        private PlayerFeedController _feedController;

        private Animator _animator;
        public Animator Animator => _animator;
        private RuntimeAnimatorController _baseRuntimeAnimator;

        public GameObject phoneCameraAnchor;
        public GameObject bagCameraAnchor;
        public GameObject catPhotoContainer;

        [Space][Header("Movement Speed")] public float moveSpeed = 1.5f;
        public float boostMultiplier = 1.5f;
        public float gravity = -9.81f;
        public float gravityMultiplier = 1f; // 3.0f;

        private void Awake()
        {
            GameRunner.Instance.Core.Player.GameObject = gameObject;
            _movementController = gameObject.AddComponent<PlayerMovementController>();
            _zoomCameraController = gameObject.AddComponent<ZoomCameraController>();
            _phoneController = gameObject.AddComponent<PlayerPhoneController>();
            _bagController = gameObject.AddComponent<PlayerBagController>();
            _captureController = gameObject.AddComponent<PlayerCaptureController>();
            _conversationController = gameObject.AddComponent<PlayerConversationController>();
            _pettingController = gameObject.AddComponent<PlayerPettingController>();
            _feedController = gameObject.AddComponent<PlayerFeedController>();
            gameObject.AddComponent<PlayerEndDayController>();

            _animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
            _baseRuntimeAnimator = _animator.runtimeAnimatorController;

            InitializePosition();
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.OnShakeHead
                .Subscribe(_ => HandleHeadShake())
                .AddTo(this);
        }

        private void InitializePosition()
        {
            var positionSet = false;

            var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
            if (currentSite == null) return;

            if (
                currentSite.PlayerX != null ||
                currentSite.PlayerY != null ||
                currentSite.PlayerZ != null
            )
            {
                _movementController.ForceSetPosition(
                    new Vector3(currentSite.PlayerX.Value, currentSite.PlayerY.Value, currentSite.PlayerZ.Value)
                );
                positionSet = true;
            }

            Debug.Log($">>player_facing<< {currentSite.PlayerFacing}");
            if (currentSite.PlayerFacing != null)
            {
                var cameraOffsetX = 0f;
                var cameraOffsetZ = 0f;
                var cameraRotationY = 0f;
                switch (currentSite.PlayerFacingDirection)
                {
                    case FacingDirection.West:
                        cameraOffsetX = 0f;
                        cameraOffsetZ = 5f;
                        cameraRotationY = 180f;
                        break;
                    case FacingDirection.South:
                        cameraOffsetX = -5f;
                        cameraOffsetZ = 0f;
                        cameraRotationY = 90f;
                        break;
                    case FacingDirection.East:
                        cameraOffsetX = 0f;
                        cameraOffsetZ = -5f;
                        cameraRotationY = 0;
                        break;
                    case FacingDirection.North:
                        cameraOffsetX = 5f;
                        cameraOffsetZ = 0f;
                        cameraRotationY = -90f;
                        break;
                    default:
                        break;
                }

                var playerRotation = transform.eulerAngles;
                playerRotation.y = cameraRotationY;
                _movementController.ForceSetRotation(playerRotation);

                var cameraRotation = GameRunner.Instance.cameras.mainVirtualCamera.transform.eulerAngles;
                cameraRotation.y = cameraRotationY;
                GameRunner.Instance.cameras.mainVirtualCamera.transform.eulerAngles = cameraRotation;

                var virtualCamera = GameRunner.Instance.cameras.mainVirtualCamera;
                var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
                transposer.m_FollowOffset = new Vector3(cameraOffsetX, 0, cameraOffsetZ);
            }

            if (positionSet) return;

            var playerAnchor = FindAnyObjectByType<PlayerAnchor>();
            if (playerAnchor != null)
            {
                _movementController.ForceSetPosition(
                    playerAnchor != null ? playerAnchor.transform.position : MainPlayerAnchor
                );
            }
        }

        private void Update()
        {
            var player = GameRunner.Instance.Core.Player;
            player.SetPosition(transform.position);
        }

        private void ResetTurnAround()
        {
            _movementController.ResetTurnAround();
        }


        public void ResetDialogueAnimator()
        {
            _animator.runtimeAnimatorController = _baseRuntimeAnimator;
        }

        private void HandleHeadShake()
        {
            _animator.SetTrigger($"ShakeHead");
        }
    }
}