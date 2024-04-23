using System;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemTypeEntity.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Panels
{
    public class HoverFrameController : MonoBehaviour
    {
        public Text nameText;
        
        public GameObject TargetItem { get; set; }

        private void Start()
        {
            GameRunner.Instance.Core.Player.Bag.OnChangeItemType
                .Subscribe(HandleItemTypeChange)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnSelectItem
                .Subscribe(HandleItemSelection)
                .AddTo(this);
        }

        private void Update()
        {
            if (TargetItem == null) return;
            gameObject.transform.position = TargetItem.transform.position;
        }

        private void HandleItemTypeChange(ItemType itemType)
        {
            gameObject.SetActive(false);
        }

        private void HandleItemSelection(Item item)
        {
            if (item == null) return;
            nameText.text = item.Name;
        }
    }
}