using SpatiumInteractive.Libraries.Unity.Platform.Debugging;

namespace SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Debugging
{
    public class GRUDebugger : SpatiumDebuggerBase
    {
        #region Private Constants 

        private const string PackagePrefix = "GRU Debugger: ";

        #endregion

        #region Public Static Methods (API)

        public static void LogMessage(string message, MessageStatus status = MessageStatus.Normal, bool doBold = false, bool doItalic = false)
        {
            LogMessage(message, status, doBold, doItalic, PackagePrefix);
        }

        public static void LogSuccess(string message, MessageStatus status = MessageStatus.Success, bool doBold = false, bool doItalic = false)
        {
            LogSuccess(message, status, doBold, doItalic, PackagePrefix);
        }
 
        public static void LogWarning(string message, MessageStatus status = MessageStatus.Warning, bool doBold = false, bool doItalic = false)
        {
            LogWarning(message, status, doBold, doItalic, PackagePrefix);
        }

        public static void LogError(string message, MessageStatus status = MessageStatus.Error, bool doBold = false, bool doItalic = false)
        {
            LogError(message, status, doBold, doItalic, PackagePrefix);
        }

        public static void LogDidYouKnow(string message, MessageStatus status = MessageStatus.DidYouKnow, bool doBold = false, bool doItalic = false)
        {
            LogDidYouKnow(message, status, doBold, doItalic, PackagePrefix);
        }

        #endregion
    }
}
