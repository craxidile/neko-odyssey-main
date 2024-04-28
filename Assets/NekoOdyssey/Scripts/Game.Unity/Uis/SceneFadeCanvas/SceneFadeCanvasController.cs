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

        private bool _animating;
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
            Debug.Log($">>fade<< {opening}");
            if (_animating) return;
            _animating = true;
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.alpha = opening ? 1f : 0f;
            DOTween.Sequence()
                .Append(_canvasGroup.DOFade(opening ? 0f : 1f, FadeDuration))
                .AppendCallback(() =>
                {
                    _animating = false;
                    if (!opening) return;
                    _canvasGroup.gameObject.SetActive(false);
                });
        }
    }
}