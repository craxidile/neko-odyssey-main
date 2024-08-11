using System;
using NekoOdyssey.Scripts.Constants;
using NekoOdyssey.Scripts.Database.Commons.Models;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;

namespace NekoOdyssey.Scripts.Database.Domains.Npc.Entities.DialogLineEntity.Models
{
    [Serializable]
    public class DialogLine : EntityBase<int>, IAggregateRoot
    {
        // [Indexed]
        // [ForeignKey(typeof(SubDialog))]
        public int SubDialogId { get; set; }

        [NotNull]
        public string Actor { get; set; }

        public string Original { get; set; }
        public string TextTh { get; set; }
        public string TextEn { get; set; }
        public string TextZhCn { get; set; }
        public string TextZhTw { get; set; }
        public string TextJa { get; set; }
        
        [Ignore]
        public LocalizedText LocalizedText => new()
        {
            Original = Original,
            Th = TextTh,
            En = TextEn,
            ZhCn = TextZhCn,
            ZhTw = TextZhTw,
            Ja = TextJa,
        };
        
        public string AnimatorParam { get; set; }
        public string AnimatorParamValue { get; set; }
        public int? AnimatorDelay { get; set; }
        
        public string Photo { get; set; }

        public string GetLocalisedText()
        {
            return Original; //bypass for now
            //--------------------------------

            var locale = GameRunner.Instance.Core.Settings.Locale;
            switch (locale)
            {
                case Locale.Th:
                    return TextTh;
                case Locale.En:
                    return TextEn;
                case Locale.ZhCn:
                    return TextZhCn;
                case Locale.ZhTw:
                    return TextZhTw;
                case Locale.Ja:
                    return TextJa;
                default:
                    return null;
            }
        }
    }
}