using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.AssetBundles
{
    public class AssetBundleLoader : MonoBehaviour
    {
        private static readonly List<string> _bundleNames = new()
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
            $"dialogue_animators",
        };

        private int _loadedAssetCount;
        private int _totalAssetCount;

        private void Awake()
        {
            if (GameRunner.Instance.AssetMap.Any())
            {
                GameRunner.Instance.SetReady(true);
                return;
            }
            StartCoroutine(LoadAssetBundle());
        }

        private IEnumerator LoadAssetBundle()
        {
            var bundleName = _bundleNames.FirstOrDefault();
            if (bundleName == null)
            {
                GameRunner.Instance.SetReady(true);
                yield break;
            }
            
            _bundleNames.RemoveAt(0);
            
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
                var assetMap = GameRunner.Instance.AssetMap;
                // Debug.Log($">>item_name<< {itemName}");
                assetMap[itemName] = item;
            }

            StartCoroutine(LoadAssetBundle());
        }

        private void OnDestroy()
        {
            // AssetBundle.UnloadAllAssetBundles(true);
        }
    }
}