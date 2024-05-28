using UnityEngine;

namespace NekoOdyssey.Scripts.Constants
{
    public class AppConstants
    {
        public const string Version = "0.1.0";

        public const string WindowsAssetBundlesFolder = "StandaloneWindows";
        public const string SwitchAssetBundlesFolder = "Switch";

        public class Stamina
        {
            public const int MaxTotal = 20000; //total amount of stamina
            public const int MaxNormal = 10000; //max stamina for each bar
            public const int NewDay = 10000; //how much stamina player start per day
            public const int LiveTime = 360; //how long (in-game minute) player can stay idle with NewDay stamina
        }
        
        public class Time
        {
            public const int MaxDayMinute = 1440;
            public const string StartDayTime = "10:00"; // 60 = 1 hours in game //600 = 10:00
            public const string EndDayTime = "22:00";
            public const float GameHourPerMinute = 0.25f; //how many in game hours per real life minute
        }

        public const float RapidExpCdfLambda = .8f;
        public const float NormalExpCdfLambda = .2f;

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