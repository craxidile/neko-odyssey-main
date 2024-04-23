using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemGrid
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
            normalImage.sprite = GameRunner.Instance.AssetMap[_itemType.NormalIcon] as Sprite;
            activeImage.sprite = GameRunner.Instance.AssetMap[_itemType.ActiveIcon] as Sprite;
        }

        public void SetCurrent(bool current)
        {
            _current = current;
            normalImage.sprite = !_current
                ? GameRunner.Instance.AssetMap[_itemType.NormalIcon] as Sprite
                : GameRunner.Instance.AssetMap[_itemType.ActiveIcon] as Sprite;
        }
    }
}