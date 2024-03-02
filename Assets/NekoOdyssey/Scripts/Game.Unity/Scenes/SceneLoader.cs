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
            SceneManager.LoadScene($"Neko03", LoadSceneMode.Additive);
            SceneManager.LoadScene($"Neko04", LoadSceneMode.Additive);
            SceneManager.LoadScene($"Neko05", LoadSceneMode.Additive);
            SceneManager.LoadScene($"Neko08", LoadSceneMode.Additive);
            SceneManager.LoadScene($"Neko09", LoadSceneMode.Additive);
            SceneManager.LoadScene($"CatScene", LoadSceneMode.Additive);
            SceneManager.LoadScene($"SkyBox", LoadSceneMode.Additive);
            SceneManager.LoadScene($"NekoRoad", LoadSceneMode.Additive);
            SceneManager.LoadScene($"GameMain2", LoadSceneMode.Additive);
        }
    }
}