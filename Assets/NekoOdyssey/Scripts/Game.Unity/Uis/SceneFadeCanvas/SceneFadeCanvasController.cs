using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.Scene;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.SceneFadeCanvas
{
    public class SceneFadeCanvasController : MonoBehaviour
    {
        private const float FadeDuration = 1.5f;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            GameRunner.Instance.GameCore.GameScene.OnChangeSceneMode.Subscribe(mode =>
            {
                if (mode == GameSceneMode.Opening)
                {
                    gameObject.SetActive(true);
                    _canvasGroup.alpha = 1;
                    _canvasGroup.DOFade(0, FadeDuration);
                }
                else
                {
                    _canvasGroup.alpha = 0;
                    _canvasGroup.DOFade(1, FadeDuration);
                    
                }
            });
        }


    }
}