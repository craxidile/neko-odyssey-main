#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.General;
using SpatiumInteractive.Libraries.Unity.Platform.CustomEditor.Window;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Debugging;
using UnityEngine.SceneManagement;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using System.IO;

namespace SpatiumInteractive.Libraries.Unity.GRU.Helpers.Bootstrapping
{
    public class ConfigureGRUForm : GRUEditorWindowBase
    {
        #region Private Fields

        private Button _configurGRUBtn;
        private ScrollView _bottomPaneMainScrollView;

        #endregion

        #region Editor Fields

        [SerializeField]
        private string gruPackageFolderLocation = GeneralConfig.DynamicCode.PACKAGE_PATH;

        [SerializeField]
        private string gruIntegrationFolderLocation = GeneralConfig.DynamicCode.INTEGRATION_FOLDER_PATH;

        #endregion

        #region Core Methods

        [MenuItem(GeneralConfig.Code.Attributes.MenuItem.GRU_CONFIGURE)]
        public static void ShowWindow()
        {
            var window = GetWindow<ConfigureGRUForm>();
            var minWindowsSize = new Vector2
            (
                GeneralConfig.Editor.ConfigureFormWindow.MIN_SIZE_WIDTH,
                GeneralConfig.Editor.ConfigureFormWindow.MIN_SIZE_HEIGHT
            );

            float initPosX = (Screen.width / 2) + (GeneralConfig.Editor.ConfigureFormWindow.MIN_SIZE_WIDTH / 2);
            float initPosY = Screen.height / 8f;
            var initialPos = new Vector2(initPosX, initPosY);

            window.titleContent = new GUIContent(GeneralConfig.Editor.ConfigureFormWindow.TAB_TITLE);
            window.minSize = minWindowsSize;
            window.position = new Rect(initialPos, minWindowsSize);
        }

        private void CreateGUI()
        {
            var GRULogoImg = this.GetImageForSprite(GeneralConfig.DynamicCode.CONFIGURE_EDITOR_IMG_PATH);

            var twoVerticalPanes = this.AddNewVerticalSplitView
            (
                GeneralConfig.Editor.ConfigureFormWindow.MIN_SIZE_HEIGHT / 2f
            );
            var topPane = twoVerticalPanes[0];
            var bottomPane = twoVerticalPanes[1];

            this
                .AddNewImageElement
                (
                    GRULogoImg,
                    topPane,
                    height: GeneralConfig.Editor.ConfigureFormWindow.MIN_SIZE_HEIGHT / 2
                )
                .AddNewScrollView
                (
                    bottomPane,
                    ref _bottomPaneMainScrollView
                )
                .AddNewInputElement
                (
                    nameof(gruPackageFolderLocation),
                    OnPackageFolderLocationChange,
                    GeneralConfig.Editor.ConfigureFormWindow.Labels.PACKAGE_FOLDER_LOCATION_LABEL,
                    _bottomPaneMainScrollView,
                    height: 40,
                    isEnabled: false,
                    marginTop: 10
                )
                .AddNewButton
                (
                    GeneralConfig.Editor.ConfigureFormWindow.Buttons.SET_PACKAGE_PATH,
                    25,
                    LengthUnit.Percent,
                    OnSetPackagePathBtnClick,
                    color: Color.gray,
                    parent: _bottomPaneMainScrollView
                )
                .AddNewInputElement
                (
                    nameof(gruIntegrationFolderLocation),
                    OnIntegrationFolderLocationChange,
                    GeneralConfig.Editor.ConfigureFormWindow.Labels.INTEGRATION_FOLDER_LOCATION_LABEL,
                    _bottomPaneMainScrollView,
                    height: 40,
                    isEnabled: false,
                    marginTop: 10
                )
                .AddNewButton
                (
                    GeneralConfig.Editor.ConfigureFormWindow.Buttons.SET_INTEGRATION_PATH,
                    25,
                    LengthUnit.Percent,
                    OnSetIntegrationPathBtnClick,
                    color: Color.gray,
                    parent: _bottomPaneMainScrollView
                )
                .AddNewButton
                (
                    ref _configurGRUBtn,
                    Color.gray,
                    GeneralConfig.Editor.ConfigureFormWindow.Buttons.CONFIGURE_CREATE_TEXT,
                    5,
                    LengthUnit.Percent,
                    OnConfigureBtnClick,
                    isEnabled: false
                )
                .BindAll();

            EnableOrDisableConfigureButton();

            var configData = GetPackageConfig(true);
            UpdateTextOfConfigureButton(configData);

            UpdatePathInputs();
        }

        #endregion

        #region In-Editor Event Handlers

        private void OnPackageFolderLocationChange()
        {

        }

        private void OnIntegrationFolderLocationChange()
        {

        }

        private void OnSetPackagePathBtnClick()
        {
            var pathSelected = EditorUtility.OpenFolderPanel
            (
                "Select the GRU Package Folder Location",
                "Assets",
                ""
            );
            if (!string.IsNullOrWhiteSpace(pathSelected))
            {
                if (pathSelected.Contains(GeneralConfig.Defaults.STREAMING_ASSETS_FOLDER))
                {
                    EditorUtility.DisplayDialog
                    (
                        MessageConfig.Generic.WARNING,
                        MessageConfig.GRUConfigureWindow.Warning.PACKAGE_FOLDER_LOCATION_IN_STREAMING_ASSETS,
                        MessageConfig.Generic.OK
                    );
                }
                else if (pathSelected.Contains(GeneralConfig.Defaults.ROOT_ASSETS_FOLDER))
                {
                    int index = pathSelected.IndexOf(GeneralConfig.Defaults.ROOT_ASSETS_FOLDER);
                    string pathFormatted = pathSelected.Substring(index + GeneralConfig.Defaults.ROOT_ASSETS_FOLDER.Length);
                    gruPackageFolderLocation = $"/{pathFormatted}";
                }
                else
                {
                    EditorUtility.DisplayDialog
                    (
                        MessageConfig.Generic.WARNING,
                        MessageConfig.GRUConfigureWindow.Warning.PACKAGE_FOLDER_LOCATION_OUTSIDE_ASSETS,
                        MessageConfig.Generic.OK
                    );
                }
            }
            EnableOrDisableConfigureButton();
        }

        private void OnSetIntegrationPathBtnClick()
        {
            var pathSelected = EditorUtility.OpenFolderPanel
            (
                "Select Integration Folder Location",
                "Assets",
                ""
            );

            if (!string.IsNullOrWhiteSpace(pathSelected))
            {
                if (pathSelected.Contains(GeneralConfig.Defaults.ROOT_ASSETS_FOLDER))
                {
                    int index = pathSelected.IndexOf(GeneralConfig.Defaults.ROOT_ASSETS_FOLDER);
                    string pathFormatted = $"/{pathSelected.Substring(index + GeneralConfig.Defaults.ROOT_ASSETS_FOLDER.Length)}";

                    bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                    if (!isInDemo)
                    {
                        var isIntegrationPathInsideGruRoot = pathFormatted.Contains(GeneralConfig.DynamicCode.PACKAGE_PATH);
                        if (isIntegrationPathInsideGruRoot)
                        {
                            EditorUtility.DisplayDialog
                            (
                                MessageConfig.Generic.WARNING,
                                MessageConfig.GRUConfigureWindow.Warning.PRODUCTION_INTEGRATION_FOLDER_INSIDE_GRU_ROOT_FOLDER,
                                MessageConfig.Generic.OK
                            );
                        }
                    }
                    else
                    {
                        var isIntegrationPathOutsideGruRoot = 
                            !pathFormatted.Contains(GeneralConfig.DynamicCode.PACKAGE_PATH) &&
                                !pathFormatted.Contains(gruPackageFolderLocation);
                        if (isIntegrationPathOutsideGruRoot)
                        {
                            EditorUtility.DisplayDialog
                            (
                                MessageConfig.Generic.WARNING,
                                MessageConfig.GRUConfigureWindow.Warning.DEMO_INTEGRATION_FOLDER_OUTSIDE_GRU_ROOT_FOLDER,
                                MessageConfig.Generic.OK
                            );
                        }
                    }

                    gruIntegrationFolderLocation = pathFormatted;
                }
                else
                {
                    EditorUtility.DisplayDialog
                    (
                        MessageConfig.Generic.WARNING,
                        MessageConfig.GRUConfigureWindow.Warning.INTEGRATION_FOLDER_LOCATION_OUTSIDE_ASSETS,
                        MessageConfig.Generic.OK
                     );
                }
            }
            EnableOrDisableConfigureButton();
        }

        private void OnConfigureBtnClick()
        {
            string configurationsDone = string.Empty;

            if (CheckForHookableProgramsRunning())
            {
                return;
            }

            var packageMetaData = new PackageMetaData
            (
                true,
                gruPackageFolderLocation,
                gruIntegrationFolderLocation
            );

            FilesManager.CreatePackageConfig(packageMetaData);
            configurationsDone += MessageConfig.GRUConfigureWindow.Success.GET_GRU_PACKAGE_CONFIG_GENERATED_SUCCESS();

            string currentPackageLocation = FilesManager.PackagePath;
            string newPackageLocation = $"{Application.dataPath}{gruPackageFolderLocation}";
            bool shouldRelocatePackage = currentPackageLocation != newPackageLocation;

            string currentIntegrationLocation = FilesManager.IntegrationFolderPath;
            string newIntegrationLocation = $"{Application.dataPath}{gruIntegrationFolderLocation}";
            bool shouldRelocateDbConfig = currentIntegrationLocation != newIntegrationLocation; ;

            FilesManager.UpdateGeneralConfigCSharpScriptFile
            (
                gruIntegrationFolderLocation,
                gruPackageFolderLocation
            );

            configurationsDone += MessageConfig.GRUConfigureWindow.Success.GENERAL_CONFIG_CONSTANTS_UPDATED;

            //1. move the dbConfig from current integration folder to its new location:
            if (shouldRelocateDbConfig)
            {
                string newDbConfigFolderLocation = $"{newIntegrationLocation}/Configs";
                if (!File.Exists(FilesManager.DbConfigFilePath))
                {
                    FilesManager.CreateEmptyDbConfig(newDbConfigFolderLocation);
                }
                configurationsDone += MessageConfig.GRUConfigureWindow.Success.GET_DB_CONFIG_RELOCATED_SUCCESS(newDbConfigFolderLocation);
            }

            //2. move the current gru package folder and all its content to the new location:
            if (shouldRelocatePackage)
            {
                FileUtil.DeleteFileOrDirectory(newPackageLocation); //workaround for a known issue, else it failes to copy with an error sayign it already exists..
                FileUtil.MoveFileOrDirectory(currentPackageLocation, newPackageLocation);

                string currentPackageMetaFile = $"{currentPackageLocation}.meta";
                FileUtil.DeleteFileOrDirectory(currentPackageMetaFile);
                FileUtil.DeleteFileOrDirectory(currentPackageLocation);

                configurationsDone +=
                    "\n\n ** GRU Package Folder And all of its content have been successfully relocated from: \n  the old path \n    (" + currentPackageLocation + ") \n  to the new path \n    (" + newPackageLocation + ")";
            }

            EditorUtility.DisplayDialog
            (
                "GRU Successfully Configured",
                "The following changes have been made: \n" + configurationsDone,
                MessageConfig.Generic.OK
            );

            GRUDebugger.LogSuccess("GRU package successfully configured!");

            AssetDatabase.Refresh();

            var configData = GetPackageConfig(false);
            UpdateTextOfConfigureButton(configData);

            Close();
        }

        #endregion

        #region Private Methods

        private PackageMetaData GetPackageConfig(bool createConfigFolderIfNotFound)
        {
            var packageConfig = FilesManager.GetPackageConfigData
            (
                createConfigFolderIfNotFound: createConfigFolderIfNotFound
            );
            return packageConfig;
        }

        private void EnableOrDisableConfigureButton()
        {
            bool hasSelectedPackagePath = !string.IsNullOrWhiteSpace(gruPackageFolderLocation);
            bool hasSelectedIntegrationPath = !string.IsNullOrWhiteSpace(gruIntegrationFolderLocation);

            bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();

            string tooltip = "";
            if (!hasSelectedPackagePath)
            {
                tooltip = MessageConfig.Tooltips.BrokenRules.PACKAGE_PATH_NOT_SET;
            }
            else if (!hasSelectedIntegrationPath)
            {
                tooltip = MessageConfig.Tooltips.BrokenRules.INTEGRATION_PATH_NOT_SET;
            }
  
            if(isInDemo)
            {
                var isIntegrationPathOutsideGruRoot = 
                    !gruIntegrationFolderLocation.Contains(GeneralConfig.DynamicCode.PACKAGE_PATH) &&
                        !gruIntegrationFolderLocation.Contains(gruPackageFolderLocation);
                if (isIntegrationPathOutsideGruRoot)
                {
                    tooltip = MessageConfig.GRUConfigureWindow.Warning.DEMO_INTEGRATION_FOLDER_OUTSIDE_GRU_ROOT_FOLDER;
                }
            }

            bool canEnableConfigureBtn = string.IsNullOrWhiteSpace(tooltip);
            _configurGRUBtn.SetEnabled(canEnableConfigureBtn);
            _configurGRUBtn.tooltip = tooltip;
        }

        private void UpdatePathInputs()
        {
            gruPackageFolderLocation = GeneralConfig.DynamicCode.PACKAGE_PATH;

            bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
            if (isInDemo)
            {
                gruIntegrationFolderLocation = GeneralConfig.DynamicCode.DEMO_INTEGRATION_FOLDER_PATH;
            }
            else
            {
                gruIntegrationFolderLocation = GeneralConfig.DynamicCode.INTEGRATION_FOLDER_PATH;
            }
        }

        private void UpdateTextOfConfigureButton(PackageMetaData config)
        {
            string configureBtnText =
                config == null ?
                    GeneralConfig.Editor.ConfigureFormWindow.Buttons.CONFIGURE_CREATE_TEXT :
                    GeneralConfig.Editor.ConfigureFormWindow.Buttons.CONFIGURE_UPDATE_TEXT;
            _configurGRUBtn.text = configureBtnText;
        }

        #endregion
    }
}

#endif