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
            _baseFontSize = _text.fontSize;
        }

        private void Start()
        {
            Debug.Log($">>locale<< start_util");
            _originalText = _text.text;
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
            Debug.Log($">>locale<< debug 05 {_originalText}");
            // if (_prevLocale == locale) return;
            _prevLocale = locale;
            Debug.Log($">>locale<< debug 06 {_originalText}");
            Debug.Log($">>locale<< change {locale} {_originalText} {GameRunner.Instance.Core.Uis.Localisation.Translate(_originalText, locale)}");
            _text.text = ThaiGlyphAdjuster.Adjust(
                GameRunner.Instance.Core.Uis.Localisation.Translate(_originalText, locale) ?? _originalText
            );
            Debug.Log($">>locale<< debug 07 {_originalText}");
            Debug.Log($">>locale<< text {_text.text}");

            var fontSize = _baseFontSize == 0 ? _text.fontSize : _baseFontSize;
            _text.fontSize = locale != Locale.Th ? fontSize : (int)Math.Ceiling(fontSize * 0.8f);
        }

        private void SetOriginalText(string text)
        {
            Debug.Log($">>locale<< debug 00 {text}");
            // if (_prevText == text) return;
            _prevText = text;
            Debug.Log($">>locale<< debug 02 {text}");
            Debug.Log($">>locale<< original_text {text}");
            _originalText = text;
            Debug.Log($">>locale<< debug 03 {text}");
            HandleLocaleChange(GameRunner.Instance.Core.Settings.Locale);
        }
    }
}