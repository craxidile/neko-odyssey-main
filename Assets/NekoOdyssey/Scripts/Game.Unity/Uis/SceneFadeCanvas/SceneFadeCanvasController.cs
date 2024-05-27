using System;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.GameScene;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Uis.SceneFadeCanvas
{
    public class SceneFadeCanvasController : MonoBehaviour
    {
        public const float FadeDuration = 1.5f;

        private Tween _tween;
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
            _tween?.Kill();
            _tween = DOTween.Sequence()
                .OnStart(() =>
                {
                    _canvasGroup.gameObject.SetActive(true);
                    if (_tween == null)
                        _canvasGroup.alpha = opening ? 1f : 0f;
                })
                .Append(_canvasGroup.DOFade(opening ? 0f : 1f, FadeDuration))
                .AppendCallback(() =>
                {
                    _tween = null;
                    if (!opening) return;
                    _canvasGroup.gameObject.SetActive(false);
                });
        }
    }
}