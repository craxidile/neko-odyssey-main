using DG.Tweening;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Shop
{
    public class SellerController : MonoBehaviour
    {
        public string itemShopCode;
        public Animator sellerAnimator;
        public GameObject shopCameraAnchor;

        public void AnimateOpening()
        {
            sellerAnimator.SetBool($"Sell", true);
            shopCameraAnchor.SetActive(true);
            GameRunner.Instance.cameras.sellerCamera.gameObject.SetActive(true);
        }

        public void AnimateClosing()
        {
            sellerAnimator.SetBool($"Sell", false);
            shopCameraAnchor.SetActive(false);
            GameRunner.Instance.cameras.sellerCamera.gameObject.SetActive(false);
        }
    }
}