using DG.Tweening;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Audios.PlayerAnimator
{
    public class PlayerBagAudioStateBehaviour : StateMachineBehaviour
    {
        private readonly int _eatState = Animator.StringToHash("EatFromBag");
            
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _eatState)
            {
                GameRunner.Instance.Core.Audios.AudioToClone.OnNext(("SFX_Eat", 3f));
            }
        }
    }
}