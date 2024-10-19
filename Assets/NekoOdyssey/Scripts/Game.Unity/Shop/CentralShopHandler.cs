using System;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Shop
{
    public class CentralShopHandler : MonoBehaviour
    {
        private void Start()
        {
            GameRunner.Instance.Core.Shop.OnSell
                .Subscribe(HandleSelling)
                .AddTo(this);
        }

        private void HandleSelling(bool selling)
        {
            var sellerController = FindFirstObjectByType<SellerController>();
            if (sellerController == null)
            {
                GameRunner.Instance.Core.Shop.SetSelling(false);
                return;
            }

            if (sellerController.sellerAnimator == null) return;
            if (selling)
            {
                sellerController.AnimateOpening();
            }
            else
            {
                sellerController.AnimateClosing();
            }
        }
    }
}