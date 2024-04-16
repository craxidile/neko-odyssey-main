using System;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Core.Cat;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Petting
{
    public class PlayerPettingAnimatorStateBehaviour : StateMachineBehaviour
    {
        private readonly List<int> _eligibleLoopStates = new()
        {
            Animator.StringToHash($"PetSideSitLoop"),
            Animator.StringToHash($"PetSideLowLoop"),
            Animator.StringToHash($"PetSideMediumLoop"),
            Animator.StringToHash($"PetSideHighLoop"),
            Animator.StringToHash($"PetStraightSitLoop"),
            Animator.StringToHash($"PetStraightLowLoop"),
            Animator.StringToHash($"PetStraightMediumLoop"),
            Animator.StringToHash($"PetStraightHighLoop"),
        };

        private readonly List<int> _eligibleFinishStates = new()
        {
            Animator.StringToHash($"PetSideSitEnd"),
            Animator.StringToHash($"PetSideLowEnd"),
            Animator.StringToHash($"PetSideMediumEnd"),
            Animator.StringToHash($"PetSideHighEnd"),
            Animator.StringToHash($"PetStraightSitEnd"),
            Animator.StringToHash($"PetStraightLowEnd"),
            Animator.StringToHash($"PetStraightMediumEnd"),
            Animator.StringToHash($"PetStraightHighEnd"),
        };

        private void Awake()
        {
            Debug.Log($">>state<< awake");
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var entered = _eligibleLoopStates.Contains(stateInfo.shortNameHash);
            if (!entered) return;
            GameRunner.Instance.Core.Cats.CurrentCat?.SetEmotion(CatEmotion.Love);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var finished = _eligibleFinishStates.Contains(stateInfo.shortNameHash);
            if (!finished) return;
            GameRunner.Instance.Core.Cats.CurrentCat?.SetEmotion(CatEmotion.None);
            GameRunner.Instance.Core.Player.Petting.Finish();
        }
    }
}