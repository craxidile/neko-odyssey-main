using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Unity.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.isEditor) return;
            SceneManager.LoadScene($"Neko2", LoadSceneMode.Single);
            SceneManager.LoadScene($"Neko8", LoadSceneMode.Additive);
            SceneManager.LoadScene($"SkyBox", LoadSceneMode.Additive);
            SceneManager.LoadScene($"NekoRoad", LoadSceneMode.Additive);
            SceneManager.LoadScene($"GameMain", LoadSceneMode.Additive);
        }
    }
}