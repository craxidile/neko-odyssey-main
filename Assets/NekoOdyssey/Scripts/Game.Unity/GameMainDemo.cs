using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Unity
{
    public class GameMainDemo : MonoBehaviour
    {
        private IEnumerator LoadBundle()
        {
            var bundlePath = System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "SwitchAssetBundles",
                "player"
            );
            Debug.Log($">>bundle_path<< 02 {bundlePath} {System.IO.File.Exists(bundlePath)}");
            GameObject go = null;
            if (System.IO.File.Exists(bundlePath))
            {
                var request = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return request;
                var player = request.assetBundle.LoadAllAssets().FirstOrDefault();
                Debug.Log($">>player<< {player}");
                if (player != null)
                {
                    go = Instantiate(player) as GameObject;
                }

                var bundle = request.assetBundle;
                Debug.Log($">>bundle_loaded<< 01 {bundle}");
            }
        }

        private async void Awake()
        {
            if (!Application.isEditor)
            {
                SceneManager.LoadScene($"Neko2", LoadSceneMode.Additive);
                SceneManager.LoadScene($"SkyBox", LoadSceneMode.Additive);
                SceneManager.LoadScene($"NekoRoad", LoadSceneMode.Additive);
                // SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
            }

            StartCoroutine(LoadBundle());
        }
    }
}