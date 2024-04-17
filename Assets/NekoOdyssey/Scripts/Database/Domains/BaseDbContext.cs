using System;
using System.IO;
using Database.Repository;
using SpatiumInteractive.Libraries.Unity.GRU.Contracts;
using SQLite4Unity3d;
using UnityEngine;
#if UNITY_SWITCH && !UNITY_EDITOR
using NekoOdyssey.Scripts.IO.FileStore.Nintendo;
#endif

namespace NekoOdyssey.Scripts.Database.Domains
{
    public abstract class BaseDbContext : IDbContext<SQLiteConnection>, IDisposable
    {
        private readonly string _databaseName;
        private readonly string _databasePath;
        private readonly DbContextOptions _options;

        public SQLiteConnection Context { get; }

        protected BaseDbContext(string databaseName, DbContextOptions options)
        {
            _databaseName = databaseName;
            _databasePath = Application.streamingAssetsPath;
            Debug.Log($">>database_path<< {_databasePath}");
            Debug.Log($">>database_name<< {_databaseName}");
            _options = options;
            CopyDatabase();
            var connector = new DataConnector(_databaseName, _databasePath);
            Context = connector.GetConnection();
        }

        private void CopyDatabase()
        {
            if (!_options.CopyRequired) return;
#if UNITY_SWITCH && !UNITY_EDITOR
            var fileName = $"{_databaseName}.db";
            var filePath = $"{_databasePath}/{fileName}";
            Debug.Log($">>file_path<< {filePath}");
            // if (NintendoFileHandler.Exists(_databaseName)) return;
            var dbData = File.ReadAllBytes(filePath);
            NintendoFileHandler.SaveRaw(dbData, fileName);
#endif
        }

        private void CommitDatabase()
        {
            if (_options.ReadOnly) return;
#if UNITY_SWITCH && !UNITY_EDITOR
            NintendoFileHandler.Commit();
#endif
        }

        public void Dispose()
        {
            Debug.Log($">>dispose_connection<< {_databaseName}");
            Context.Dispose();
            CommitDatabase();
        }
    }
}