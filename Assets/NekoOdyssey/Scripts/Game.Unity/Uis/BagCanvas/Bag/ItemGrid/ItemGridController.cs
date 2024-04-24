using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;
using NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemTypeHeader;
using NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Panels;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemGrid
{
    public class ItemGridController : MonoBehaviour
    {
        private const float ItemsRearrangementDelay = .3f;
        private const float FirstItemSelectionDelay = .1f;

        private readonly string _itemButtonKey = "ItemButton".ToLower();

        private readonly Dictionary<BagItemV001, GameObject> _bagItemButtonMap = new();
        private readonly Dictionary<BagItemV001, GameObject> _animatingBagItemButtonMap = new();
        private readonly List<BagItemV001> _bagItemsToMove = new();

        private void Start()
        {
            GameRunner.Instance.Core.Player.Bag.OnChangeItemType
                .Subscribe(HandleItemTypeChange)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnSelectBagItem
                .Subscribe(HandleItemSelection)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnBagItemPositionsReady
                .Subscribe(HandleItemPositions)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnChangeConfirmationVisibility
                .Subscribe(HandleConfirmationVisibility)
                .AddTo(this);
        }

        private void InitializeItems()
        {
            var bagCanvasController = GetComponent<BagCanvasController>();
            var bagItemsContainer = bagCanvasController.bagItemsContainer;

            if (!GameRunner.Instance.AssetMap.ContainsKey(_itemButtonKey)) return;
            bagItemsContainer.GetComponent<CanvasGroup>().alpha = 0;
            foreach (var itemButton in _bagItemButtonMap.Values)
            {
                Destroy(itemButton);
            }

            _bagItemButtonMap.Clear();
            GameRunner.Instance.Core.Player.Bag.ClearItemPositions();
            var bagItems = GameRunner.Instance.Core.Player.Bag.FilteredBagItems;
            foreach (var bagItem in bagItems)
            {
                var bagItemButton = Instantiate(
                    GameRunner.Instance.AssetMap[_itemButtonKey],
                    bagItemsContainer.transform
                ) as GameObject;
                if (bagItemButton == null) continue;
                _bagItemButtonMap.Add(bagItem, bagItemButton);
                var controller = bagItemButton.GetComponent<ItemButtonController>();
                controller.BagItem = bagItem;
                controller.ReadOnly = false;
                controller.SetVisible(false, false);
            }

            bagItemsContainer.GetComponent<CanvasGroup>().alpha = 1;
        }

        private void HandleItemTypeChange(ItemType itemType)
        {
            InitializeItems();
        }

        private void HandleItemSelection(BagItemV001 bagItem)
        {
            if (bagItem == null) return;
            var itemButton = _bagItemButtonMap[bagItem];
            var controller = itemButton.GetComponent<ItemButtonController>();
            if (
                controller != null &&
                controller.button != null &&
                EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject != controller.button.gameObject
            ) EventSystem.current.SetSelectedGameObject(controller.button.gameObject);

            var bagCanvasController = GetComponent<BagCanvasController>();
            var hoverFrame = bagCanvasController.hoverFrame;
            var hoverFrameController = hoverFrame.GetComponent<HoverFrameController>();
            hoverFrameController.TargetItem = itemButton;
        }

        private void HandleItemPositions(Dictionary<BagItemV001, Vector3> itemPositions)
        {
            var bagCanvasController = GetComponent<BagCanvasController>();
            var itemAnimationDock = bagCanvasController.itemAnimationDock;

            if (_animatingBagItemButtonMap.Count == 0)
            {
                InitializeAnimatingItemButton(itemPositions);
                itemAnimationDock.GetComponent<CanvasGroup>().alpha = 0;
                itemAnimationDock.SetActive(false);
                ShowUnmovingItemButtons();
                var controller = _bagItemButtonMap.Values.First().GetComponent<ItemButtonController>();
                EventSystem.current.SetSelectedGameObject(controller.button.gameObject);
                return;
            }

            _bagItemsToMove.Clear();

            itemAnimationDock.SetActive(true);
            itemAnimationDock.GetComponent<CanvasGroup>().alpha = 1;

            foreach (var item in _animatingBagItemButtonMap.Keys)
            {
                var bagItemButton = _animatingBagItemButtonMap[item];
                var controller = bagItemButton.GetComponent<ItemButtonController>();
                if (!itemPositions.TryGetValue(item, out var position))
                {
                    controller.SetVisible(false, true);
                }
                else
                {
                    bagItemButton.transform.DOMove(position, ItemsRearrangementDelay);
                    if (_animatingBagItemButtonMap.ContainsKey(item)) _bagItemsToMove.Add(item);
                }
            }

            ShowUnmovingItemButtons();

            DOVirtual.DelayedCall(ItemsRearrangementDelay, () =>
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
            
            var bagItem = GameRunner.Instance.Core.Player.Bag.CurrentBagItem;
            if (bagItem == null) return;
            var bagItemButton = _bagItemButtonMap[bagItem];
            
            DOVirtual.DelayedCall(
                FirstItemSelectionDelay,
                () => EventSystem.current.SetSelectedGameObject(bagItemButton)
            );
        }

        private void ShowUnmovingItemButtons()
        {
            foreach (var (item, itemButton) in _bagItemButtonMap)
            {
                var controller = itemButton.GetComponent<ItemButtonController>();
                controller.SetVisible(!_bagItemsToMove.Contains(item), true);
            }
        }

        private void ShowMovingItemButtons()
        {
            foreach (var bagItem in _bagItemsToMove)
            {
                if (!_bagItemButtonMap.ContainsKey(bagItem)) continue;
                var itemButton = _bagItemButtonMap[bagItem];
                var controller = itemButton.GetComponent<ItemButtonController>();
                if (!_bagItemsToMove.Contains(bagItem)) return;
                controller.SetVisible(true, false);
            }
        }

        private void InitializeAnimatingItemButton(Dictionary<BagItemV001, Vector3> itemPositions)
        {
            var bagCanvasController = GetComponent<BagCanvasController>();
            var itemAnimationDock = bagCanvasController.itemAnimationDock;

            foreach (var bagItem in _animatingBagItemButtonMap.Values) Destroy(bagItem);

            _animatingBagItemButtonMap.Clear();
            if (!GameRunner.Instance.AssetMap.ContainsKey(_itemButtonKey)) return;
            var updatedItems = itemPositions.Keys;
            foreach (var bagItemV001 in updatedItems)
            {
                var bagItemButton = _bagItemButtonMap[bagItemV001];
                var animatingItemButton = Instantiate(
                    GameRunner.Instance.AssetMap[_itemButtonKey],
                    itemAnimationDock.transform
                ) as GameObject;
                if (animatingItemButton == null) continue;
                var controller = bagItemButton.GetComponent<ItemButtonController>();
                var animatingController = animatingItemButton.GetComponent<ItemButtonController>();
                controller.CopyTo(animatingController);
                animatingController.SetVisible(true, false);
                animatingController.ReadOnly = true;
                _animatingBagItemButtonMap.Add(bagItemV001, animatingItemButton);
            }
        }
    }
}