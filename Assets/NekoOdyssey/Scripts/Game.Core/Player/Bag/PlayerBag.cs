using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NekoOdyssey.Scripts.Game.Core.Player.Bag
{
    public class PlayerBag
    {
        public int GridColumnCount { get; private set; }
        public ItemType CurrentItemType { get; private set; }
        public Item CurrentItem { get; private set; }
        public bool ConfirmationVisible { get; private set; }
        public List<Item> Items { get; } = new();
        public Dictionary<Item, Vector3> ItemPositionMap { get; } = new();

        public List<Item> FilteredItems => Items
            .Where(i => CurrentItemType.IsAll || i.Type.Code == CurrentItemType.Code)
            .ToList();

        public Subject<ItemType> OnChangeItemType { get; } = new();
        public Subject<bool> OnChangeConfirmationVisibility { get; } = new();
        public Subject<Item> OnSelectItem { get; } = new();
        public Subject<Item> OnUseItem { get; } = new();
        public Subject<Dictionary<Item, Vector3>> OnItemPositionsReady = new();

        public void Bind()
        {
            InitializeDatabase();
        }

        public void Start()
        {
            if (GameRunner.Instance.Core.MasterData.ItemsMasterData.Ready)
                InitializeItems();
            else
                GameRunner.Instance.Core.MasterData.ItemsMasterData.OnReady
                    .Subscribe(_ => InitializeItems())
                    .AddTo(GameRunner.Instance);

            GameRunner.Instance.PlayerInputHandler.OnCancelTriggerred
                .Subscribe(_ => HandleCancellation())
                .AddTo(GameRunner.Instance);
        }

        public void Unbind()
        {
        }

        private void InitializeDatabase()
        {
        }

        private void InitializeItems()
        {
            SetDefaultItemType();
            CreateRandomItems();
        }

        private void HandleCancellation()
        {
            Debug.Log($">>handle_cancellation<< {GameRunner.Instance.Core.Player.Mode} {PlayerMode.OpenBag}");
            if (GameRunner.Instance.Core.Player.Mode != PlayerMode.OpenBag) return;
            SetConfirmationVisible(false);
        }

        private void CreateRandomItems()
        {
            var masterItems = GameRunner.Instance.Core.MasterData.ItemsMasterData.Items.ToList();
            for (var i = 0; i < 50; i++)
            {
                var randomIndex = Random.Range(0, masterItems.Count);
                Items.Add(masterItems[randomIndex].Clone() as Item);
            }
        }

        public void SetItemType(ItemType itemType)
        {
            var eligibleToSelectFirstItem = itemType?.Code != CurrentItem?.Code;
            CurrentItemType = itemType;
            OnChangeItemType.OnNext(CurrentItemType);
            if (eligibleToSelectFirstItem)
                DOVirtual.DelayedCall(.3f, () => SelectItem(FilteredItems.FirstOrDefault()));
        }

        public void SetDefaultItemType()
        {
            SetItemType(GameRunner.Instance.Core.MasterData.ItemsMasterData.ItemTypes.First(it => it.IsAll));
            DOVirtual.DelayedCall(.3f, () => SelectItem(FilteredItems.FirstOrDefault()));
        }

        public void SelectItem(Item item)
        {
            CurrentItem = item;
            OnSelectItem.OnNext(CurrentItem);
        }

        public void UseItem()
        {
            OnUseItem.OnNext(CurrentItem);
            var index = FilteredItems.IndexOf(CurrentItem);
            Items.Remove(CurrentItem);
            index = Math.Min(FilteredItems.Count - 1, Math.Max(0, index - 1));
            SelectItem(null);
            SetItemType(CurrentItemType);
            DOVirtual.DelayedCall(.3f, () => SelectItem(FilteredItems.Count > 0 ? FilteredItems[index] : null));
        }

        public void SetConfirmationVisible(bool visible)
        {
            ConfirmationVisible = visible;
            OnChangeConfirmationVisibility.OnNext(visible);
        }

        public void ClearItemPositions()
        {
            ItemPositionMap.Clear();
        }

        public void SetItemPosition(Item item, Vector3 position)
        {
            ItemPositionMap[item] = position;
            if (ItemPositionMap.Count != FilteredItems.Count) return;
            OnItemPositionsReady.OnNext(ItemPositionMap);
        }

        public void SetGridColumnCount(int count)
        {
            GridColumnCount = count;
        }

        public void Finish()
        {
            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Move);
        }
    }
}