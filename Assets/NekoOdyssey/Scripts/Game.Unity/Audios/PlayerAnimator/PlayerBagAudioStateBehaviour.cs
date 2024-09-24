using DG.Tweening;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Audios.PlayerAnimator
{
    public class PlayerBagAudioStateBehaviour : StateMachineBehaviour
    {
        private readonly int EatState = Animator.StringToHash("EatFromBag");
            
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == EatState)
            {
                GameRunner.Instance.Core.Audios.AudioToClone.OnNext(("SFX_Eat", 3f));
            }
        }
    }
}