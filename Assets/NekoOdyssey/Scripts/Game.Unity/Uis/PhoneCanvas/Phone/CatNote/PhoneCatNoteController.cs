using System;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Database.Domains.Cats.Entities.CatProfileEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.PlayerCatEntity.Models;
using NekoOdyssey.Scripts.Game.Core.Player.Phone;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone.CatNote
{
    public class PhoneCatNoteController : MonoBehaviour
    {
        private readonly string _catBadgeCellName = $"CatBadge".ToLower();

        private Dictionary<string, CatBadgeCellController> _controllerMap = new();
        private GameObject _grid;
        private ScrollRect _scrollRect;

        private void Awake()
        {
            var phoneCanvasController = GameRunner.Instance.Core.Player.Phone.GameObject
                .GetComponent<PhoneCanvasController>();
            var phoneCanvasUi = phoneCanvasController.phoneUiList
                .FirstOrDefault(ui => ui.phoneApp == PlayerPhoneApp.CatNote);
            if (phoneCanvasUi == null) return;
            _grid = phoneCanvasUi.grid;
            _scrollRect = phoneCanvasUi.scrollRect;

            AssetBundleUtils.OnReady(GenerateCatBadgeCells);
        }

        private void Start()
        {
            GameRunner.Instance.Core.Player.Phone.OnChangeApp
                .Subscribe(AdjustScrollRect)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Phone.CatNote.OnChangePlayerCats
                .Subscribe(UpdateCollectedCats)
                .AddTo(this);
        }

        private void AdjustScrollRect(PlayerPhoneApp app)
        {
            if (app != PlayerPhoneApp.CatNote) return;

            var contentPosition = _scrollRect.content.anchoredPosition;
            contentPosition.y = 0;
            _scrollRect.content.anchoredPosition = contentPosition;
        }

        private void GenerateCatBadgeCells()
        {
            var assetMap = GameRunner.Instance.AssetMap;
            if (!assetMap.ContainsKey(_catBadgeCellName)) return;

            var catProfiles = GameRunner.Instance.Core.MasterData.CatsMasterData.CatProfiles;
            foreach (var catProfile in catProfiles)
            {
                AddCatBadgeCell(catProfile);
            }
        }

        private void AddCatBadgeCell(CatProfile catProfile)
        {
            var assetMap = GameRunner.Instance.AssetMap;
            var catBadgeCell = Instantiate(assetMap[_catBadgeCellName], _grid.transform);
            var controller = catBadgeCell.GetComponent<CatBadgeCellController>();
            controller.CatProfile = catProfile;
            controller.Collected = GameRunner.Instance.Core.Player.Phone.CatNote.IsCatCollected(catProfile.Code);
            _controllerMap.Add(catProfile.Code, controller);
        }

        private void UpdateCollectedCats(ICollection<PlayerCatV001> playerCats)
        {
            foreach (var playerCat in playerCats)
            {
                var catCode = playerCat.CatCode;
                if (!_controllerMap.ContainsKey(catCode)) continue;
                _controllerMap[catCode].Collected = true;
            }
        }
    }
}