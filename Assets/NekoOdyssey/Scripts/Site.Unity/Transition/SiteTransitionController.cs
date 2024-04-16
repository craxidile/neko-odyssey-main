using System;
using System.Linq;
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
                LoadScenes();
                return;
            }

            SiteRunner.Instance.Core.Site.OnReady
                .Subscribe(HandleSiteReady)
                .AddTo(this);
        }

        private void HandleSiteReady(Unit _)
        {
            LoadScenes();
        }

        private void LoadScenes()
        {
            var currentSite = Core.Site.Site.CurrentSite;
            var scenes = currentSite.Scenes.OrderBy(s => s.Id);
            var mainScene = scenes.FirstOrDefault();
            if (mainScene == null) return;
            var otherScenes = scenes.Skip(1);
            
            // foreach (var scene in scenes)
            // {
            //     var sceneName = scene.Name;
            //     Debug.Log($">>load_scene<< {sceneName}");
            // }
        }
    }
}