using System;
using DG.Tweening;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.SoundEffects;
using UnityEngine.Rendering.PostProcessing;

namespace NekoOdyssey.Scripts.Game.Unity.Player
{
    public class PlayerPhoneController : MonoBehaviour
    {
        private bool _previousActive;
        private bool _active;

        // Game Objects
        private GameObject _phoneScreen;

        // private GameObject _blurPlane;
        private Animator _animator;
        private SpriteRenderer _renderer;
        private DepthOfField _depthOfField;

        private void SetActive(PlayerMode mode)
        {
            _previousActive = _active;
            _active = mode == PlayerMode.Phone;

            _phoneScreen.SetActive(_active);
            GameRunner.Instance.playerCamera.gameObject.SetActive(_active);
            // _blurPlane.SetActive(_active);

            if (_active)
                _animator.SetLayerWeight(_animator.GetLayerIndex($"Phone"), 1f);

            switch (_previousActive)
            {
                case false when _active:
                    AnimateOpening();
                    break;
                case true when !_active:
                    AnimateClosing();
                    break;
            }
        }

        private void Awake()
        {
            var playerController = GameRunner.Instance.Core.Player.GameObject.GetComponent<PlayerController>();
            _phoneScreen = playerController.phoneScreen;
            // _blurPlane = playerController.phoneBlurPlane;
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode.Subscribe(SetActive).AddTo(this);
        }

        private void Update()
        {
            if (!GameRunner.Instance.playerCamera.gameObject.activeSelf) return;
            GameRunner.Instance.playerCamera.fieldOfView = GameRunner.Instance.mainCamera.fieldOfView;
        }

        private void AnimateOpening()
        {
            Debug.Log($">>animate<< opening");
            _renderer.flipX = false;
            SoundEffectController.Instance.openPhone.Play();
            // GameRunner.Instance.playerCamera.transform.DOLocalMoveZ(-4f, 0.5f);
        }

        private void AnimateClosing()
        {
            Debug.Log($">>animate<< closing");
            SoundEffectController.Instance.closePhone.Play();
            // GameRunner.Instance.playerCamera.transform.DOLocalMoveZ(0, 0.8f);
        }
    }
}