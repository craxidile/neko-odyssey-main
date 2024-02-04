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
        private GameObject _blurPlane;
        private Animator _animator;
        private SpriteRenderer _renderer;
        private DepthOfField _depthOfField;

        private void SetActive(PlayerMode mode)
        {
            _previousActive = _active;
            _active = mode == PlayerMode.Phone;
            _phoneScreen.SetActive(_active);
            _blurPlane.SetActive(_active);

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
            _blurPlane = playerController.phoneBlurPlane;
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();

            var mainCamera = Camera.main;
            if (mainCamera == null) return;
            var postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
            _depthOfField = postProcessVolume.profile.GetSetting<DepthOfField>();
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode.Subscribe(SetActive).AddTo(this);
        }

        private void AnimateOpening()
        {
            _renderer.flipX = false;
            SoundEffectController.Instance.openPhone.Play();

            DOTween.To(
                () => (double)_depthOfField.focalLength.value,
                value => _depthOfField.focalLength.value = (float)value,
                50f,
                1f
            );
        }

        private void AnimateClosing()
        {
            SoundEffectController.Instance.closePhone.Play();
            DOTween.To(
                () => (double)_depthOfField.focalLength.value,
                value => _depthOfField.focalLength.value = (float)value,
                1.5f,
                1f
            );
        }
    }
}