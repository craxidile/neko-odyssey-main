using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Unity.Demo
{
    public class DemoFinishController : MonoBehaviour
    {
        //public CanvasGroup fading;

        private void Start()
        {
            if (GameRunner.Instance.CsvHolder.playFinishDemoVideo)
            {
                Debug.Log("load DemoFinishVideo 1");

                SiteRunner.Instance.Core.Site.OnChangeSite.Subscribe(_ =>
                {
                    Debug.Log("load DemoFinishVideo 2");

                    Core.EndDay.EndDayController.endDayStep = Core.EndDay.EndDayStep.FinishDemo;
                    SceneManager.LoadScene("DemoFinishVideo");
                }).AddTo(this);
            }
        }

    }
}