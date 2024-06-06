﻿using System;
using NekoOdyssey.Scripts.Constants;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.Utils
{
    public class UiTextLocaliser : MonoBehaviour
    {
        private Text _text;
        private string _originalText;
        
        private void Awake()
        {
            _text = gameObject.GetComponent<Text>();
        }

        private void Start()
        {
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
            Debug.Log($">>locale<< change {locale}");
            _text.text = GameRunner.Instance.Core.Uis.Localisation.Translate(_originalText, locale);
        }
    }
}