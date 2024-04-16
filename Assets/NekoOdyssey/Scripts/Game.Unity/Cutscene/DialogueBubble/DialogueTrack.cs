using NekoOdyssey.Scripts.Game.Unity.Uis.DialogCanvas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[TrackBindingType(typeof(DialogCanvasController), TrackBindingFlags.None)]
[TrackColor(0.5f, 0.5f, 0.5f)]
[TrackClipType(typeof(DialogueClip))]
public class DialogueTrack : TrackAsset
{

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<DialogueTrackMixer>.Create(graph, inputCount);

    }
}
