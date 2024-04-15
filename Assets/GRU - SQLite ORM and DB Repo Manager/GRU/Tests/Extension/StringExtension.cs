namespace SpatiumInteractive.Libraries.Unity.GRU.Tests.Extension
{
    public static class StringExtension
    {
        public static bool IsSelectedInDb(this string str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) || str.Length == 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsNotSelectedInDb(this string str)
        {
            return !IsSelectedInDb(str);
        }
    }
}
