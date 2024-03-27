using System;
using System.Collections.Generic;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.Petting
{
    public class PlayerPettingAnimatorStateBehaviour : StateMachineBehaviour
    {
        private readonly List<int> EligibleFinishStates = new()
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

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var finished = EligibleFinishStates.Contains(stateInfo.shortNameHash);
            Debug.Log($">>state<< finished_petting? {finished}");
            if (!finished) return;
            GameRunner.Instance.Core.Player.Petting.Finish();
        }
    }
}