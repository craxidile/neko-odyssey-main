using System;
using System.Collections.Generic;
using UnityEngine;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages;

namespace SpatiumInteractive.Libraries.Unity.GRU.Configs.General
{
    [Serializable]
    public class DbMetaData
    {
        #region Public Fields

        public string DbName;
        public string DbPath;
        public List<string> TableNames;
        public string DbHandlerGOName;
        public string DbContextName;
        public string CreatedAt;

        #endregion

        #region Constructors

        public DbMetaData
        (
            string dbName,
            string dbPath,
            List<string> tableNames,
            string dbHandlerGOName,
            string dbContextName
        )
        {
            DbName = dbName;
            DbPath = dbPath;
            TableNames = tableNames;
            DbHandlerGOName = dbHandlerGOName;
            DbContextName = dbContextName;
            CreatedAt = DateTime.Now.ToString();
        }

        #endregion

        #region Public Methods

        public string GetDbFilePathForEditor()
        {
            if (Application.isEditor)
            {
                string dbPath = DbPath;
                string dbFileName = DbName.AddSuffix(GeneralConfig.Defaults.DATABASE_FILE_EXTENSION);
                string filePath = $"{dbPath}/{dbFileName}";
                return filePath;
            }
            else
            {
                throw new ApplicationException
                (
                    MessageConfig.Debugging.Exception.EXCP_GET_DB_FILE_PATH_FOR_EDITOR_CALL_FROM_NON_EDITOR_PLATFORM
                );
            }
        }

        #endregion
    }
}
