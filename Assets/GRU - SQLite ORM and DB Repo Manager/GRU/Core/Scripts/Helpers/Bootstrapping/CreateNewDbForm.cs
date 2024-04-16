#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Database.Repository;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.General;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Debugging;
using SpatiumInteractive.Libraries.Unity.Platform.CustomEditor.Window;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Enums;
using UnityEngine.SceneManagement;

namespace SpatiumInteractive.Libraries.Unity.GRU.Helpers.Bootstrapping
{
    public class CreateNewDbForm : GRUEditorWindowBase
    {
        #region Private Fields

        private Button _createDatabaseBtn;
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

        #endregion

        #region Editor Fields

        [SerializeField]
        private string databaseName = GeneralConfig.Defaults.DATABASE_NAME;

        [SerializeField]
        private string databasePath = GeneralConfig.Defaults.DATABASE_PATH;

        #endregion

        #region Core Methods

        [MenuItem(GeneralConfig.Code.Attributes.MenuItem.GRU_NEW_DATABASE)]
        public static void ShowWindow()
        {
            var window = GetWindow<CreateNewDbForm>();

            float minWidth = window.ScreenWidth / 3f;
            float minHeight = window.ScreenHeight / 2f;
            var minWindowsSize = new Vector2(minWidth, minHeight);

            float initPosX = window.ScreenWidth / 6f;
            float initPosY = 0f;
            var initialPos = new Vector2(initPosX, initPosY);

            window.titleContent = new GUIContent(GeneralConfig.Editor.CreateNewDbFormWindow.TAB_TITLE);
            window.minSize = minWindowsSize;
            window.position = new Rect(initialPos, minWindowsSize);
        }

        private void CreateGUI()
        {
            var GRULogoImg = this.GetImageForSprite(GeneralConfig.DynamicCode.BASE_EDITOR_HEADER_IMG_PATH);
            var TablesScannedImg = this.GetImageForSprite(GeneralConfig.DynamicCode.SCANNED_TABLE_ICO_IMG_PATH);

            float minSizeHeight = ScreenHeight / 5f;
            var twoVerticalPanes = this.AddNewVerticalSplitView(minSizeHeight);

            var topPane = twoVerticalPanes[0];
            var bottomPane = twoVerticalPanes[1];

            var tableNames = GetAllTableNames(false);
            List<object> allTables = GetAllDbTablesAsObjects();

            var isInDemo = SceneManager.GetActiveScene().IsGRUDemo();

            this
                .AddNewImageElement
                (
                    GRULogoImg,
                    topPane,
                    height: ScreenHeight / 2f
                )
                .AddNewScrollView
                (
                    bottomPane,
                    ref _bottomPaneMainScrollView
                )
                .AddNewInputElement
                (
                    nameof(databaseName),
                    OnDatabaseNameChange,
                    GeneralConfig.Editor.CreateNewDbFormWindow.Labels.DATABASE_NAME_LABEL,
                    _bottomPaneMainScrollView,
                    height: 40,
                    marginTop: 20
                )
                .AddNewInputElement
                (
                    nameof(databasePath),
                    label: GeneralConfig.Editor.CreateNewDbFormWindow.Labels.DATABASE_PATH_LABEL,
                    parent: _bottomPaneMainScrollView,
                    isEnabled: false,
                    marginTop: 3,
                    height: 40
                )
                .AddNewDropDown
                (
                    ref _assemblyChoicesDdl,
                    GeneralConfig.Editor.CreateNewDbFormWindow.Labels.ASSEMBLY_SELECTION_DDL_LABEL,
                    isInDemo ? _assemblyChoicesDemo : _assemblyChoicesProd,
                    GeneralConfig.Editor.CreateNewDbFormWindow.DropDowns.GET_ASSEMBLY_SELECTION_DDL_TOOLTIP(),
                    onClickCallback: OnAssemblyChoiceClick,
                    parent: _bottomPaneMainScrollView
                )
                .AddNewLabel
                (
                    GeneralConfig.Editor.CreateNewDbFormWindow.Labels.DATABASE_TABLES_LABEL,
                    color: Color.white,
                    _bottomPaneMainScrollView, 13,
                    marginTop: 20
                )
                .AddNewImageElement
                (
                    TablesScannedImg,
                    _bottomPaneMainScrollView,
                    height: 80,
                    paddingBottom: 20
                )
                .AddNewScrollableCheckboxList
                (
                    tableNames,
                    allTables,
                    OnTableInListCbxClick,
                    ref _tablesScrollViewCheckboxes,
                    _bottomPaneMainScrollView
                )
                .AddNewLabelIfConditionTrue
                (
                    GeneralConfig.Editor.CreateNewDbFormWindow.Labels.NO_TABLES_DETECTED_ERROR_MSG,
                    color: Color.red,
                    _bottomPaneMainScrollView,
                    fontSize: 14,
                    marginTop: 5,
                    condition: !tableNames.Any()
                )
                .AddNewButton
                (
                    ref _createDatabaseBtn,
                    Color.gray,
                    GeneralConfig.Editor.CreateNewDbFormWindow.Buttons.CREATE_DB_BUTTON_TEXT,
                    5,
                    LengthUnit.Percent,
                    OnCreateDatabaseBtnClick,
                    isEnabled: false
                )
                .BindAll();

            RefreshTablesListByAssembly(_assemblyChoicesDdl.value);
        }

        #endregion

        #region In-Editor Event Handlers

        private void OnDatabaseNameChange()
        {
            EnableOrDisableCreateButton();
        }

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
            EnableOrDisableCreateButton();
            return _tablesScrollViewCheckboxes;
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

            EnableOrDisableCreateButton();
        }

        private void OnCreateDatabaseBtnClick()
        {
            var selectedTables = GetSelectedTableNames(_tablesScrollViewCheckboxes);

            string dbContextName = string.Empty;
            List<string> filePathsGenerated = new List<string>();

            GRUDebugger.LogMessage(MessageConfig.Debugging.Info.CREATING_DB_CONTEXT, doItalic: true);
            var dbContextFilePath = FilesManager.CreateNewDbContext
            (
                databaseName,
                out dbContextName
            );
            filePathsGenerated.Add(dbContextFilePath);

            GRUDebugger.LogMessage(MessageConfig.Debugging.Info.CREATING_DB_REPO, doItalic: true);
            var repoFilePaths = FilesManager.CreateNewRepositories
            (
                databaseName,
                selectedTables,
                dbContextName
            );
            filePathsGenerated.AddRange(repoFilePaths);

            GRUDebugger.LogMessage(MessageConfig.Debugging.Info.CREATING_SQLITE_DB, doItalic: true);
            _selectedTables = GetSelectedTables(_tablesScrollViewCheckboxes);
            FilesManager.CreateNewOrUpdateExistingSQLiteDatabase
            (
                databaseName,
                databasePath,
                _selectedTables
            );
            string dbFilePath = $"{databasePath.ToDatabasePath()}{databaseName.ToDatabaseName()}";
            filePathsGenerated.Add(dbFilePath);

            string dbHandlerGOName = GetNewDbHandlerGOName(databaseName);
            FilesManager.CreateDbConfig
            (
                databaseName,
                databasePath,
                selectedTables,
                dbHandlerGOName,
                dbContextName
            );

            AddDbHandlerGameObjectToScene(dbHandlerGOName);

            string filesCreated = string.Join("\n", filePathsGenerated);
            EditorUtility.DisplayDialog
            (
                MessageConfig.Generic.SUCCESS,
                $"GRU had to create these files in your Unity project: \n \n {filesCreated}",
                MessageConfig.Generic.OK
            );

            EnableOrDisableCreateButton();
        }

        #endregion

        #region Private Methods

        private GameObject AddDbHandlerGameObjectToScene(string goName)
        {
            GameObject gruInstance = GameObject.Find("GRU");
            if (gruInstance == null)
            {
                gruInstance = new GameObject("GRU");
            }

            var dbHandlerGO = new GameObject(goName, typeof(DBHandler));
            dbHandlerGO.transform.parent = gruInstance.transform;

            return dbHandlerGO;
        }

        private void EnableOrDisableCreateButton()
        {
            bool hasSetDatabaseName = !string.IsNullOrWhiteSpace(databaseName) &&
                                        databaseName.ToLower().Trim() != GeneralConfig.Defaults.DATABASE_NAME.ToLower().Trim();
            bool hasSetDatabasePath = !string.IsNullOrWhiteSpace(databasePath);

            bool hasSelectedTables = _tablesScrollViewCheckboxes.Any(t => t.value == true && t.visible);

            bool isDbNameValid = databaseName.IsValidDatabaseName();

            var existingDbMetaDataForName = FilesManager.GetDbConfigEntry(databaseName);
            bool doesDbWithNameAlreadyExist = existingDbMetaDataForName != null;

            string tooltip = "";
            if (!hasSetDatabaseName)
            {
                tooltip = MessageConfig.Tooltips.BrokenRules.ENTER_DB_NAME;
            }
            else if (!isDbNameValid)
            {
                tooltip = MessageConfig.Tooltips.BrokenRules.ALPHA_NUMERIC_CHARS_ONLY;
            }
            else if (doesDbWithNameAlreadyExist)
            {
                tooltip = MessageConfig.Tooltips.BrokenRules.DB_WITH_NAME_ALREADY_EXISTS;
                EditorGUIUtility.PingObject(GameObject.Find(existingDbMetaDataForName.DbHandlerGOName));
            }
            else if (!hasSetDatabasePath)
            {
                tooltip = MessageConfig.Tooltips.BrokenRules.SELECT_PATH_ON_DISK_FOR_DB;
            }
            else if (!hasSelectedTables)
            {
                tooltip = MessageConfig.Tooltips.BrokenRules.SELECT_AT_LEAST_ONE_TABLE;
            }
            _createDatabaseBtn.tooltip = tooltip;

            bool canEnableCreateBtn = string.IsNullOrWhiteSpace(tooltip);
            _createDatabaseBtn.SetEnabled(canEnableCreateBtn);
        }

        #endregion

        #region Editor Validators

        [MenuItem(GeneralConfig.Code.Attributes.MenuItem.GRU_NEW_DATABASE, true)]
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
