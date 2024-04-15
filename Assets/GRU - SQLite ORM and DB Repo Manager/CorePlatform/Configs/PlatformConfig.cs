namespace SpatiumInteractive.Libraries.Unity.Platform.Configs
{
    public struct Config
    {
        public struct Editor
        {
            public struct Window
            {
                public struct General
                {
                    public const int DEFAULT_WINDOW_HEIGHT = 500;
                    public const int DEFAULT_WINDOW_WIDTH = 500;
                }

                public struct PaneSplitView
                {
                    public const int DEFAULT_MIN_SIZE_HEIGHT = 250;
                    public const int DEFAULT_MIN_SIZE_WIDTH = 500;
                }

                public struct Button
                {
                    public const string DEFAULT_TITLE = "Button";
                }

                public struct Checkbox
                {
                    public const string DEFAULT_LABEL = "Checkbox";
                }
            }
        }
    }
}
