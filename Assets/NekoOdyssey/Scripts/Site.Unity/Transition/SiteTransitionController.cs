using System;
using System.Linq;
using NekoOdyssey.Scripts.Game.Unity.Player;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Site.Unity.Transition
{
    public class SiteTransitionController : MonoBehaviour
    {
        private static bool _screenInitialized;

        private void Start()
        {
            if (SiteRunner.Instance.Core.Site.Ready)
            {
                LoadScenes();
                return;
            }

            SiteRunner.Instance.Core.Site.OnReady
                .Subscribe(_ => LoadScenes())
                .AddTo(this);
        }

        private void InitializeScreen()
        {
            if (_screenInitialized) return;
            if (Application.isEditor) return;
            var support16To10 = Screen.resolutions.Any(resolution =>
                Math.Abs((float)resolution.width / resolution.height - 1.6f) < 0.05f);
            /*Screen.SetResolution(1920,
                Math.Abs((float)Screen.width / (float)Screen.height - 1.6f) < 0.05f ? 1200 : 1080, true);*/
            Screen.SetResolution(1440, support16To10 ? 900 : 810, true);
            _screenInitialized = true;
        }

        private void LoadScenes()
        {
            var currentSite = Core.Site.Site.CurrentSite;

            var scenes = currentSite.Scenes.OrderBy(s => s.Id);

            var mainScene = scenes.FirstOrDefault();
            if (mainScene == null) return;
            SceneManager.LoadScene(mainScene.Name, LoadSceneMode.Single);
            Debug.Log($">>load_scene<< main {mainScene.Name}");

            var otherScenes = scenes.Skip(1);

            foreach (var scene in otherScenes)
            {
                SceneManager.LoadScene(scene.Name, LoadSceneMode.Additive);
                Debug.Log($">>load_scene<< other {scene.Name}");
            }
        }
    }
}