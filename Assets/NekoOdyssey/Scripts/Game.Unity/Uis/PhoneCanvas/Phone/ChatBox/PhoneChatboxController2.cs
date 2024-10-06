using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Models;

using DG.Tweening;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone.Chatbox
{
    public class PhoneChatboxController2 : MonoBehaviour
    {
        private GameObject _gridCell;
        private ScrollRect _scrollRect;

        private void Awake()
        {
            var phoneCanvasController = GameRunner.Instance.Core.Player.Phone.GameObject
                .GetComponent<PhoneCanvasController>();
            var phoneCanvasUi = phoneCanvasController.phoneUiList
                .FirstOrDefault(ui => ui.phoneApp == PlayerPhoneApp.SocialNetwork);
            if (phoneCanvasUi == null) return;
            _gridCell = phoneCanvasUi.gridCell;
            _scrollRect = phoneCanvasUi.scrollRect;
            //_likeCountText = phoneCanvasController.likeCountText;
            //_followerCountText = phoneCanvasController.followerCountText;

            //_likeCountText.text = "0";

            AssetBundleUtils.OnReady(() =>
            {
                //GenerateSocialPostGrid(GameRunner.Instance.Core.Player.Phone.SocialNetwork.Posts);
            });
        }

        private void Start()
        {
            //GameRunner.Instance.Core.Player.Phone.SocialNetwork.OnChangeFeeds
            //    .Subscribe(GenerateSocialPostGrid)
            //    .AddTo(this);
            //GameRunner.Instance.Core.Player.OnChangeLikeCount
            //    .Subscribe(HandleLikeCountChange)
            //    .AddTo(this);
            //GameRunner.Instance.Core.Player.OnChangeFollowerCount
            //    .Subscribe(HandleFollowerCountChange)
            //    .AddTo(this);
        }
        private void GenerateSocialPostGrid(/*ICollection<SocialPostV001> posts*/)
        {
            //if (_previousFeedCount == posts.Count)
            //{
            //    Debug.Log(">>grid_posts<< update");
            //    foreach (var post in posts)
            //    {
            //        if (!_socialFeedMap.ContainsKey(post.Id)) continue;
            //        var controller = _socialFeedMap[post.Id];
            //        controller.likeText.text = $"{post.LikeCount}";
            //    }

            //    return;
            //}

            //_previousFeedCount = posts.Count;

            //foreach (var socialFeedCells in _socialFeedCells)
            //{
            //    Destroy(socialFeedCells);
            //}

            //_socialFeedCells.Clear();
            //foreach (var post in posts)
            //{
            //    AddSocialPostCell(post);
            //}

            //var phoneCanvasController = GetComponent<PhoneCanvasController>();
            //var contentPosition = _scrollRect.content.anchoredPosition;
            //contentPosition.y = 0;
            //_scrollRect.content.anchoredPosition = contentPosition;
        }
        private void AddSocialPostCell(/*SocialPostV001 post*/)
        {
            var newPostObject = Instantiate(_gridCell, _gridCell.transform.parent);
            //var controller = newPostObject.GetComponent<SocialFeedCellController>();
            //controller.likeText.text = post.LikeCount.ToString("N0");
            //var photoTransform = controller.photoTransform;
            //var assetBundleName = $"{post.Photo.AssetBundleName.ToLower()}snap";
            //if (GameRunner.Instance.AssetMap.TryGetValue(assetBundleName, out var asset))
            //{
            //    var catPhoto = Instantiate(asset, newPostObject.transform) as GameObject;
            //    if (catPhoto == null) return;
            //    var catPhotoTransform = catPhoto.GetComponent<RectTransform>();
            //    catPhotoTransform.SetSiblingIndex(1);
            //    catPhotoTransform.anchorMin = photoTransform.anchorMin;
            //    catPhotoTransform.anchorMax = photoTransform.anchorMax;
            //    catPhotoTransform.anchoredPosition = photoTransform.anchoredPosition;
            //    catPhotoTransform.sizeDelta = photoTransform.sizeDelta;
            //}

            //_socialFeedCells.Add(newPostObject);
            //_socialFeedMap[post.Id] = controller;
            //newPostObject.SetActive(true);
        }
    }
}
