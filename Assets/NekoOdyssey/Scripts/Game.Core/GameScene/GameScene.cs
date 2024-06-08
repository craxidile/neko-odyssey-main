﻿using DG.Tweening;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.GameScene
{
    public class GameScene
    {
        private GameSceneMode _sceneMode;

        public Subject<GameSceneMode> OnChangeSceneMode { get; } = new();
        public Subject<GameSceneMode> OnChangeSceneFinish { get; } = new();
        
        public void OpenScene()
        {
            _sceneMode = GameSceneMode.Opening;
            OnChangeSceneMode.OnNext(_sceneMode);
        }

        public void CloseScene()
        {
            _sceneMode = GameSceneMode.Closing;
            OnChangeSceneMode.OnNext(_sceneMode);
        }

        public void Bind()
        {
        }

        public void Start()
        {
            DOVirtual.DelayedCall(1f, OpenScene , false);
        }

        public void Unbind()
        {
        }
    }
}