using System;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace NekoOdyssey.Scripts.Game.Unity.Player
{
    public class PlayerController : MonoBehaviour
    {
        [FormerlySerializedAs("blurPlane")]
        [Header("Game Objects")]
        public GameObject phoneBlurPlane;
        public GameObject phoneScreen;
        public GameObject captureBlurPlane;
        public GameObject captureScreen;

        [Space] [Header("Movement Speed")] public float moveSpeed = 1.5f;
        public float boostMultiplier = 1.5f;
        public float gravity = -9.81f;
        [SerializeField] public float gravityMultiplier = 1; // 3.0f;

        private PlayerMovementController _movementController;
        private PlayerPhoneController _phoneController;
        private PlayerCaptureController _captureController;

        private void Awake()
        {
            GameRunner.Instance.GameCore.Player.GameObject = gameObject;
            _movementController = gameObject.AddComponent<PlayerMovementController>();
            _phoneController = gameObject.AddComponent<PlayerPhoneController>();
            _captureController = gameObject.AddComponent<PlayerCaptureController>();
        }

        private void Start()
        {
            var playerAnchor = FindAnyObjectByType<PlayerAnchor>();
            if (playerAnchor != null && Camera.main != null)
            {
                transform.position = playerAnchor.transform.position;
            }
        }

        private void ResetTurnAround()
        {
            _movementController.ResetTurnAround();
        }
    }
}