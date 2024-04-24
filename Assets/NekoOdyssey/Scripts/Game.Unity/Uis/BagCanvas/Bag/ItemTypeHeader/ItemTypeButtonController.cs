using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemTypeHeader
{
    public class ItemTypeButtonController : MonoBehaviour
    {
        private ItemType _itemType;
        private bool _current;

        public Image normalImage;
        public Image activeImage;
        public ButtonHover button;

        public ItemType ItemType
        {
            get => _itemType;
            set => SetItemType(value);
        }

        private void Start()
        {
            button.onClick.AddListener(HandleTabChange);

            GameRunner.Instance.Core.Player.Bag.OnChangeConfirmationVisibility
                .Subscribe(HandleConfirmationVisibility)
                .AddTo(this);
        }

        private void HandleConfirmationVisibility(bool visible)
        {
            if (button == null) return;
            button.enabled = !visible;
        }

        private void HandleTabChange()
        {
            if (GameRunner.Instance.Core.Player.Bag.CurrentItemType == ItemType) return;
            GameRunner.Instance.Core.Player.Bag.SetItemType(ItemType);
        }

        private void SetItemType(ItemType itemType)
        {
            if (itemType == null) return;
            _itemType = itemType;
            var normalIcon = _itemType.NormalIcon.ToLower();
            var activeIcon = _itemType.ActiveIcon.ToLower();
            if (
                !GameRunner.Instance.AssetMap.ContainsKey(normalIcon) ||
                !GameRunner.Instance.AssetMap.ContainsKey(activeIcon) 
            ) return;
            normalImage.sprite = GameRunner.Instance.AssetMap[normalIcon] as Sprite;
            activeImage.sprite = GameRunner.Instance.AssetMap[activeIcon] as Sprite;
        }

        public void SetCurrent(bool current)
        {
            _current = current;
            var normalIcon = _itemType.NormalIcon.ToLower();
            var activeIcon = _itemType.ActiveIcon.ToLower();
            if (
                !GameRunner.Instance.AssetMap.ContainsKey(normalIcon) ||
                !GameRunner.Instance.AssetMap.ContainsKey(activeIcon) 
            ) return;
            normalImage.sprite = !_current
                ? GameRunner.Instance.AssetMap[normalIcon] as Sprite
                : GameRunner.Instance.AssetMap[activeIcon] as Sprite;
        }
    }
}