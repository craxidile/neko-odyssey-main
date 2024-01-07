using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Unity
{
    
    public class GameMainDemo : MonoBehaviour
    {
        
        private GameObject _player;
        
        private IEnumerator LoadBundle()
        {
            var bundlePath = System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "SwitchAssetBundles",
                "player"
            );
            Debug.Log($">>bundle_path<< 02 {bundlePath} {System.IO.File.Exists(bundlePath)}");
            if (System.IO.File.Exists(bundlePath))
            {
                var request = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return request;
                var player = request.assetBundle.LoadAllAssets().FirstOrDefault();
                Debug.Log($">>player<< {player}");
                if (player != null)
                {
                    _player = Instantiate(player) as GameObject;
                }

                var bundle = request.assetBundle;
                Debug.Log($">>bundle_loaded<< 01 {bundle}");
            }
        }

        private async void Awake()
        {
            StartCoroutine(LoadBundle());
        }

        
        private void Update()
        {
            var playerPosition = _player.transform.position;
            var cameraPosition = new Vector3(
                playerPosition.x - 5.27125f,
                playerPosition.y + 1,
                Math.Max(-36.42f, Math.Min(-16.82f, playerPosition.z))
            );
            if (Camera.main == null) return;
            Camera.main.transform.position = cameraPosition;
        }
    }
}