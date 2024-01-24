using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Capture;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;

namespace NekoOdyssey.Scripts.Game.Unity.Player
{
    public class PlayerCaptureController : MonoBehaviour
    {
        private bool _active;

        // Game Objects
        private GameObject _captureScreen;
        private GameObject _captureBlurPlane;
        private Animator _animator;
        private SpriteRenderer _renderer;

        private IDisposable _playerModeChangedSubscription;
        private IDisposable _activeChangeSubscription;

        private void SetActive(PlayerMode mode)
        {
            _active = mode == PlayerMode.Capture;
            if (!_active) return;
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Capture"), 1f);
            _renderer.flipX = false;
            
            GameRunner.Instance.GameCore.Player.GameObject.transform.position =
                GameRunner.Instance.GameCore.Player.Capture.TargetPosition;
            string trigger = null;
            switch (GameRunner.Instance.GameCore.Player.Capture.Mode)
            {
                case CaptureMode.StandCaptureTop:
                    trigger = $"StartCaptureTop";
                    break;
                case CaptureMode.StandCaptureMiddle:
                    trigger = $"StartCaptureMiddle";
                    break;
                case CaptureMode.StandCaptureBottom:
                    trigger = $"StartCaptureBottom";
                    break;
            }
            if (trigger != null) _animator.SetTrigger(trigger);
            DOVirtual.DelayedCall(2f, () =>
            {
                // _captureScreen.SetActive(true);
                _captureBlurPlane.SetActive(true);
            });
            DOVirtual.DelayedCall(8f, () =>
            {
                // _captureScreen.SetActive(false);
                _captureBlurPlane.SetActive(false);
            });
            DOVirtual.DelayedCall(9f, () =>
            {
                _animator.SetTrigger($"EndCapture");
            });

            DOVirtual.DelayedCall(10f, () =>
            {
                GameRunner.Instance.GameCore.Player.SetMode(PlayerMode.Move);
            });
        }


        public void Awake()
        {
            var playerController = GameRunner.Instance.GameCore.Player.GameObject.GetComponent<PlayerController>();
            _captureScreen = playerController.captureScreen;
            _captureBlurPlane = playerController.captureBlurPlane;
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