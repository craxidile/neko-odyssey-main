using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace NekoOdyssey.Scripts.Game.Unity.Demo
{
    public class DemoFinishVideoController : MonoBehaviour
    {
        private void Start()
        {
            DOVirtual.DelayedCall(65f, () =>
            {
                SceneManager.LoadSceneAsync("SceneLoader");
            });
        }
    }
}