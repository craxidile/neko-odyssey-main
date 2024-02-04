using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Capture;
using UnityEngine;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Models;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Player
{
    public class PlayerCaptureController : MonoBehaviour
    {
        private bool _active;

        // Game Objects
        private GameObject _captureScreen;

        // private GameObject _captureBlurPlane;
        private GameObject _catPhotoContainer;
        private GameObject _catPhoto;
        private Animator _animator;
        private SpriteRenderer _renderer;

        private IDisposable _playerModeChangedSubscription;
        private IDisposable _activeChangeSubscription;

        private void SetActive(PlayerMode mode)
        {
            _active = mode == PlayerMode.Capture;
            if (!_active) return;
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Capture"), 1f);
            // _renderer.flipX = false;

            GameRunner.Instance.Core.Player.Phone.SocialNetwork.Add(new SocialFeed()
            {
                CatCode = GameRunner.Instance.Core.Player.Capture.CatCode,
            });

            GameRunner.Instance.Core.Player.Phone.PhotoGallery.Add(new PhotoGalleryEntry()
            {
                CatCode = GameRunner.Instance.Core.Player.Capture.CatCode,
            });

            var assetBundleName = $"{GameRunner.Instance.Core.Player.Capture.CatCode.ToLower()}snap";
            if (_catPhoto) Destroy(_catPhoto);
            if (GameRunner.Instance.AssetMap.TryGetValue(assetBundleName, out var asset))
            {
                _catPhoto = Instantiate(asset, _catPhotoContainer.transform) as GameObject;
            }

            // GameRunner.Instance.GameCore.Player.GameObject.transform.position =
            //     GameRunner.Instance.GameCore.Player.Capture.TargetPosition;
            string trigger = null;
            switch (GameRunner.Instance.Core.Player.Capture.Mode)
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
                // _captureBlurPlane.SetActive(true);


                var mainCamera = Camera.main;
                if (mainCamera == null) return;
                var postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
                var depthOfField = postProcessVolume.profile.GetSetting<DepthOfField>();
                DOTween.To(
                    () => (double)depthOfField.focusDistance.value,
                    value => depthOfField.focusDistance.value = (float)value,
                    .1f,
                    1f
                );
                DOTween.To(
                    () => (double)depthOfField.aperture.value,
                    value => depthOfField.aperture.value = (float)value,
                    .1f,
                    1f
                );
                DOTween.To(
                    () => (double)depthOfField.focalLength.value,
                    value => depthOfField.focalLength.value = (float)value,
                    2f,
                    1f
                );

                var canvasGroup = _catPhotoContainer.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;
                var rectTransform = _catPhotoContainer.GetComponent<RectTransform>();
                rectTransform.DOScale(5f, 0);
                rectTransform.DOScale(1f, 1f).SetEase(Ease.InSine);
                canvasGroup.DOFade(1, 1f);
            });
            DOVirtual.DelayedCall(6f, () =>
            {
                // _captureScreen.SetActive(false);
                var canvasGroup = _catPhotoContainer.GetComponent<CanvasGroup>();
                canvasGroup.DOFade(0, .5f);

                var mainCamera = Camera.main;
                if (mainCamera == null) return;
                var postProcessVolume = mainCamera.GetComponent<PostProcessVolume>();
                var depthOfField = postProcessVolume.profile.GetSetting<DepthOfField>();

                DOTween.To(
                    () => (double)depthOfField.focusDistance.value,
                    value => depthOfField.focusDistance.value = (float)value,
                    1.5f,
                    1f
                );
                DOTween.To(
                    () => (double)depthOfField.aperture.value,
                    value => depthOfField.aperture.value = (float)value,
                    7f,
                    1f
                );
                DOTween.To(
                    () => (double)depthOfField.focalLength.value,
                    value => depthOfField.focalLength.value = (float)value,
                    38f,
                    1f
                );
            });
            DOVirtual.DelayedCall(7f, () => { _animator.SetTrigger($"EndCapture"); });

            DOVirtual.DelayedCall(8f, () => { GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move); });
        }


        public void Awake()
        {
            var playerController = GameRunner.Instance.Core.Player.GameObject.GetComponent<PlayerController>();
            _captureScreen = playerController.captureScreen;
            // _captureBlurPlane = playerController.captureBlurPlane;
            _catPhotoContainer = playerController.catPhotoContainer;
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            _playerModeChangedSubscription = GameRunner.Instance.Core.Player.OnChangeMode.Subscribe(SetActive);
        }

        public void OnDestroy()
        {
            _playerModeChangedSubscription.Dispose();
        }
    }
}