using System;
using System.Collections;
using System.Collections.Generic;
using Assets.NekoOdyssey.Scripts.Audio;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public partial class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private List<EventInstance> eventInstances;

    private List<StudioEventEmitter> eventEmitters;

    private EventInstance ambienceEventInstance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more thane one Audio Manager in the scene.");
        }

        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        initializeAmbience(FMODEvents.instance.ambience);
    }

    private void initializeAmbience(EventReference ambienceEventReference)
    {
        ambienceEventInstance = CreateInstance(ambienceEventReference);
        ambienceEventInstance.start();
    }

    public void PlayONeShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }
}
