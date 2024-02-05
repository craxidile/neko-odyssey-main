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
        private int _totalAssetCount;

        private void Awake()
        {
            var bundleNames = new List<string>();
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
            bundleNames.Add("cat_snaps");

            foreach (var action in Enum.GetValues(typeof(PlayerMenuAction)))
            {
                if ((PlayerMenuAction)action == PlayerMenuAction.None) continue;
                var actionName = Enum.GetName(typeof(PlayerMenuAction), action);
                var bundleName = $"{actionName.ToLower()}action";
                bundleNames.Add(bundleName);
            }

            foreach (var name in bundleNames)
            {
                LoadAssetBundle(name);
            }
            
            GameRunner.Instance.SetReady(true);
        }
        
        private void LoadAssetBundle(string name)
        {
            var bundlePath = System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "SwitchAssetBundles",
                name
            );
            Debug.Log($">>load<< 00 {name} {System.IO.File.Exists(bundlePath)}");
            if (!System.IO.File.Exists(bundlePath)) return;

            var request = AssetBundle.LoadFromFile(bundlePath);
            var asset = request.LoadAllAssets().FirstOrDefault();

            foreach (var item in request.LoadAllAssets())
            {
                if (asset == null) continue;
                var itemName = item.name.ToLower();
                var assetMap = GameRunner.Instance.AssetMap;
                assetMap[itemName] = asset;
            }
        }

        private void OnDestroy()
        {
            AssetBundle.UnloadAllAssetBundles(true);
        }
    }
}