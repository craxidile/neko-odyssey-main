using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleClip : PlayableAsset
{
    public string lineIndexID;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);
        SubtitleBehaviour subtitleBehaviour = playable.GetBehaviour();
        subtitleBehaviour.lineIndexID = lineIndexID;

        return playable;
    }
}
