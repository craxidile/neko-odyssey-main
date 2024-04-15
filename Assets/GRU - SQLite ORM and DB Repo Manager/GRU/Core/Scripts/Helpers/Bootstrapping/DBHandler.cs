#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using SQLite4Unity3d;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Debugging;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages;
using SpatiumInteractive.Libraries.Unity.GRU.Helpers.Bootstrapping;
using SpatiumInteractive.Libraries.Unity.Platform.CustomEditor.Inspector.Attributes;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using System.IO;
using System;

namespace Database.Repository
{
    /// <summary>
    /// Put in scene by GRU's db create editor.
    /// Responsible for establishing DB connections on startup.
    /// </summary>
    [ExecuteInEditMode]
    public class DBHandler : MonoBehaviour
    {
        private const bool ENABLE_FIELDS_IN_EDITOR = false;

        #region Inspector Fields

#if UNITY_EDITOR
        [ShowIf(ActionOnConditionFail.JustDisable, ConditionOperator.And, nameof(ENABLE_FIELDS_IN_EDITOR))]
#endif
        [Tooltip(MessageConfig.Tooltips.DbHandler.DB_NAME)]
        [SerializeField] private string _databaseName;

#if UNITY_EDITOR
        [ShowIf(ActionOnConditionFail.JustDisable, ConditionOperator.And, nameof(ENABLE_FIELDS_IN_EDITOR))]
#endif
        [Tooltip(MessageConfig.Tooltips.DbHandler.DB_PATH)]
        [SerializeField] private string _databasePath;

        #endregion

        #region Private Fields

        private DataConnector _dbBridge;

        private string _goUnchangableName;

        #endregion

        #region Private Props

        private bool isConnected
        {
            get
            {
                return _dbBridge != null;
            }
        }

        #endregion

        #region Lifecycle

        private void Awake()
        {
            ConnectWithDatabase(_databaseName, _databasePath);
        }

        private void Update()
        {
#if UNITY_EDITOR

            if (!Application.isPlaying && Application.isEditor)
            {
                if (string.IsNullOrWhiteSpace(_goUnchangableName))
                {
                    var dbMetaData = FilesManager.GetDbConfigEntry(this);
                    if (dbMetaData == null)
                    {
                        GRUDebugger.LogWarning
                        (
                            MessageConfig.Debugging.Warning.GET_DB_HANDLER_VALIDATION_FAILED_WARNING(gameObject.name)
                        );
                        return;
                    }

                    _goUnchangableName = dbMetaData.DbHandlerGOName;
                }
                else
                {
                    if (gameObject.name != _goUnchangableName)
                    {
                        gameObject.name = _goUnchangableName;
                        EditorUtility.DisplayDialog
                        (
                            "Renaming Forbidden",
                            MessageConfig.Debugging.Warning.GET_DB_HANDLER_RENAME_FORBIDDEN_WARNING(),
                            MessageConfig.Generic.OK
                        );
                        Selection.activeGameObject = this.gameObject;
                    }
                }
            }

#endif
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// If not already connected, connects to a database using parameters passed in.
        /// If there's no such db yet, it will create it.
        /// </summary>
        public void ConnectWithDatabase
        (
            string databaseName,
            string databasePath,
            List<EntityBase<int>> tables = null
        )
        {
            if (!isConnected)
            {
                try
                {
                    if (_dbBridge == null)
                    {
                        _dbBridge = new DataConnector(databaseName, databasePath);
                        if (tables != null && tables.Any())
                        {
                            _dbBridge.CreateDbTables(tables);
                        }
                    }
                }
                catch
                {
                    var dbFilePath = $"{databasePath}/{databaseName}";
                    if (!File.Exists(dbFilePath))
                    {
                        GRUDebugger.LogWarning(MessageConfig.Debugging.Warning.WARNG_WHILE_CREATING_DB);
                    }
                    else
                    {
                        GRUDebugger.LogWarning
                        (
                            MessageConfig.Debugging.Warning.GET_ISSUE_WHILE_CONNECTING_TO_EXISTING_DB_WARN(databaseName, this.gameObject.name)
                        );
                    }
                }
            }
        }

        public string GetDatabaseName()
        {
            return _databaseName;
        }

        public SQLiteConnection GetConnection()
        {
            return _dbBridge.GetConnection();
        }

        #endregion

        #region Editor Checks

#if UNITY_EDITOR

        private void OnValidate()
        {
            var dbMetaData = FilesManager.GetDbConfigEntry(this);
            if (dbMetaData != null)
            {
                _databaseName = dbMetaData.DbName;
                _databasePath = dbMetaData.DbPath.ToAbsoluteDatabasePath();
                _goUnchangableName = dbMetaData.DbHandlerGOName;
            }
        }

        private void OnDestroy()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                if (Event.current != null && Event.current.commandName.Contains("Delete"))
                {
                    bool shouldDelete = EditorUtility.DisplayDialog(
                        MessageConfig.Generic.WARNING,
                        $"If you delete Db handler {this.name} from the scene, then you won't be able to communicate to {this._databaseName} database. Are you sure you want to delete it anyway?",
                        "Yes, I know what I'm doing",
                        MessageConfig.Generic.CANCEL
                    );

                    if (!shouldDelete)
                    {
                        var GRUParent = GameObject.Find("GRU").transform;
                        var parent = GRUParent ?? this.transform.parent;
                        var newDbHandlerInstance = Instantiate(this, parent);
                        newDbHandlerInstance.gameObject.SetActive(true);
                        newDbHandlerInstance.gameObject.name = this.name;
                    }
                }
            }
        }

#endif

        #endregion
    }
}