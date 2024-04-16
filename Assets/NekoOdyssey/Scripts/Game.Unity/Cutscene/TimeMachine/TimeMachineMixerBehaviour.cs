using DG.Tweening.Core.Easing;
using NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeMachineMixerBehaviour : PlayableBehaviour
{
    public Dictionary<string, double> markerClips;
    private PlayableDirector director;

    public override void OnPlayableCreate(Playable playable)
    {
        director = (playable.GetGraph().GetResolver() as PlayableDirector);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var manager = playerData as DialogueManager;
        if (!Application.isPlaying)
        {
            return;
        }

        int inputCount = playable.GetInputCount();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<TimeMachineBehaviour> inputPlayable = (ScriptPlayable<TimeMachineBehaviour>)playable.GetInput(i);
            TimeMachineBehaviour input = inputPlayable.GetBehaviour();

            if (inputWeight > 0f)
            {
                if (!input.clipExecuted)
                {
                    switch (input.action)
                    {
                        case TimeMachineBehaviour.TimeMachineAction.Pause:
                            if (input.ConditionMet(playerData))
                            {
                                //director.playableGraph.GetRootPlayable(0).SetSpeed(0d);
                                input.clipExecuted = true; //this prevents the command to be executed every frame of this clip
                            }
                            break;
                        case TimeMachineBehaviour.TimeMachineAction.ResetWaitInput:
                            if (input.ConditionMet(playerData))
                            {
                                manager.nextDialogue = false;
                                Debug.Log("set waitInput value = false");
                                input.clipExecuted = true;
                            }
                            break;

                        case TimeMachineBehaviour.TimeMachineAction.JumpToTime:
                        case TimeMachineBehaviour.TimeMachineAction.JumpToMarker:
                            if (input.ConditionMet(playerData))
                            {
                                //Rewind
                                if (input.action == TimeMachineBehaviour.TimeMachineAction.JumpToTime)
                                {
                                    //Jump to time
                                    (playable.GetGraph().GetResolver() as PlayableDirector).time = (double)input.timeToJumpTo;
                                }
                                else
                                {
                                    //Jump to marker
                                    double t = markerClips[input.markerToJumpTo];
                                    (playable.GetGraph().GetResolver() as PlayableDirector).time = t;
                                }
                                input.clipExecuted = false; //we want the jump to happen again!
                            }
                            break;

                    }

                }
            }
        }
    }
}