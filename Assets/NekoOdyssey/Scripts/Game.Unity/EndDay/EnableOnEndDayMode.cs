using NekoOdyssey.Scripts;
using NekoOdyssey.Scripts.Game.Core.Routine;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;
using NekoOdyssey.Scripts.Game.Core.EndDay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class EnableOnEndDayMode : MonoBehaviour
{
    [SerializeField] EndDayMode targetEndDayMode;

    private void Start()
    {
        if (GameRunner.Instance.Ready)
        {
            CheckCondition();
        }
        else
        {
            GameRunner.Instance.OnReady.Subscribe(_ =>
            {
                CheckCondition();
            }).AddTo(this);
        }

    }

    void CheckCondition()
    {
        var endDayMode = EndDayController.endDayMode;
        if (targetEndDayMode == endDayMode)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }
}
