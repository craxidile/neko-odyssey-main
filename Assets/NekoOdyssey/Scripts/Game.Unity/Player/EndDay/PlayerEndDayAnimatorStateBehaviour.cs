using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Player.EndDay
{
    public class PlayerEndDayAnimatorStateBehaviour : StateMachineBehaviour
    {
        //private readonly List<int> _eligibleLoopStates = new()
        //{
        //    Animator.StringToHash($"PetSideSitLoop"),
        //};

        //private readonly List<int> _eligibleFinishStates = new()
        //{
        //    Animator.StringToHash($"Hungry"),
        //    Animator.StringToHash($"Yawn"),
        //};

        //private readonly int _hungryState = Animator.StringToHash($"Hungry");
        //private readonly int _yawnState = Animator.StringToHash($"Yawn");

        private void Awake()
        {
            //Debug.Log($">>state<< awake");
        }

        //public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        ////    var entered = _eligibleLoopStates.Contains(stateInfo.shortNameHash);
        ////    if (!entered) return;
        ////    GameRunner.Instance.Core.Cats.CurrentCat?.SetEmotion(CatEmotion.Love);
        //}

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //var finished = _eligibleFinishStates.Contains(stateInfo.shortNameHash);
            //if (!finished) return;

            //GameRunner.Instance.Core.Player.Stamina.StaminaOutFinish();

            //foreach (var state in _eligibleFinishStates)
            //{
            //    if (state.Equals(stateInfo.shortNameHash))
            //    {

            //    }
            //}

            if (stateInfo.shortNameHash == Animator.StringToHash($"Hungry"))
            {
                Debug.Log("Animetor state finish : Hungry");
                GameRunner.Instance.Core.EndDay.StaminaOutFinish();
            }
            if (stateInfo.shortNameHash == Animator.StringToHash($"Yawn"))
            {
                Debug.Log("Animetor state finish : Yawn");
                GameRunner.Instance.Core.EndDay.TimeOutFinish();
            }
        }
    }

}