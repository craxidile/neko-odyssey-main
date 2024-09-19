using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        public static bool isSetActiveScene;

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

        private string _currentSiteName;
        private List<AsyncOperation> _asyncOperationList = new();
        private List<string> _scenesToPreload = new();
        private List<string> _scenesToLoad = new();
        private StreamWriter _logWriter;
        private bool _ready;

        public GameObject loading;

        private void Awake()
        {
            InitializeScreen();
            isSetActiveScene = false;
            // try
            // {
            //     var logPath = Path.Combine(Application.persistentDataPath, "loading_log.txt");
            //     _logWriter = new StreamWriter(new FileStream(logPath, FileMode.Append));
            // }
            // catch (Exception ex)
            // {
            //     LogLine($">>load_scene_async<< error_log {ex.Message}\n{ex.StackTrace}");
            // }
        }

        private void Start()
        {
            LoadAll();
        }

        private void InitializeScreen()
        {
            if (_screenInitialized) return;
            if (Application.isEditor) return;
            var systemWidth = (float)Display.main.systemWidth;
            var systemHeight = (float)Display.main.systemHeight;
            var support16To10 = Math.Abs(systemWidth / systemHeight - 1.6f) < .0001f;
            Screen.SetResolution(1440, support16To10 ? 900 : 810, true);
            _screenInitialized = true;
        }

        private void LogLine(string text)
        {
            // if (_logWriter?.BaseStream == null) return;
            // _logWriter?.WriteLine($"[${DateTime.Now:MM/dd/yyyy HH:mm:ss}] {text}");
        }

        private void LoadAll()
        {
            if (_ready)
            {
                LogLine($">>load_all<< scene_ready repetitive start [[not_accepted]]");
                return;
            }

            LogLine($">>load_all<< scene_ready start");
            _ready = true;
            var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
            var scenes = currentSite.Scenes.OrderBy(s => s.Id).Select(s => s.Name).ToList();
            _currentSiteName = currentSite.Name;
            _scenesToPreload.AddRange(scenes);
            _scenesToLoad.AddRange(scenes);
            LogLine($">>load_all<< start");
            Application.backgroundLoadingPriority = ThreadPriority.High;
            StartCoroutine(LoadAssetBundle());
        }

        private IEnumerator LoadAssetBundle()
        {
            var bundleName = BundleNames.FirstOrDefault();
            if (bundleName == null)
            {
                LogLine($">>load_asset_bundle<< done goto preload_scene_async");
                StartCoroutine(PreloadSceneAsync());
                yield break;
            }

            LogLine($">>load_asset_bundle<< bundle_name {bundleName}");
            BundleNames.RemoveAt(0);

            var bundlePath = Path.Combine(
                Application.streamingAssetsPath,
                AppConstants.AssetBundlesFolder,
                bundleName
            );
            LogLine($">>load_asset_bundle<< bundle_path {bundlePath} {File.Exists(bundlePath)}");
            if (!File.Exists(bundlePath))
            {
                StartCoroutine(LoadAssetBundle());
                yield break;
            }

            var request = AssetBundle.LoadFromFileAsync(bundlePath);
            while (!request.isDone)
            {
                yield return null;
            }

            LogLine($">>load_asset_bundle<< load_all_assets");
            foreach (var item in request.assetBundle.LoadAllAssets())
            {
                var itemName = item.name.ToLower();
                var assetMap = GameRunner.StaticAssetMap;
                LogLine($">>load_asset_bundle<< load_asset {itemName}");
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
            LogLine($">>preload_scene_async<< scene_name {sceneName}");
            if (sceneName == null)
            {
                StartCoroutine(LoadSceneAsync());
                LogLine($">>preload_scene_async<< done goto load_scene_async");
                yield break;
            }

            _scenesToPreload.RemoveAt(0);

            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (asyncOperation != null)
            {
                asyncOperation.allowSceneActivation = false;
                _asyncOperationList.Add(asyncOperation);
                while (asyncOperation.progress < 0.9f)
                {
                    LogLine($">>preload_scene_async<< progress_start {sceneName} {asyncOperation.progress}");
                    // Debug.Log($">>load_scene<< progress {sceneName} {asyncOperation.progress}");
                    yield return null;
                }

                LogLine($">>preload_scene_async<< progress_done {sceneName} {asyncOperation.progress}");
                // Debug.Log($">>load_scene<< progress {sceneName} {asyncOperation.progress}");
            }

            StartCoroutine(PreloadSceneAsync());
        }

        private IEnumerator LoadSceneAsync()
        {
            var asyncOperation = _asyncOperationList.FirstOrDefault();
            var scene = _scenesToLoad.FirstOrDefault();
            LogLine($">>load_scene_async<< scene_name {scene}");

            if (asyncOperation == null)
            {
                LogLine($">>load_scene_async<< done goto prepare_scene");
                StartCoroutine(PrepareScene());
                yield break;
            }

            _asyncOperationList.RemoveAt(0);
            _scenesToLoad.RemoveAt(0);

            LogLine($">>load_scene_async<< allow_scene_activation {scene}");
            asyncOperation.allowSceneActivation = true;
            while (!asyncOperation.isDone)
            {
                yield return null;
            }

            // if (scene == "GameMain") loading.SetActive(false);
            LogLine($">>load_scene_async<< start_wait .1 ms");
            yield return new WaitForSeconds(.1f);
            LogLine($">>load_scene_async<< end_wait .1 ms");
            StartCoroutine(LoadSceneAsync());
        }

        private IEnumerator PrepareScene()
        {
            LogLine($">>load_scene_async<< prepare_scene start");
            Application.backgroundLoadingPriority = ThreadPriority.BelowNormal;
            loading.SetActive(false);
            LogLine($">>load_scene_async<< loading invisible");
            LogLine($">>load_scene_async<< set_active_scene start");
            var currentSite = SiteRunner.Instance.Core.Site.CurrentSite;
            var scenes = currentSite.Scenes.OrderBy(s => s.Id).Select(s => s.Name);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenes.First()));
            _ready = false;
            LogLine($">>load_scene_async<< done");

            // try
            // {
            //     _logWriter.Close();
            // }
            // catch (Exception ex)
            // {
            //     LogLine($">>load_scene_async<< error_log {ex.Message}\n{ex.StackTrace}");
            // }

            isSetActiveScene = true;
            SiteRunner.Instance.Core.Site.SetReady();
            MarkSiteAsVisited();
            yield break;
        }

        private void MarkSiteAsVisited()
        {
            if (_currentSiteName == null) return;
            var gameRunner = GameRunner.Instance;
            if (!gameRunner) return;
            gameRunner.Core.Player.MarkSiteAsVisited(_currentSiteName);
                
        }
    }
}