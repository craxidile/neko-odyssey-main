using System;
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

        //public static Vector3 MainPlayerAnchor = new(27, -1.6f, -13f);
        //public static Vector3 MainPlayerAnchor = new(25, -1.662279f, -25.688f);
        public static Vector3 MainPlayerAnchor = new(28, -1.662279f, -41.5f);
        private PlayerMovementController _movementController;
        private PlayerPhoneController _phoneController;
        private PlayerCaptureController _captureController;
        private PlayerConversationController _conversationController;
        private PlayerPettingController _pettingController;

        public GameObject phoneScreen;
        public GameObject catPhotoContainer;

        [Space] [Header("Movement Speed")] public float moveSpeed = 1.5f;
        public float boostMultiplier = 1.5f;
        public float gravity = -9.81f;
        public float gravityMultiplier = 1f; // 3.0f;

        private void Awake()
        {
            GameRunner.Instance.Core.Player.GameObject = gameObject;
            _movementController = gameObject.AddComponent<PlayerMovementController>();
            _phoneController = gameObject.AddComponent<PlayerPhoneController>();
            _captureController = gameObject.AddComponent<PlayerCaptureController>();
            _conversationController = gameObject.AddComponent<PlayerConversationController>();
            _pettingController = gameObject.AddComponent<PlayerPettingController>();

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