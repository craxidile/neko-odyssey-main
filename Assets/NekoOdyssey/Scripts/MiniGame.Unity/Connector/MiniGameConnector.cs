using System;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace NekoOdyssey.Scripts.MiniGame.Unity.Connector
{
    public class MiniGameConnector : MonoBehaviour
    {
        private const float DelayBeforeFadeOut = 2f;
        private const float FadeDuration = 1.5f;

        public int inputValue = MiniGameRunner.Instance?.Core.Site.SiteValue ?? 0;
        public CanvasGroup fading;

        private void Awake()
        {
            inputValue = MiniGameRunner.Instance?.Core.Site.SiteValue ?? 0;
            Debug.Log($">>difficulty<< input {MiniGameRunner.Instance?.Core.Site.SiteValue ?? -1}");
        }

        public void UseItem(List<ValueTuple<string, int>> itemCounts)
        {
            foreach (var itemCount in itemCounts)
            {
                var (code, count) = itemCount;
                MiniGameRunner.Instance.Core.PlayerItems.Consume(code, count);
                Debug.Log($">>use_item<< {code} {count}");
            }
        }

        public void ReceiveItem(List<ValueTuple<string, int>> itemCounts)
        {
            foreach (var itemCount in itemCounts)
            {
                var (code, count) = itemCount;
                MiniGameRunner.Instance.Core.PlayerItems.Collect(code, count);
                Debug.Log($">>receive_item<< {code} {count}");
            }
        }

        public void EnterSite()
        {
            Debug.Log($">>enter_site<<");
            DOVirtual.DelayedCall(1f, () => fading.DOFade(0f, FadeDuration));
        }

        public void LeaveSite()
        {
            Debug.Log($">>leave_site<<");
            DOVirtual.DelayedCall(DelayBeforeFadeOut, () =>
            {
                DOTween.Sequence()
                    .Append(fading.DOFade(1f, FadeDuration))
                    .AppendCallback(() =>
                    {
                        SiteRunner.Instance.Core.Site.MoveToPreviousSite(
                            MiniGameRunner.Instance.Core.Site.PreviousPlayerPosition
                        );
                    });
            });
        }
    }
}