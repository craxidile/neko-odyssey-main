using System;
using DG.Tweening;
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
        public bool isActive = false;


        [SerializeField] private HorizontalLayoutGroup topLeftLayoutGroup;
        [SerializeField] private HorizontalLayoutGroup topRightLayoutGroup;

        [SerializeField] Image[] foodImage;

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
            canvasGroup.DOFade(1, 0.3f).SetDelay(0.1f).OnStart(() => { isActive = true; });
        }

        // Start is called before the first frame update
        void Start()
        {
            phoneButton.onClick.AddListener(HandlePhoneClick);
            bagButton.onClick.AddListener(HandleBackClick);

            GameRunner.Instance.Core.Player.Stamina.OnStaminaChange.Subscribe(HandleStaminaChanged).AddTo(this);
            HandleStaminaChanged(0);
        }

        // Update is called once per frame
        void Update()
        {
            CheckActivation();
            if (!isActive) return;

            LayoutRebuilder.MarkLayoutForRebuild(topLeftLayoutGroup.GetComponent<RectTransform>());
            LayoutRebuilder.MarkLayoutForRebuild(topRightLayoutGroup.GetComponent<RectTransform>());

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

        private void HandlePhoneClick()
        {
            GameRunner.Instance.Core.Player.SetPhoneMode();
        }

        private void HandleBackClick()
        {
            GameRunner.Instance.Core.Player.SetBagMode();
        }

        void CheckActivation()
        {
            if (isActive != canvasGroup.interactable)
            {
                canvasGroup.interactable = isActive;
                var targetAlpha = isActive ? 1 : 0;
                canvasGroup.DOFade(targetAlpha, 0.3f);
            }
        }


        void HandleStaminaChanged(float deltaStamina)
        {
            var stamina = GameRunner.Instance.Core.Player.Stamina.Stamina;

            float foodGuage = (float)stamina / 100f;
            for (int i = 0; i < foodImage.Length; i++)
            {
                if (foodGuage > i + 1)
                {
                    foodImage[i].fillAmount = 1;
                }
                else if (foodGuage > i)
                {
                    foodImage[i].fillAmount = foodGuage - i;
                }
                else
                {
                    foodImage[i].fillAmount = 0;
                }
            }
        }
    }
}