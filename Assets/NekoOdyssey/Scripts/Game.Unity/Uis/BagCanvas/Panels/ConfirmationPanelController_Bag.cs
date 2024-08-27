using DG.Tweening;
using NekoOdyssey.Scripts.Database.Domains.SaveV001.BagItemEntity.Models;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.BagCanvas.Panels
{
    public class ConfirmationPanelController_Bag : MonoBehaviour
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
            GameRunner.Instance.Core.Player.Bag.OnSelectBagItem
                .Subscribe(HandleItemSelection)
                .AddTo(this);
        }

        private void HandleCancellation()
        {
            GameRunner.Instance.Core.Player.Bag.SetConfirmationVisible(false);
        }

        private void HandleConfirmation()
        {
            GameRunner.Instance.Core.Player.Bag.UseBagItem();
            GameRunner.Instance.Core.Player.Bag.SetConfirmationVisible(false);
        }

        private void HandleItemSelection(BagItemV001 bagItem)
        {
            if (bagItem == null) return;
            var itemsMasterData = GameRunner.Instance.Core.MasterData.ItemsMasterData;
            var name = itemsMasterData.GetLocalisedItemName(bagItem.Item);
            var description = itemsMasterData.GetLocalisedItemDescription(bagItem.Item);
            descriptionText.text = $"{name}\n{description}";
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