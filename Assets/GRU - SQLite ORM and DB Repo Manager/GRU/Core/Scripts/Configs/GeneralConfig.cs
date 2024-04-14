using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using UnityEngine.SceneManagement;

namespace SpatiumInteractive.Libraries.Unity.GRU.Configs.General
{
    public struct GeneralConfig
    {
        //constants which can be changed by the work of outside scripts 
        //e.g. relative paths that can be changed by configuration window logic.
        public struct DynamicCode
        {
            #region Global

            public const string PACKAGE_PATH = "/GRU - SQLite ORM and DB Repo Manager"; //global, same for everything, this is the path to the GRU package - a root folder, if you will.
            public static string PACKAGE_CONFIG_CLASSES_FOLDER_PATH = $"{PACKAGE_PATH}/GRU/Core/Scripts/Configs"; //path to where GeneralConfig.cs file is stored and others
            public static string GRU_CONFIG_FILE_PATH = $"{PACKAGE_PATH}/GRU/Core/Configs"; //path to where gruConfig.json (and demoGruConfig.json) files are stored (it's the same folder for both of them)
            public static string BASE_EDITOR_HEADER_IMG_PATH = $"Assets{PACKAGE_PATH}/GRU/Core/Images/GRUEditorHeaderImg.png";
            public static string CONFIGURE_EDITOR_IMG_PATH = $"Assets{PACKAGE_PATH}/GRU/Core/Images/GRUConfigureEditorHeaderImg.png";
            public static string SCANNED_TABLE_ICO_IMG_PATH = $"Assets{PACKAGE_PATH}/GRU/Core/Images/GRUScannedTablesImg.png";

            #endregion

            #region Production

            public const string INTEGRATION_FOLDER_PATH = "/NekoOdyssey/Scripts/Database";
            public const string DB_CONTEXT_FILE_PATH = "/NekoOdyssey/Scripts/Database/DbContexts";
            public const string REPO_FILE_PATH = "/NekoOdyssey/Scripts/Database/Repositories";
            public const string IREPO_FILE_PATH = "/NekoOdyssey/Scripts/Database/Repositories";
            public const string DB_CONFIG_FILE_PATH = "/NekoOdyssey/Scripts/Database/Configs";

            #endregion

            #region Demo Scene

            public static string DEMO_INTEGRATION_FOLDER_PATH = $"{PACKAGE_PATH}/DemoScene/Scripts/Integration/GRU";
            public static string DEMO_DB_CONTEXT_FILE_PATH = $"{PACKAGE_PATH}/DemoScene/Scripts/Integration/GRU/DbContexts";
            public static string DEMO_REPO_FILE_PATH = $"{PACKAGE_PATH}/DemoScene/Scripts/Integration/GRU/Repositories";
            public static string DEMO_IREPO_FILE_PATH = $"{PACKAGE_PATH}/DemoScene/Scripts/Integration/GRU/Repositories";
            public static string DEMO_DB_CONFIG_FILE_PATH = $"{PACKAGE_PATH}/DemoScene/Scripts/Integration/GRU/Configs";

            #endregion
        }

        public struct Code
        {
            public const string BODY_OPEN = "{";
            public const string BODY_CLOSE = "}";
            public const string INTERFACE_PREFIX = "I";
            public const string SCRIPT_FILE_SUFFIX = ".cs";
            public const string EMPTY_LINE = "\n";
            public const string INDENT = "\t";
            public const string DOUBLE_INDENT = "\t\t";
            public const string DB_CONTEXT_CLASS_USINGS = "using SpatiumInteractive.Libraries.Unity.GRU.Contracts;\nusing SQLite4Unity3d;";
            public const string DB_CONTEXT_CLASS_NAMESPACE = "namespace Assets.Scripts.Integration.GRU.DbContexts";

            public struct Attributes
            {
                public struct MenuItem
                {
                    public const string GRU_CONFIGURE = "Tools/Spatium Interactive/GRU/Configure";
                    public const string GRU_NEW_DATABASE = "Tools/Spatium Interactive/GRU/Create New Database";
                    public const string GRU_EXISTING_DATABASES = "Tools/Spatium Interactive/GRU/Update Existing Database(s)";
                }
            }
        }

        public struct Defaults
        {
            public const string ROOT_ASSETS_FOLDER = "/Assets/";
            public const string STREAMING_ASSETS_FOLDER = "StreamingAssets";
            public const string DATABASE_PATH = "Assets/StreamingAssets";
            public const string DATABASE_FILE_EXTENSION = ".db";
            public const string ASMDEF_FILENAME = "SpatiumInteractive.Libraries.Unity.GRU.asmdef";
            public const string DEMO_SCENE_NAME = "SpatiumInteractive.Unity.GRU.BasicExamplesScene";
            public const string DATABASE_NAME = "DatabaseName";
            public const string BUTTON_TITLE = "Button Title";
        }

        public struct Editor
        {
            public struct ConfigureFormWindow
            {
                public const string TAB_TITLE = "GRU Configuration";
                public const int MIN_SIZE_WIDTH = 650;
                public const int MIN_SIZE_HEIGHT = 550;

                public struct Buttons
                {
                    public const string CONFIGURE_CREATE_TEXT = "Create Configuration";
                    public const string CONFIGURE_UPDATE_TEXT = "Update Configuration";
                    public const string SET_PACKAGE_PATH = "Set Package Path";
                    public const string SET_INTEGRATION_PATH = "Set Integration Path";
                }

                public struct Labels
                {
                    public const string PACKAGE_FOLDER_LOCATION_LABEL = "Package Folder Location";
                    public const string INTEGRATION_FOLDER_LOCATION_LABEL = "Integration Folder Location";
                }
            }

            public struct UpdateOrDeleteDbFormWindow
            {
                public const string TAB_TITLE = "GRU Update Editor";
                public const int MIN_SIZE_WIDTH = 500;
                public const int MIN_SIZE_HEIGHT = 700;

                public struct Buttons
                {
                    public const string UPDATE_DB_BUTTON_TEXT = "Update";
                    public const string DELETE_DB_BUTTON_TEXT = "Delete";
                    public const string PING_DB_BUTTON_TEXT = "Ping";
                }

                public struct DropDowns
                {
                    public const string ASSEMBLY_SELECTION_DDL_TOOLTIP = "Select an assembly that you want to be considered as the scope of the table scan. Everywhere = looks for table classes in all assemblies, inside GRU = looks for in DemoScene.ExampleUsages and GRU assembly , outside GRU = scans all assemblies except the GRU related assemblies";
                }

                public struct Labels
                {
                    public const string UPDATE_TABLES = "Update tables:";
                    public const string NO_DATABASES_DETECTED_ERROR_MSG = "Seems like you haven't created a database yet. \nTo get started, click on Create New Database menu and create your first db.";
                    public const string NO_TABLES_DETECTED_ERROR_MSG = "No tables have been detected in the project. \nPlease create at least one table by inheriting from entity base.";
                    public const string DATABASE_TABLES_LIST = "Databases:";
                    public const string ASSEMBLY_SELECTION_DDL_LABEL = "Scan for tables in";
                }
            }

            public struct CreateNewDbFormWindow
            {
                public const string TAB_TITLE = "GRU Create Editor";
                public const int MIN_SIZE_WIDTH = 600;
                public const int MIN_SIZE_HEIGHT = 750;

                public struct Buttons
                {
                    public const string CREATE_DB_BUTTON_TEXT = "Create";
                }

                public struct DropDowns
                {
                    public static string GET_ASSEMBLY_SELECTION_DDL_TOOLTIP()
                    {
                        var isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                        var tooltip = "";
                        if (isInDemo)
                        {
                            tooltip += "You are in the GRU DEMO scene, only tables inside GRU assembly are getting scaned. This is done on purpose to keep the demo scene as clean as possible! If you want to create database with your own tables - simply do it in a different scene. ";
                        }
                        else
                        {
                            tooltip += "Select an assembly that you want to be considered as the scope of the table scan. Everywhere = looks for table classes in all assemblies, inside GRU = looks for in DemoScene.ExampleUsages and GRU assembly , outside GRU = scans all assemblies except the GRU related assemblies";
                        }
                        return tooltip;
                    }
                }

                public struct Labels
                {
                    public const string NO_TABLES_DETECTED_ERROR_MSG = "No tables have been detected in the project. \nPlease create at least one table by inheriting from entity base.";
                    public const string DATABASE_NAME_LABEL = "Database Name *";
                    public const string DATABASE_PATH_LABEL = "Database Path *";
                    public const string DATABASE_TABLES_LABEL = "Database Tables *";
                    public const string DATABASE_TABLES_LIST = "Databases:";
                    public const string ASSEMBLY_SELECTION_DDL_LABEL = "Scan for tables in";
                }
            }

            public struct RightClickMenu
            {
                public struct DbSchema
                {
                    public const string PROCESS_EXE = "C:\\Program Files\\DbSchema\\DbSchema.exe";
                }

                public struct SQLiteBrowser
                {
                    public const string PROCESS_EXE = "C:\\Program Files\\DB Browser for SQLite\\DB Browser for SQLite.exe";
                }
            }
        }
    }
}
