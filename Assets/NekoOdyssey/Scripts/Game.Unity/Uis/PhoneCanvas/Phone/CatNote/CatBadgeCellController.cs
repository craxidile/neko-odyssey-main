using NekoOdyssey.Scripts.Database.Domains.Cats.Entities.CatProfileEntity.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.PhoneCanvas.Phone.CatNote
{
    public class CatBadgeCellController : MonoBehaviour
    {
        private CatProfile _catProfile;
        private bool _collected;

        public CatProfile CatProfile
        {
            get => _catProfile;
            set => SetCat(value);
        }

        public bool Collected
        {
            get => _collected;
            set => SetCollected(value);
        }

        public Image catBadgeImage;
        public Image catSilhouetteImage;
        public Text catNameText;

        private void SetCat(CatProfile catProfile)
        {
            _catProfile = catProfile;
            SetCollected(false);

            var assetMap = GameRunner.Instance.AssetMap;
            var badgeName = catProfile.BadgeName.ToLower();
            if (!assetMap.ContainsKey(badgeName)) return;

            var texture = GameRunner.Instance.AssetMap[badgeName] as Texture2D;
            if (texture == null) return;
            
            catBadgeImage.sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(.5f, .5f)
            );
        }

        private void SetCollected(bool collected)
        {
            _collected = collected;
            catNameText.text = !collected
                ? $"???"
                : GameRunner.Instance.Core.MasterData.CatsMasterData.GetLocalisedCatName(CatProfile);
            catSilhouetteImage.gameObject.SetActive(!collected);

            var mask = catBadgeImage.GetComponent<Mask>();
            mask.showMaskGraphic = collected;
        }
    }
}

