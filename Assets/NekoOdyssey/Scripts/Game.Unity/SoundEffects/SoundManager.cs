using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public bool debug;
    public AudioTrack[] tracks;

    private Hashtable m_AudioTable; //relationship between audio type (key) and auido tracks (value)
    private Hashtable m_JobTable; // relationship between audio type (key) and jobs (value) (corutine,IEnumerator)

    [System.Serializable]
    public class AudioObject
    {
        public SoundType type;
        public AudioClip clip;
    }
    [System.Serializable]
    public class AudioTrack
    {
        public AudioSource source;
        public AudioObject[] audios;
    }
    private class AudioJob
    {
        public AudioAction action;
        public SoundType type;
        public bool fade;
        public float delay;
        public AudioJob(AudioAction _action, SoundType _type ,bool _fade, float _delay)
        {
            action = _action;
            type = _type;
            fade = _fade;
            delay = _delay;
        }
    }
    private enum AudioAction
    {
        Start,
        Stop,
        Restart
    }

    private void Awake()
    {
        if (!Instance)
        {
            Configured();
        }
    }


    private void OnDisable()
    {
        Dispose();
    }

    public void PlayAudio(SoundType _type, bool _fade = false, float _delay = 0f)
    {
        AddJob(new AudioJob(AudioAction.Start, _type, _fade, _delay));
    }
    public void StopAudio(SoundType _type, bool _fade = false, float _delay = 0f)
    {
        AddJob(new AudioJob(AudioAction.Stop, _type, _fade, _delay));
    }
    public void RestartAudio(SoundType _type, bool _fade = false, float _delay = 0f)
    {
        AddJob(new AudioJob(AudioAction.Restart, _type, _fade, _delay));
    }
    private void Configured()
    {
        Instance = this;
        m_AudioTable = new Hashtable();
        m_JobTable = new Hashtable(); 
        GenerateAudioTable();
    }
    private void Dispose()
    {
        foreach(DictionaryEntry _entry in m_JobTable) 
        {
            IEnumerator _job = (IEnumerator)_entry.Value;
            StopCoroutine(_job);
        }
    }

    private void GenerateAudioTable()
    {
        foreach(var _track in tracks)
        {
            foreach (var _obj in _track.audios)
            {
                if (m_AudioTable.ContainsKey(_obj.type))
                {
                    LogWarning("You are trying to register audio [" + _obj.type + "] that has already been registered.");
                }
                else
                {
                    m_AudioTable.Add(_obj.type, _track);
                    Log("Registering audio [" + _obj.type + "].");
                }
            }
        }
    }
    private IEnumerator RunAudioJob(AudioJob _job)
    {
        yield return new WaitForSeconds(_job.delay);
        AudioTrack _track = (AudioTrack)m_JobTable[_job.type];
        _track.source.clip = GetAudioClipFromAudioTrack(_job.type, _track);
        switch (_job.action)
        {
            case AudioAction.Start:
                _track.source.Play();
                break;
            case AudioAction.Stop:
                if (!_job.fade)
                {
                    _track.source.Stop();
                }
                break;
            case AudioAction.Restart:
                _track.source.Stop();
                _track.source.Play();
                break;
        }
        if (_job.fade)
        {
            float _initial = _job.action == AudioAction.Start || _job.action == AudioAction.Restart ? 0f : 1f;
            float _target = _initial == 0 ? 1 : 0;
            float _duration = 1f;
            float _timer = 0f;

            while (_timer < _duration)
            {
                _track.source.volume = Mathf.Lerp(_initial, _target, _timer / _duration);
                _timer += Time.deltaTime;
                yield return null;
            }

            if (_job.action == AudioAction.Stop)
            {
                _track.source.Stop();
            }
        }
        m_JobTable.Remove(_job.type);
        Log("Job count: " + m_JobTable.Count);
        yield return null;
    }

    private void AddJob(AudioJob _job)
    {
        RemoveConflictingJobs(_job.type);

        IEnumerator _jobRunner = RunAudioJob(_job);
        m_JobTable.Add(_job.type, _jobRunner);
        StartCoroutine(_jobRunner);
        Log("Starting job on [" + _job.type + "] with operation: " + _job.action);

    }

    private void RemoveJob(SoundType _type)
    {
        if (!m_JobTable.ContainsKey(_type))
        {
            LogWarning("Trying to stop a job [" + _type + "] that is now running.");
            return;
        }
        IEnumerator _runningJob = (IEnumerator)m_JobTable[_type];
        StopCoroutine(_runningJob);
        m_JobTable.Remove(_type);
    }


    private void RemoveConflictingJobs(SoundType _type)
    {
        if (m_JobTable.ContainsKey(_type))
        {
            RemoveJob(_type);
        }
        SoundType _conflictAudio = SoundType.None;
        foreach (DictionaryEntry _enty in m_JobTable)
        {
            SoundType _audioType = (SoundType)_enty.Key;
            AudioTrack _audioTrackInUse = (AudioTrack)m_AudioTable[_audioType];
            AudioTrack _audioTrackNeeded = (AudioTrack)m_AudioTable[_type];
            if (_audioTrackNeeded.source == _audioTrackInUse.source)
            {
                _conflictAudio = _audioType;
            }
        }
        if (_conflictAudio != SoundType.None) 
        {
            RemoveJob(_conflictAudio);
        }
    }

    private AudioClip GetAudioClipFromAudioTrack(SoundType _type, AudioTrack _track)
    {
        foreach (var _obj in _track.audios)
        {
            if (_obj.type == _type)
            {
                return _obj.clip;
            }
        }
        return null;
    }

    private void Log(string _msg)
    {
        if (!debug) return;
        Debug.Log("[Audio]: " + _msg);
    }
    private void LogWarning(string _msg) 
    {
        if (!debug) return;
        Debug.LogWarning("[Audio]: " + _msg);
    }

    //[SerializeField] public Dictionary<SoundEffect, AudioSource> audioDictionary;

    //HashSet<SoundEffect> _frameDuplicateSound = new HashSet<SoundEffect>();

    //void Awake()
    //{
    //    Instance = this;
    //}

    //internal void PlaySoundEffect(in SoundEffect soundEffect, in bool forcePlay = false)
    //{
    //    if (soundEffect == SoundEffect.None || !isEnableSound) 
    //        return;

    //    if (!forcePlay && audioDictionary[soundEffect].isPlaying)
    //        return;

    //    if (!_frameDuplicateSound.Add(soundEffect)) 
    //        return;

    //    StartCoroutine(FrameDuplicateSoundCheck(soundEffect));

    //    //audioDictionary[soundEffect].gameObject.SetActive(true);
    //    //audioDictionary[soundEffect].Play();
    //}

    //IEnumerator FrameDuplicateSoundCheck(SoundEffect soundEffect)
    //{
    //    yield return new WaitForEndOfFrame();
    //    _frameDuplicateSound.Remove(soundEffect);
    //}

    //public void MakeSoundCheckDelay(float delay)
    //{
    //    StartCoroutine(soundCheck(delay));
    //}
    //IEnumerator soundCheck(float delay)
    //{
    //    isEnableSound = false;
    //    yield return new WaitForSeconds(delay);
    //    isEnableSound = true;
    //}
}
