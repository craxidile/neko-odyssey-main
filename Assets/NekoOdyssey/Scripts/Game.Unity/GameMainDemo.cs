using System;
using System.Collections;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Unity
{
    
    public class GameMainDemo : MonoBehaviour
    {
        
        private GameObject _player;
        
        // private IEnumerator LoadBundle()
        // {
        //     var bundlePath = System.IO.Path.Combine(
        //         Application.streamingAssetsPath,
        //         "SwitchAssetBundles",
        //         "player"
        //     );
        //     Debug.Log($">>bundle_path<< 02 {bundlePath} {System.IO.File.Exists(bundlePath)}");
        //     if (System.IO.File.Exists(bundlePath))
        //     {
        //         var request = AssetBundle.LoadFromFileAsync(bundlePath);
        //         yield return request;
        //         var player = request.assetBundle.LoadAllAssets().FirstOrDefault();
        //         Debug.Log($">>player<< {player}");
        //         if (player != null)
        //         {
        //             _player = Instantiate(player) as GameObject;
        //         }
        //
        //         var bundle = request.assetBundle;
        //         Debug.Log($">>bundle_loaded<< 01 {bundle}");
        //     }
        //     
        //     if (!_player) yield break;
        //     Debug.Log($">>follow<<");
        //     var camera = Camera.main.transform.gameObject;
        //     var virtualCamera = camera.GetComponent<CinemachineVirtualCamera>();
        //     virtualCamera.Follow = _player.transform;
        //     virtualCamera.LookAt = _player.transform;
        // }

        private async void Awake()
        {
            // StartCoroutine(LoadBundle());
        }
        
    }
}