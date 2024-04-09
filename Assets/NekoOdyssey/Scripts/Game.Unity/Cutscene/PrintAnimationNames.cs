using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[ExecuteAlways]
public class PrintAnimationNames : MonoBehaviour
{
    public Animator anim; 
    public PlayableDirector director;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (anim == null)
            return;

        director = GetComponent<PlayableDirector>();
        if (director == null || !director.playableGraph.IsValid())
            return;
        Debug.Log("hello");
        // goes through each animation track
        var animationOutputs = director.playableGraph.GetOutputCountByType<AnimationPlayableOutput>();
        for (int i = 0; i < animationOutputs; i++)
        {
            var output = (AnimationPlayableOutput)director.playableGraph.GetOutputByType<AnimationPlayableOutput>(i);
            if (output.GetTarget() != anim)
                continue;

            Debug.Log("hello2");
            var root = output.GetSourcePlayable();
            if (!root.IsValid())
                continue;
            Debug.Log("hello3");
            // walks the playable graph, searching for animation clips
            var port = output.GetSourceOutputPort();
            if (port >= 0)
                root = root.GetInput(0);

            Debug.Log("hello4");
            var queue = new Queue<Playable>();
            queue.Enqueue(root);
            Debug.Log("Number is "+ queue.Count.ToString());
            while (queue.Count > 0)
            {
                var playable = queue.Dequeue();
                for (int j = 0; j < playable.GetInputCount(); j++)
                {
                    // skips playables with 0 weight or disabled
                    if (playable.GetInput(j).GetPlayState() == PlayState.Playing)
                        queue.Enqueue(playable.GetInput(j));
                }

                if (playable.IsPlayableOfType<AnimationClipPlayable>())
                {
                    var clipPlayable = (AnimationClipPlayable)playable;
                    // skips the editor generated clip of the default pose
                    if (clipPlayable.GetAnimationClip() != null && clipPlayable.GetAnimationClip().name != "DefaultPose")
                        Debug.Log(clipPlayable.GetAnimationClip().name);
                }
            }

            while (queue.Count > 0)
            {
                var playable = queue.Dequeue();
                for (int j = 0; j < playable.GetInputCount(); j++)
                {
                    if (playable.IsPlayableOfType<AnimationMixerPlayable>() && playable.GetInputWeight(j) <= 0.01f)
                        continue;

                    queue.Enqueue(playable.GetInput(j));
                }

                if (playable.IsPlayableOfType<AnimationClipPlayable>())
                {
                    var clipPlayable = (AnimationClipPlayable)playable;
                    // skips the editor generated clip of the default pose
                    if (clipPlayable.GetAnimationClip() != null && clipPlayable.GetAnimationClip().name != "DefaultPose")
                        Debug.Log(clipPlayable.GetAnimationClip().name);
                }
            }
        }
    }

}
