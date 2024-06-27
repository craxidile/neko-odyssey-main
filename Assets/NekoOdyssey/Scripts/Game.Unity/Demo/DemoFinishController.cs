using System;
using DG.Tweening;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Demo
{
    public class DemoFinishController : MonoBehaviour
    {
        public CanvasGroup fading;

        private void Start()
        {
            fading.GetComponent<CanvasGroup>().alpha = 0;
            DOVirtual.DelayedCall(10f, () => { fading.DOFade(1f, 2f); });
            DOVirtual.DelayedCall(12f, () => { SiteRunner.Instance.Core.Site.SetSite("FinishDemo"); });
        }
    }
}