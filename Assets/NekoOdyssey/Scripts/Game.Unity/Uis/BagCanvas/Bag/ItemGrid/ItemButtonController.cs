using System.Collections;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemGrid
{
    public class ItemButtonController : MonoBehaviour
    {
        private BagItemV001 _bagItem;
        private bool _visible;
        private bool _hovered;
        private float _hoverAlphaTimeCount;

        public CanvasGroup hoverCanvasGroup;
        public Image iconImage;
        public Text nameText;
        public CanvasGroup canvasGroup;
        public ButtonHover button;
        public float hoverFadeDuration = .2f;


        public BagItemV001 BagItem
        {
            get => _bagItem;
            set => SetBagItem(value);
        }

        public bool ReadOnly { get; set; }


        private void Awake()
        {
            if (ReadOnly) return;
            button.onHover.AddListener(HandleButtonHover);
            button.onClick.AddListener(HandleButtonClick);
        }

        private void Start()
        {
            if (ReadOnly)
            {
                Destroy(button);
                button = null;
            }
            else
                StartCoroutine(MeasureDimension());

            GameRunner.Instance.Core.Player.Bag.OnChangeConfirmationVisibility
                .Subscribe(HandleConfirmationVisibility)
                .AddTo(this);
        }

        void Update()
        {
            float startAlpha = ReadOnly || _hovered ? 0 : 1;
            float endAlpha = ReadOnly || _hovered ? 1 : 0;

            var evaluateAlpha = Mathf.Lerp(startAlpha, endAlpha, _hoverAlphaTimeCount);
            _hoverAlphaTimeCount += Time.deltaTime / hoverFadeDuration;

            hoverCanvasGroup.alpha = evaluateAlpha;
        }

        private void HandleButtonHover(bool hovered)
        {
            var confirmationVisible = GameRunner.Instance.Core.Player.Bag.ConfirmationVisible;
            if (ReadOnly || !_visible || confirmationVisible) return;
            _hovered = hovered;
            _hoverAlphaTimeCount = Mathf.Max(1 - _hoverAlphaTimeCount, 0);
            GameRunner.Instance.Core.Player.Bag.SelectBagItem(
                GameRunner.Instance.Core.Player.Bag.CurrentBagItem == BagItem && !hovered ? null : BagItem
            );
        }

        private void HandleButtonClick()
        {
            var confirmationVisible = GameRunner.Instance.Core.Player.Bag.ConfirmationVisible;
            if (ReadOnly || !_visible || confirmationVisible) return;
            GameRunner.Instance.Core.Player.Bag.SetConfirmationVisible(true);
        }

        private void HandleConfirmationVisibility(bool visible)
        {
            if (button == null) return;
            button.enabled = !visible;
        }

        private IEnumerator MeasureDimension()
        {
            yield return null;
            var position = transform.position;
            GameRunner.Instance.Core.Player.Bag.SetItemPosition(BagItem, position);
        }

        private void SetBagItem(BagItemV001 bagItem)
        {
            _bagItem = bagItem;
            
            name = $"Item{bagItem.Item.Code} ({bagItem.Item.Name})";
            nameText.text = bagItem.Item.Name;
            
            var normalIcon = bagItem.Item.NormalIcon.ToLower();
            if (!GameRunner.Instance.AssetMap.ContainsKey(normalIcon)) return;
            iconImage.sprite = GameRunner.Instance.AssetMap[normalIcon] as Sprite;
            iconImage.enabled = true;
        }

        public void SetVisible(bool visible, bool animating)
        {
            _visible = visible;
            if (!animating)
                canvasGroup.alpha = visible ? 1 : 0;
            else
                canvasGroup.DOFade(visible ? 1 : 0, .2f);
        }

        public void CopyTo(ItemButtonController other)
        {
            other.BagItem = BagItem;
            other.transform.position = transform.position;
            var rectTransform = GetComponent<RectTransform>();
            var otherRectTransform = other.GetComponent<RectTransform>();
            otherRectTransform.sizeDelta = rectTransform.sizeDelta;
        }
    }
}