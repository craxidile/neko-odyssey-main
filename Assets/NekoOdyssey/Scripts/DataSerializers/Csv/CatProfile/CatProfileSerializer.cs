using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NekoOdyssey.Scripts.DataSerializers.Csv.CatProfile
{
    public class CatProfileSerializer : IDataSerializer<Models.CatProfile>
    {

        private const int ColumnSkip = 1;
        private const int LineSkip = 0;
        
        public string Serialize(Models.CatProfile data)
        {
            throw new NotImplementedException();
        }

        public string[] DeserializeHeadColumns(string text)
        {
            using var reader = new StringReader(text);
            
            var line = reader.ReadLine();
            var parts = line?.Split($",");
            
            if (parts == null || parts.Length <= ColumnSkip)
                return Array.Empty<string>();

            return parts.Skip(ColumnSkip).ToArray();
        }

        public Models.CatProfile DeserializeLines(string text, int columnIndex = 0)
        {
            columnIndex += ColumnSkip;
            Debug.Log($">>columns<< {columnIndex}");
            try
            {
                using var reader = new StringReader(text);
                var catProfile = new Models.CatProfile
                {
                    CatCode = MultiColumnCsvParser.ParseColumn<string>(reader.ReadLine(), columnIndex),
                    AnimatorControllerName = MultiColumnCsvParser.ParseColumn<string>(reader.ReadLine(), columnIndex),
                    NearestPlayerDistance = MultiColumnCsvParser.ParseColumn<float>(reader.ReadLine(), columnIndex),
                    MoveSpeed = MultiColumnCsvParser.ParseColumn<float>(reader.ReadLine(), columnIndex),
                    JumpSpeed = MultiColumnCsvParser.ParseColumn<float>(reader.ReadLine(), columnIndex),
                    EatingDuration = MultiColumnCsvParser.ParseColumn<float>(reader.ReadLine(), columnIndex),
                    HasCallToFeedBehaviour = MultiColumnCsvParser.ParseColumn<bool>(reader.ReadLine(), columnIndex),
                    HasFollowPlayerBehaviour = MultiColumnCsvParser.ParseColumn<bool>(reader.ReadLine(), columnIndex),
                };
                return catProfile;
            }
            catch (Exception ex)
            {
                Debug.Log($">>cat_profile_parsing_error<< {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }
        
    }
}