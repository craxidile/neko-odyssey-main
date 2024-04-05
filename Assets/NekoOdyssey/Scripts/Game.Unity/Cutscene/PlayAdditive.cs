using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[ExecuteInEditMode]
[RequireComponent(typeof(PlayableDirector))]
public class PlayAdditive : MonoBehaviour
{
    private AnimationClipPlayable clipPlayable;

    public AnimationClip clip;
    public AvatarMask mask;
    public Animator who;

    private double startTime;


    void Update()
    {
        if (clip == null || who == null)
            return;

        if (!clipPlayable.IsValid())
        {
            var graph = GetComponent<PlayableDirector>().playableGraph;
            if (graph.IsValid())
            {
                startTime = graph.GetRootPlayable(0).GetTime(); // capture the current time
            }

            for (int i = 0; i < graph.GetOutputCount(); i++)
            {
                var output = (AnimationPlayableOutput)graph.GetOutputByType<AnimationPlayableOutput>(i);
                if (output.GetTarget() == who)
                {
                    // get the root of the playable graph
                    var outputPlayable = output.GetSourcePlayable().GetInput(output.GetSourceOutputPort());

                    // search the subgraph for the layer mixer
                    while (outputPlayable.GetInputCount() > 0 && !outputPlayable.IsPlayableOfType<AnimationLayerMixerPlayable>())
                        outputPlayable = outputPlayable.GetInput(0);

                    if (outputPlayable.IsValid())
                    {
                        var layerMixer = (AnimationLayerMixerPlayable)outputPlayable;
                        clipPlayable = AnimationClipPlayable.Create(graph, clip);
                        var inputIndex = layerMixer.AddInput(clipPlayable, 0, 1);

                        layerMixer.SetLayerAdditive((uint)inputIndex, true);
                        layerMixer.SetLayerMaskFromAvatarMask((uint)inputIndex, mask);

                        startTime = GetComponent<PlayableDirector>().time;
                    }
                }
            }
        }

        if (clipPlayable.IsValid())
        {
            clipPlayable.SetTime(GetComponent<PlayableDirector>().time - startTime);
        }

    }
}