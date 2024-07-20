using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts;
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
        [SerializeField] CanvasGroup popUpCanvasGroup;
        [SerializeField] Text popUpText;
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


        void HandleItemPopUp(ItemObtainPopUpDetail popUpDetail)
        {
            _itemPopUpQuene.Enqueue(popUpDetail);

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
            var popUpDetail = _itemPopUpQuene.Dequeue();

            var obtainedText = "obtained";
            var text = $"{popUpDetail.itemName} - {popUpDetail.itemQty} {obtainedText}";
            popUpText.text = text;


            if (string.IsNullOrEmpty(popUpDetail.itemImageName))
            {
                imageGroupObject.SetActive(false);
            }
            else
            {
                imageGroupObject.SetActive(true);

            }

            _debugBreak++;
            if(_debugBreak >= 100)
            {
                Debug.LogError("Loop more than 100");
                return;
            }


            popUpCanvasGroup.LerpAlpha(1, 0.3f, onComplete: () =>
            {
                DOVirtual.DelayedCall(1, () =>
                {
                    popUpCanvasGroup.LerpAlpha(0, 0.3f, onComplete: () =>
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
        public string itemName, itemImageName;
        public int itemQty;
    }
}