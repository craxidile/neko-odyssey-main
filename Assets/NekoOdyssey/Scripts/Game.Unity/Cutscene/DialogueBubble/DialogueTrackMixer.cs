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
    int maxLength;
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
                maxLength = input.lineIndexID.Length;
                if (canvasController.indexCount >= maxLength - 1 && !canvasController.isCutSceneLooped) 
                {
                    input.indexCount = canvasController.indexCount;
                }
                var textData = DialogueManager.GetDialogue(input.lineIndexID[input.indexCount]);
                currentText = textData.DialogueSentance;
                canvasController.gameObject.transform.position = input.bubbleObject.transform.position;
                canvasController.gameObject.transform.forward = input.bubbleObject.transform.forward;
                if (input.indexCount < maxLength - 1)
                {
                    canvasController.SetOpened(canvasController.isCutSceneLooped);
                }
            }
        }
        if (Application.isPlaying)
        {
            if (canvasController.indexCount >= maxLength-1)
            {
                canvasController.isCutSceneLooped = false;
            }
            else
            {
                canvasController.isCutSceneLooped = true;
            }
            if (canvasController.nextLineId)
            {
                canvasController.indexCount++;
                canvasController.nextLineId = false;
            }
        }
        else
        {
            canvasController.indexCount = 0;
        }
        canvasController.SetText(currentText);
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
