using System;
using NekoOdyssey.Scripts.Game.Unity.Player.Bag;
using NekoOdyssey.Scripts.Game.Unity.Player.Cameras;
using NekoOdyssey.Scripts.Game.Unity.Player.Capture;
using NekoOdyssey.Scripts.Game.Unity.Player.Conversations;
using NekoOdyssey.Scripts.Game.Unity.Player.Movement;
using NekoOdyssey.Scripts.Game.Unity.Player.Petting;
using NekoOdyssey.Scripts.Game.Unity.Player.Phone;
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

        public GameObject phoneCameraAnchor;
        public GameObject bagCameraAnchor;
        public GameObject catPhotoContainer;

        [Space] [Header("Movement Speed")] public float moveSpeed = 1.5f;
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

            InitializePosition();
        }

        private void InitializePosition()
        {
            var currentSite = Site.Core.Site.Site.CurrentSite;
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
                return;
            }

            var playerAnchor = FindAnyObjectByType<PlayerAnchor>();
            _movementController.ForceSetPosition(
                playerAnchor != null ? playerAnchor.transform.position : MainPlayerAnchor
            );
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
    }
}