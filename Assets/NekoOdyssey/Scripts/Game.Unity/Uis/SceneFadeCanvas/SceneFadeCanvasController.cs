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
            gameObject.SetActive(true);
            
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 1f;
        }

        private void Start()
        {
            GameRunner.Instance.Core.GameScene.OnChangeSceneMode
                .Subscribe(mode => AnimateFade(mode == GameSceneMode.Opening))
                .AddTo(this);
        }

        private void AnimateFade(bool opening)
        {
            _canvasGroup.alpha = opening ? 1f : 0f;
            _canvasGroup.DOFade(opening ? 0f : 1f, FadeDuration);
        }
    }
}