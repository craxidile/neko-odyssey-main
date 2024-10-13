using System.Collections;
using System.Collections.Generic;
using NekoOdyssey.Scripts.Game.Unity.Uis.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class SubtitleTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var text = playerData as Text;
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
                var textData = SubtitleManager.GetSubtitle(input.lineIndexID);
                currentText = textData.SubtitleSentence;
                currentAlpha = inputWeight;
            }
        }

        if (!ThaiGlyphAdjuster.IsThaiString(currentText))
        {
            text.fontSize = 36;
            text.lineSpacing = 1f;
        }
        else
        {
            text.fontSize = 32;
            text.lineSpacing = 1.35f;
        }
        text.text = currentText;
        //text.color.a = currentAlpha;
    }
}
