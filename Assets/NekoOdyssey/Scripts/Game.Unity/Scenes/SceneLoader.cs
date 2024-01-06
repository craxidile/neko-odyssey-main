using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Game.Unity.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        private void Awake()
        {
            if (!Application.isEditor)
            {
                SceneManager.LoadScene($"Neko2", LoadSceneMode.Single);
                SceneManager.LoadScene($"SkyBox", LoadSceneMode.Additive);
                SceneManager.LoadScene($"NekoRoad", LoadSceneMode.Additive);
                SceneManager.LoadScene($"GameMain", LoadSceneMode.Additive);
                // SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
            }
        }
    }
}