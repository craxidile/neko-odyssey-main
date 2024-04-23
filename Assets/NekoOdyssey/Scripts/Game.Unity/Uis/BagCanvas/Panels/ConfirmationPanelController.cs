using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.Items.Entities.ItemEntity.Models;
using UniRx;
using UniRx.InternalUtil;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Panels
{
    public class ConfirmationPanelController : MonoBehaviour
    {
        private const float FadingDelay = .3f;

        public CanvasGroup canvasGroup;
        public Text descriptionText;
        public ButtonHover confirmButton;
        public ButtonHover cancelButton;

        private bool _visible;

        private void Awake()
        {
            SetVisible(false, false);
        }

        private void Start()
        {
            cancelButton.onClick.AddListener(HandleCancellation);
            confirmButton.onClick.AddListener(HandleConfirmation);

            GameRunner.Instance.Core.Player.Bag.OnChangeConfirmationVisibility
                .Subscribe(HandleConfirmationVisibilityChange)
                .AddTo(this);
            GameRunner.Instance.Core.Player.Bag.OnSelectItem
                .Subscribe(HandleItemSelection)
                .AddTo(this);
        }

        private void HandleCancellation()
        {
            GameRunner.Instance.Core.Player.Bag.SetConfirmationVisible(false);
        }

        private void HandleConfirmation()
        {
            GameRunner.Instance.Core.Player.Bag.UseItem();
            GameRunner.Instance.Core.Player.Bag.SetConfirmationVisible(false);
        }

        private void HandleItemSelection(Item item)
        {
            if (item == null) return;
            descriptionText.text = $"{item.Name}\n{item.Description}";
        }

        private void HandleConfirmationVisibilityChange(bool visible)
        {
            
            if (visible) EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
            SetVisible(visible);
        }

        public void SetVisible(bool visible, bool animating = true)
        {
            _visible = visible;
            var canvasGroupAlpha = !visible ? 0 : 1;
            if (!animating)
            {
                canvasGroup.alpha = canvasGroupAlpha;
                SetActive(false);
                return;
            }

            canvasGroup.DOFade(canvasGroupAlpha, FadingDelay)
                .OnStart(() =>
                {
                    if (visible) return;
                    SetActive(false);
                })
                .OnComplete(() =>
                {
                    if (!visible) return;
                    SetActive(true);
                });
        }

        private void SetActive(bool active)
        {
            canvasGroup.blocksRaycasts = active;
            canvasGroup.interactable = active;
            EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
        }
    }
}