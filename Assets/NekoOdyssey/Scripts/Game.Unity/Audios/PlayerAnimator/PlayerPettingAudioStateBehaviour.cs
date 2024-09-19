using System.Collections.Generic;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Audios.PlayerAnimator
{
    public class PlayerPettingAudioStateBehaviour : StateMachineBehaviour
    {
        private readonly List<int> _eligibleStates = new()
        {
            Animator.StringToHash("PetSideSitHighLoop"),
            Animator.StringToHash("PetSideSitLoop"),
            Animator.StringToHash("PetSideLowLoop"),
            Animator.StringToHash("PetSideMediumLoop"),
            Animator.StringToHash("PetSideHighLoop"),
            Animator.StringToHash("PetSideUpperLowLoop"),
            Animator.StringToHash("PetStraightSitHighLoop"),
            Animator.StringToHash("PetStraightSitLoop"),
            Animator.StringToHash("PetStraightLowLoop"),
            Animator.StringToHash("PetStraightMediumLoop"),
            Animator.StringToHash("PetStraightHighLoop"),
            Animator.StringToHash("PetStraightUpperLowLoop")
        };
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_eligibleStates.Contains(stateInfo.shortNameHash)) return;
            GameRunner.Instance.Core.Audios.ActiveAudio.OnNext("SFX_Pet");
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!_eligibleStates.Contains(stateInfo.shortNameHash)) return;
            GameRunner.Instance.Core.Audios.InactiveAudio.OnNext("SFX_Pet");
        }
    }
}