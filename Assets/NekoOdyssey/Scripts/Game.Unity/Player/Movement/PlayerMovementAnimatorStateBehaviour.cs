using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Core.Cat;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Feed
{
    public class PlayerMovementAnimatorStateBehaviour : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash != Animator.StringToHash($"ShakeHead")) return;
            GameRunner.Instance.Core.Player.FinishHeadShake();
        }
    }
}