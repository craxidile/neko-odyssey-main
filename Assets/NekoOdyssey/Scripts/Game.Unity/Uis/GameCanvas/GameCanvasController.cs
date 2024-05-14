using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

using NekoOdyssey.Scripts.Game.Core.Routine;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.GameCanvas
{
    public class GameCanvasController : MonoBehaviour
    {
        private const float MaxStaminaDelay = 1f;

        private bool _initialized;
        private Tween _staminaTween;

        public bool isActive;

        [SerializeField] private HorizontalLayoutGroup topLeftLayoutGroup;
        [SerializeField] private HorizontalLayoutGroup topRightLayoutGroup;

        [SerializeField] Image[] foodImages;
        float _staminaGaugeRatio;

        [SerializeField] TextMeshProUGUI socialLikeText, followerText, moneyText;
        [SerializeField] TextMeshProUGUI gameTimeText;
        [SerializeField] ButtonHover phoneButton, bagButton;
        [SerializeField] CanvasGroup socialNotificationCanvasGroup, bagNotificationCanvasGroup;
        [SerializeField] TextMeshProUGUI socialNotificationText, bagNotificationText;

        CanvasGroup canvasGroup;

        [Header("testing")][SerializeField] int testNumber;
        //[SerializeField] public float hungryValue;
        [SerializeField] int socialLikeCount, followerCount, moneyCount;
        [SerializeField] int socialNotificationCount, bagNotificationCount;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.3f).SetDelay(.1f).OnStart(() => { isActive = true; });
        }

        // Start is called before the first frame update
        void Start()
        {
            phoneButton.onClick.AddListener(HandlePhoneClick);
            bagButton.onClick.AddListener(HandleBackClick);


            AssetBundleUtils.OnReady(RebuildLayout);

            GameRunner.Instance.Core.Player.Stamina.OnChangeStamina
                .Subscribe(HandleStaminaChange)
                .AddTo(this);

            UpdateStamina(GameRunner.Instance.Core.Player.Stamina.Stamina);


            //gameTimeText.text = System.DateTime.Now.ToString("HH:mm:ss"); //change later
            var currentTimeText = TimeRoutine.currentTime.ToString();
            if (currentTimeText.StartsWith("0")) currentTimeText = currentTimeText.Substring(1);
            string timeAffixText = " AM";
            var midDayTime = new TimeHrMin("12:00");
            if (TimeRoutine.currentTime > midDayTime)
                timeAffixText = " PM";
            gameTimeText.text = currentTimeText + timeAffixText;


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
            //UpdateStaminaGuage();
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

            var staminaRatio = (float)stamina / AppConstants.Stamina.MaxNormal;

            //var foodImage = foodImages[0];

            if (!_initialized)
            {
                //foodImage.fillAmount = staminaRatio;
                _staminaGaugeRatio = staminaRatio;
                UpdateStaminaGuage();
                _initialized = true;
            }

            //var staminaDelay = foodImage.fillAmount * MaxStaminaDelay;
            var staminaDelay = _staminaGaugeRatio * MaxStaminaDelay;
            _staminaTween = DOTween.To(
                () => _staminaGaugeRatio,
                s =>
                {
                    _staminaGaugeRatio = s;
                    UpdateStaminaGuage();
                },
                staminaRatio,
                staminaDelay
            );

            _staminaTween.OnComplete(() => { _staminaTween = null; });

            Debug.Log($"UpdateStamina target ratio : {staminaRatio} , current stamina : {stamina}");
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


        void UpdateStaminaGuage()
        {
            for (int i = 0; i < foodImages.Length; i++)
            {
                if (_staminaGaugeRatio > i + 1)
                {
                    foodImages[i].fillAmount = 1;
                }
                else if (_staminaGaugeRatio > i)
                {
                    foodImages[i].fillAmount = _staminaGaugeRatio - i;
                }
                else
                {
                    foodImages[i].fillAmount = 0;
                }
            }
        }
    }
}