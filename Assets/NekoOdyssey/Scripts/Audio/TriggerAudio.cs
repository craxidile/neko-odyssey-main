// using UnityEngine;
//
// namespace Assets.NekoOdyssey.Scripts.Audio
// {
//     public abstract class TriggerAudio : MonoBehaviour
//     {
//         [FMODUnity.EventRef]
//         public static string Event;
//         public bool PlayAwake;
//
//         public abstract void PlayOneShot();
//         {
//             FMODUnity.RuntimeManager.PlayOneShotAttached(Event, GameObject);
//         }
//
//         private void Start()
//         {
//             if (PlayAwake)
//                 PlayOneShot();
//         }
//     }
// }