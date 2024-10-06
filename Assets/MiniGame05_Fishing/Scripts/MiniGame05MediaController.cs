using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace MiniGame05_Fishing.Scripts
{
    public class MiniGame05MediaController : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> _audioSwitchMap = new();

        public List<GameObject> audioSwitches;

        private void Awake()
        {
            foreach (var gameObjectSwitch in audioSwitches)
            {
                _audioSwitchMap.Add(gameObjectSwitch.name, gameObjectSwitch);
            }
        }

        private void Start()
        {
            MiniGame05.Instance.OnPlaySfx
                .Subscribe(name =>
                {
                    PlaySfx(name);
                    DOVirtual.DelayedCall(3f, () => StopSfx(name));
                })
                .AddTo(this);
            MiniGame05.Instance.OnPlayLoopSfx
                .Subscribe(PlaySfx)
                .AddTo(this);
            MiniGame05.Instance.OnStopSfx
                .Subscribe(StopSfx)
                .AddTo(this);
            MiniGame05.Instance.OnPlayCloneSfx
                .Subscribe(CloneAndPlaySfx)
                .AddTo(this);
        }

        private void CloneAndPlaySfx(string gameObjectName)
        {
            if (!_audioSwitchMap.TryGetValue(gameObjectName, out var switchGameObject)) return;
            var clonedGameObject = Instantiate(switchGameObject, switchGameObject.transform.parent);
            clonedGameObject.SetActive(true);
            DOVirtual.DelayedCall(3f, () =>
            {
                clonedGameObject.SetActive(false);
                Destroy(clonedGameObject);
            });
        }

        private void PlaySfx(string gameObjectName)
        {
            if (!_audioSwitchMap.TryGetValue(gameObjectName, out var switchGameObject)) return;
            switchGameObject.SetActive(true);
        }

        private void StopSfx(string gameObjectName)
        {
            if (!_audioSwitchMap.TryGetValue(gameObjectName, out var switchGameObject)) return;
            switchGameObject.SetActive(false);
        }
    }
}