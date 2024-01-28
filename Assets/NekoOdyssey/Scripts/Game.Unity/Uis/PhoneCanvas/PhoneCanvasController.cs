using System;
using NekoOdyssey.Scripts.Game.Unity;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas
{
    public class PhoneCanvasController : MonoBehaviour
    {
        private const float PositionTransitionDuration = 0.2f;

        public Transform openPositionTransform;
        public Transform closePositionTransform;
        public Transform phoneTransform;

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

        private IDisposable _playerModeChangedSubscription;

        private void SetActive(PlayerMode mode)
        {
            if (_previousMode != PlayerMode.Phone && mode != PlayerMode.Phone)
            {
                _previousMode = mode;
                return;
            }

            _canvasGroup = GetComponent<CanvasGroup>();
            _playerAnimator = GameRunner.Instance.GameCore.Player.GameObject.GetComponent<Animator>();
            _active = mode == PlayerMode.Phone;
            _transitionActive = true;
            _positionTransitionTimeCount = 0f;
            _startPosition = (_active ? closePositionTransform : openPositionTransform).position;
            _endPosition = (_active ? openPositionTransform : closePositionTransform).position;
            _previousMode = mode;
        }

        private void Awake()
        {
            GameRunner.Instance.GameCore.Player.Phone.GameObject = gameObject;
            gameObject.AddComponent<PhoneSocialNetworkController>();
            gameObject.AddComponent<PhonePhotoGalleryController>();
        }

        private void Start()
        {
            _playerModeChangedSubscription = GameRunner.Instance.GameCore.Player.OnChangeMode.Subscribe(SetActive);
            GameRunner.Instance.PlayerInputHandler.OnMove.Subscribe(input =>
            {
                if (GameRunner.Instance.GameCore.Player.Mode != PlayerMode.Phone || input.y == 0) return;
                var contentPostition = socialFeedScrollRect.content.anchoredPosition;
                contentPostition.y -= Time.deltaTime * input.y * 1000;
                socialFeedScrollRect.content.anchoredPosition = contentPostition;
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


        Vector3 _tempSlideCheck_ScrollRectPosition;

        void UpdateSwipeCharacterAnimation()
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
                TriggerGirlSlide();
            }
        }

        float _slideDelayTime;

        void TriggerGirlSlide()
        {
            if (Time.time >= _slideDelayTime)
            {
                _slideDelayTime = Time.time + 0.5f;
                _playerAnimator.SetTrigger("Swipe");
            }
        }
    }
}