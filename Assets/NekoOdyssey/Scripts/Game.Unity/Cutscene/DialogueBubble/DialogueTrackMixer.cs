using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class DialogueTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var text = playerData as Text;
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
                if (Application.isPlaying)
                {
                    if (input.waitPlayerSummit)
                    {

                        var director = (playable.GetGraph().GetResolver() as PlayableDirector);
                        while (director.state != PlayState.Paused && !input.isPaused)
                        {
                            Debug.Log($">>behavior<< pausing");
                            //DialogueManager.IsEndDialogue = input.endDialogue;
                            //if (input.startDialogue)
                            //{
                            //    DialogueManager.StartDialogue();
                            //}
                            //DialogueManager.NextDialogue(input.PositionReference);
                            input.isPaused = true;
                            director.Pause();
                        }
                    }
                }
            }
        }

        text.text = currentText;
    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //Debug.Log(">>behavior<< play");
        var duration = playable.GetDuration();
        playable.SetTime(duration);
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        //Debug.Log($">>behavior<< pause");
    }
}
