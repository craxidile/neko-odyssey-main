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
            bundleNames.Add("test");
            bundleNames.Add($"dialogcanvas");
            bundleNames.Add($"a01sleepsnap");
            bundleNames.Add($"a01snap");
            bundleNames.Add($"a02snap");
            bundleNames.Add($"a04snap");
            bundleNames.Add($"a06snap");
            bundleNames.Add($"a07snap");
            bundleNames.Add($"a11snap");
            bundleNames.Add($"a12snap");
            bundleNames.Add($"a14snap");
            bundleNames.Add($"b01snap");
            bundleNames.Add($"b08snap");
            bundleNames.Add($"b09snap");
            bundleNames.Add($"b12boxsnap");
            bundleNames.Add($"b12snap");
            bundleNames.Add($"b13snap");
            bundleNames.Add($"b14snap");
            bundleNames.Add($"c04snap");
            bundleNames.Add($"c05snap");
            bundleNames.Add($"c10snap");

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