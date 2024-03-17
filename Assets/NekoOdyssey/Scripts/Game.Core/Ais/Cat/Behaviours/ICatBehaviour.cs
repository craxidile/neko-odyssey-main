using System.Collections;

namespace NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours
{
    public interface ICatBehaviour
    {
        public CatAi CatAi { get; }
        public bool Enabled { get; }
        public float CoolDownDelay { get; }
        public bool IsExecutable { get; }

        public void Start();
    }
}