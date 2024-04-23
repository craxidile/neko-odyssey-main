using UnityEngine;

namespace NekoOdyssey.Scripts.Constants
{
    public class AppConstants
    {
        public const string Version = "0.1.0";
        
        public static string BaseAppFilePath { get; private set; } = null;

        public static void Intialize()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_OSX
            Debug.Log($">>app_constants<< initialize {Application.persistentDataPath}");
            BaseAppFilePath = Application.persistentDataPath;   
            #endif
        }
    }
}