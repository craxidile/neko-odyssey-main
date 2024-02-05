using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using NekoOdyssey.Scripts.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone.SocialNetwork
{
    public class PhoneSocialNetworkController : MonoBehaviour
    {
        private GameObject _gridCell;
        private ScrollRect _scrollRect;

        private readonly List<GameObject> _socialFeedCells = new();

        private void Awake()
        {
            var phoneCanvasController = GameRunner.Instance.Core.Player.Phone.GameObject
                .GetComponent<PhoneCanvasController>();
            var phoneCanvasUi = phoneCanvasController.phoneUiList
                .FirstOrDefault(ui => ui.phoneApp == PlayerPhoneApp.SocialNetwork);
            if (phoneCanvasUi == null) return;
            _gridCell = phoneCanvasUi.gridCell;
            _scrollRect = phoneCanvasUi.scrollRect;
            
            DOVirtual.DelayedCall(1f, () =>
            {
                if (GameRunner.Instance.Ready)
                {
                    GenerateSocialFeedGrid(GameRunner.Instance.Core.Player.Phone.SocialNetwork.Feeds);
                }
                else
                {
                    GameRunner.Instance.OnReady.Subscribe(ready =>
                    {
                        if (!ready) return;
                        GenerateSocialFeedGrid(GameRunner.Instance.Core.Player.Phone.SocialNetwork.Feeds);
                    });
                }
            });
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.Phone.SocialNetwork.OnChangeFeeds.Subscribe(GenerateSocialFeedGrid);
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
            var contentPosition = _scrollRect.content.anchoredPosition;
            contentPosition.y = 0;
            _scrollRect.content.anchoredPosition = contentPosition;
        }

        private void AddSocialFeedCell(SocialFeed feed)
        {
            var newPostObject = Instantiate(_gridCell, _gridCell.transform.parent);
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