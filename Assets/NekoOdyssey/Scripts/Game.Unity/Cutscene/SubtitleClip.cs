using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SubtitleClip : PlayableAsset
{
    public string lineIndexID;
    public bool waitPlayerSummit;
    public GameObject balloonPopUpPosition;

    public SubtitleBehaviour template = new SubtitleBehaviour();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);
        SubtitleBehaviour subtitleBehaviour = playable.GetBehaviour();
        subtitleBehaviour.lineIndexID = lineIndexID;
        subtitleBehaviour.waitPlayerSummit = waitPlayerSummit;
        subtitleBehaviour.isPaused = false;
        subtitleBehaviour.balloonPopUpPosition = balloonPopUpPosition;

        return playable;
    }
}
