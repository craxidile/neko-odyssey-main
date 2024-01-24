using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.AssetBundles
{
    public class AssetBundleLoader : MonoBehaviour
    {
        private int _loadedAssetCount;

        private void Awake()
        {
            var bundleNames = new List<string>();

            foreach (var action in Enum.GetValues(typeof(PlayerMenuAction)))
            {
                if ((PlayerMenuAction)action == PlayerMenuAction.None) continue;
                var actionName = Enum.GetName(typeof(PlayerMenuAction), action);
                var bundleName = $"{actionName.ToLower()}action";
                bundleNames.Add(bundleName);
            }

            foreach (var name in bundleNames)
            {
                StartCoroutine(LoadAssetBundle(name, bundleNames.Count));
            }
        }

        private IEnumerator LoadAssetBundle(string name, int length)
        {
            var bundlePath = System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "SwitchAssetBundles",
                name
            );
            Debug.Log($">>load<< 00 {name} {System.IO.File.Exists(bundlePath)}");
            if (!System.IO.File.Exists(bundlePath)) yield break;

            var request = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return request;
            var asset = request.assetBundle.LoadAllAssets().FirstOrDefault();

            Debug.Log($">>load<< 01 {name} {asset}");
            if (asset == null) yield break;

            GameRunner.Instance.AssetMap.Add(name, asset);

            _loadedAssetCount++;

            Debug.Log($">>loaded<< {_loadedAssetCount} / {length}");
            if (_loadedAssetCount == length) GameRunner.Instance.SetReady(true);
        }

        private void OnDestroy()
        {
            AssetBundle.UnloadAllAssetBundles(true);
        }
    }
}