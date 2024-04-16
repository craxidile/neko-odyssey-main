using SpatiumInteractive.Libraries.Unity.GRU.Configs.General;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using SpatiumInteractive.Libraries.Unity.GRU.Helpers.Bootstrapping;
using UnityEngine.SceneManagement;

namespace SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages
{
    public struct MessageConfig
    {
        #region Props

        private static string PackageConfigNameUsed
        {
            get
            {
                var isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                var name = isInDemo ?
                    FilesManager.DEMO_PACKAGE_CONFIG_FILE :
                        FilesManager.PACKAGE_CONFIG_FILE;
                return name;
            }
        }

        private static string DbConfigNameUsed
        {
            get
            {
                var isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                var name = isInDemo ?
                    FilesManager.DEMO_DB_CONFIG_FILE :
                        FilesManager.DB_CONFIG_FILE;
                return name;
            }
        }

        #endregion

        #region Structs 

        public struct GRUWindowBase
        {
            public struct Warning
            {
                public static string GET_SCRIPTS_WILL_GET_OVERRIDEN_WARN()
                {
                    var warning = $"WARNING: You are about to override some of the existing repo classes and interfaces in {FilesManager.RepoFilePath} folder because you've selected some table(s) that are already a part of one of the existing databases (which already has its respective repos created). Are you sure you want to do this? ";
                    return warning;
                }

                public const string CONFIRM_SCRIPTS_OVERRIDE = "Yes, override those files";
            }
        }

        public struct UpdateOrDeleteDBWindow
        {
            public const string DELETE_DATABSE_DIALOG_TITLE = "Delete Database";

            public const string UPDATE_DATABSE_DIALOG_TITLE = "Update Database";

            public const string DELETE_DATABSE_DIALOG_CONFIRM = "Yes, delete it";

            public struct Success
            {
                public static string GET_DB_DELETE_PROC_DONE_SUCCESS()
                {
                    var error = $"SUCCESS: Database file, dbcontext class and database handler game object have been deleted successfully. {DbConfigNameUsed} has been updated.";
                    return error;
                }
            }


            public struct Warning
            {
                public static string GET_DELETE_DB_HEADSUP_WARNING()
                {
                    var error = $"WARNING: This will delete database file, database handler object in the scene, db context class in DbContexts and remove the entry from {DbConfigNameUsed}. Repo classes and interfaces won't get deleted. Are you sure you want to do this?";
                    return error;
                }
            }

            public struct Error
            {
                public static string GET_DB_FAILED_TO_UPDATE_ERR(string databaseName)
                {
                    var error = $"ERROR: Database {databaseName} has failed to update. Please check that everything's okay with your {DbConfigNameUsed} file and that you're not missing any DLLs in Plugins folder.";
                    return error;
                }
            }
        }

        public struct GRUConfigureWindow
        {
            public struct Success
            {
                public static string GENERAL_CONFIG_CONSTANTS_UPDATED = $"\n\n** {nameof(GeneralConfig)}.cs constants have been successfully updated";

                public static string GET_GRU_PACKAGE_CONFIG_GENERATED_SUCCESS()
                {
                    var success = $"\n ** {PackageConfigNameUsed} has been successfully generated.";
                    return success;
                }

                public static string GET_DB_CONFIG_RELOCATED_SUCCESS(string newPath)
                {
                    var success = $"\n\n ** {DbConfigNameUsed} file has been successfully relocated from: \n  the old path \n  ( {FilesManager.DbConfigPath} ) \n  to the new path: \n  ( {newPath} )";
                    return success;
                }
            }

            public struct Warning
            {
                public static string GET_NO_GRU_ASMDEF_FILE_FOUND_WARN()
                {
                    string lastValidPackagePath = GeneralConfig.DynamicCode.PACKAGE_PATH;
                    string warning = $"WARNING: GRU Assembly file could not be found in the selected folder or any of its subfolders. Did you, perhaps, delete, rename or manually move SpatiumInteractive.Libraries.Unity.GRU.asmdef file from the {lastValidPackagePath} root folder (this is, at least, its last known name) ?\n If so, revert that change and put it back where it was!";
                    return warning;
                }
                public const string CLOSE_RUNNING_PROGRAMS = "Close Running Programs";
                public const string RUNNING_PROGRAMS_DETECTED = "The following programs have been detected as currently running. Please close them and then try again.";
                public const string PACKAGE_FOLDER_LOCATION_IN_STREAMING_ASSETS = "StreamingAssets folder cannot be used for storing GRU package. This is a folder reserved for other things...";
                public const string PACKAGE_FOLDER_LOCATION_OUTSIDE_ASSETS = "You can't select a path outside of the 'Assets/' folder nor can you select the 'Assets' folder itself. A folder that you're allowed select is a folder that's in your your unity project and that's inside Assets folder - can be at any level of depth as long as it's a child of the Assets folder. If you don't want to change the default location of the GRU asset, then that's fine as well, in that case, just ignore this and click the button below";
                public const string INTEGRATION_FOLDER_LOCATION_OUTSIDE_ASSETS = "You can't select a path outside of the 'Assets/' folder nor can you select the 'Assets' folder itself. A folder that you're allowed select is a folder that's in your your unity project and that's inside Assets folder - can be at any level of depth as long as it's a child of the Assets folder. If you are not in the demo scene, then the selected folder must also be outside of the GRU folder. If you are in the demo scene, then the selected folder must also be inside of the GRU folder.";
                public const string DEMO_INTEGRATION_FOLDER_OUTSIDE_GRU_ROOT_FOLDER = "YOU ARE IN THE DEMO SCENE THEREFORE - Selecting any location that's outside of the GRU root folder is not allowed! \n Please select a folder that's inside of the GRU folder. It can be any folder inside of the GRU folder, altough, it might make the most sense to keep it in the 'GRU - SQLite ORM and DB Repo Manager/DemoScene/Scripts/Integration/GRU' folder (given that you haven't manually renamed anything there already..) .";
                public const string PRODUCTION_INTEGRATION_FOLDER_INSIDE_GRU_ROOT_FOLDER = "YOU ARE NOT IN THE DEMO SCENE THEREFORE- Selecting any location that's inside of the GRU root folder is not allowed! \n Please select a folder that's outside of the GRU folder. It can be any folder outside of the GRU folder, although, as the YT tutorial recomends, it might make the most sense to keep it in the 'Assets/Integration/GRU' folder.";
            }
        }

        public struct Generic
        {
            public const string CANCEL = "Cancel";
            public const string CLOSE = "Close";
            public const string OK = "Ok";
            public const string SUCCESS = "Success";
            public const string WARNING = "Warning";
        }

        public struct RightClickMenu
        {
            public struct DbSchema
            {
                public const string AUTO_RUNNING_FAILED_WARNING = "Couldn't autorun DbSchema.exe on path: C:\\Program Files\\DbSchema\\DbSchema.exe \n\nYou either don't have it installed on your system or path needs to be adjusted. You can grab it from their official page by clicking download button below.";
            }

            public struct SQLiteBrowser
            {
                public const string AUTO_RUNNING_FAILED_WARNING = "Couldn't autorun DB Browser for SQLite.exe on path: C:\\Program Files\\DB Browser for SQLite\\DB Browser for SQLite.exe \n\nYou either don't have it installed on your system or path needs to be adjusted. You can grab it from their official page by clicking download button below.";
            }
        }

        public struct Tooltips
        {
            public struct DbHandler
            {
                public const string DB_NAME = "Name of the database you've created using the GRU editor window.";
                public const string DB_PATH = "Path to where your database is stored. Never ever should you move it from Assets/StreamingAssets !";
            }

            public struct BrokenRules
            {
                public const string ENTER_DB_NAME = "Enter name for your database.";
                public const string DB_WITH_NAME_ALREADY_EXISTS = "Database name is already taken. Please choose a name that is not already used.";
                public const string ALPHA_NUMERIC_CHARS_ONLY = "Database name can only contain alpha numeric characters. No special characters, or file extensions, allowed.";
                public const string SELECT_PATH_ON_DISK_FOR_DB = "Select a path on your disk where you want the database file to be stored in.";
                public const string SELECT_AT_LEAST_ONE_TABLE = "You must select at least one table.";
                public const string CANT_UPDATE_WITH_NO_DB_SELECTED = "Can't do an update with no database selected";
                public const string CANT_UPDATE_WITH_NO_TABLE_SELECTED = "Can't do an update with no table selected";
                public const string PACKAGE_PATH_NOT_SET = "Package path isn't set";
                public const string INTEGRATION_PATH_NOT_SET = "Integration folder path isn't set";
            }
        }

        public struct Debugging
        {
            public struct Info
            {
                public const string CREATING_DB_CONTEXT = "INFO: Creating Db Context...";
                public const string CREATING_DB_REPO = "INFO: Creating Db Repo(s)...";
                public const string CREATING_SQLITE_DB = "INFO: Creating SQLite Database...";
            }

            public struct Success
            {
                public const string DB_CONTEXT_CREATED = "SUCCESS: Successfully created a new db context class on path: ";
                public const string SUCCESS_MESSAGE_DB_CONNECTION_ESTABLISHED = "SUCCESS: Connection with the database has been established successfully! Database path: ";
                public const string SUCCESS_MESSAGE_DB_WRITTEN = "SUCCESS: Database written";
            }

            public struct Warning
            {
                public const string WARNG_DB_NOT_IN_PERSISTENT_DATA_PATH = "WARNING: Database not in Persistent Data Path (Application.persistentDataPath)";

                public const string WARNG_WHILE_CREATING_DB = "ERROR: A non-critical error occured while creating the database file. If you see this warning, first confirm that the database is not created and only if it's not created/you still don't see it in the project, should you try again. If the error persists, you might be missing required dlls in plugin folder. Make sure that you use the correct dll - based on the platform that you're shipping to. Don't just throw in all dlls there as that can sometimes cause errors.";

                public static string GET_ISSUE_WHILE_CONNECTING_TO_EXISTING_DB_WARN(string dbName, string dbHandlerGO)
                {
                    var warning = $"WARNING: Database {dbName} exists in the StremingAssets folder however, DBHandler {dbHandlerGO} was not able to connect to it in one of the attempts. You should still be totally safe as this usually only happens in editor sometimes, BUT if you get the similiar connection-issue while in Play mode, and you're not able to fetch data from your DB,  then it might be a sign that something went wrong with sqlite dlls in Plugin folder...";
                    return warning;
                }

                public static string GET_CANNOT_DELETE_DB_ON_PATH_WARNING(string filePath)
                {
                    var warning = $"WARNING: Couldn't delete database on path: {filePath} No database file found on that path.";
                    return warning;
                }

                public static string GET_CANNOT_DELETE_DBCONTEXT_ON_PATH_WARNING(string filePath, string scriptFileName)
                {
                    var warning = $"WARNING: Couldn't delete db context: {scriptFileName} on path: {filePath} . No such db context file found on that path.";
                    return warning;
                }

                public static string GET_CANNOT_REMOVE_ENTRY_FROM_DB_CONFIG_WARNING(string databaseName)
                {
                    var dbConfig = DbConfigNameUsed;
                    var warning = $"WARNING: Couldn't remove db entry from: \n {dbConfig} \n for database named: {databaseName} . \n Either there's no such entry or there are multiple entries registered for this same database. Check your {dbConfig} file located in : \n {FilesManager.DbConfigFilePath}";
                    return warning;
                }

                public static string GET_DB_HANDLER_VALIDATION_FAILED_WARNING(string dbHandlerGameObjectName)
                {
                    var dbConfig = DbConfigNameUsed;
                    var warning = $"WARNING: Validation for DBHandler.cs on Game Object: {dbHandlerGameObjectName} has failed. Couldn't fetch its db meta data from the <u>corresponding</u> <i>{dbConfig}</i>. Have you manually altered its content are you, perhaps, using the wrong db config ? Please inspect your {dbConfig} file located in: \n {FilesManager.DbConfigFilePath}";
                    return warning;
                }

                public static string GET_DB_HANDLER_RENAME_FORBIDDEN_WARNING()
                {
                    var dbConfig = DbConfigNameUsed;
                    var warning = $"WARNING: You're not allowed to rename db handlers because the {dbConfig} file uses them as keys when reading data.\nNo worries! This dbHandler will get renamed back to its default name automatically, after you close this popup.";
                    return warning;
                }

                public static string GET_NO_GRU_CONFIG_JSON_WARNING()
                {
                    var warning = $"WARNING: Couldn't find {PackageConfigNameUsed} in the project. Create one by going to Tools > SpatiumInteractive > GRU > Configure. Once all paths have been set, do not delete it, do not rename it and do not move it!";
                    return warning;
                }
            }

            public struct Error
            {
                public const string ERROR_WHILE_CREATING_DB_TABLES = "ERROR: Error while creating tables in database. Tables list is empty, most likely you don't have any EntityBase derivatives in your project and you need to create table classes.";

                public static string GET_DB_HANDLER_PRESENT_BUT_NO_DB_CONFIG_JSON_ERR()
                {
                    var error = $"ERROR: Db Handler object is in the scene, but no file named <i>{DbConfigNameUsed}</i> has been found in <i> {FilesManager.DbConfigPath} </i>. Either you've deleted it, renamed it, or moved it elsewhere. If you have moved it elsewhere, please put it back in <i> {FilesManager.DbConfigPath} </i> and don't move it from there anymore!";
                    return error;
                }

                public static string GET_PROGRAMS_BLOCKING_THREAD_ERR(string filePath, System.Exception ex)
                {
                    var error = $"ERROR: Couldn't delete database on path: {filePath}. \n Database may be missing from the path or is currently opened in another program (e.g. sqlite browser or something similar that you might be using). \n\n exception details: \n\n {ex.Message}";
                    return error;
                }

                public static string GET_FAILED_TO_LOAD_METADATA_VIA_DB_HANDLER_GO_ERR(string dbHandlerGameObjectName)
                {
                    var dbConfig = DbConfigNameUsed;
                    var error = $"ERROR: Failed to load meta data from: {dbConfig} \n for db handler game object: {dbHandlerGameObjectName} . No single entry for such db handler has been found in {dbConfig}. \n There are multiple entries registered by the same db handler. Please check your {dbConfig} file located in: \n {FilesManager.DbConfigFilePath}";
                    return error;
                }

                public static string GET_FAILED_TO_LOAD_METADATA_VIA_DB_NAME_ERR(string databaseName)
                {
                    var dbConfig = DbConfigNameUsed;
                    var error = $"ERROR: Failed to load meta data from {dbConfig} \n for database named: {databaseName} . No single entry for such db name has been found in {dbConfig} \n. There are multiple entries registered with the same database name. Please check your {dbConfig} file located in: \n  {FilesManager.DbConfigFilePath}";
                    return error;
                }
            }

            public struct Exception
            {
                public const string EXCP_GET_DB_FILE_PATH_FOR_EDITOR_CALL_FROM_NON_EDITOR_PLATFORM = "DbMetaData.GetDbFilePatheEditor method can only be called when in unity editor. Calling it in builds is not allowed.";
            }
        }

        #endregion
    }
}
