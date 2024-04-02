using System;
using UnityEngine;

namespace NekoOdyssey.Scripts.DataSerializers.Csv
{
    public class MultiColumnCsvParser
    {
        public static T ParseColumn<T>(string line, int column)
        {
            var parts = (line ?? "").TrimEnd().Split(",");
            if (parts.Length <= column)
                throw new Exception("Incomplete data");

            var returnType = typeof(T);

            if (returnType == typeof(string))
            {
                return (T)Convert.ChangeType(parts[column], returnType);
            }

            if (returnType == typeof(float) && float.TryParse(parts[column], out var floatValue))
            {
                return (T)Convert.ChangeType(floatValue, returnType);
            }

            if (returnType == typeof(bool) && bool.TryParse(parts[column].ToLower(), out var boolValue))
            {
                return (T)Convert.ChangeType(boolValue, returnType);
            }

            throw new Exception($"Invalid type: {line}");
        }
    }
}