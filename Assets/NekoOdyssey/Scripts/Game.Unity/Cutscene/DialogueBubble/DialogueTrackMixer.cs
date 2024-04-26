using NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DialogueTrackMixer : PlayableBehaviour
{
    DialogCanvasController canvasController;
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        canvasController = playerData as DialogCanvasController;
        string currentText = "";
        int currentArrayLength;
        canvasController.SetOpened(false);
        if (!canvasController) { return; }
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);

            if (inputWeight > 0f)
            {
                ScriptPlayable<DialogueBehaviour> inputPlayable = (ScriptPlayable<DialogueBehaviour>)playable.GetInput(i);
                DialogueBehaviour input = inputPlayable.GetBehaviour();
                currentArrayLength = input.lineIndexID.Length - 1;
                canvasController.gameObject.transform.position = input.bubbleObject.transform.position;
                canvasController.gameObject.transform.forward = input.bubbleObject.transform.forward;
                if (!input.enterClip)
                {
                    canvasController.endDialogue = false;
                    Debug.Log(canvasController.endDialogue);
                    input.enterClip = true;

                }
                
                if (Application.isPlaying)
                {
                    if (canvasController.goNextLineId)
                    {
                        input.indexCount++;
                        canvasController.goNextLineId = false;
                    }

                    if (input.indexCount >= currentArrayLength || currentArrayLength == 0)
                    {
                        canvasController.lastLineId = true;
                    }
                    else
                    {
                        canvasController.lastLineId = false;
                    }

                    var textData = DialogueManager.GetDialogue(input.lineIndexID[input.indexCount]);
                    currentText = textData.DialogueSentance;
                    canvasController.SetOpened(!canvasController.endDialogue);

                }
                else
                {
                    var textData = DialogueManager.GetDialogue(input.lineIndexID[0]);
                    currentText = textData.DialogueSentance;
                    canvasController.SetOpened(true);
                }
            }
            canvasController.SetText(currentText);
        }
    }

}
