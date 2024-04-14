namespace SpatiumInteractive.Libraries.Unity.GRU.Tests.Extension
{
    public static class IntExtension
    {
        public static bool IsSelectedInDb(this int num)
        {
            if (num == default)
            {
                return false;
            }
            return true;
        }

        public static bool IsNotSelectedInDb(this int num)
        {
            return !IsSelectedInDb(num);
        }
    }
}
