using System;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;

namespace NekoOdyssey.Scripts.Game.Unity.Player
{
    public class PlayerPhoneController : MonoBehaviour
    {
        private bool _active;

        // Game Objects
        private GameObject _phoneScreen;
        private GameObject _blurPlane;
        private Animator _animator;
        private SpriteRenderer _renderer;
        
        private IDisposable _playerModeChangedSubscription;
        private IDisposable _activeChangeSubscription;

        private void SetActive(PlayerMode mode)
        {
            _active = mode == PlayerMode.Phone;
            _phoneScreen.SetActive(_active);
            _blurPlane.SetActive(_active);

            if (!_active) return;
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Phone"), 1f);
            _renderer.flipX = false;
        }

        
        public void Awake()
        {
            var playerController = GameRunner.Instance.GameCore.Player.GameObject.GetComponent<PlayerController>();
            _phoneScreen = playerController.phoneScreen;
            _blurPlane = playerController.phoneBlurPlane;
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            _playerModeChangedSubscription = GameRunner.Instance.GameCore.Player.OnChangeMode.Subscribe(SetActive);
        }

        public void OnDestroy()
        {
            _playerModeChangedSubscription.Dispose();
        }
        
    }
}