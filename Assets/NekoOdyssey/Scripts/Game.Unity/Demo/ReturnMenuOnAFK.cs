using NekoOdyssey.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;

public class ReturnMenuOnAFK : MonoBehaviour
{
    int duration = 120;

    float targetTime;

    // Start is called before the first frame update
    void Start()
    {
        ResetTime();

        GameRunner.Instance.PlayerInputHandler.OnMove
               .Subscribe(input =>
               {
                   if (input.magnitude != 0)
                   {
                       ResetTime();
                   }
               })
               .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            ResetTime();
        }

        if (Time.time >= targetTime)
        {
            ResetTime();

            GameRunner.Instance.Core.Player.SetMode(NekoOdyssey.Scripts.Game.Unity.Game.Core.PlayerMode.Stop);
            GameRunner.Instance.Core.GameScene.CloseScene();
            GameRunner.Instance.Core.GameScene.OnChangeSceneFinish.Subscribe(_ =>
            {
                SiteRunner.Instance.Core.Site.SetSite("DemoTitle");
            });
        }
    }

    void ResetTime()
    {
        //Debug.Log("ReturnMenuOnAFK ResetTime");
        targetTime = Time.time + duration;
    }
}
