using SpatiumInteractive.Libraries.Unity.Platform.Extensions;
using UnityEngine;

namespace SpatiumInteractive.Libraries.Unity.GRU.Extensions
{
    public static class PathExtension
    {
        public static string ToDatabasePath(this string displayPath)
        {
            string dbPath = displayPath.Replace("\\", "/");
            if (!dbPath.EndsWith("/"))
            {
                dbPath = dbPath.AddSuffix("/");
            }
            return dbPath;
        }

        /// <summary>
        /// returns absolute path to StreamingAssets folder in the editor.
        /// </summary>
        /// <param name="relativeDbPath">Assets/StreamingAssets/</param>
        /// <returns><i>C:/Something/Something/Assets/StreamingAssets/</i></returns>
        public static string ToAbsoluteDatabasePath(this string str)
        {
            string assetsFolderPath = Application.dataPath;
            string dbPath = $"{assetsFolderPath}\\StreamingAssets";
            return dbPath;
        }
    }
}
