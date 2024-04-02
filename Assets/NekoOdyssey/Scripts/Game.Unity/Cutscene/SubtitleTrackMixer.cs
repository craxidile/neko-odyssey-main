using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI text = playerData as TextMeshProUGUI;
        string currentText = "";
        float currentAlpha = 0f;

        if (!text) { return; }

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            if (inputWeight > 0f)
            {
                ScriptPlayable<SubtitleBehaviour> inputPlayable = (ScriptPlayable<SubtitleBehaviour>)playable.GetInput(i);
                SubtitleBehaviour input = inputPlayable.GetBehaviour();
                var textData = SubtitleCSV.GetDialogue(input.lineIndexID);
                currentText = textData.DialogueSentance;
                currentAlpha = inputWeight;
                if(Application.isPlaying)
                {
                    if (input.waitPlayerSummit)
                    {
                        var director = (playable.GetGraph().GetResolver() as PlayableDirector);
                        while (director.state != PlayState.Paused)
                        {
                            Debug.Log($">>behavior<< pausing");
                            director.Pause();
                        }
                    }
                }
            }
        }

        text.text = currentText;
        text.alpha = currentAlpha;
    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log(">>behavior<< play");
        var duration = playable.GetDuration();
        playable.SetTime(duration);
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        Debug.Log($">>behavior<< pause");
    }
}
