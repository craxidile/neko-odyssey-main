using FMODUnity;
using UnityEngine;

namespace Assets.NekoOdyssey.Scripts.Audio
{
    public class FMODEvents : MonoBehaviour
    {
        [field: Header("Player SFX")]
        [field: SerializeField] public EventReference playerFootSteps { get; private set; }
        
        [field: Header("Ambience")]
        [field: SerializeField] public EventReference ambience { get; private set; }
        public static FMODEvents instance { get; private set; }

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Found more thane one FMOD Events instance in the scene.");
            }

            instance = this;
        }
    }
}