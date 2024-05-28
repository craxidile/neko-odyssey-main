using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace NekoOdyssey.Assets.Scenes.MiniGame.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] List<AudioClip> SFXList = new List<AudioClip>();
        [SerializeField] List<AudioClip> BGMList = new List<AudioClip>();
    
        public AudioMixerGroup sfxMixerGroup;
        public AudioMixerGroup bgmMixerGroup;
    
        List<AudioSource> sfxAudioSources = new List<AudioSource>();
        AudioSource bgmAudioSource;
        
        Dictionary<AudioSource, AudioClip> playingSFX = new Dictionary<AudioSource, AudioClip>();
        void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = sfxMixerGroup;
                sfxAudioSources.Add(source);
            }
            
            bgmAudioSource = gameObject.AddComponent<AudioSource>();
            bgmAudioSource.outputAudioMixerGroup = bgmMixerGroup;
        }
    
        public void PlaySFX(string name, bool loop)
        {
            AudioClip clipToPlay = FindAudioClipByName(SFXList, name);
    
            if (clipToPlay != null)
            {
                if (!IsSFXPlaying(clipToPlay))
                {
                    AudioSource freeSource = GetFreeSFXAudioSource();
                    if (freeSource != null)
                    {
                        freeSource.clip = clipToPlay;
                        freeSource.loop = loop;
                        freeSource.Play();
                        if (loop)
                        {
                            // Add the playing source and its associated clip to the dictionary
                            playingSFX.Add(freeSource, clipToPlay);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No free AudioSource available to play SFX: " + name);
                    }
                }
            }
            else
            {
                Debug.LogWarning("SFX clip with name " + name + " not found.");
            }
        }
        public void StopLoopedSFX(string name)
        {
            foreach (var pair in playingSFX)
            {
                if (pair.Value.name == name)
                {
                    pair.Key.Stop();
                    playingSFX.Remove(pair.Key);
                    return;
                }
            }
        }
        private bool IsSFXPlaying(AudioClip clip)
        {
            foreach (var pair in playingSFX)
            {
                if (pair.Value == clip)
                {
                    return true;
                }
            }
            return false;
        }
    
        public void PlayBGM(string name, bool loop)
        {
            AudioClip clipToPlay = FindAudioClipByName(BGMList, name);
    
            if (clipToPlay != null)
            {
                bgmAudioSource.clip = clipToPlay;
                bgmAudioSource.loop = loop;
                bgmAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("BGM clip with name " + name + " not found.");
            }
        }
        public void StopBGM()
        {
            if (bgmAudioSource.isPlaying)
            {
                bgmAudioSource.Stop();
            }
            else
            {
                Debug.LogWarning("No BGM is currently playing.");
            }
        }
    
        private AudioSource GetFreeSFXAudioSource()
        {
            foreach (AudioSource source in sfxAudioSources)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }
            return null;
        }
    
        private AudioClip FindAudioClipByName(List<AudioClip> clipList, string name)
        {
            return clipList.Find(clip => clip.name == name);
        }
    
    }
}
