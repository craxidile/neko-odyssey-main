using NekoOdyssey.Scripts;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

[RequireComponent(typeof(EventPointInteractive))]
public class EventPoint_Cutscene : MonoBehaviour
{
    public string siteName = "";

    private void Awake()
    {
        GetComponent<EventPointInteractive>().OnInteractive += () =>
        {
            Debug.Log($"EventPoint_Cutscene : {siteName}");

            GameRunner.Instance.Core.Player.SetMode(PlayerMode.Stop);
            GameRunner.Instance.Core.GameScene.CloseScene();
            GameRunner.Instance.Core.GameScene.OnChangeSceneFinish.Subscribe(_ =>
            {
                SiteRunner.Instance.Core.Site.SetSite(siteName);
            });
        };

    }

}
