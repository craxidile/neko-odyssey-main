using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Repo;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Bag
{
    public class PlayerBag
    {
        private const float SelectBagItemDelay = .3f;

        public int GridColumnCount { get; private set; }
        public ItemType CurrentItemType { get; private set; }
        public BagItemV001 CurrentBagItem { get; private set; }
        public bool ConfirmationVisible { get; private set; }
        public List<BagItemV001> BagItems { get; } = new();
        public Dictionary<BagItemV001, Vector3> ItemPositionMap { get; } = new();

        public List<BagItemV001> FilteredBagItems => CurrentItemType.IsAll
            ? BagItems.ToList()
            : BagItems
                .Where(bi => bi.Item.Type.Code == CurrentItemType.Code)
                .ToList();

        public Subject<ItemType> OnChangeItemType { get; } = new();
        public Subject<bool> OnChangeConfirmationVisibility { get; } = new();
        public Subject<BagItemV001> OnSelectBagItem { get; } = new();
        public Subject<BagItemV001> OnUseBagItem { get; } = new();
        public Subject<Dictionary<BagItemV001, Vector3>> OnBagItemPositionsReady { get; } = new();
        public Subject<Unit> OnSearchBag { get; } = new();
        public Subject<Unit> OnEat { get; } = new();

        public void Bind()
        {
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
            GameRunner.Instance.PlayerInputHandler.OnPrevTabTriggerred
                .Subscribe(_ => HandlePrevTab())
                .AddTo(GameRunner.Instance);
            GameRunner.Instance.PlayerInputHandler.OnNextTabTriggerred
                .Subscribe(_ => HandleNextTab())
                .AddTo(GameRunner.Instance);
        }

        public void Unbind()
        {
        }
        
        private void InitializeItems()
        {
            SetDefaultItemType();
            LoadBagItems();
        }

        private void HandleCancellation()
        {
            if (GameRunner.Instance.Core.Player.Mode != PlayerMode.OpenBag) return;
            SetConfirmationVisible(false);
        }

        private void HandlePrevTab()
        {
            if (GameRunner.Instance.Core.Player.Mode != PlayerMode.OpenBag) return;
            var itemTypes = GameRunner.Instance.Core.MasterData.ItemsMasterData.ItemTypes.ToList();
            var index = itemTypes.IndexOf(CurrentItemType);
            SetItemType(itemTypes[Math.Max(0, index - 1)]);
        }
        
        private void HandleNextTab()
        {
            if (GameRunner.Instance.Core.Player.Mode != PlayerMode.OpenBag) return;
            var itemTypes = GameRunner.Instance.Core.MasterData.ItemsMasterData.ItemTypes.ToList();
            var index = itemTypes.IndexOf(CurrentItemType);
            SetItemType(itemTypes[Math.Min(itemTypes.Count - 1, index + 1)]);
        }

        private void LoadBagItems()
        {
            var masterItems = GameRunner.Instance.Core.MasterData.ItemsMasterData.Items.ToList();
            ICollection<BagItemV001> bagItems;

            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = true }))
            {
                var bagItemRepo = new BagItemV001Repo(dbContext);
                bagItems = bagItemRepo.List();
            }

            if (bagItems == null) return;
            foreach (var bagItem in bagItems)
            {
                var item = masterItems.FirstOrDefault(i => i.Code == bagItem.ItemCode);
                bagItem.Item = item;
                BagItems.Add(bagItem);
            }
        }

        public void SetItemType(ItemType itemType, bool forceFirstItemSelection = true)
        {
            var eligibleToSelectFirstItem = itemType?.Code != CurrentItemType?.Code;
            CurrentItemType = itemType;
            OnChangeItemType.OnNext(CurrentItemType);
            if (forceFirstItemSelection && eligibleToSelectFirstItem)
                DOVirtual.DelayedCall(SelectBagItemDelay, () => SelectBagItem(FilteredBagItems.FirstOrDefault()));
        }

        public void SetDefaultItemType()
        {
            SetItemType(GameRunner.Instance.Core.MasterData.ItemsMasterData.ItemTypes.First(it => it.IsAll));
            DOVirtual.DelayedCall(SelectBagItemDelay, () => SelectBagItem(FilteredBagItems.FirstOrDefault()));
        }

        public void AddBagItem(Item item)
        {
            var bagItem = new BagItemV001(item);
            BagItems.Add(bagItem);
            using (var dbContext = new SaveV001DbContext(new() { CopyMode = DbCopyMode.DoNotCopy, ReadOnly = false }))
            {
                var bagItemRepo = new BagItemV001Repo(dbContext);
                bagItemRepo.Add(bagItem);
            }
        }

        public void SelectBagItem(BagItemV001 bagItem)
        {
            CurrentBagItem = bagItem;
            OnSelectBagItem.OnNext(CurrentBagItem);
        }

        public void UseBagItem()
        {
            //GameRunner.Instance.Core.Player.AddStamina(CurrentBagItem.Item.Stamina);
            var bagItem = CurrentBagItem;
            var itemType = bagItem.Item.Type;
            if (itemType.Name == "Food")
                OnEat.OnNext(default);
            else
                OnSearchBag.OnNext(default);

            // GameRunner.Instance.Core.Player.AddStamina(CurrentBagItem.Item.Stamina);
            
            OnUseBagItem.OnNext(CurrentBagItem);

            var index = FilteredBagItems.IndexOf(CurrentBagItem);

            if (!CurrentBagItem.Item.SingleUse) return;

            BagItems.Remove(CurrentBagItem);
            GameRunner.Instance.Core.Player.SaveDbWriter.Add(dbContext =>
            {
                var bagItemRepo = new BagItemV001Repo(dbContext);
                bagItemRepo.Remove(CurrentBagItem);
            });

            index = Math.Min(FilteredBagItems.Count - 1, Math.Max(0, index - 1));
            // SelectBagItem(null);
            SetItemType(CurrentItemType, false);

            DOVirtual.DelayedCall(
                SelectBagItemDelay,
                () => SelectBagItem(FilteredBagItems.Count > 0 ? FilteredBagItems[index] : null)
            );
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

        public void SetItemPosition(BagItemV001 item, Vector3 position)
        {
            ItemPositionMap[item] = position;
            if (ItemPositionMap.Count != FilteredBagItems.Count) return;
            OnBagItemPositionsReady.OnNext(ItemPositionMap);
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