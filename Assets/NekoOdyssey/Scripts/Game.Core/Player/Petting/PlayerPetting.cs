using NekoOdyssey.Scripts.Game.Core.Petting;
using UniRx;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Player.Petting
{
    public class PlayerPetting
    {
        public Vector3 TargetPosition { get; set; }
        public PettingMode Mode { get; set; }

        public Subject<Unit> OnFinishPetting { get; } = new();

        public void Bind()
        {
        }

        public void Start()
        {
        }

        public void Unbind()
        {
        }

        public void Finish()
        {
            OnFinishPetting.OnNext(Unit.Default);
        }
    }
}