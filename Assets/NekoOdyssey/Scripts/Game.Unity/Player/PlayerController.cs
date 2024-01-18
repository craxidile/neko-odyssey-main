using System;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Game Objects")]
        public GameObject blurPlane;
        public GameObject phoneScreen;

        [Space]
        [Header("Movement Speed")]
        public float moveSpeed = 1.5f;
        public float boostMultiplier = 1.5f;
        public float gravity = -9.81f;
        [SerializeField] public float gravityMultiplier = 1; // 3.0f;

        private PlayerMovementController _movementController;
        private PlayerPhoneController _phoneController;

        private void Awake()
        {
            GameRunner.Instance.GameCore.Player.GameObject = gameObject;
            _movementController = gameObject.AddComponent<PlayerMovementController>();
            _phoneController = gameObject.AddComponent<PlayerPhoneController>();
        }

        private void ResetTurnAround()
        {
            _movementController.ResetTurnAround();
        }
    }
}