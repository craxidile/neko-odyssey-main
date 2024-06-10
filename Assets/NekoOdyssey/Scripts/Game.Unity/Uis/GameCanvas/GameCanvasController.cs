using System;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Extensions;
using NekoOdyssey.Scripts.Game.Unity.AssetBundles;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Unity.Uis.Utils;
using TMPro;
using UniRx;
// using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.GameCanvas
{
    public class GameCanvasController : MonoBehaviour
    {
        private const float MaxStaminaDelay = 1f;

        private bool _initialized;
        private Tween _staminaTween;
        private bool _gamepadConnected;

        public bool isActive;

        [SerializeField] private HorizontalLayoutGroup topLeftLayoutGroup;
        [SerializeField] private HorizontalLayoutGroup topRightLayoutGroup;

        [SerializeField] Image[] foodImages;
        float _staminaGaugeRatio;

        [SerializeField] TextMeshProUGUI socialLikeText, followerText, moneyText;
        [SerializeField] Text gameTimeText, gameDayText;
        [SerializeField] ButtonHover phoneButton, bagButton;
        [SerializeField] CanvasGroup socialNotificationCanvasGroup, bagNotificationCanvasGroup;
        [SerializeField] TextMeshProUGUI socialNotificationText, bagNotificationText;

        public Image keyboardPhoneKey;
        public Image keyboardBagKey;
        public Image psPhoneKey;
        public Image psBagKey;
        public Image xboxPhoneKey;
        public Image xboxBagKey;
        public Text activeMissionText;
        public Text finishedMissionText;

        CanvasGroup canvasGroup;

        [Header("testing")][SerializeField] int testNumber;
        [SerializeField] public float hungryValue;
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
            GameRunner.Instance.TimeRoutine.OnTimeUpdate
                .Subscribe(_ => HandleTimeChange())
                .AddTo(this);
            GameRunner.Instance.Core.Player.OnChangeLikeCount
                .Subscribe(HandleLikeCountChange)
                .AddTo(this);
            GameRunner.Instance.Core.Player.OnChangeFollowerCount
                .Subscribe(HandleFollowerCountChange)
                .AddTo(this);

            UpdateMissionText(default);
            GameRunner.Instance.Core.Player.OnFinishDemo
                .Subscribe(UpdateMissionText)
                .AddTo(this);

            UpdateStamina(GameRunner.Instance.Core.Player.Stamina.Stamina);

            HandleTimeChange();

            socialLikeText.text = $"{GameRunner.Instance.Core.Player.LikeCount}";
            followerText.text = $"{GameRunner.Instance.Core.Player.FollowerCount}";
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
            var gamepadCount = Gamepad.all.Count();
            if (gamepadCount == 0)
            {
                _gamepadConnected = false;
                UpdateGamepadButtons();
                return;
            }

            if (gamepadCount > 0 && !_gamepadConnected)
            {
                _gamepadConnected = true;
                Debug.Log($">>gamepad_count<< {gamepadCount}");
                UpdateGamepadButtons();
            }
        }

        private void UpdateMissionText(Unit _)
        {
            var finished = GameRunner.Instance.Core.Player.DemoFinished;
            Debug.Log($">>finished<< {finished}");
            activeMissionText.gameObject.SetActive(!finished);
            finishedMissionText.gameObject.SetActive(finished);
        }

        private void UpdateGamepadButtons()
        {
            var gamepad = Gamepad.current;
            if (gamepad == null)
            {
                keyboardPhoneKey.gameObject.SetActive(true);
                keyboardBagKey.gameObject.SetActive(true);
                psPhoneKey.gameObject.SetActive(false);
                psBagKey.gameObject.SetActive(false);
                xboxPhoneKey.gameObject.SetActive(false);
                xboxBagKey.gameObject.SetActive(false);
            }
            else if (gamepad is DualShockGamepad)
            {
                keyboardPhoneKey.gameObject.SetActive(false);
                keyboardBagKey.gameObject.SetActive(false);
                psPhoneKey.gameObject.SetActive(true);
                psBagKey.gameObject.SetActive(true);
                xboxPhoneKey.gameObject.SetActive(false);
                xboxBagKey.gameObject.SetActive(false);
                print(">>gamepad<< Playstation gamepad");
            }
            else if (gamepad is XInputController) 
            {
                keyboardPhoneKey.gameObject.SetActive(false);
                keyboardBagKey.gameObject.SetActive(false);
                psPhoneKey.gameObject.SetActive(false);
                psBagKey.gameObject.SetActive(false);
                xboxPhoneKey.gameObject.SetActive(true);
                xboxBagKey.gameObject.SetActive(true);
                print(">>gamepad<< Xbox gamepad");
            }
            
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

        private void HandleLikeCountChange(int likeCount)
        {
            socialLikeText.text = likeCount.ToString("N0");
            RebuildLayout();
        }

        private void HandleFollowerCountChange(int followerCount)
        {
            Debug.Log($">>follower_count<< {followerCount}");
            followerText.text = followerCount.ToString("N0");
            RebuildLayout();
        }

        private void HandleTimeChange()
        {
            var timeRoutine = GameRunner.Instance.TimeRoutine;
            gameTimeText.text = timeRoutine.GetUiTimeText();

            var dayText = timeRoutine.CurrentDay.ToText();
            var gameDayLocaliser = gameDayText.GetComponent<UiTextLocaliser>();
            gameDayLocaliser.OriginalText = dayText;

            RebuildLayout();
        }

        private void RebuildLayout()
        {
            LayoutRebuilder.MarkLayoutForRebuild(topLeftLayoutGroup.GetComponent<RectTransform>());
            LayoutRebuilder.MarkLayoutForRebuild(topRightLayoutGroup.GetComponent<RectTransform>());
        }

        private void UpdateStamina(int stamina)
        {
            Debug.Log($">>stamina<< {stamina}");
            _staminaTween?.Kill();

            var staminaRatio = (float)stamina / AppConstants.Stamina.MaxNormal;

            //var foodImage = foodImages[0];

            if (!_initialized)
            {
                //foodImage.fillAmount = staminaRatio;
                _staminaGaugeRatio = staminaRatio;
                UpdateStaminaGauge();
                _initialized = true;
            }

            //var staminaDelay = foodImage.fillAmount * MaxStaminaDelay;
            var staminaDelay = _staminaGaugeRatio * MaxStaminaDelay;
            _staminaTween = DOTween.To(
                () => _staminaGaugeRatio,
                s =>
                {
                    _staminaGaugeRatio = s;
                    UpdateStaminaGauge();
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


        private void UpdateStaminaGauge()
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