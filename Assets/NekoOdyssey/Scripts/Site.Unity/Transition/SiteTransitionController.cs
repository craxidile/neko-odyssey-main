using System;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Site.Unity.Transition
{
    public class SiteTransitionController : MonoBehaviour
    {
        private void Start()
        {
            if (SiteRunner.Instance.Core.Site.Ready)
            {
                HandleSiteReady(default);
                return;
            }
            SiteRunner.Instance.Core.Site.OnReady
                .Subscribe(HandleSiteReady)
                .AddTo(this);
        }

        private void HandleSiteReady(Unit _)
        {
            var currentSite = Core.Site.Site.CurrentSite;
            var scenes = currentSite.Scenes;
            foreach (var scene in scenes)
            {
                var sceneName = scene.Name;
                Debug.Log($">>load_scene<< {sceneName}");
            }
        }
    }
}