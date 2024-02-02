using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas
{
    public class PhoneCanvasController : MonoBehaviour
    {
        private const float PositionTransitionDuration = 0.2f;

        public Transform openPositionTransform;
        public Transform closePositionTransform;
        public Transform phoneTransform;

        public CanvasGroup socialFeedCanvas;
        public CanvasGroup photoGalleryCanvas;

        public GameObject socialFeedCell;
        public GameObject photoGalleryEntryCell;

        public ScrollRect socialFeedScrollRect;

        private bool _active;
        private bool _isOpen;
        private bool _transitionActive;
        private float _positionTransitionTimeCount;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private CanvasGroup _canvasGroup;
        private Animator _playerAnimator;
        private PlayerMode _previousMode;
        private Vector3 _tempSlideCheck_ScrollRectPosition;
        private float _slideDelayTime;
        private List<CanvasGroup> _phoneAppCanvases;
        private CanvasGroup _prevPhoneAppCanvas;
        private CanvasGroup _currentPhoneAppCanvas;

        private IDisposable _playerModeChangedSubscription;

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
            _phoneAppCanvases = new List<CanvasGroup>
            {
                socialFeedCanvas,
                photoGalleryCanvas,
            };
            _currentPhoneAppCanvas = _phoneAppCanvases.First();
        }

        private void Start()
        {
            _playerModeChangedSubscription = GameRunner.Instance.Core.Player.OnChangeMode.Subscribe(SetActive);
            GameRunner.Instance.PlayerInputHandler.OnMove.Subscribe(input =>
            {
                if (GameRunner.Instance.Core.Player.Mode != PlayerMode.Phone || input.y == 0) return;
                var contentPostition = socialFeedScrollRect.content.anchoredPosition;
                contentPostition.y -= Time.deltaTime * input.y * 1000;
                socialFeedScrollRect.content.anchoredPosition = contentPostition;
            });
            GameRunner.Instance.PlayerInputHandler.OnNextMenuTriggerred.Subscribe(_ =>
            {
                var index = _phoneAppCanvases.IndexOf(_currentPhoneAppCanvas);
                if (index < 0) return;
                var nextIndex = Math.Min(_phoneAppCanvases.Count - 1, index + 1);
                if (nextIndex == index) return;
                _prevPhoneAppCanvas = _currentPhoneAppCanvas;
                _currentPhoneAppCanvas = _phoneAppCanvases[nextIndex];
                Debug.Log($">>next<< {index} {nextIndex} {_phoneAppCanvases.IndexOf(_currentPhoneAppCanvas)}");
                AnimateCanvasSwap();
            });
            GameRunner.Instance.PlayerInputHandler.OnPrevMenuTriggerred.Subscribe(_ =>
            {
                var index = _phoneAppCanvases.IndexOf(_currentPhoneAppCanvas);
                if (index < 0) return;
                var prevIndex = Math.Max(0, index - 1);
                if (prevIndex == index) return;
                Debug.Log($">>prev<< {index} {prevIndex}");
                _prevPhoneAppCanvas = _currentPhoneAppCanvas;
                _currentPhoneAppCanvas = _phoneAppCanvases[prevIndex];
                AnimateCanvasSwap();
            });
        }

        private void Update()
        {
            if (_isOpen)
            {
                UpdateSwipeCharacterAnimation();
            }

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

        private void OnDestroy()
        {
            _playerModeChangedSubscription.Dispose();
        }

        private void AnimateCanvasSwap()
        {
            _prevPhoneAppCanvas.DOFade(0, 0.3f);
            _currentPhoneAppCanvas.DOFade(1, 0.3f);
        }

        private void UpdateSwipeCharacterAnimation()
        {
            var contentPosition = socialFeedScrollRect.content.position;
            var scrollRectDelta = contentPosition - _tempSlideCheck_ScrollRectPosition;
            _tempSlideCheck_ScrollRectPosition = contentPosition;

            //if (scrollRectDelta.magnitude != 0)
            //{
            //Debug.Log($"scrollRectDelta : {scrollRectDelta}");

            //}
            if (Mathf.Abs(scrollRectDelta.y) > 12)
            {
                TriggerSwipeAnimation();
            }
        }


        void TriggerSwipeAnimation()
        {
            if (!(Time.time >= _slideDelayTime)) return;
            _slideDelayTime = Time.time + 0.5f;
            _playerAnimator.SetTrigger($"Swipe");
        }
    }
}