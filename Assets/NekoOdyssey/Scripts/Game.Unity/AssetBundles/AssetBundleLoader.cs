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
            var bundleNames = new List<string>
            {
                $"dialogcanvas",
                $"menu_actions",
                $"cat_snaps",
                $"cat_profiles",
                $"cat_emotions",
                $"scriptableobject"
            };

            foreach (var bundleName in bundleNames)
            {
                LoadAssetBundle(bundleName);
            }
            
            GameRunner.Instance.SetReady(true);
        }
        
        private void LoadAssetBundle(string bundleName)
        {
            var bundlePath = System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "SwitchAssetBundles",
                bundleName
            );
            if (!System.IO.File.Exists(bundlePath)) return;

            var request = AssetBundle.LoadFromFile(bundlePath);
            foreach (var item in request.LoadAllAssets())
            {
                var itemName = item.name.ToLower();
                var assetMap = GameRunner.Instance.AssetMap;
                Debug.Log($">>item_name<< {itemName}");
                assetMap[itemName] = item;
            }
        }

        private void OnDestroy()
        {
            AssetBundle.UnloadAllAssetBundles(true);
        }
    }
}