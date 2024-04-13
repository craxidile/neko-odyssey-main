using NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DialogueTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var text = playerData as DialogCanvasController;
        string currentText = "";

        if (!text) { return; }

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            if (inputWeight > 0f)
            {
                ScriptPlayable<DialogueBehaviour> inputPlayable = (ScriptPlayable<DialogueBehaviour>)playable.GetInput(i);
                DialogueBehaviour input = inputPlayable.GetBehaviour();
                var textData = DialogueManager.GetDialogue(input.lineIndexID);
                currentText = textData.DialogueSentance;
                //if (Application.isPlaying)
                //{
                //    if (input.waitPlayerSummit)
                //    {

                //        var director = (playable.GetGraph().GetResolver() as PlayableDirector);
                //        while (director.state != PlayState.Paused && !input.isLooped)
                //        {
                //            input.isLooped = true;
                //            director.Pause();
                //        }
                //    }
                //}
            }
        }

        text.SetText(currentText);
    }
    //public override void OnBehaviourPlay(Playable playable, FrameData info)
    //{
    //    Debug.Log(">>behavior<< play");
    //    var duration = playable.GetDuration();
    //    playable.SetTime(duration);
    //}
    //public override void OnBehaviourPause(Playable playable, FrameData info)
    //{
    //    Debug.Log($">>behavior<< pause");
    //}
}
