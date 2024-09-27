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
            Debug.Log("DemoFinishVideoController 1".SetColor(Color.red));
            DOVirtual.DelayedCall(65f, () =>
            {
                Debug.Log("DemoFinishVideoController 2".SetColor(Color.red));
                //SceneManager.LoadSceneAsync("SceneLoader");
                SiteRunner.Instance.Core.Site.SetSite("GamePlayZone3_02");
            });
        }
    }
}