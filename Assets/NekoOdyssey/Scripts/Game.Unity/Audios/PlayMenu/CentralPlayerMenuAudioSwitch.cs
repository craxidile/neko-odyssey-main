using System;
using System.Collections.Generic;
using DG.Tweening;
using NekoOdyssey.Scripts.Game.Core.PlayerMenu;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Audios.PlayMenu
{
    public class CentralPlayerMenuAudioSwitch : MonoBehaviour
    {
        private readonly List<PlayerMenuAction> _eligibleDoorActions = new()
        {
            PlayerMenuAction.Enter,
            PlayerMenuAction.Exit,
            PlayerMenuAction.LeftEnter,
            PlayerMenuAction.RightEnter,
            PlayerMenuAction.LeftExit,
            PlayerMenuAction.RightExit
        };
        
        private void Start()
        {
            GameRunner.Instance.Core.PlayerMenu.OnCommitAction
                .Subscribe(action =>
                {
                    HandleDoorAudio(action);
                })
                .AddTo(this);
        }

        private void HandleDoorAudio(PlayerMenuAction action)
        {
            if (!_eligibleDoorActions.Contains(action)) return;
            GameRunner.Instance.Core.Audios.ActiveAudio.OnNext("SFX_Door");
            DOVirtual.DelayedCall(3f, () => GameRunner.Instance.Core.Audios.InactiveAudio.OnNext("SFX_Door"));
        }
    }
}