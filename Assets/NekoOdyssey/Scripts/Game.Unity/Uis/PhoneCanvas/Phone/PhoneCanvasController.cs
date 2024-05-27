using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone.CatNote;
using NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone.PhotoGallery;
using NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone.SocialNetwork;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone
{
    public class PhoneCanvasController : MonoBehaviour
    {
        private const float PositionTransitionDuration = 0.2f;
        private const float AppSwapDuration = 0.3f;
        private const float ScrollAnimationTriggerMinDelta = 5f; // 12f;
        private const float ScrollAnimationTriggerMaxDelta = 300f;
        private const float SlideDelayTimeDelta = 0.5f;
        private const float ContentScrollTimeFactor = 1000f;

        public List<PhoneCanvasUi> phoneUiList = new();

        public Transform openPositionTransform;
        public Transform closePositionTransform;
        public Transform phoneTransform;

        public TextMeshProUGUI likeCountText;

        private bool _active;
        private bool _isOpen;
        private bool _transitionActive;
        private float _positionTransitionTimeCount;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private bool _appSwapped;
        private CanvasGroup _canvasGroup;
        private Animator _playerAnimator;
        private PlayerMode _previousMode;
        private Vector3 _tempSlideCheckScrollRectPosition;
        private float _slideDelayTime;

        private void SetActive(PlayerMode mode)
        {
            Debug.Log($">>mode_mode<< {_previousMode} {mode}");
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
            gameObject.AddComponent<PhoneCatNoteController>();
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(SetActive)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Phone.OnScroll
                .Subscribe(ScrollAppPane)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Phone.OnStopScrolling
                .Subscribe(HandleScrollingStop)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Phone.OnChangeApp
                .Subscribe(AnimateCanvasSwap)
                .AddTo(this);
        }

        private void Update()
        {
            if (_isOpen) UpdateSwipeAnimation();
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
            var currentApp = GameRunner.Instance.Core.Player.Phone.CurrentApp;
            var phoneCanvasUi = phoneUiList.FirstOrDefault(ui => ui.phoneApp == currentApp);
            if (phoneCanvasUi == null) return;

            var scrollRect = phoneCanvasUi.scrollRect;
            var contentPosition = scrollRect.content.anchoredPosition;
            contentPosition.y -= Time.deltaTime * input.y * ContentScrollTimeFactor;
            scrollRect.content.anchoredPosition = contentPosition;
        }

        private void HandleScrollingStop(Unit _)
        {
        }

        private void AnimateCanvasSwap(PlayerPhoneApp _)
        {
            var previousApp = GameRunner.Instance.Core.Player.Phone.PreviousApp;
            var currentApp = GameRunner.Instance.Core.Player.Phone.CurrentApp;
            var prevCanvasUi = phoneUiList.FirstOrDefault(ui => ui.phoneApp == previousApp);
            var currentCanvasUi = phoneUiList.FirstOrDefault(ui => ui.phoneApp == currentApp);
            if (prevCanvasUi == null || currentCanvasUi == null) return;
            if (prevCanvasUi == currentCanvasUi) return;

            _appSwapped = true;

            prevCanvasUi.canvasGroup.DOFade(0f, AppSwapDuration);
            currentCanvasUi.canvasGroup.DOFade(1f, AppSwapDuration);
            
            _playerAnimator.SetTrigger($"Swipe");
        }

        private void UpdateSwipeAnimation()
        {
            var currentApp = GameRunner.Instance.Core.Player.Phone.CurrentApp;
            var phoneCanvasUi = phoneUiList.FirstOrDefault(ui => ui.phoneApp == currentApp);
            if (phoneCanvasUi == null) return;

            var contentPosition = phoneCanvasUi.scrollRect.content.position;
            var scrollRectDelta = contentPosition - _tempSlideCheckScrollRectPosition;
            _tempSlideCheckScrollRectPosition = contentPosition;

            if (_appSwapped)
            {
                _appSwapped = false;
                return;
            }

            var deltaY = Mathf.Abs(scrollRectDelta.y);
            if (deltaY <= ScrollAnimationTriggerMinDelta || deltaY >= ScrollAnimationTriggerMaxDelta) return;
            
            Debug.Log($">>swapped<< {deltaY} {_transitionActive}");

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