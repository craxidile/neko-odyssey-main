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

        private void Awake()
        {
            InitializeScreen();
        }

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
            var support16To10 = Math.Abs((float)Display.main.systemWidth / Display.main.systemHeight - 1.6f) < .0001f;
            Screen.SetResolution(1440, support16To10 ? 900 : 810, true);
            _screenInitialized = true;
        }

        private void LoadScenes()
        {
            var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
            var scenes = currentSite.Scenes.OrderBy(s => s.Id);

            var mainScene = scenes.FirstOrDefault();
            if (mainScene == null) return;
            SceneManager.LoadSceneAsync(mainScene.Name, LoadSceneMode.Single);
            Debug.Log($">>load_scene<< main {mainScene.Name}");

            var otherScenes = scenes.Skip(1);

            foreach (var scene in otherScenes)
            {
                SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
                Debug.Log($">>load_scene<< other {scene.Name}");
            }
        }
    }
}