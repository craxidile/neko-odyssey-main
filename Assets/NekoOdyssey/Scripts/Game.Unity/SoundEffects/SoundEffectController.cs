using System;
using UnityEngine;

namespace NekoOdyssey.Scripts.Game.Unity.SoundEffects
{
    public class SoundEffectController : MonoBehaviour
    {
        public static SoundEffectController Instance;
        
        public AudioSource shutter;
        public AudioSource openPhone;
        public AudioSource closePhone;
        public AudioSource talk;
        public AudioSource hungry;

        private void Awake()
        {
            Instance = this;
        }
    }
}