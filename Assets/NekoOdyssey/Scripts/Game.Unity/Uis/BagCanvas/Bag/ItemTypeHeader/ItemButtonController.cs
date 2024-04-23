using System.Collections;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Bag.ItemTypeHeader
{
    public class ItemButtonController : MonoBehaviour
    {
        private Item _item;
        private bool _visible;
        private bool _hovered;
        private float _hoverAlphaTimeCount;

        public CanvasGroup hoverCanvasGroup;
        public Image iconImage;
        public Text nameText;
        public CanvasGroup canvasGroup;
        public ButtonHover button;
        public float hoverFadeDuration = .2f;


        public Item Item
        {
            get => _item;
            set => SetItem(value);
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
            GameRunner.Instance.Core.Player.Bag.SelectItem(
                GameRunner.Instance.Core.Player.Bag.CurrentItem == Item && !hovered ? null : Item
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
            GameRunner.Instance.Core.Player.Bag.SetItemPosition(Item, position);
        }

        private void SetItem(Item item)
        {
            _item = item;
            nameText.text = item.Name;
            if (!GameRunner.Instance.AssetMap.ContainsKey(item.NormalIcon)) return;
            iconImage.sprite = GameRunner.Instance.AssetMap[item.NormalIcon] as Sprite;
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
            other.Item = Item;
            other.transform.position = transform.position;
            var rectTransform = GetComponent <RectTransform>();
            var otherRectTransform = other.GetComponent<RectTransform>();
            otherRectTransform.sizeDelta = rectTransform.sizeDelta;
        }
    }
}