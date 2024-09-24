using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.Audios
{
    public class SfxAudioSwitchBox : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> _switchMap = new();

        public List<GameObject> switches;

        private void Start()
        {
            foreach (var gameObjectSwitch in switches) _switchMap.Add(gameObjectSwitch.name, gameObjectSwitch);

            if (GameRunner.Instance == null) return;

            GameRunner.Instance.Core.Audios.ActiveAudio
                .Subscribe(gameObjectName =>
                {
                    if (!_switchMap.ContainsKey(gameObjectName)) return;
                    _switchMap[gameObjectName].SetActive(true);
                })
                .AddTo(this);

            GameRunner.Instance.Core.Audios.InactiveAudio
                .Subscribe(gameObjectName =>
                {
                    if (!_switchMap.ContainsKey(gameObjectName)) return;
                    _switchMap[gameObjectName].SetActive(false);
                })
                .AddTo(this);

            GameRunner.Instance.Core.Audios.AudioToClone
                .Subscribe((gameObjectAndDelay) =>
                {
                    var (gameObjectName, delay) = gameObjectAndDelay;
                    if (!_switchMap.ContainsKey(gameObjectName)) return;
                    var source = _switchMap[gameObjectName];
                    var switchGameObject = Instantiate(source, source.transform.parent);
                    switchGameObject.SetActive(true);
                    DOVirtual.DelayedCall(delay, () => Destroy(switchGameObject));
                })
                .AddTo(this);
        }
    }
}