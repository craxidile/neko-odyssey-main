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
        }

        private void PlaySfx(string name)
        {
            if (!_audioSwitchMap.ContainsKey(name)) return;
            var switchGameObject = _audioSwitchMap[name];
            switchGameObject.SetActive(true);
        }

        private void StopSfx(string name)
        {
            if (!_audioSwitchMap.ContainsKey(name)) return;
            var switchGameObject = _audioSwitchMap[name];
            switchGameObject.SetActive(false);
        }
    }
}