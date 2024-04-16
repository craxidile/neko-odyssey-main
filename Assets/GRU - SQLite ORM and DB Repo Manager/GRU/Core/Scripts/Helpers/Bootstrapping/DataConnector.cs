#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SQLite4Unity3d;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Debugging;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;

namespace Database.Repository
{
    /// <summary>
    /// Bridge/Connector class that should be used 
    /// as a middle-man between lower level code (<see cref="SQLiteConnection"/>  - ORM script)
    /// and higher level calls. In future versions of
    /// GRU, this class will probably get completly deleted 
    /// and all its content migrated to <see cref="DBHandler"> DBHandler.cs </see>script.
    /// </summary>
    public class DataConnector
    {
        private SQLiteConnection _connection;

        #region Constructors/Initalization

        /// <summary>
        /// Initializes new database connection based on the passed params.
        /// If there's no database with given name, on given path, an error is thrown.
        /// This works like a bridge between the consumer class and the underlying infrastructure
        /// (SQLite4Unity3d).
        /// </summary>
        /// <param name="databaseName">raw database name, no formatting applied, just a plain simple name with no extensions added</param>
        /// <param name="databasePath">raw database path - a path in the format of: <i>C:\something\subSomething\Assets\StreamingAssets\</i></param>
        public DataConnector(string databaseName, string databasePath)
        {
            databaseName = databaseName.ToDatabaseName();

#if UNITY_EDITOR
            var dbFilePath = string.Format("{0}/{1}", databasePath, databaseName);
#elif UNITY_SWITCH
            SQLite3.NlibSetDirectory(SQLite3.SQLiteNlibDirectoryType.Data, "save:");
            var dbFilePath = databaseName;
            Debug.Log($">>db<< browse_data_save {dbFilePath}");
#else
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, databaseName);

        if (!File.Exists(filepath))
        {
            GRUDebugger.LogMessage(MessageConfig.Debugging.Warning.WARNG_DB_NOT_IN_PERSISTENT_DATA_PATH);
#if UNITY_ANDROID
            HandleDbFileOnAndroid(databaseName, filepath);
#elif UNITY_IOS
            HandleDbFileOniOS(databaseName, filepath);
#elif UNITY_WP8
            HandleDbFileOnWP8(databaseName, filepath);
#elif UNITY_WINRT
            HandleDbFileOnWindowsRuntime(databaseName, filepath);
#else
            HandleDbFileOnUndetectedPlatform(databaseName, filepath);
#endif
            GRUDebugger.LogSuccess(MessageConfig.Debugging.Success.SUCCESS_MESSAGE_DB_WRITTEN);
        }

        var dbFilePath = filepath;
#endif
            Debug.Log($">>db_file_path<< {dbFilePath}");
            _connection = new SQLiteConnection(dbFilePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            GRUDebugger.LogSuccess(MessageConfig.Debugging.Success.SUCCESS_MESSAGE_DB_CONNECTION_ESTABLISHED + dbFilePath);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds the path to <i>StreamingAssets</i> folder in android, from there it copies the db file
        /// named <i>databaseName</i> and then finally pastes it in <i>filepath</i>.
        /// </summary>
        /// <param name="databaseName">name of the database to look for in Android version of StreamingAssets path</param>
        /// <param name="filepath">think of destination path - a path to which database should get copied to</param>
        private void HandleDbFileOnAndroid(string databaseName, string filepath)
        {
            string streamingAssetsFolderInAndroid = $"jar:file://{Application.dataPath}!/assets/{databaseName}";
            var loadDb = new WWW(streamingAssetsFolderInAndroid); 
            while (!loadDb.isDone) { }
            File.WriteAllBytes(filepath, loadDb.bytes);
        }

        /// <summary>
        /// Finds the path to <i>StreamingAssets</i> folder in iOS, from there it copies the db file
        /// named <i>databaseName</i> and then finally pastes it in <i>filepath</i>.
        /// </summary>
        /// <param name="databaseName">name of the database to look for in iOS version of StreamingAssets path</param>
        /// <param name="filepath">think of destination path - a path to which database should get copied to</param>
        private void HandleDbFileOniOS(string databaseName, string filepath)
        {
            string streamingAssetsFolderIniOS = $"{Application.dataPath}/Raw/{databaseName}";
            var loadDb = streamingAssetsFolderIniOS;
            File.Copy(loadDb, filepath);
        }

        /// <summary>
        /// Finds the path to <i>StreamingAssets</i> folder in wp8, from there it copies the db file
        /// named <i>databaseName</i> and then finally pastes it in <i>filepath</i>.
        /// </summary>
        /// <param name="databaseName">name of the database to look for in wp8 version of StreamingAssets path</param>
        /// <param name="filepath">think of destination path - a path to which database should get copied to</param>
        private void HandleDbFileOnWP8(string databaseName, string filepath)
        {
            string streamingAssetsFolderInWP8 = $"{Application.dataPath}/StreamingAssets/{databaseName}";
            var loadDb = streamingAssetsFolderInWP8;
            File.Copy(loadDb, filepath);
        }

        /// <summary>
        /// Finds the path to <i>StreamingAssets</i> folder in windows runtime, from there it copies the db file
        /// named <i>databaseName</i> and then finally pastes it in <i>filepath</i>.
        /// </summary>
        /// <param name="databaseName">name of the database to look for in windows runtime version of StreamingAssets path</param>
        /// <param name="filepath">think of destination path - a path to which database should get copied to</param>
        private void HandleDbFileOnWindowsRuntime(string databaseName, string filepath)
        {
            string streamingAssetsFolderInWinRT = $"{Application.dataPath}/StreamingAssets/{databaseName}";
            var loadDb = streamingAssetsFolderInWinRT;
            File.Copy(loadDb, filepath);
        }

        /// <summary>
        /// Finds the path to <i>StreamingAssets</i> folder in undetected platform, from there it copies the db file
        /// named <i>databaseName</i> and then finally pastes it in <i>filepath</i>.
        /// </summary>
        /// <param name="databaseName">name of the database to look for in undetected platform version of StreamingAssets path</param>
        /// <param name="filepath">think of destination path - a path to which database should get copied to</param>
        private void HandleDbFileOnUndetectedPlatform(string databaseName, string filepath)
        {
            string streamingAssetsFolderInUndetectedPlatform = $"{Application.dataPath}/StreamingAssets/{databaseName}";
            var loadDb = streamingAssetsFolderInUndetectedPlatform;
            File.Copy(loadDb, filepath);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Call <b>only after successfull</b> <i><see cref="DataConnector"/> </i> instantiation.
        /// <br/><br/>
        /// This creates a list of tables in the database that's in a path
        /// which was specified in an above-mentioned instantiation.
        /// Class names will correspond to table names. 
        /// <br/>
        /// E.G. <i>Book.cs</i>
        /// will get mapped to <i>Book</i> table, <i>Author.cs</i> to <i>Author</i> table, etc.
        /// <br/> <br/>
        /// Class' property names will correspond to column names and class' property types
        /// will correspond to column types in the database.
        /// <br/>
        /// E.G. <i>Title (string)</i> property of <i>Book </i>class will get mapped to <i>Title (varchar)</i> column in the <i>Book</i> table in the database.
        /// <br/>
        /// Similarly, say <i>DateOfBirth (datetime)</i> in <i>Author</i> class, will get mapped to <i>DateOfBirth (datetime)</i> column in the <i>Author</i> table in the database
        /// </summary>
        public void CreateDbTables(List<EntityBase<int>> tables)
        {
            if (tables == null || !tables.Any())
            {
                GRUDebugger.LogError
                (
                    MessageConfig.Debugging.Error.ERROR_WHILE_CREATING_DB_TABLES
                );
            }

            for (int i = 0; i < tables.Count; i++)
            {
                var table = tables[i].GetType();
                _connection.CreateTable(table);
            }
        }

        public SQLiteConnection GetConnection()
        {
            return _connection;
        }

        #endregion
    }
}