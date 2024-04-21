using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Buttons;
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
        private const string ItemTypeButtonKey = "itemtypebutton";
        private const string ItemButtonKey = "itembutton";

        private readonly Dictionary<string, ItemTypeButtonController> _itemTypeButtonControllerMap = new();
        private readonly Dictionary<Item, GameObject> _itemButtonMap = new();
        private readonly Dictionary<Item, GameObject> _animatingItemButtonMap = new();
        private readonly List<Item> _itemsToMove = new();

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
            AssetBundleUtils.OnReady(InitializeHeader);

            var gridLayoutGroup = bagItemsContainer.GetComponent<GridLayoutGroup>();
            GameRunner.Instance.Core.Player.Bag.SetGridColumnCount(gridLayoutGroup.constraintCount);

            GameRunner.Instance.Core.Player.OnChangeMode
                .Subscribe(HandleModeChange)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnChangeItemType
                .Subscribe(HandleItemTypeChange)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnSelectItem
                .Subscribe(HandleItemSelection)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnItemPositionsReady
                .Subscribe(HandleItemPositions)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnChangeConfirmationVisibility
                .Subscribe(HandleConfirmationVisibility)
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
                _canvasGroup.alpha = 1; // !_active && !_transitionActive ? 0 : 1;
                _canvasGroup.interactable = _isOpen;
                _canvasGroup.blocksRaycasts = _isOpen;
            }

            if (!hoverFrame.activeSelf) return;

            var item = GameRunner.Instance.Core.Player.Bag.CurrentItem;
            if (item != null && _itemButtonMap.TryGetValue(item, out var itemButton))
            {
                itemNameText.text = item.Name;
                hoverFrame.transform.position = itemButton.transform.position;
            }

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

        private void InitializeHeader()
        {
            _itemTypeButtonControllerMap.Clear();
            var itemTypes = GameRunner.Instance.Core.MasterData.ItemsMasterData.ItemTypes;
            foreach (var itemType in itemTypes)
            {
                if (!GameRunner.Instance.AssetMap.ContainsKey(ItemTypeButtonKey)) continue;
                var itemTypeButton = Instantiate(
                    GameRunner.Instance.AssetMap[ItemTypeButtonKey],
                    bagHeaderContainer.transform
                ) as GameObject;
                if (itemTypeButton == null) continue;
                itemTypeButton.name = $"ItemType{itemType.Code}";
                var controller = itemTypeButton.GetComponent<ItemTypeButtonController>();
                _itemTypeButtonControllerMap.Add(itemType.Code, controller);
                controller.ItemType = itemType;
            }

            HandleItemTypeChange(GameRunner.Instance.Core.Player.Bag.CurrentItemType);
        }

        private void InitializeItems()
        {
            hoverFrame.SetActive(false);
            if (!GameRunner.Instance.AssetMap.ContainsKey(ItemButtonKey)) return;
            bagItemsContainer.GetComponent<CanvasGroup>().alpha = 0;
            foreach (var itemButton in _itemButtonMap.Values)
            {
                Destroy(itemButton);
            }

            _itemButtonMap.Clear();
            GameRunner.Instance.Core.Player.Bag.ClearItemPositions();
            var items = GameRunner.Instance.Core.Player.Bag.FilteredItems;
            foreach (var item in items)
            {
                var itemButton = Instantiate(
                    GameRunner.Instance.AssetMap[ItemButtonKey],
                    bagItemsContainer.transform
                ) as GameObject;
                if (itemButton == null) continue;
                itemButton.name = $"Item{item.Code} ({item.Name})";
                _itemButtonMap.Add(item, itemButton);
                var controller = itemButton.GetComponent<ItemButtonController>();
                controller.Item = item;
                controller.ReadOnly = false;
                controller.SetVisible(false, false);
            }

            bagItemsContainer.GetComponent<CanvasGroup>().alpha = 1;
        }

        private void HandleModeChange(PlayerMode mode)
        {
            if (_previousMode != PlayerMode.OpenBag && mode != PlayerMode.OpenBag)
            {
                _previousMode = mode;
                return;
            }

            _active = mode == PlayerMode.OpenBag;
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

        private void HandleItemTypeChange(ItemType itemType)
        {
            if (itemType == null || !_itemTypeButtonControllerMap.ContainsKey(itemType.Code)) return;
            var itemTypeButtonController = _itemTypeButtonControllerMap[itemType.Code];
            itemTypeButtonController.SetCurrent(true);
            var otherItemTypeControllers = _itemTypeButtonControllerMap.Keys
                .Where(code => code != itemType.Code)
                .Select(it => _itemTypeButtonControllerMap[it])
                .ToList();
            foreach (var controller in otherItemTypeControllers)
                controller.SetCurrent(false);
            InitializeItems();
        }

        private void HandleItemSelection(Item item)
        {
            if (item == null || !_itemButtonMap.ContainsKey(item))
            {
                hoverFrame.SetActive(false);
                return;
            }

            hoverFrame.SetActive(true);
            var itemButton = _itemButtonMap[item];
            itemNameText.text = item.Name;
            hoverFrame.transform.position = itemButton.transform.position;

            var controller = itemButton.GetComponent<ItemButtonController>();
            if (EventSystem.current.currentSelectedGameObject != controller.button.gameObject)
                EventSystem.current.SetSelectedGameObject(controller.button.gameObject);
        }

        private void HandleItemPositions(Dictionary<Item, Vector3> itemPositions)
        {
            hoverFrame.SetActive(false);

            if (_animatingItemButtonMap.Count == 0)
            {
                InitializeAnimatingItemButton(itemPositions);
                itemAnimationDock.GetComponent<CanvasGroup>().alpha = 0;
                itemAnimationDock.SetActive(false);
                ShowUnmovingItemButtons();
                var controller = _itemButtonMap.Values.First().GetComponent<ItemButtonController>();
                EventSystem.current.SetSelectedGameObject(controller.button.gameObject);
                return;
            }

            _itemsToMove.Clear();

            itemAnimationDock.SetActive(true);
            itemAnimationDock.GetComponent<CanvasGroup>().alpha = 1;
            var itemButtonControllersToShow = new List<ItemButtonController>();
            foreach (var item in _animatingItemButtonMap.Keys)
            {
                var itemButton = _animatingItemButtonMap[item];
                var controller = itemButton.GetComponent<ItemButtonController>();
                if (!itemPositions.TryGetValue(item, out var position))
                {
                    controller.SetVisible(false, true);
                }
                else
                {
                    itemButtonControllersToShow.Add(controller);
                    itemButton.transform.DOMove(position, .3f);
                    if (_animatingItemButtonMap.ContainsKey(item))
                        _itemsToMove.Add(item);
                }
            }

            ShowUnmovingItemButtons();

            DOVirtual.DelayedCall(.3f, () =>
            {
                ShowMovingItemButtons();
                itemAnimationDock.GetComponent<CanvasGroup>().alpha = 0;
                InitializeAnimatingItemButton(itemPositions);
                itemAnimationDock.SetActive(false);
            });
        }

        private void HandleConfirmationVisibility(bool visible)
        {
            if (visible) return;
            var item = GameRunner.Instance.Core.Player.Bag.CurrentItem;
            if (item == null) return;
            var itemButton = _itemButtonMap[item];
            DOVirtual.DelayedCall(.1f, () => EventSystem.current.SetSelectedGameObject(itemButton));
        }

        private void ShowUnmovingItemButtons()
        {
            foreach (var (item, itemButton) in _itemButtonMap)
            {
                var controller = itemButton.GetComponent<ItemButtonController>();
                controller.SetVisible(!_itemsToMove.Contains(item), true);
            }
        }

        private void ShowMovingItemButtons()
        {
            foreach (var item in _itemsToMove)
            {
                if (!_itemButtonMap.ContainsKey(item)) continue;
                var itemButton = _itemButtonMap[item];
                var controller = itemButton.GetComponent<ItemButtonController>();
                if (!_itemsToMove.Contains(item)) return;
                controller.SetVisible(true, false);
            }
        }

        private void InitializeAnimatingItemButton(Dictionary<Item, Vector3> itemPositions)
        {
            foreach (var item in _animatingItemButtonMap.Values)
                Destroy(item);

            _animatingItemButtonMap.Clear();
            if (!GameRunner.Instance.AssetMap.ContainsKey(ItemButtonKey)) return;
            var updatedItems = itemPositions.Keys;
            foreach (var item in updatedItems)
            {
                var itemButton = _itemButtonMap[item];
                var animatingItemButton = Instantiate(
                    GameRunner.Instance.AssetMap[ItemButtonKey],
                    itemAnimationDock.transform
                ) as GameObject;
                if (animatingItemButton == null) continue;
                var controller = itemButton.GetComponent<ItemButtonController>();
                var animatingController = animatingItemButton.GetComponent<ItemButtonController>();
                controller.CopyTo(animatingController);
                animatingController.SetVisible(true, false);
                animatingController.ReadOnly = true;
                _animatingItemButtonMap.Add(item, animatingItemButton);
            }
        }
    }
}