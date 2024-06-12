using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.CatPhotoEntity.Models;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Models;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone.PhotoGallery
{
    public class PhonePhotoGalleryController : MonoBehaviour
    {
        private GameObject _gridCell;
        private ScrollRect _scrollRect;

        private readonly List<GameObject> _photoGalleryEntryCells = new();

        private void Awake()
        {
            var phoneCanvasController = GameRunner.Instance.Core.Player.Phone.GameObject
                .GetComponent<PhoneCanvasController>();
            var phoneCanvasUi = phoneCanvasController.phoneUiList
                .FirstOrDefault(ui => ui.phoneApp == PlayerPhoneApp.PhotoGallery);
            if (phoneCanvasUi == null) return;
            _gridCell = phoneCanvasUi.gridCell;
            _scrollRect = phoneCanvasUi.scrollRect;

            AssetBundleUtils.OnReady(() =>
            {
                GeneratePhotoGalleryEntryGrid(GameRunner.Instance.Core.Player.Phone.PhotoGallery.Photos);
            });
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.Phone.PhotoGallery.OnChangePhotos
                .Subscribe(GeneratePhotoGalleryEntryGrid)
                .AddTo(this);
        }

        private void GeneratePhotoGalleryEntryGrid(ICollection<CatPhotoV001> photoGalleryEntries)
        {
            // foreach (var socialFeedCells in _photoGalleryEntryCells)
            // {
            //     Destroy(socialFeedCells);
            // }
            //
            // _photoGalleryEntryCells.Clear();
            // foreach (var photoGalleryEntry in photoGalleryEntries)
            // {
            //     AddPhotoGalleryEntryCell(photoGalleryEntry);
            // }
            //
            // var phoneCanvasController = GetComponent<PhoneCanvasController>();
            // var contentPosition = _scrollRect.content.anchoredPosition;
            // contentPosition.y = 0;
            // _scrollRect.content.anchoredPosition = contentPosition;
        }

        private void AddPhotoGalleryEntryCell(CatPhotoV001 catPhotoEntry)
        {
            var newPhotoEntryCellObject = Instantiate(_gridCell, _gridCell.transform.parent);
            var photoTransform = newPhotoEntryCellObject.GetComponent<PhotoGalleryEntryCellController>().photoTransform;
            var assetBundleName = $"{catPhotoEntry.CatCode.ToLower()}snap";
            if (GameRunner.Instance.AssetMap.TryGetValue(assetBundleName, out var asset))
            {
                var catPhoto = Instantiate(asset, newPhotoEntryCellObject.transform) as GameObject;
                if (catPhoto == null) return;
                var catPhotoTransform = catPhoto.GetComponent<RectTransform>();
                catPhotoTransform.SetSiblingIndex(1);
                catPhotoTransform.anchorMin = photoTransform.anchorMin;
                catPhotoTransform.anchorMax = photoTransform.anchorMax;
                catPhotoTransform.anchoredPosition = photoTransform.anchoredPosition;
                catPhotoTransform.sizeDelta = photoTransform.sizeDelta;
            }

            _photoGalleryEntryCells.Add(newPhotoEntryCellObject);
            newPhotoEntryCellObject.SetActive(true);
        }
    }
}