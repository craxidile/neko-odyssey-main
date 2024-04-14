namespace SpatiumInteractive.Libraries.Unity.GRU.Tests.Extension
{
    public static class BoolExtension
    {
        public static bool IsSelectedInDb(this bool boolean, bool expectedValue)
        {
            return boolean == expectedValue;
        }

        public static bool IsNotSelectedInDb(this bool boolean, bool expectedValue)
        {
            return !IsSelectedInDb(boolean, expectedValue);
        }
    }
}
