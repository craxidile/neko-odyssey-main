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
            //fading.GetComponent<CanvasGroup>().alpha = 0;
            //DOVirtual.DelayedCall(10f, () => { fading.DOFade(1f, 2f); });
            //DOVirtual.DelayedCall(15f, () =>
            //{
            //    Debug.Log("load DemoFinishVideo 0 ");
            //    if (GameRunner.Instance.CsvHolder.playFinishDemoVideo)
            //    {
            //        Debug.Log("load DemoFinishVideo 1 ");
            //        SceneManager.LoadScene("DemoFinishVideo");
            //    }
            //    else
            //    {
            //        SiteRunner.Instance.Core.Site.SetSite("FinishDemo");
            //    }
            //});


            if (GameRunner.Instance.CsvHolder.playFinishDemoVideo)
            {
                Debug.Log("load DemoFinishVideo 1");

                SiteRunner.Instance.Core.Site.OnChangeSite.Subscribe(_ =>
                {
                    Debug.Log("load DemoFinishVideo 2");

                    SceneManager.LoadScene("DemoFinishVideo");
                }).AddTo(this);
            }
        }

    }
}