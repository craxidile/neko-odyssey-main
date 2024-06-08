using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Site.Unity.Transition
{
    public class SiteReloadController : MonoBehaviour
    {
        private void Start()
        {
            SiteRunner.Instance.Core.Site.OnChangeSite
                .Subscribe(_ =>
                {
                    Debug.Log($">>reload<<");
                    SceneManager.LoadScene("SceneLoader");
                })
                .AddTo(this);
        }
    }
}