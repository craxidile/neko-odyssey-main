using UnityEngine;

namespace SpatiumInteractive.Libraries.Unity.Platform.Debugging
{
    public class ConsoleToGUI : MonoBehaviour
    {
        private string _log = "*begin log ";
        private string _filename = "";
        private bool _doShow = true;
        private int _charDisplayLimit = 2000;

        void OnEnable()
        {
            Application.logMessageReceived += Log;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= Log;
        }

        private void Start()
        {
#if UNITY_ANDROID
            _log += " (tap on the screen - show/hide) ";
#elif UNITY_IOS
            _log += " (tap on the screen - show/hide) ";
#else
            _log += " (left ctrl - show/hide) ";
#endif
        }

        void Update()
        {
#if UNITY_ANDROID
            if (Input.GetMouseButtonDown(0)) { _doShow = !_doShow; }
#elif UNITY_IOS
            if (Input.GetMouseButtonDown(0)) { _doShow = !_doShow; }
#else
            if (Input.GetKeyDown(KeyCode.LeftControl)) { _doShow = !_doShow; }
#endif
        }

        public void Log(string logString, string stackTrace, LogType type)
        {
            // display the log on screen
            _log = _log + "\n" + logString;
            if (_log.Length > _charDisplayLimit)
            {
                _log = _log.Substring(_log.Length - _charDisplayLimit);
            }

            // write down the log to a file on disk
            if (_filename == "")
            {
                string folderPath = string.Empty;

#if UNITY_EDITOR
                folderPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Desktop) +
                    "/Unity Debug-Log Build Outputs";
#elif UNITY_ANDROID
                folderPath = Application.persistentDataPath + "/Unity Debug-Log Build Outputs-android";
#else
                folderPath = Application.persistentDataPath + "/Unity Debug-Log Build Outputs";
#endif

                System.IO.Directory.CreateDirectory(folderPath);
                string r = UnityEngine.Random.Range(1000, 9999).ToString();
                _filename = folderPath + "/log-" + r + ".txt";
            }

            try { System.IO.File.AppendAllText(_filename, logString + "\n"); }
            catch { }
        }

        void OnGUI()
        {
            if (!_doShow) { return; }
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
               new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
            GUI.TextArea(new Rect(10, 10, 540, 370), _log);
        }
    }
}
