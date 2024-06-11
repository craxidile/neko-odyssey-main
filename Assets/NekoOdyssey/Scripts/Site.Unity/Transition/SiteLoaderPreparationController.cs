using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NekoOdyssey.Scripts.Site.Unity.Transition
{
    public class SiteLoaderPreparationController : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadScene("SceneLoader", LoadSceneMode.Single);
        }
    }
}