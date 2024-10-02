using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Audios.BirdAnimator
{
    public class BirdAnimatorStateBehaviour : StateMachineBehaviour
    {
        private readonly int _eatState = Animator.StringToHash("Fly");

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _eatState)
            {
                GameRunner.Instance.Core.Audios.AudioToClone.OnNext(("SFX_Bird_Flap", 3f));
            }
        }
    }
}