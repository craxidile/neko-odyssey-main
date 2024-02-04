using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas
{
    public class PhoneCanvasController : MonoBehaviour
    {
        private const float PositionTransitionDuration = 0.2f;
        private const float AppSwapDuration = 0.3f;
        private const float ScrollAnimationTriggerDelta = 12f;
        private const float SlideDelayTimeDelta = 0.5f;
        private const float ContentScrollTimeFactor = 1000f;

        private readonly Dictionary<PlayerPhoneApp, CanvasGroup> _phoneModeCanvasMap = new();
        private readonly Dictionary<PlayerPhoneApp, ScrollRect> _phoneModeScrollRectMap = new();

        public Transform openPositionTransform;
        public Transform closePositionTransform;
        public Transform phoneTransform;

        public CanvasGroup socialFeedCanvas;
        public CanvasGroup photoGalleryCanvas;

        public GameObject socialFeedCell;
        public GameObject photoGalleryEntryCell;

        public ScrollRect socialFeedScrollRect;
        public ScrollRect photoGalleryScrollRect;

        private bool _active;
        private bool _isOpen;
        private bool _transitionActive;
        private float _positionTransitionTimeCount;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private CanvasGroup _canvasGroup;
        private Animator _playerAnimator;
        private PlayerMode _previousMode;
        private Vector3 _tempSlideCheckScrollRectPosition;
        private float _slideDelayTime;

        private void SetActive(PlayerMode mode)
        {
            if (_previousMode != PlayerMode.Phone && mode != PlayerMode.Phone)
            {
                _previousMode = mode;
                return;
            }

            _canvasGroup = GetComponent<CanvasGroup>();
            _playerAnimator = GameRunner.Instance.Core.Player.GameObject.GetComponent<Animator>();
            _active = mode == PlayerMode.Phone;
            _transitionActive = true;
            _positionTransitionTimeCount = 0f;
            _startPosition = (_active ? closePositionTransform : openPositionTransform).position;
            _endPosition = (_active ? openPositionTransform : closePositionTransform).position;
            _previousMode = mode;
        }

        private void Awake()
        {
            GameRunner.Instance.Core.Player.Phone.GameObject = gameObject;
            gameObject.AddComponent<PhoneSocialNetworkController>();
            gameObject.AddComponent<PhonePhotoGalleryController>();
            
            _phoneModeCanvasMap.Add(PlayerPhoneApp.SocialNetwork, socialFeedCanvas);
            _phoneModeCanvasMap.Add(PlayerPhoneApp.PhotoGallery, photoGalleryCanvas);
            
            _phoneModeScrollRectMap.Add(PlayerPhoneApp.SocialNetwork, socialFeedScrollRect);
            _phoneModeScrollRectMap.Add(PlayerPhoneApp.PhotoGallery, photoGalleryScrollRect);
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(SetActive)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Phone.OnScroll
                .Subscribe(ScrollAppPane)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Phone.OnChangeApp
                .Subscribe(AnimateCanvasSwap)
                .AddTo(this);
        }

        private void Update()
        {
            if (!_isOpen) UpdateSwipeAnimation();

            if (!_transitionActive) return;
            var targetPosition = Vector3.Lerp(_startPosition, _endPosition, _positionTransitionTimeCount);
            _positionTransitionTimeCount += Time.deltaTime / PositionTransitionDuration;
            phoneTransform.position = targetPosition;
            _transitionActive = Vector3.Distance(targetPosition, _endPosition) >= 1f;
            _canvasGroup.alpha = !_active && !_transitionActive ? 0 : 1;
            _isOpen = _active && !_transitionActive;
            _canvasGroup.interactable = _isOpen;
            _canvasGroup.blocksRaycasts = _isOpen;
        }

        private void ScrollAppPane(Vector2 input)
        {
            var contentPosition = socialFeedScrollRect.content.anchoredPosition;
            contentPosition.y -= Time.deltaTime * input.y * ContentScrollTimeFactor;
            var currentApp = GameRunner.Instance.Core.Player.Phone.CurrentApp;
            if (!_phoneModeScrollRectMap.ContainsKey(currentApp)) return;
            var scrollRect = _phoneModeScrollRectMap[currentApp];
            scrollRect.content.anchoredPosition = contentPosition;
        }

        private void AnimateCanvasSwap(PlayerPhoneApp _)
        {
            var previousApp = GameRunner.Instance.Core.Player.Phone.PreviousApp;
            var currentApp = GameRunner.Instance.Core.Player.Phone.CurrentApp;
            if (!_phoneModeCanvasMap.ContainsKey(previousApp) || !_phoneModeCanvasMap.ContainsKey(currentApp)) return;
            
            var prevPhoneAppCanvas = _phoneModeCanvasMap[previousApp];
            var currentPhoneAppCanvas = _phoneModeCanvasMap[currentApp];
            if (prevPhoneAppCanvas == currentPhoneAppCanvas) return;
            
            prevPhoneAppCanvas.DOFade(0f, AppSwapDuration);
            currentPhoneAppCanvas.DOFade(1f, AppSwapDuration);
        }

        private void UpdateSwipeAnimation()
        {
            var contentPosition = socialFeedScrollRect.content.position;
            var scrollRectDelta = contentPosition - _tempSlideCheckScrollRectPosition;
            _tempSlideCheckScrollRectPosition = contentPosition;
            if (Mathf.Abs(scrollRectDelta.y) <= ScrollAnimationTriggerDelta) return;
            TriggerSwipeAnimation();
        }

        private void TriggerSwipeAnimation()
        {
            if (!_playerAnimator || Time.time < _slideDelayTime) return;
            _slideDelayTime = Time.time + SlideDelayTimeDelta;
            _playerAnimator.SetTrigger($"Swipe");
        }
    }
}