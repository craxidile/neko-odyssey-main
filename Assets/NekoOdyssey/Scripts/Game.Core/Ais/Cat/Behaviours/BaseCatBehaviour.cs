using System.Collections;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Core.Ais.Cat.Behaviours
{
    public abstract class BaseCatBehaviour : ICatBehaviour
    {
        private float _coolDownTime;

        public bool Enabled { get; set; } = true;
        public bool IsExecutable => Enabled && Time.time >= _coolDownTime;
        public CatAi CatAi { get; }
        
        public abstract float CoolDownDelay { get; }

        protected BaseCatBehaviour(CatAi catAi)
        {
            CatAi = catAi;
        }

        public abstract void Start();

        protected void CoolDown(float additionalDelay)
        {
            _coolDownTime = Time.time + CoolDownDelay + additionalDelay;
        }

    }
}