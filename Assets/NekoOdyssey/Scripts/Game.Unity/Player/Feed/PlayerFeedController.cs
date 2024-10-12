using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Feed;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Feed
{
    public class PlayerFeedController : MonoBehaviour
    {
        public const float FeedDelay = 4f;

        private bool _active;

        private GameObject _catPhoto;
        private Animator _animator;
        private SpriteRenderer _renderer;

        public void Awake()
        {
            var player = GameRunner.Instance.Core.Player;
            var playerController = player.GameObject.GetComponent<PlayerController>();
            _animator = playerController.GetComponent<Animator>();
            _renderer = playerController.GetComponent<SpriteRenderer>();
        }

        public void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(SetActive)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Feed.OnFinishFeed
                .Subscribe(HandleFeedEnd)
                .AddTo(this);
        }


        private void SetActive(PlayerMode mode)
        {
            _active = mode == PlayerMode.Feed;
            Debug.Log($">>feed<< active {_active}");
            if (!_active) return;
            HandleFeed();
        }


        private void HandleFeed()
        {
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Feed"), 1f);

            Debug.Log($">>feed<< feed_mode {GameRunner.Instance.Core.Player.Feed.Mode}");
            string feedMode;
            switch (GameRunner.Instance.Core.Player.Feed.Mode)
            {
                case FeedMode.Sit:
                    feedMode = $"FeedSit";
                    break;
                case FeedMode.SitHigh:
                    feedMode = $"FeedSitHigh";
                    break;
                case FeedMode.Low:
                    feedMode = $"FeedLow";
                    break;
                case FeedMode.UpperLow:
                    feedMode = $"FeedUpperLow";
                    break;
                case FeedMode.Medium:
                    feedMode = $"FeedMedium";
                    break;
                case FeedMode.High:
                    feedMode = $"FeedHigh";
                    break;
                case FeedMode.None:
                default:
                    return;
            }

            var mainCamera = GameRunner.Instance.cameras.mainCamera;
            if (!mainCamera) return;
            var cameraTransform = mainCamera.transform;
            var forward = cameraTransform.forward;
            var left = -cameraTransform.right;

            var playerPosition = GameRunner.Instance.Core.Player.Position;
            var capturePosition = GameRunner.Instance.Core.Player.Feed.TargetPosition;
            var delta = capturePosition - playerPosition;
            var forwardX = Math.Abs(forward.x) < .0001f ? 0f : forward.x;
            var forwardZ = Math.Abs(forward.z) < .0001f ? 0f : forward.z;
            var leftX = Math.Abs(left.x) < .0001f ? 0f : left.x;
            var leftZ = Math.Abs(left.z) < .0001f ? 0f : left.z;
            var deltaDepth = forwardX != 0f ? forwardX * delta.x : forwardZ * delta.z;
            var deltaSide = forwardX != 0f ? leftZ * delta.z : leftX * delta.x;

            var angle = Mathf.Rad2Deg * Mathf.Atan2(Mathf.Abs(deltaSide), Mathf.Abs(deltaDepth));

            _animator.SetBool(feedMode, true);
            _animator.SetFloat($"FacingToCat", deltaDepth <= 0f ? 0f : 1f);
            _animator.SetFloat($"CaptureAngle", angle);
            _renderer.flipX = deltaSide > 0f;

            DOVirtual.DelayedCall(FeedDelay, () => _animator.SetBool(feedMode, false));
        }

        private void HandleFeedEnd(Unit _)
        {
            _animator.SetLayerWeight(_animator.GetLayerIndex($"Feed"), 0);
        }
    }
}