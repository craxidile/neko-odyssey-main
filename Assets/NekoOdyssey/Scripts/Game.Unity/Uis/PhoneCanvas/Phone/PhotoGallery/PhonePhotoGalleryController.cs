using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
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

            DOVirtual.DelayedCall(1f, () =>
            {
                if (GameRunner.Instance.Ready)
                {
                    GeneratePhotoGalleryEntryGrid(GameRunner.Instance.Core.Player.Phone.PhotoGallery.Entries);
                }
                else
                {
                    GameRunner.Instance.OnReady.Subscribe(ready =>
                    {
                        if (!ready) return;
                        GeneratePhotoGalleryEntryGrid(GameRunner.Instance.Core.Player.Phone.PhotoGallery.Entries);
                    });
                }
            });
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.Phone.PhotoGallery.OnChangeEntries.Subscribe(GeneratePhotoGalleryEntryGrid);
        }

        private void GeneratePhotoGalleryEntryGrid(List<PhotoGalleryEntry> photoGalleryEntries)
        {
            foreach (var socialFeedCells in _photoGalleryEntryCells)
            {
                Destroy(socialFeedCells);
            }

            _photoGalleryEntryCells.Clear();
            foreach (var photoGalleryEntry in photoGalleryEntries)
            {
                AddPhotoGalleryEntryCell(photoGalleryEntry);
            }

            var phoneCanvasController = GetComponent<PhoneCanvasController>();
            var contentPosition = _scrollRect.content.anchoredPosition;
            contentPosition.y = 0;
            _scrollRect.content.anchoredPosition = contentPosition;
        }

        private void AddPhotoGalleryEntryCell(PhotoGalleryEntry photoGalleryEntry)
        {
            var newPhotoEntryCellObject = Instantiate(_gridCell, _gridCell.transform.parent);
            var photoTransform = newPhotoEntryCellObject.GetComponent<PhotoGalleryEntryCellController>().photoTransform;
            var assetBundleName = $"{photoGalleryEntry.CatCode.ToLower()}snap";
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