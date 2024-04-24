using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemGrid;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemTypeHeader
{
    public class ItemTypeHeaderController: MonoBehaviour
    {
        private readonly string _itemTypeButtonKey = "ItemTypeButton".ToLower();

        private readonly Dictionary<string, ItemTypeButtonController> _itemTypeButtonControllerMap = new();

        private void Start()
        {
            AssetBundleUtils.OnReady(InitializeHeader);

            GameRunner.Instance.Core.Player.Bag.OnChangeItemType
                .Subscribe(HandleItemTypeChange)
                .AddTo(this);
        }

        private void InitializeHeader()
        {
            if (!GameRunner.Instance.AssetMap.ContainsKey(_itemTypeButtonKey)) return;
            
            var bagCanvasController = GetComponent<BagCanvasController>();
            var bagHeaderContainer = bagCanvasController.bagHeaderContainer;

            _itemTypeButtonControllerMap.Clear();
            var itemTypes = GameRunner.Instance.Core.MasterData.ItemsMasterData.ItemTypes;
            foreach (var itemType in itemTypes)
            {
                var itemTypeButton = Instantiate(
                    GameRunner.Instance.AssetMap[_itemTypeButtonKey],
                    bagHeaderContainer.transform
                ) as GameObject;
                if (itemTypeButton == null) continue;
                itemTypeButton.name = $"ItemType{itemType.Code} ({itemType.Name})";
                var controller = itemTypeButton.GetComponent<ItemTypeButtonController>();
                _itemTypeButtonControllerMap.Add(itemType.Code, controller);
                controller.ItemType = itemType;
            }
            HandleItemTypeChange(GameRunner.Instance.Core.Player.Bag.CurrentItemType);
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
        }
    }
}