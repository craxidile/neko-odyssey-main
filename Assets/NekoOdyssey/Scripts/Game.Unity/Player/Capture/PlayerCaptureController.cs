using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models;
using NekoOdyssey.Scripts.Game.Core.Capture;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Models;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Capture
{
    public class PlayerCaptureController : MonoBehaviour
    {
        private bool _active;

        private GameObject _catPhotoContainer;
        private GameObject _catPhoto;
        private Animator _animator;
        private SpriteRenderer _renderer;

        private void SetActive(PlayerMode mode)
        {
            _active = mode == PlayerMode.Capture;
            if (!_active) return;
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Capture"), 1f);
            // _renderer.flipX = false;

            GameRunner.Instance.Core.Player.Phone.SocialNetwork.Add(
                GameRunner.Instance.Core.Player.Capture.CatCode
            );

            GameRunner.Instance.Core.Player.Phone.PhotoGallery.Add(new CatPhotoV001(
                GameRunner.Instance.Core.Player.Capture.CatCode,
                GameRunner.Instance.Core.Player.Capture.CatCode
            ));

            var assetBundleName = $"{GameRunner.Instance.Core.Player.Capture.CatCode.ToLower()}snap";
            Debug.Log($">>cat_code<< {_catPhotoContainer == null}");
            if (_catPhoto) Destroy(_catPhoto);
            if (GameRunner.Instance.AssetMap.TryGetValue(assetBundleName, out var asset))
            {
                _catPhoto = Instantiate(asset, _catPhotoContainer.transform) as GameObject;
            }

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


            var mainCamera = GameRunner.Instance.cameras.mainCamera;
            if (!mainCamera) return;
            var cameraTransform = mainCamera.transform;
            var forward = cameraTransform.forward;
            var left = -cameraTransform.right;

            var playerPosition = GameRunner.Instance.Core.Player.Position;
            var capturePosition = GameRunner.Instance.Core.Player.Capture.TargetPosition;
            var delta = capturePosition - playerPosition;
            var forwardX = Math.Abs(forward.x) < .0001f ? 0f : forward.x;
            var forwardZ = Math.Abs(forward.z) < .0001f ? 0f : forward.z;
            var leftX = Math.Abs(left.x) < .0001f ? 0f : left.x;
            var leftZ = Math.Abs(left.z) < .0001f ? 0f : left.z;
            var deltaDepth = forwardX != 0f ? forwardX * delta.x : forwardZ * delta.z;
            var deltaSide = forwardX != 0f ? leftZ * delta.z : leftX * delta.x;

            // Debug.Log($">>forward<< {forward} {left} {deltaDepth} {deltaSide}");

            var angle = Mathf.Rad2Deg * Mathf.Atan2(Mathf.Abs(deltaSide), Mathf.Abs(deltaDepth));


            if (trigger != null) _animator.SetTrigger(trigger);
            _animator.SetFloat($"FacingToCat", deltaDepth <= 0f ? 0.0f : 1.0f);
            _animator.SetFloat($"CaptureAngle", angle);
            _renderer.flipX = deltaSide > 0f;

            Debug.Log(
                $">>delta_position<< {angle} "); // {deltaX} {deltaZ} >>facing_to_cat<< {trigger} {(deltaX <= 0f ? 0.0f : 1.0f)}");
            DOVirtual.DelayedCall(2f, () =>
            {
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

                GameRunner.Instance.Core.Player.Capture.Begin();
            });
            DOVirtual.DelayedCall(6f, () =>
            {
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

            DOVirtual.DelayedCall(8f, () =>
            {
                GameRunner.Instance.Core.Player.Capture.Finish();
                // GameRunner.Instance.Core.PlayerMenu.SetCurrentSiteActive();
            });
        }


        public void Awake()
        {
            var player = GameRunner.Instance.Core.Player;
            var playerController = player.GameObject.GetComponent<PlayerController>();
            _catPhotoContainer = playerController.catPhotoContainer;
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(SetActive)
                .AddTo(this);
        }
    }
}