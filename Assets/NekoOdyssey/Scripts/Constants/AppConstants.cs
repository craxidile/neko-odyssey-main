using UnityEngine;

namespace NekoOdyssey.Scripts.Constants
{
    public class AppConstants
    {
        public const string Version = "0.1.0";

        public const string WindowsAssetBundlesFolder = "StandaloneWindows";
        public const string SwitchAssetBundlesFolder = "Switch";

        public static class Stamina
        {
            // Total stamina limit
            public const int MaxTotal = 20000;

            // Max stamina on each gauge
            public const int MaxNormal = 10000;

            // Stamina player obtains when day starts
            public const int NewDay = 10000;

            // Length of tie (in-game minutes) that player can stay idle with `NewDay` stamina
            public const int LiveTime = 480;
        }

        public static class Time
        {
            public const int MaxDayMinute = 1440;
            public const string StartDayTime = "8:00"; // 60 = 1 hours in game; 600 = 10:00
            public const string EndDayTime = "21:59";
            public const float GameHourPerMinute = 0.25f; //how many in game hours per real life minute
        }

        public static string AssetBundlesFolder
        {
            get
            {
#if UNITY_SWITCH
                return SwitchAssetBundlesFolder;
#else
                return WindowsAssetBundlesFolder;
#endif
            }
        }

        public static string BaseAppFilePath { get; private set; } = null;

        public static void Initialize()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_OSX
            Debug.Log($">>app_constants<< initialize {Application.persistentDataPath}");
            BaseAppFilePath = Application.persistentDataPath;
#endif
        }
    }
}