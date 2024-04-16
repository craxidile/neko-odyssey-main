using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Unity.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.isEditor) return;
            var support16To10 = Screen.resolutions.Any(resolution => Math.Abs((float)resolution.width / resolution.height - 1.6f) < 0.05f);
            /*Screen.SetResolution(1920,
                Math.Abs((float)Screen.width / (float)Screen.height - 1.6f) < 0.05f ? 1200 : 1080, true);*/
            Screen.SetResolution(1440, support16To10 ? 900 : 810, true);
            SceneManager.LoadScene($"Neko2", LoadSceneMode.Single);
            // SceneManager.LoadScene($"Neko03", LoadSceneMode.Additive);
            // SceneManager.LoadScene($"Neko04", LoadSceneMode.Additive);
            // SceneManager.LoadScene($"Neko05", LoadSceneMode.Additive);
            SceneManager.LoadScene($"Neko08", LoadSceneMode.Additive);
            // SceneManager.LoadScene($"Neko09", LoadSceneMode.Additive);
            SceneManager.LoadScene($"CatScene", LoadSceneMode.Additive);
            SceneManager.LoadScene($"SkyBox", LoadSceneMode.Additive);
            SceneManager.LoadScene($"NekoRoad", LoadSceneMode.Additive);
           // SceneManager.LoadScene($"GameMainZone4", LoadSceneMode.Additive);
            //SceneManager.LoadScene($"GameMainStart", LoadSceneMode.Additive);
            SceneManager.LoadScene($"GameMain", LoadSceneMode.Additive);
            //SceneManager.LoadScene($"GameMain2", LoadSceneMode.Additive);
        }
    }
}