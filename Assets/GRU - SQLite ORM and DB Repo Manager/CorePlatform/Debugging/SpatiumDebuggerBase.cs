using UnityEngine;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace SpatiumInteractive.Libraries.Unity.Platform.Debugging
{
    public abstract class SpatiumDebuggerBase
    {
        #region Constants 

        private const string DEFAULT_PACKAGE_PREFIX = "SpatiumInteractive.Libraries.Unity.Platform";
        private const string NORMAL_COLOR = "<color=white>";
        private const string SUCCESS_COLOR = "<color=#FFDFB1>";
        private const string WARNING_COLOR = "<color=yellow>";
        private const string ERROR_COLOR = "<color=red>";
        private const string DID_YOU_KNOW_COLOR = "<color=gray>";
        private const string COLOR_END_TAG = "</color>";

        #endregion

        #region Enums

        public enum MessageStatus
        {
            Normal = 0,
            Warning = 1,
            Error = 2,
            Success = 3,
            DidYouKnow = 4
        }

        #endregion

        #region Methods

        protected static string ColorMessageByStatus(MessageStatus status, string message)
        {
            string color = string.Empty;
            switch (status)
            {
                case MessageStatus.Normal:
                    color = $"{NORMAL_COLOR}{message}{COLOR_END_TAG}";
                    break;
                case MessageStatus.Warning:
                    color = $"{WARNING_COLOR}{message}{COLOR_END_TAG}";
                    break;
                case MessageStatus.Error:
                    color = $"{ERROR_COLOR}{message} {COLOR_END_TAG} ";
                    break;
                case MessageStatus.Success:
                    color = $"{SUCCESS_COLOR}{message}{COLOR_END_TAG}";
                    break;
                case MessageStatus.DidYouKnow:
                    color = $"{DID_YOU_KNOW_COLOR}{message}{COLOR_END_TAG}";
                    break;
                default:
                    break;
            }
            return color;
        }

        public static void LogSuccess(string message, MessageStatus status = MessageStatus.Success, bool doBold = false, bool doItalic = false, string packagePrefix = DEFAULT_PACKAGE_PREFIX)
        {
            LogMessage(message, status, doBold, doItalic, packagePrefix);
        }

        public static void LogWarning(string message, MessageStatus status = MessageStatus.Warning, bool doBold = false, bool doItalic = false, string packagePrefix = DEFAULT_PACKAGE_PREFIX)
        {
            LogMessage(message, status, doBold, doItalic, packagePrefix);
        }

        public static void LogError(string message, MessageStatus status = MessageStatus.Error, bool doBold = false, bool doItalic = false, string packagePrefix = DEFAULT_PACKAGE_PREFIX)
        {
            LogMessage(message, status, doBold, doItalic, packagePrefix);
        }

        public static void LogDidYouKnow(string message, MessageStatus status = MessageStatus.DidYouKnow, bool doBold = false, bool doItalic = false, string packagePrefix = DEFAULT_PACKAGE_PREFIX)
        {
            LogMessage(message, status, doBold, doItalic, packagePrefix);
        }

        public static void LogMessage(string message, MessageStatus status = MessageStatus.Normal, bool doBold = false, bool doItalic = false, string packagePrefix = DEFAULT_PACKAGE_PREFIX)
        {
            string output = message;

            if (doBold)
            {
                output = output.WrapInBoldTags();
            }

            if (doItalic)
            {
                output = output.WrapInItalicTags();
            }

            output = ColorMessageByStatus(status, output);

            string prefixFormatted = packagePrefix.WrapInBoldTags();
            output = output.AddPrefix(prefixFormatted);

            Debug.Log(output);
        }

        #endregion
    }
}
