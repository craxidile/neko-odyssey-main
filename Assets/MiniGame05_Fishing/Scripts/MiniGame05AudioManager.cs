using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace MiniGame05_Fishing.Scripts
{
    public class MiniGame05AudioManager : MonoBehaviour
    {
        [SerializeField] public List<AudioClip> SFXList = new();
        [SerializeField] public List<AudioClip> BGMList = new();

        public AudioMixerGroup sfxMixerGroup;
        public AudioMixerGroup bgmMixerGroup;

        private List<AudioSource> _sfxAudioSources = new();
        private AudioSource _bgmAudioSource;
        private Dictionary<AudioSource, AudioClip> _playingSFX = new();

        private void Awake()
        {
            for (var i = 0; i < 8; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = sfxMixerGroup;
                _sfxAudioSources.Add(source);
            }

            _bgmAudioSource = gameObject.AddComponent<AudioSource>();
            _bgmAudioSource.outputAudioMixerGroup = bgmMixerGroup;
        }

        public void PlaySfx(string name, bool loop)
        {
            var clipToPlay = FindAudioClipByName(SFXList, name);

            if (clipToPlay != null)
            {
                if (!IsSfxPlaying(clipToPlay))
                {
                    var freeSource = GetFreeSfxAudioSource();
                    if (freeSource != null)
                    {
                        freeSource.clip = clipToPlay;
                        freeSource.loop = loop;
                        freeSource.Play();
                        if (loop)
                        {
                            // Add the playing source and its associated clip to the dictionary
                            _playingSFX.Add(freeSource, clipToPlay);
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

        public void StopLoopedSfx(string name)
        {
            foreach (var pair in _playingSFX.Where(pair => pair.Value.name != name))
            {
                pair.Key.Stop();
                _playingSFX.Remove(pair.Key);
                return;
            }
        }

        private bool IsSfxPlaying(AudioClip clip)
        {
            return _playingSFX.Any(pair => pair.Value == clip);
        }

        public void PlayBgm(string name, bool loop)
        {
            var clipToPlay = FindAudioClipByName(BGMList, name);
            if (clipToPlay == null)
            {
                Debug.LogWarning("BGM clip with name " + name + " not found.");
                return;
            }

            _bgmAudioSource.clip = clipToPlay;
            _bgmAudioSource.loop = loop;
            _bgmAudioSource.Play();
        }

        public void StopBgm()
        {
            if (_bgmAudioSource.isPlaying)
            {
                _bgmAudioSource.Stop();
            }
            else
            {
                Debug.LogWarning("No BGM is currently playing.");
            }
        }

        private AudioSource GetFreeSfxAudioSource()
        {
            return _sfxAudioSources.FirstOrDefault(source => !source.isPlaying);
        }

        private static AudioClip FindAudioClipByName(List<AudioClip> clipList, string name)
        {
            return clipList.Find(clip => clip.name == name);
        }
    }
}