#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using UnityEngine;
using SpatiumInteractive.Libraries.Unity.GRU.Helpers.Bootstrapping;
using System;

namespace SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Editor
{
    [InitializeOnLoad]
    public static class DbFileDeletionChecker
    {
        private const float CHECK_PERIOD_COOLDOWN_TIME = 2f;

        private const float COOLDOWN_AFTER_INITIAL_CREATION_TIME = 10f;

        private static DateTime? _timeChecked;
        private static DateTime TimeChecked
        {
            get
            {
                if (_timeChecked == null)
                {
                    _timeChecked = DateTime.Now;
                }

                return _timeChecked.Value;
            }
        }

        static DbFileDeletionChecker()
        {
            EditorApplication.update += CheckForDeletedFile;
        }

        private static void CheckForDeletedFile()
        {
            var diff = Math.Abs((TimeChecked - DateTime.Now).TotalSeconds);

            if (diff > CHECK_PERIOD_COOLDOWN_TIME)
            {
                var dbConfig = FilesManager.GetDbConfigData();
                if (dbConfig != null)
                {
                    foreach (var dbMeta in dbConfig.DbMetaData)
                    {
                        DateTime dateCreated = DateTime.Now;
                        bool successfullParse = DateTime.TryParseExact(dbMeta.CreatedAt, "d.M.yyyy. H:mm:ss", null, System.Globalization.DateTimeStyles.None, out dateCreated);

                        if (successfullParse && dateCreated != null)
                        {
                            var timeSinceCreated = Math.Abs((dateCreated - DateTime.Now).TotalSeconds);
                            if (timeSinceCreated > COOLDOWN_AFTER_INITIAL_CREATION_TIME)
                            {
                                var dbFilePath = dbMeta.GetDbFilePathForEditor();
                                if (!File.Exists(dbFilePath))
                                {
                                    var manuallyDeletedDatabase = dbMeta.DbName;
                                    UpdateOrDeleteDbForm.DeleteDbProcedure(manuallyDeletedDatabase);
                                    Debug.Log("DbFileDeletionChecker.cs has triggered the post delete cleanup...");
                                }
                            }
                        }

                    }
                }
                _timeChecked = DateTime.Now;
            }
        }
    }
}
#endif
