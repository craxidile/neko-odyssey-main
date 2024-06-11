using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Game.Unity.Player;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Site.Unity.Transition
{
    public class SiteTransitionController : MonoBehaviour
    {
        private static bool _screenInitialized;

        private static readonly List<string> BundleNames = new()
        {
            $"dialogcanvas",
            $"menu_actions",
            $"cat_snaps",
            $"cat_profiles",
            $"cat_badges",
            $"cat_emotions",
            $"scriptableobject",
            $"items",
            $"camera_boundaries",
        };

        private List<AsyncOperation> _asyncOperationList = new();
        private List<string> _scenesToPreload = new();
        private List<string> _scenesToLoad = new();
        private float _startTime = 0;

        public GameObject loading;

        private void Awake()
        {
            InitializeScreen();
        }

        private void Start()
        {
            if (SiteRunner.Instance.Core.Site.Ready)
            {
                LoadAll();
                return;
            }

            SiteRunner.Instance.Core.Site.OnReady
                .Subscribe(_ => LoadAll())
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

        public static float GetCurrentSeconds()
        {
            var jan1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var javaSpan = DateTime.UtcNow - jan1970;
            return (float)javaSpan.TotalMilliseconds / 1000;
        }

        private void LoadAll()
        {
            _startTime = GetCurrentSeconds();
            var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
            var scenes = currentSite.Scenes.OrderBy(s => s.Id).Select(s => s.Name).ToList();
            _scenesToPreload.AddRange(scenes);
            _scenesToLoad.AddRange(scenes);
            StartCoroutine(LoadAssetBundle());
        }

        private IEnumerator LoadAssetBundle()
        {
            var bundleName = BundleNames.FirstOrDefault();
            if (bundleName == null)
            {
                StartCoroutine(PreloadSceneAsync());
                yield break;
            }

            BundleNames.RemoveAt(0);

            var bundlePath = System.IO.Path.Combine(
                Application.streamingAssetsPath,
                AppConstants.AssetBundlesFolder,
                bundleName
            );
            if (!System.IO.File.Exists(bundlePath))
            {
                StartCoroutine(LoadAssetBundle());
                yield break;
            }

            var request = AssetBundle.LoadFromFileAsync(bundlePath);
            while (!request.isDone)
            {
                yield return null;
            }

            foreach (var item in request.assetBundle.LoadAllAssets())
            {
                var itemName = item.name.ToLower();
                var assetMap = GameRunner.StaticAssetMap;
                // Debug.Log($">>item_name<< {itemName}");
                assetMap[itemName] = item;
            }

            StartCoroutine(LoadAssetBundle());
        }

        private IEnumerator PreloadSceneAsync()
        {
            // var mainScene = scenes.FirstOrDefault();
            // if (mainScene == null) yield break;
            // var asyncOperation = SceneManager.LoadSceneAsync(mainScene.Name, LoadSceneMode.Additive);
            // asyncOperation.allowSceneActivation = false;
            // asyncOperationList.Add(asyncOperation);
            // Debug.Log($">>load_scene<< main {mainScene.Name}");
            // while (asyncOperation.progress < 0.9f)
            // {
            //     Debug.Log($">>load_scene<< progress {mainScene.Name} {asyncOperation.progress}");
            //     yield return null;
            // }
            //
            // Debug.Log($">>load_scene<< progress {mainScene.Name} {asyncOperation.progress}");
            //
            // var otherScenes = scenes.Skip(1);

            var sceneName = _scenesToPreload.FirstOrDefault();
            if (sceneName == null)
            {
                var now = GetCurrentSeconds();
                var timeDiff = now - _startTime;
                Debug.Log($">>load_scene<< ready {timeDiff}");
                yield return new WaitForSeconds(Math.Max(0, 2f - timeDiff));
                StartCoroutine(LoadSceneAsync());
                yield break;
            }

            _scenesToPreload.RemoveAt(0);

            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            _asyncOperationList.Add(asyncOperation);
            while (asyncOperation.progress < 0.9f)
            {
                Debug.LogError($">>load_scene<< progress {sceneName} {asyncOperation.progress}");
                yield return null;
            }

            Debug.LogError($">>load_scene<< progress {sceneName} {asyncOperation.progress}");
            StartCoroutine(PreloadSceneAsync());
        }

        private IEnumerator LoadSceneAsync()
        {
            var asyncOperation = _asyncOperationList.FirstOrDefault();
            var scene = _scenesToLoad.FirstOrDefault();
            if (asyncOperation == null)
            {
                StartCoroutine(PrepareScene());
                yield break;
            }

            _asyncOperationList.RemoveAt(0);
            _scenesToLoad.RemoveAt(0);

            asyncOperation.allowSceneActivation = true;
            while (!asyncOperation.isDone)
            {
                yield return null;
            }

            // if (scene == "GameMain") loading.SetActive(false);
            yield return new WaitForSeconds(.1f);
            StartCoroutine(LoadSceneAsync());
        }

        private IEnumerator PrepareScene()
        {
            loading.SetActive(false);
            var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
            var scenes = currentSite.Scenes.OrderBy(s => s.Id).Select(s => s.Name);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenes.First()));
            yield break;
        }
    }
}