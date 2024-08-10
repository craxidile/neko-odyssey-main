using NekoOdyssey.Scripts.Constants;

namespace NekoOdyssey.Scripts.Database.Commons.Models
{
    public class LocalizedText
    {
        public string Original { get; set; }
        public string Th { get; set; }
        public string En { get; set; }
        public string Ja { get; set; }
        public string ZhCn { get; set; }
        public string ZhTw { get; set; }

        public string ToLocalizedString(Locale locale)
        {
            switch (locale)
            {
                case Locale.En:
                    return En;
                case Locale.Th:
                    return Th;
                case Locale.Ja:
                    return Ja;
                case Locale.ZhCn:
                    return ZhCn;
                case Locale.ZhTw:
                    return ZhTw;
                case Locale.None:
                default:
                    return Original;
            }
        }
    }
}