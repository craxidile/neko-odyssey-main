using System.Collections.Generic;
using Assets.NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas;
using DG.Tweening;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Models;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas
{
    public class PhoneSocialNetworkController : MonoBehaviour
    {
        private GameObject _socialFeedCell;

        private List<GameObject> _socialFeedCells = new();

        private void Awake()
        {
            var phoneCanvasController = GameRunner.Instance.GameCore.Player.Phone.GameObject
                .GetComponent<PhoneCanvasController>();
            _socialFeedCell = phoneCanvasController.socialFeedCell;
            
            DOVirtual.DelayedCall(1f, () =>
            {
                Debug.Log($">>player_menu_ready<< awake");
                if (GameRunner.Instance.Ready)
                {
                    GenerateSocialFeedGrid(GameRunner.Instance.GameCore.Player.Phone.SocialNetwork.Feeds);
                }
                else
                {
                    GameRunner.Instance.OnReady.Subscribe(ready =>
                    {
                        if (!ready) return;
                        GenerateSocialFeedGrid(GameRunner.Instance.GameCore.Player.Phone.SocialNetwork.Feeds);
                    });
                }
            });
        }

        private void Start()
        {
            GameRunner.Instance.GameCore.Player.Phone.SocialNetwork.OnChangeFeeds.Subscribe(GenerateSocialFeedGrid);
        }

        private void GenerateSocialFeedGrid(List<SocialFeed> feeds)
        {
            foreach (var socialFeedCells in _socialFeedCells)
            {
                Destroy(socialFeedCells);
            }

            _socialFeedCells.Clear();
            foreach (var feed in feeds)
            {
                Debug.Log($">>feed<< {feed}");
                AddSocialFeedCell(feed);
            }

            var phoneCanvasController = GetComponent<PhoneCanvasController>();
            var contentPosition = phoneCanvasController.socialFeedScrollRect.content.anchoredPosition;
            contentPosition.y = 0;
            phoneCanvasController.socialFeedScrollRect.content.anchoredPosition = contentPosition;
        }

        private void AddSocialFeedCell(SocialFeed feed)
        {
            var newPostObject = Instantiate(_socialFeedCell, _socialFeedCell.transform.parent);
            var photoTransform = newPostObject.GetComponent<SocialFeedCellController>().photoTransform;
            var assetBundleName = $"{feed.CatCode.ToLower()}snap";
            if (GameRunner.Instance.AssetMap.TryGetValue(assetBundleName, out var asset))
            {
                var catPhoto = Instantiate(asset, newPostObject.transform) as GameObject;
                if (catPhoto == null) return;
                var catPhotoTransform = catPhoto.GetComponent<RectTransform>();
                catPhotoTransform.SetSiblingIndex(1);
                catPhotoTransform.anchorMin = photoTransform.anchorMin;
                catPhotoTransform.anchorMax = photoTransform.anchorMax;
                catPhotoTransform.anchoredPosition = photoTransform.anchoredPosition;
                catPhotoTransform.sizeDelta = photoTransform.sizeDelta;
            }

            _socialFeedCells.Add(newPostObject);
            newPostObject.SetActive(true);
        }
    }
}