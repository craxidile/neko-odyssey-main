using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemTypeHeader;
using NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Panels;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemGrid
{
    public class ItemGridController : MonoBehaviour
    {
        private readonly string _itemButtonKey = "ItemButton".ToLower();
        
        private readonly Dictionary<Item, GameObject> _itemButtonMap = new();
        private readonly Dictionary<Item, GameObject> _animatingItemButtonMap = new();
        private readonly List<Item> _itemsToMove = new();
        
        private void Start()
        {
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

        private void InitializeItems()
        {
            var bagCanvasController = GetComponent<BagCanvasController>();
            var bagItemsContainer = bagCanvasController.bagItemsContainer;
            
            if (!GameRunner.Instance.AssetMap.ContainsKey(_itemButtonKey)) return;
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
                    GameRunner.Instance.AssetMap[_itemButtonKey],
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

        private void HandleItemTypeChange(ItemType itemType)
        {
            InitializeItems();
        }

        private void HandleItemSelection(Item item)
        {
            if (item == null) return;
            var itemButton = _itemButtonMap[item];
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

        private void HandleItemPositions(Dictionary<Item, Vector3> itemPositions)
        {
            var bagCanvasController = GetComponent<BagCanvasController>();
            var itemAnimationDock = bagCanvasController.itemAnimationDock;
            
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
            var bagCanvasController = GetComponent<BagCanvasController>();
            var itemAnimationDock = bagCanvasController.itemAnimationDock;
            
            foreach (var item in _animatingItemButtonMap.Values)
                Destroy(item);

            _animatingItemButtonMap.Clear();
            if (!GameRunner.Instance.AssetMap.ContainsKey(_itemButtonKey)) return;
            var updatedItems = itemPositions.Keys;
            foreach (var item in updatedItems)
            {
                var itemButton = _itemButtonMap[item];
                var animatingItemButton = Instantiate(
                    GameRunner.Instance.AssetMap[_itemButtonKey],
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