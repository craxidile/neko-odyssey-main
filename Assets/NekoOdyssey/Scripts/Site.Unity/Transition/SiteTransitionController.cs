using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Unity.Player;
using NekoOdyssey.Scripts.Game.Unity.Scenes;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Site.Unity.Transition
{
    public class SiteTransitionController : MonoBehaviour
    {
        private static bool _screenInitialized;
        
        private List<AsyncOperation> _asyncLoading = new();

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
            _asyncLoading.Clear();
            Debug.Log($">>load_scene<< start");
            StartCoroutine(LoadSceneAsync());
            DOVirtual.DelayedCall(10f, () =>
            {
                GameObject.Find("Loading").SetActive(false);
            });
        }

        private IEnumerator LoadSceneAsync()
        {
            var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
            var scenes = currentSite.Scenes.OrderBy(s => s.Id);

            var mainScene = scenes.FirstOrDefault();
            Debug.Log($">>load_scene<< {mainScene}");
            if (mainScene == null) yield break;
            
            var asyncLoad = SceneManager.LoadSceneAsync(mainScene.Name, LoadSceneMode.Single);
            Debug.Log($">>load_scene<< main {mainScene.Name}");
            _asyncLoading.Add(asyncLoad);
            
            var otherScenes = scenes.Skip(1);
            Debug.Log($">>load_scene<< other_scenes {otherScenes.Count()}");

            foreach (var scene in otherScenes)
            {
                var otherAsyncLoad = SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
                Debug.Log($">>load_scene<< other {scene.Name}");
                _asyncLoading.Add(otherAsyncLoad);
            }
            
            SceneManager.LoadScene($"Loading", LoadSceneMode.Additive);
            
            while (_asyncLoading.Any(al => al.progress < .9f)) yield return null;
        }
    }
}