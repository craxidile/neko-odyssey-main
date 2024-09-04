using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using NekoOdyssey.Scripts;
using UniRx;
using NekoOdyssey.Scripts.Game.Unity.Game.Core;

public class EventPointInteractive : MonoBehaviour
{
    public Action OnInteractive { get; set; }



    float _delayTime;

    // Start is called before the first frame update
    void Start()
    {

        GameRunner.Instance.PlayerInputHandler.OnFireTriggerred
                .Subscribe(_ => CheckInput())
                .AddTo(gameObject);

        GameRunner.Instance.Core.Player.OnChangeMode
            .Subscribe(HandlePlayerModeChange)
            .AddTo(GameRunner.Instance);
    }


    void HandlePlayerModeChange(PlayerMode mode) //do first dialogue from action
    {
        //Debug.Log($"HandlePlayerModeChange {mode}");
        if (mode != PlayerMode.Conversation) return;

        if (GameRunner.Instance.Core.PlayerMenu.GameObject.transform.IsChildOf(transform))
        {
            Debug.Log($"Pressed interactive on {name}");
            OnInteractive?.Invoke();


        }

        _delayTime = Time.time + 0.1f;
    }
    private void CheckInput() //continue to next dialogue
    {
        if (Time.time < _delayTime) return;

        if (GameRunner.Instance.Core.Player.Mode != PlayerMode.QuestConversation) return;

        if (GameRunner.Instance.Core.PlayerMenu.GameObject.transform.IsChildOf(transform))
        {
            Debug.Log($"Pressed interactive on {name}");
            OnInteractive?.Invoke();
        }

        _delayTime = Time.time + 0.1f;
    }

    public void Interactive()
    {
        OnInteractive?.Invoke();
    }
}
