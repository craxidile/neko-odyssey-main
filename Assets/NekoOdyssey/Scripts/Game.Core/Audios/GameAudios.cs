using System;
using UniRx;

namespace NekoOdyssey.Scripts.Game.Core.Audios
{
    public class GameAudios
    {
        public Subject<string> ActiveAudio { get; set; } = new();
        public Subject<string> InactiveAudio { get; set; } = new();
        public Subject<ValueTuple<string, float>> AudioToClone { get; set; } = new();

        public GameAudios()
        {
        }

        public void Bind()
        {
        }

        public void Unbind()
        {
        }

        public void Start()
        {
        }
    }
}