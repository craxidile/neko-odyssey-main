#if UNITY_EDITOR
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using SpatiumInteractive.Libraries.Unity.GRU.Helpers.Bootstrapping;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.General;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages;

namespace SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Editor
{
    public static class DbFileMenuItem
    {
        #region Private Fields

        private static DbMetaData _lastSelectedDbMetaData;

        #endregion

        #region Private Methods

        private static bool ValidateIsDatabaseFileSelected()
        {
            string fileSelected = Selection.activeObject.name;
            var dbMetaData = FilesManager.GetDbConfigEntry(fileSelected);
            bool isRecognizedDb = dbMetaData != null;
            _lastSelectedDbMetaData = dbMetaData;
            return isRecognizedDb;
        }

        #endregion

        #region Editor Right-Click-Menu Items' Methods

        #region Menu-Item-Ping

        [MenuItem("Assets/GRU/Ping", true)]
        private static bool PingInTheScene_OnValidate()
        {
            bool isRecognized = ValidateIsDatabaseFileSelected();
            return isRecognized;
        }

        [MenuItem("Assets/GRU/Ping")]
        private static void PingInTheScene()
        {
            EditorGUIUtility.PingObject(GameObject.Find(_lastSelectedDbMetaData.DbHandlerGOName));
        }

        #endregion

        #region Menu-Item-Edit-In-Unity

        [MenuItem("Assets/GRU/Edit in Unity", true)]
        private static bool EditSelectedDatabaseFileInUnity_OnValidate()
        {
            bool isRecognized = ValidateIsDatabaseFileSelected();
            return isRecognized;
        }

        [MenuItem("Assets/GRU/Edit in Unity")]
        private static void EditSelectedDatabaseFileInUnity()
        {
            UpdateOrDeleteDbForm.ShowWindow();
            Button databaseUpdateBtn = UpdateOrDeleteDbForm.GetButtonForDatabase(_lastSelectedDbMetaData.DbName);
            using (var e = new MouseUpEvent() { target = databaseUpdateBtn })
            {
                databaseUpdateBtn.SendEvent(e);
            }
        }

        #endregion

        #region Menu-Item-Open-In-DbSchema

        [MenuItem("Assets/GRU/Open in DbSchema", true)]
        private static bool ViewSelectedDatabaseFileInDbSchemaProgram_OnValidate()
        {
            bool isRecognized = ValidateIsDatabaseFileSelected();
            return isRecognized;
        }

        [MenuItem("Assets/GRU/Open in DbSchema")]
        private static void ViewSelectedDatabaseFileInDbSchemaProgram()
        {
            try
            {
                var dbFilePath = _lastSelectedDbMetaData.GetDbFilePathForEditor();
                Process.Start(GeneralConfig.Editor.RightClickMenu.DbSchema.PROCESS_EXE, dbFilePath);
            }
            catch
            {
                var shouldDownload = EditorUtility.DisplayDialog
                (
                    "Error opening DbSchema.exe",
                    MessageConfig.RightClickMenu.DbSchema.AUTO_RUNNING_FAILED_WARNING,
                    "Download DbSchema",
                    MessageConfig.Generic.CLOSE
                );
                if (shouldDownload)
                {
                    Application.OpenURL("https://dbschema.com/download.html");
                }
            }
        }

        #endregion

        #region Menu-Item-Open-In-SQLite-Browser

        [MenuItem("Assets/GRU/Open in SQLite browser", true)]
        private static bool ViewSelectedDatabaseFileInSqliteBrowserProgram_OnValidate()
        {
            bool isRecognized = ValidateIsDatabaseFileSelected();
            return isRecognized;
        }

        [MenuItem("Assets/GRU/Open in SQLite browser")]
        private static void ViewSelectedDatabaseFileInSqliteBrowserProgram()
        {
            try
            {
                var dbFilePath = _lastSelectedDbMetaData.GetDbFilePathForEditor();
                Process.Start(GeneralConfig.Editor.RightClickMenu.SQLiteBrowser.PROCESS_EXE, dbFilePath);
            }
            catch
            {
                var shouldDownload = EditorUtility.DisplayDialog
                (
                    "Error opening DB Browser for SQLite.exe",
                    MessageConfig.RightClickMenu.SQLiteBrowser.AUTO_RUNNING_FAILED_WARNING,
                    "Download DB Browser",
                    MessageConfig.Generic.CLOSE
                );
                if (shouldDownload)
                {
                    Application.OpenURL("https://sqlitebrowser.org/dl/");
                }
            }
        }

        #endregion

        #endregion
    }
}

#endif
