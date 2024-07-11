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
    //public float interactiveDistance = 1;

    //public static EventPointInteractive NearestPoint;

    public Action OnInteractive;


    //bool _isActive = false;

    float _delayTime;

    // Start is called before the first frame update
    void Start()
    {
        //if (NearestPoint == null)
        //    NearestPoint = this;


        GameRunner.Instance.PlayerInputHandler.OnFireTriggerred
                .Subscribe(_ => CheckInput())
                .AddTo(gameObject);

        GameRunner.Instance.Core.Player.OnChangeMode
            .Subscribe(HandlePlayerModeChange)
            .AddTo(GameRunner.Instance);
    }

    // Update is called once per frame
    //void Update()
    //{
        //if (NekoOdyssey.Scripts.GameRunner.Instance == null) return;

        //var player = NekoOdyssey.Scripts.GameRunner.Instance.Core.Player.GameObject;
        //var thisPointDistance = Vector3.Distance(this.transform.position, player.transform.position);

        //Debug.DrawLine(this.transform.position, player.transform.position, Color.red);
        //Debug.DrawLine(NearestPoint.transform.position + (Vector3.up * 0.3f), player.transform.position, Color.yellow);

        //if (thisPointDistance <= interactiveDistance) //inside range
        //{
            //if (NearestPoint == this)
            //{
            //    //if (Keyboard.current.spaceKey.wasPressedThisFrame)
            //    //{
            //    //    Debug.Log($"Pressed space on {name}");
            //    //    OnInteractive?.Invoke();
            //    //}

            //}
            //else
            //{
            //    var nearestPointDistance = Vector3.Distance(NearestPoint.transform.position, player.transform.position);

            //    if (thisPointDistance <= nearestPointDistance)
            //    {
            //        NearestPoint = this;
            //    }



            //}

            //_isActive = NearestPoint == this;
        //}
        //else
        //{
        //    _isActive = false;
        //}


    //}

    void HandlePlayerModeChange(PlayerMode mode)
    {
        Debug.Log($"HandlePlayerModeChange {mode}");
        if (mode != PlayerMode.Conversation) return;

        //if (_isActive)
        if (GameRunner.Instance.Core.PlayerMenu.GameObject.transform.IsChildOf(transform))
        {
            Debug.Log($"Pressed interactive on {name}");
            OnInteractive?.Invoke();

           
        }

        _delayTime = Time.time + 0.1f;
    }
    private void CheckInput()
    {
        var playerMode = GameRunner.Instance.Core.Player.Mode;
        if (
            playerMode != PlayerMode.Move &&
            playerMode != PlayerMode.Conversation &&
            playerMode != PlayerMode.QuestConversation
        ) return;
        
        if (Time.time < _delayTime) return;

        if (GameRunner.Instance.Core.Player.Mode != PlayerMode.QuestConversation) return;

        //if (_isActive)
        if (GameRunner.Instance.Core.PlayerMenu.GameObject.transform.IsChildOf(transform))
        {
            Debug.Log($"Pressed interactive on {name}");
            OnInteractive?.Invoke();
        }

        _delayTime = Time.time + 0.1f;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    //Gizmos.color = Color.yellow;
    //    //Gizmos.DrawWireSphere(transform.position, interactiveDistance);


    //    var gizColor = Color.yellow;

    //    gizColor.a = 0.5f;
    //    Gizmos.color = gizColor;
    //    Gizmos.DrawWireSphere(transform.position, interactiveDistance);
    //    gizColor.a = 0.1f;
    //    Gizmos.color = gizColor;
    //    Gizmos.DrawSphere(transform.position, interactiveDistance);
    //}
}
