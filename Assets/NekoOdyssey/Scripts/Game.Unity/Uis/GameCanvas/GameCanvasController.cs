using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.GameCanvas
{
    public class GameCanvasController : MonoBehaviour
    {
        private const float MaxStaminaDelay = 1f;
        
        private Tween _staminaTween;

        public bool isActive;
        
        [SerializeField] private HorizontalLayoutGroup topLeftLayoutGroup;
        [SerializeField] private HorizontalLayoutGroup topRightLayoutGroup;

        [SerializeField] Image foodImage;

        [SerializeField] TextMeshProUGUI socialLikeText, followerText, moneyText;
        [SerializeField] TextMeshProUGUI gameTimeText;
        [SerializeField] ButtonHover phoneButton, bagButton;
        [SerializeField] CanvasGroup socialNotificationCanvasGroup, bagNotificationCanvasGroup;
        [SerializeField] TextMeshProUGUI socialNotificationText, bagNotificationText;

        CanvasGroup canvasGroup;

        [Header("testing")] [SerializeField] int testNumber;
        [SerializeField] public float hungryValue;
        [SerializeField] int socialLikeCount, followerCount, moneyCount;
        [SerializeField] int socialNotificationCount, bagNotificationCount;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.3f).SetDelay(0.1f).OnStart(() => { isActive = true; });
        }

        // Start is called before the first frame update
        void Start()
        {
            phoneButton.onClick.AddListener(HandlePhoneClick);
            bagButton.onClick.AddListener(HandleBackClick);

            AssetBundleUtils.OnReady(RebuildLayout);

            GameRunner.Instance.Core.Player.OnChangeStamina
                .Subscribe(HandleStaminaChange)
                .AddTo(this);

            UpdateStamina(GameRunner.Instance.Core.Player.Stamina);

            gameTimeText.text = DateTime.Now.ToString("HH:mm:ss"); //change later

            socialLikeText.text = socialLikeCount.ToString("N0");
            followerText.text = followerCount.ToString("N0");
            moneyText.text = moneyCount.ToString("N0");

            socialNotificationCanvasGroup.alpha = socialNotificationCount == 0 ? 0 : 1;
            socialNotificationText.text = socialNotificationCount.ToString("N0");

            bagNotificationCanvasGroup.alpha = bagNotificationCount == 0 ? 0 : 1;
            bagNotificationText.text = bagNotificationCount.ToString("N0");

            testNumber = (int)Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            CheckActivation();
        }

        private void HandlePhoneClick()
        {
            GameRunner.Instance.Core.Player.SetPhoneMode();
        }

        private void HandleBackClick()
        {
            GameRunner.Instance.Core.Player.SetBagMode();
        }

        private void HandleStaminaChange(int stamina)
        {
            UpdateStamina(stamina);
            RebuildLayout();
        }

        private void RebuildLayout()
        {
            LayoutRebuilder.MarkLayoutForRebuild(topLeftLayoutGroup.GetComponent<RectTransform>());
            LayoutRebuilder.MarkLayoutForRebuild(topRightLayoutGroup.GetComponent<RectTransform>());
        }

        private void UpdateStamina(int stamina)
        {
            _staminaTween?.Kill();
            
            var staminaRatio = (float)stamina / AppConstants.MaxStamina;
            var staminaDelay = foodImage.fillAmount * MaxStaminaDelay;
            
            _staminaTween = DOTween.To(
                () => foodImage.fillAmount,
                s => foodImage.fillAmount = s,
                staminaRatio,
                staminaDelay
            );
            _staminaTween.OnComplete(() =>
            {
                _staminaTween = null;
            });
        }

        private void CheckActivation()
        {
            if (isActive != canvasGroup.interactable)
            {
                canvasGroup.interactable = isActive;
                var targetAlpha = isActive ? 1 : 0;
                canvasGroup.DOFade(targetAlpha, 0.3f);
            }

            if (isActive) RebuildLayout();
        }
    }
}