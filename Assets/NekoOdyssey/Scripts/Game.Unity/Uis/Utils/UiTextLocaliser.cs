using System;
using NekoOdyssey.Scripts.Constants;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.Utils
{
    public class UiTextLocaliser : MonoBehaviour
    {
        private Locale? _prevLocale;
        private string _prevText;
        private Text _text;
        private string _originalText;
        private int _baseFontSize;

        public string OriginalText
        {
            get => _originalText;
            set => SetOriginalText(value);
        }
        
        private void Awake()
        {
            _text = gameObject.GetComponent<Text>();
        }

        private void Start()
        {
            Debug.Log($">>locale<< start_util");
            _originalText = _text.text;
            _baseFontSize = _text.fontSize;
            if (GameRunner.Instance.Core.Uis.Ready)
            {
                HandleLocalisationReady(default);
            }
            else
            {
                GameRunner.Instance.Core.Uis.OnReady
                    .Subscribe(HandleLocalisationReady)
                    .AddTo(this);
            }

            GameRunner.Instance.Core.Settings.OnChangeLocale
                .Subscribe(HandleLocaleChange)
                .AddTo(this);
        }

        private void HandleLocalisationReady(Unit _)
        {
            HandleLocaleChange(GameRunner.Instance.Core.Settings.Locale);
        }

        private void HandleLocaleChange(Locale locale)
        {
            if (_prevLocale == locale) return;
            _prevLocale = locale;
            Debug.Log($">>locale<< change {locale}");
            _text.text = GameRunner.Instance.Core.Uis.Localisation.Translate(_originalText, locale);
            Debug.Log($">>locale<< text {GameRunner.Instance.Core.Uis.Localisation.Translate(_originalText, locale)}");
            _text.fontSize = locale != Locale.Th ? _baseFontSize : (int)Math.Ceiling(_baseFontSize * 0.8f);
        }

        private void SetOriginalText(string text)
        {
            if (_prevText == text) return;
            _prevText = text;
            Debug.Log($">>locale<< original_text {text}");
            _originalText = text;
            HandleLocaleChange(GameRunner.Instance.Core.Settings.Locale);
        }
    }
}