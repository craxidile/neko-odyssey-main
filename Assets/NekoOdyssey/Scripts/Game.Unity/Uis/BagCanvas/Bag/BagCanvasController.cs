using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;
using NekoOdyssey.Scripts.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemGrid;
using NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemTypeHeader;
using NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Panels;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag
{
    public class BagCanvasController : MonoBehaviour
    {
        private const float PositionTransitionDuration = .2f;

        private GameObject _lastSelectedObject;
        private bool _active;
        private bool _isOpen;
        private bool _transitionActive;
        private float _positionTransitionTimeCount;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private CanvasGroup _canvasGroup;
        private PlayerMode _previousMode;
        private float _slideDelayTime;

        public Transform openPositionTransform;
        public Transform closePositionTransform;
        public Transform bagTransform;

        public GameObject bagHeaderContainer;
        public SmoothScrollRect bagItemsScrollRect;
        public GameObject bagItemsContainer;
        public GameObject hoverFrame;
        public GameObject itemAnimationDock;
        public Text itemNameText;

        private void Start()
        {
            var gridLayoutGroup = bagItemsContainer.GetComponent<GridLayoutGroup>();
            GameRunner.Instance.Core.Player.Bag.SetGridColumnCount(gridLayoutGroup.constraintCount);

            gameObject.AddComponent<ItemTypeHeaderController>();
            gameObject.AddComponent<ItemGridController>();

            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(HandleModeChange)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnSelectBagItem
                .Subscribe(HandleItemSelection)
                .AddTo(this);
        }

        private void Update()
        {
            if (_canvasGroup != null)
            {
                var targetPosition = Vector3.Lerp(_startPosition, _endPosition, _positionTransitionTimeCount);
                _positionTransitionTimeCount += Time.deltaTime / PositionTransitionDuration;
                bagTransform.position = targetPosition;
                _transitionActive = Vector3.Distance(targetPosition, _endPosition) >= 1f;
                _isOpen = _active && !_transitionActive;
                _canvasGroup.alpha = !_active && !_transitionActive ? 0 : 1;
                _canvasGroup.interactable = _isOpen;
                _canvasGroup.blocksRaycasts = _isOpen;
            }

            if (!_active) return;

            if (EventSystem.current.currentSelectedGameObject == null && _lastSelectedObject != null)
            {
                EventSystem.current.SetSelectedGameObject(_lastSelectedObject);
            }
            else
            {
                _lastSelectedObject = EventSystem.current.currentSelectedGameObject;
            }

            if (!hoverFrame.activeSelf) return;

            var scrollRectRectTransform = bagItemsScrollRect.GetComponent<RectTransform>();
            var itemHoverRectTransform = hoverFrame.GetComponent<RectTransform>();
            
            var viewPortCorners = new Vector3[4];
            scrollRectRectTransform.GetWorldCorners(viewPortCorners);
            
            var itemHoverCorners = new Vector3[4];
            itemHoverRectTransform.GetWorldCorners(itemHoverCorners);
            
            if (itemHoverCorners[1].y > viewPortCorners[1].y)
            {
                var contentPosition = bagItemsScrollRect.content.anchoredPosition;
                contentPosition.y -= Time.deltaTime * 1 * 1000;
                bagItemsScrollRect.content.anchoredPosition = contentPosition;
            }
            else if (itemHoverCorners[0].y < viewPortCorners[0].y)
            {
                var contentPosition = bagItemsScrollRect.content.anchoredPosition;
                contentPosition.y -= Time.deltaTime * -1 * 1000;
                bagItemsScrollRect.content.anchoredPosition = contentPosition;
            }
        }

        private void HandleModeChange(PlayerMode mode)
        {
            if (_previousMode != PlayerMode.OpenBag && mode != PlayerMode.OpenBag)
            {
                _previousMode = mode;
                return;
            }

            _active = mode == PlayerMode.OpenBag;
            if (_active)
            {
                GameRunner.Instance.Core.Player.Bag.SetDefaultItemType();
            }

            DOVirtual.DelayedCall(_active ? .6f : 0, () =>
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                _transitionActive = true;
                _positionTransitionTimeCount = 0f;
                _startPosition = (_active ? closePositionTransform : openPositionTransform).position;
                _endPosition = (_active ? openPositionTransform : closePositionTransform).position;
                _previousMode = mode;
            });
        }

        private void HandleItemSelection(BagItemV001 item)
        {
            hoverFrame.SetActive(item != null);
        }
    }
}