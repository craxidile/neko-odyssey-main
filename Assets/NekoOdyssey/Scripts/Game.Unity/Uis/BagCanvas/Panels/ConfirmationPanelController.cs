using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.ConfirmationPanel
{
    public class ConfirmationPanelController : MonoBehaviour
    {
        private const float FadingDelay = .3f;

        public CanvasGroup canvasGroup;
        public Text descriptionText;
        public ButtonHover confirmButton;
        public ButtonHover cancelButton;

        private bool _visible;


        public UnityAction OnConfirmation_OneTime { get; set; }
        public UnityAction OnCancellation_OneTime { get; set; }

        private void Awake()
        {
            GameRunner.Instance.Core.Player.ConfirmationPanel.canvasPanel = this;
            SetVisible(false, false);
        }

        private void Start()
        {
            //GameRunner.Instance.Core.Player.ConfirmationPanel.OnShowPopUp.Subscribe(HandleItemPopUp).AddTo(this);

            confirmButton.onClick.AddListener(HandleConfirmation);
            cancelButton.onClick.AddListener(HandleCancellation);

            //GameRunner.Instance.Core.Player.Bag.OnChangeConfirmationVisibility
            //    .Subscribe(HandleConfirmationVisibilityChange)
            //    .AddTo(this);
            //GameRunner.Instance.Core.Player.Bag.OnSelectBagItem
            //    .Subscribe(HandleItemSelection)
            //    .AddTo(this);
        }

        private void HandleConfirmation()
        {
            SetVisible(false);
            //GameRunner.Instance.Core.Player.Bag.UseBagItem();
            //GameRunner.Instance.Core.Player.Bag.SetConfirmationVisible(false);
            OnConfirmation_OneTime?.Invoke();
            OnConfirmation_OneTime = null;


        }
        private void HandleCancellation()
        {
            SetVisible(false);
            //GameRunner.Instance.Core.Player.Bag.SetConfirmationVisible(false);
            OnCancellation_OneTime?.Invoke();
            OnCancellation_OneTime = null;

        }


        //private void HandleItemSelection(BagItemV001 bagItem)
        //{
        //    if (bagItem == null) return;
        //    var itemsMasterData = GameRunner.Instance.Core.MasterData.ItemsMasterData;
        //    var name = itemsMasterData.GetLocalisedItemName(bagItem.Item);
        //    var description = itemsMasterData.GetLocalisedItemDescription(bagItem.Item);
        //    descriptionText.text = $"{name}\n{description}";
        //}

        //private void HandleConfirmationVisibilityChange(bool visible)
        //{

        //    if (visible) EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
        //    SetVisible(visible);
        //}

        public void SetDescription(string titleName, string description)
        {
            //var localize_name = itemsMasterData.GetLocalisedItemName(bagItem.Item);
            //var localize_description = itemsMasterData.GetLocalisedItemDescription(bagItem.Item);
            descriptionText.text = $"{titleName}\n{description}";
        }

        public void SetVisible(bool visible, bool animating = true)
        {
            _visible = visible;
            var canvasGroupAlpha = !visible ? 0 : 1;
            if (!animating)
            {
                canvasGroup.alpha = canvasGroupAlpha;
                SetActive(visible);
                return;
            }

            canvasGroup.DOFade(canvasGroupAlpha, FadingDelay)
                .OnStart(() =>
                {
                    if (!visible)
                    {
                        SetActive(false);
                    }
                    else
                    {
                        EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
                    }
                })
                .OnComplete(() =>
                {
                    if (visible)
                    {
                        SetActive(true);
                    }
                });
        }

        private void SetActive(bool active)
        {
            canvasGroup.blocksRaycasts = active;
            canvasGroup.interactable = active;
        }
    }
}