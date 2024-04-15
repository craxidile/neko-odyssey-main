#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.General;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Debugging;
using Database.Repository;
using SpatiumInteractive.Libraries.Unity.Platform.CustomEditor.Window;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Enums;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages;
using UnityEngine.SceneManagement;

namespace SpatiumInteractive.Libraries.Unity.GRU.Helpers.Bootstrapping
{
    public class UpdateOrDeleteDbForm : GRUEditorWindowBase
    {
        #region Private Fields

        private Button _deleteDatabaseBtn;
        private Button _pingDatabaseBtn;
        private Button _updateDatabaseBtn;
        private ScrollView _bottomPaneMainScrollView;
        private List<EntityBase<int>> _selectedTables = new List<EntityBase<int>>();
        private List<Toggle> _tablesScrollViewCheckboxes = new List<Toggle>();
        private DropdownField _assemblyChoicesDdl;
        private List<string> _assemblyChoicesProd = new List<string>()
        {
            "Everywhere",
            "Outside GRU",
            "Inside GRU"
        };
        private List<string> _assemblyChoicesDemo = new List<string>()
        {
            "Inside GRU"
        };
        
        private List<Button> _databasesScrollViewButtons = new List<Button>();
        private readonly Color _dbNameButtonClickedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
        private readonly Color _deleteDbButtonColor = new Color(0.49f, 0.4f, 0.41f, 1f);
        private string _currentlySelectedDatabaseName;
        private static UpdateOrDeleteDbForm _instance;

        #endregion

        #region Core Methods

        [MenuItem(GeneralConfig.Code.Attributes.MenuItem.GRU_EXISTING_DATABASES)]
        public static void ShowWindow()
        {
            var window = GetWindow<UpdateOrDeleteDbForm>();

            float minWidth = window.ScreenWidth / 3f;
            float minHeight = window.ScreenHeight / 2f;
            var minWindowsSize = new Vector2(minWidth, minHeight);

            float initPosX = window.ScreenWidth / 6f;
            float initPosY = 0f;
            var initialPos = new Vector2(initPosX, initPosY);

            window.titleContent = new GUIContent(GeneralConfig.Editor.UpdateOrDeleteDbFormWindow.TAB_TITLE);
            window.minSize = minWindowsSize;
            window.position = new Rect(initialPos, minWindowsSize);

            _instance = window;
        }

        private void CreateGUI()
        {
            var TablesScannedImg = this.GetImageForSprite
            (
                GeneralConfig.DynamicCode.SCANNED_TABLE_ICO_IMG_PATH
            );

            float minSizeHeight = ScreenHeight / 4.5f;
            var twoVerticalPanes = this.AddNewVerticalSplitView(minSizeHeight);

            var topPane = twoVerticalPanes[0];
            var bottomPane = twoVerticalPanes[1];

            var currentlyExistingDatabaseNames = GetExistingDatabasesNames();
            var tableNames = GetAllTableNames(false);
            List<object> allTables = GetAllDbTablesAsObjects();

            var isInDemo = SceneManager.GetActiveScene().IsGRUDemo();

            this
                .AddNewLabel
                (
                    GeneralConfig.Editor.UpdateOrDeleteDbFormWindow.Labels.DATABASE_TABLES_LIST, 
                    color: Color.white, 
                    topPane, 13, 
                    marginTop: 20
                )
                .AddNewLabelIfConditionTrue
                (
                    GeneralConfig.Editor.UpdateOrDeleteDbFormWindow.Labels.NO_DATABASES_DETECTED_ERROR_MSG, 
                    color: Color.red, 
                    topPane, 13, 
                    marginTop: 20, 
                    condition: !currentlyExistingDatabaseNames.Any()
                )
                .AddNewScrollView
                (
                    bottomPane, ref _bottomPaneMainScrollView
                )
                .AddNewScrollableButtonsList
                (
                    currentlyExistingDatabaseNames, 
                    null, 
                    OnDatabaseInListBtnClick, 
                    ref _databasesScrollViewButtons, 
                    topPane, 
                    defaultIsEnabled: true, 35, 
                    listHeight: 200
                )
                .AddNewDropDown
                (
                    ref _assemblyChoicesDdl, 
                    GeneralConfig.Editor.UpdateOrDeleteDbFormWindow.Labels.ASSEMBLY_SELECTION_DDL_LABEL, 
                    isInDemo ? _assemblyChoicesDemo : _assemblyChoicesProd,
                    GeneralConfig.Editor.CreateNewDbFormWindow.DropDowns.GET_ASSEMBLY_SELECTION_DDL_TOOLTIP(),
                    onClickCallback: OnAssemblyChoiceClick, 
                    parent: _bottomPaneMainScrollView
                )
                .AddNewLabel
                (
                    GeneralConfig.Editor.UpdateOrDeleteDbFormWindow.Labels.UPDATE_TABLES, 
                    color: Color.white, 
                    _bottomPaneMainScrollView, 
                    13, 
                    marginTop: 20
                )
                .AddNewImageElement
                (
                    TablesScannedImg, 
                    _bottomPaneMainScrollView, 
                    height: 80, 
                    "Select a database to update first", 
                    paddingBottom: 20
                )
                .AddNewScrollableCheckboxList
                (
                    tableNames, 
                    allTables, 
                    OnTableInListCbxClick, 
                    ref _tablesScrollViewCheckboxes, 
                    _bottomPaneMainScrollView, 
                    defaultIsEnabled: false
                )
                .AddNewLabelIfConditionTrue
                (
                    GeneralConfig.Editor.UpdateOrDeleteDbFormWindow.Labels.NO_TABLES_DETECTED_ERROR_MSG, 
                    color: Color.red, 
                    _bottomPaneMainScrollView, 
                    fontSize: 14, 
                    marginTop: 5, 
                    condition: !tableNames.Any()
                )
                .AddNewButton
                (
                    ref _deleteDatabaseBtn, 
                    _deleteDbButtonColor, 
                    GeneralConfig.Editor.UpdateOrDeleteDbFormWindow.Buttons.DELETE_DB_BUTTON_TEXT, 
                    5, 
                    LengthUnit.Percent, 
                    OnDeleteDatabaseBtnClick, 
                    isEnabled: false
                )
                .AddNewButton
                (
                    ref _pingDatabaseBtn, 
                    Color.gray, 
                    GeneralConfig.Editor.UpdateOrDeleteDbFormWindow.Buttons.PING_DB_BUTTON_TEXT, 
                    5, 
                    LengthUnit.Percent, 
                    OnPingDatabaseBtnClick, 
                    isEnabled: false
                )
                .AddNewButton
                (
                    ref _updateDatabaseBtn, 
                    Color.gray, 
                    GeneralConfig.Editor.UpdateOrDeleteDbFormWindow.Buttons.UPDATE_DB_BUTTON_TEXT, 
                    5, 
                    LengthUnit.Percent, 
                    OnUpdateDatabaseBtnClick, 
                    isEnabled: false
                )
                .BindAll();

            RefreshTablesListByAssembly(_assemblyChoicesDdl.value);
        }

        #endregion

        #region API

        public static void DeleteDbProcedure(string dbName)
        {
            try
            {
                var dbMetaData = FilesManager.GetDbConfigEntry(dbName);
                FilesManager.DeleteDatabase(dbMetaData);
                FilesManager.DeleteDbContext(dbMetaData);
                FilesManager.RemoveEntryFromDbConfig(dbMetaData);
                DestroyDbHandlerGO(dbMetaData.DbName);
                EditorUtility.DisplayDialog
                (
                    "Delete Successfull",
                    MessageConfig.UpdateOrDeleteDBWindow.Success.GET_DB_DELETE_PROC_DONE_SUCCESS(),
                    MessageConfig.Generic.CLOSE
                );
            }
            catch
            {
                EditorUtility.DisplayDialog
                (
                    "Delete Failed",
                    "One or more delete actions have failed. Check the console for more details",
                    MessageConfig.Generic.CLOSE
                );
            }
        }

        #endregion

        #region In-Editor Event Handlers

        private List<Toggle> OnTableInListCbxClick(Toggle cbxEvent)
        {
            for (int i = 0; i < _tablesScrollViewCheckboxes.Count; i++)
            {
                var tableCbx = _tablesScrollViewCheckboxes[i];
                if (tableCbx.label == cbxEvent.label)
                {
                    tableCbx.value = cbxEvent.value;
                }
            }

            EnableOrDisableUpdateButton();

            return _tablesScrollViewCheckboxes;
        }

        private void OnDatabaseInListBtnClick(Button btnEvent)
        {
            for (int i = 0; i < _databasesScrollViewButtons.Count; i++)
            {
                var databaseBtn = _databasesScrollViewButtons[i];
                if (databaseBtn.text == btnEvent.text)
                {
                    databaseBtn.style.backgroundColor = _dbNameButtonClickedColor;
                    databaseBtn.style.color = Color.black;
                    RenderTableListViewForDatabase(btnEvent.text);
                    EnableActionButtons();
                    _currentlySelectedDatabaseName = databaseBtn.text;
                }
                else
                {
                    databaseBtn.style.backgroundColor = Color.grey;
                    databaseBtn.style.color = Color.white;
                }
            }
            EnableOrDisableUpdateButton();
        }

        private void OnAssemblyChoiceClick(DropdownField ddlEvent)
        {
            RefreshTablesListByAssembly(ddlEvent.text);
        }

        private void RefreshTablesListByAssembly(string assemblyName)
        {
            AssemblySelection selectedAssembly = assemblyName.GetAssemblySelected();
            List<string> selectedAssemblyTables = GetAllTableNames(false, selectedAssembly);
            foreach (var tableCbx in _tablesScrollViewCheckboxes)
            {
                bool isVisible = selectedAssemblyTables.Any(x => x == tableCbx.label);
                tableCbx.visible = isVisible;
            }

            EnableOrDisableUpdateButton();
        }

        private void OnUpdateDatabaseBtnClick()
        {
            var dbMetaData = FilesManager.GetDbConfigEntry(_currentlySelectedDatabaseName);
            _selectedTables = GetSelectedTables(_tablesScrollViewCheckboxes);
            var selectedTablesNames = GetTableNames(_selectedTables);
            var newTables = GetNewTables(dbMetaData.TableNames, selectedTablesNames);
            dbMetaData.TableNames = selectedTablesNames;

            try
            {
                GRUDebugger.LogMessage(MessageConfig.Debugging.Info.CREATING_DB_REPO, doItalic: true);
                var repoFilePaths = FilesManager.CreateNewRepositories
                (
                    dbMetaData.DbName,
                    newTables,
                    dbMetaData.DbContextName
                );
                string filesCreated = string.Join("\n", repoFilePaths);
                EditorUtility.DisplayDialog
                (
                    MessageConfig.Generic.SUCCESS,
                    $"GRU had to create these files in your Unity project: \n \n {filesCreated}",
                    MessageConfig.Generic.OK
                );

                FilesManager.CreateNewOrUpdateExistingSQLiteDatabase
                (
                    dbMetaData.DbName,
                    dbMetaData.DbPath,
                    _selectedTables
                );
                FilesManager.UpdateEntryInDbConfig(dbMetaData);
                EditorUtility.DisplayDialog
                (
                    MessageConfig.UpdateOrDeleteDBWindow.UPDATE_DATABSE_DIALOG_TITLE,
                    $"Database {dbMetaData.DbName} has been updated: tables and columns have been successfully altered",
                    MessageConfig.Generic.CLOSE
                );
            }
            catch
            {
                EditorUtility.DisplayDialog
                (
                    "Update Failed",
                    MessageConfig.UpdateOrDeleteDBWindow.Error.GET_DB_FAILED_TO_UPDATE_ERR(dbMetaData.DbName),
                    MessageConfig.Generic.CLOSE
                );
            }

            RenderTableListViewForDatabase(_currentlySelectedDatabaseName);
        }

        private void OnDeleteDatabaseBtnClick()
        {
            if (CheckForHookableProgramsRunning())
            {
                return;
            }

            bool isDeleteConfirmed = EditorUtility.DisplayDialog
            (
                MessageConfig.UpdateOrDeleteDBWindow.DELETE_DATABSE_DIALOG_TITLE,
                MessageConfig.UpdateOrDeleteDBWindow.Warning.GET_DELETE_DB_HEADSUP_WARNING(),
                MessageConfig.UpdateOrDeleteDBWindow.DELETE_DATABSE_DIALOG_CONFIRM,
                MessageConfig.Generic.CANCEL
            );
            if (isDeleteConfirmed)
            {
                DeleteDbProcedure(_currentlySelectedDatabaseName);
            }
        }

 
        /// <summary>
        /// Highlights (pings) selected database's .db file
        /// in file explorer and its db handler in the hierarchy
        /// </summary>
        private void OnPingDatabaseBtnClick()
        {
            var dbMetaData = FilesManager.GetDbConfigEntry(_currentlySelectedDatabaseName);
            var dbHandlerGO = GameObject.Find(dbMetaData.DbHandlerGOName);
            EditorGUIUtility.PingObject(dbHandlerGO);

            string dbFilePath = dbMetaData.GetDbFilePathForEditor();
            EditorUtility.RevealInFinder(dbFilePath);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Call when db in list gets clicked.
        /// This re-renders table list view for that db.
        /// </summary>
        /// <param name="databaseName"></param>
        private void RenderTableListViewForDatabase(string databaseName)
        {
            var dbTables = GetAllTableNames(databaseName);
            foreach (var tableCbx in _tablesScrollViewCheckboxes)
            {
                bool isTableAlreadyPartOfDb = dbTables.Any(x => x == tableCbx.label.ToRawTableName());
                if (isTableAlreadyPartOfDb)
                {
                    tableCbx.SetEnabled(false);
                    tableCbx.value = true;
                }
                else
                {
                    tableCbx.SetEnabled(true);
                    tableCbx.value = false;
                }
            }
        }

        private void EnableActionButtons()
        {
            _deleteDatabaseBtn.SetEnabled(true);
            _pingDatabaseBtn.SetEnabled(true);
            _updateDatabaseBtn.SetEnabled(true);
        }

        private void EnableOrDisableUpdateButton()
        {
            bool hasSelectedDb = !string.IsNullOrWhiteSpace(_currentlySelectedDatabaseName);
            bool hasSelectedTables = _tablesScrollViewCheckboxes.Any(t => t.value == true && t.visible);

            string tooltip = "";
            if (!hasSelectedDb)
            {
                tooltip = MessageConfig.Tooltips.BrokenRules.CANT_UPDATE_WITH_NO_DB_SELECTED;
            }
            else if (!hasSelectedTables)
            {
                tooltip = MessageConfig.Tooltips.BrokenRules.CANT_UPDATE_WITH_NO_TABLE_SELECTED;
            }
            _updateDatabaseBtn.tooltip = tooltip;

            bool canEnableUpdateBtn = string.IsNullOrWhiteSpace(tooltip);
            _updateDatabaseBtn.SetEnabled(canEnableUpdateBtn);
        }

        private static void DestroyDbHandlerGO(string databaseName)
        {
            var allDbHandlersGOInTheScene = GameObject.FindObjectsOfType<DBHandler>();
            if (allDbHandlersGOInTheScene != null)
            {
                var dbHandler = allDbHandlersGOInTheScene.SingleOrDefault(x => x.GetDatabaseName() == databaseName);
                if (dbHandler != null)
                {
                    var dbHandlerGO = dbHandler.gameObject;
                    DestroyImmediate(dbHandlerGO);
                }
                else
                {
                    GRUDebugger.LogWarning
                    (
                        $"Warning: Couldn't delete db handler scene instance for datatabse: {databaseName} because its corresponding dbhandler game object instance was not found in the scene"
                    );
                }
            }
        }

        #endregion

        #region Public Methods

        public static Button GetButtonForDatabase(string dbName)
        {
            var button = _instance._databasesScrollViewButtons.Where(x => x.text == dbName).FirstOrDefault();
            return button;
        }

        #endregion

        #region Editor Validators

        [MenuItem(GeneralConfig.Code.Attributes.MenuItem.GRU_EXISTING_DATABASES, true)]
        public static bool OnShowWindow()
        {
            var packageConfig = FilesManager.GetPackageConfigData(true);
            bool canShowWindow = (packageConfig != null) && packageConfig.HasSetPaths;
            return canShowWindow;
        }

        #endregion
    }
}

#endif

