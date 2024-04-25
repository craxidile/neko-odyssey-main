using NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DialogueTrackMixer : PlayableBehaviour
{
    DialogCanvasController canvasController;
    int indexCount;
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        indexCount = 0;
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        canvasController = playerData as DialogCanvasController;
        string currentText = "";

        if (!canvasController) { return; }
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            if (inputWeight > 0f)
            {
                ScriptPlayable<DialogueBehaviour> inputPlayable = (ScriptPlayable<DialogueBehaviour>)playable.GetInput(i);
                DialogueBehaviour input = inputPlayable.GetBehaviour();
                var textData = DialogueManager.GetDialogue(input.lineIndexID[indexCount]);
                currentText = textData.DialogueSentance;
                canvasController.gameObject.transform.position = input.bubbleObject.transform.position;
                canvasController.gameObject.transform.forward = input.bubbleObject.transform.forward;
                canvasController.SetText(currentText);
            }
        }
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
