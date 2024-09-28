using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Extensions;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Uis.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


namespace NekoOdyssey.Scripts.Game.Unity.Uis.ItemObtain
{
    public class ItemObtainCanvasController : MonoBehaviour
    {
        [SerializeField] float popUpShowDuration = 1, popUpFadeDuration = 0.3f;

        [Space]

        [SerializeField] CanvasGroup popUpCanvasGroup;
        [SerializeField] Text popUpText;
        [SerializeField] Image popUpImage;
        [SerializeField] GameObject imageGroupObject;


        Queue<ItemObtainPopUpDetail> _itemPopUpQuene = new Queue<ItemObtainPopUpDetail>();
        bool _isActive = false;

        private void Awake()
        {
            GameRunner.Instance.Core.Player.ItemObtainPopUp.GameObject = gameObject;
        }

        // Start is called before the first frame update
        void Start()
        {

            GameRunner.Instance.Core.Player.ItemObtainPopUp.OnShowPopUp.Subscribe(HandleItemPopUp).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {

        }


        void HandleItemPopUp(ItemObtainPopUpDetail itemPopUp)
        {
            _itemPopUpQuene.Enqueue(itemPopUp);

            if (_isActive)
            {
                return;
            }

            _isActive = true;

            ShowItemPopUpQuene();
        }
        int _debugBreak;
        void ShowItemPopUpQuene()
        {
            var item = _itemPopUpQuene.Dequeue();

            var obtainedText = GameRunner.Instance.Core.Uis.Localisation.TranslateCurrent("obtained");
            var text = $"{item.itemName} - {item.itemQty} {obtainedText}";
            popUpText.text = text;


            if (item.itemIcon == default)
            {
                imageGroupObject.SetActive(false);
            }
            else
            {
                popUpImage.sprite = item.itemIcon;
                imageGroupObject.SetActive(true);
            }

            _debugBreak++;
            if (_debugBreak >= 100)
            {
                Debug.LogError("Loop more than 100");
                return;
            }


            popUpCanvasGroup.LerpAlpha(1, popUpFadeDuration, onComplete: () =>
            {
                DOVirtual.DelayedCall(popUpShowDuration, () =>
                {
                    popUpCanvasGroup.LerpAlpha(0, popUpFadeDuration, onComplete: () =>
                    {
                        if (_itemPopUpQuene.Count > 0)
                        {
                            ShowItemPopUpQuene();
                        }
                        else
                        {
                            _isActive = false;
                        }
                    });
                });
            });
        }
    }


    public class ItemObtainPopUpDetail
    {
        public Item item;
        public string itemName;
        public Sprite itemIcon;
        public int itemQty;

        public ItemObtainPopUpDetail(Item item, int itemQty)
        {
            this.item = item;
            itemName = GameRunner.Instance.Core.MasterData.ItemsMasterData.GetLocalisedItemName(item);

            var normalIcon = item.NormalIcon.ToLower();
            if (GameRunner.Instance.AssetMap.ContainsKey(normalIcon))
            {
                itemIcon = GameRunner.Instance.AssetMap[normalIcon] as Sprite;
            }

            this.itemQty = itemQty;
        }
    }
}