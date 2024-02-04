﻿using System;
using NekoOdyssey.Scripts.Game.Unity.Player.Capture;
using NekoOdyssey.Scripts.Game.Unity.Player.Conversations;
using NekoOdyssey.Scripts.Game.Unity.Player.Movement;
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
        public static readonly Vector3 MainPlayerAnchor = new(25, -1.662279f, -25.688f);

        private PlayerMovementController _movementController;
        private PlayerPhoneController _phoneController;
        private PlayerCaptureController _captureController;
        private PlayerConversationController _conversationController;
        
        public GameObject phoneScreen;
        public GameObject captureBlurPlane;
        public GameObject captureScreen;
        public GameObject catPhotoContainer;
        public GameObject catPhoto;

        [Space]
        [Header("Movement Speed")]
        public float moveSpeed = 1.5f;
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

            var playerAnchor = FindAnyObjectByType<PlayerAnchor>();
            var aa = playerAnchor == null ? Vector3.zero : playerAnchor.transform.position;
            if (playerAnchor != null)
            {
                _movementController.ForceSetPosition(playerAnchor.transform.position);
            }
            else 
            {
                _movementController.ForceSetPosition(MainPlayerAnchor);
            }
        }

        private void ResetTurnAround()
        {
            _movementController.ResetTurnAround();
        }
    }
}