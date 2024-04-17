#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.IO;
using System.Linq;
using System;
using System.Text;
using System.Collections.Generic;
using SQLite4Unity3d;
using Database.Repository;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.General;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Debugging;
using SpatiumInteractive.Libraries.Unity.GRU.Extensions;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using SpatiumInteractive.Libraries.Unity.Platform.Debugging;
using System.Text.RegularExpressions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Helpers.Bootstrapping
{
    /// <summary>
    /// Helper class that allows creating and
    /// deleting files and folders for GRU library
    /// </summary>
    public static class FilesManager
    {
        #region Constants

        #region Global

        private const string SQLITE_CONNECTION_CLASS = nameof(SQLiteConnection);
        private const string REPO_CLASS_USINGS = "using SpatiumInteractive.Libraries.Unity.GRU.Contracts;\nusing SpatiumInteractive.Libraries.Unity.GRU.Domain;\nusing Assets.Scripts.Integration.GRU.DbContexts;\nusing Database.Repository;";
        private const string REPO_CLASS_NAMESPACE = "namespace Assets.Scripts.Integration.GRU.Repositories";
        private const string IREPO_INTERFACE_USINGS = "using SpatiumInteractive.Libraries.Unity.GRU.Domain;\nusing Database.Repository;";
        private const string IREPO_INTERFACE_NAMESPACE = "namespace Assets.Scripts.Integration.GRU.Repositories";

        #endregion

        #region Production

        public const string DB_CONFIG_FILE = "dbConfig.json";
        public const string PACKAGE_CONFIG_FILE = "gruConfig.json";

        #endregion

        #region Demo

        public const string DEMO_DB_CONFIG_FILE = "gruDemoDbConfig.json";
        public const string DEMO_PACKAGE_CONFIG_FILE = "gruDemoConfig.json";

        #endregion

        #endregion

        #region Public Static Properties

        public static string DbConfigFile
        {
            get
            {
                bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                if (isInDemo)
                {
                    return DEMO_DB_CONFIG_FILE;
                }
                return DB_CONFIG_FILE;
            }
        }

        public static string DbConfigPath
        {
            get
            {
                bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                if (isInDemo)
                {
                    return $"{Application.dataPath}{GeneralConfig.DynamicCode.DEMO_DB_CONFIG_FILE_PATH}";
                }
                return $"{Application.dataPath}{GeneralConfig.DynamicCode.DB_CONFIG_FILE_PATH}";
            }
        }

        public static string DbConfigFilePath
        {
            get
            {
                bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                if (isInDemo)
                {
                    return $"{Application.dataPath}{GeneralConfig.DynamicCode.DEMO_DB_CONFIG_FILE_PATH}/{DEMO_DB_CONFIG_FILE}";
                }
                return $"{Application.dataPath}{GeneralConfig.DynamicCode.DB_CONFIG_FILE_PATH}/{DB_CONFIG_FILE}";
            }
        }

        public static string PackageConfigPath
        {
            get
            {
                bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                if (isInDemo)
                {
                    return $"{Application.dataPath}{GeneralConfig.DynamicCode.GRU_CONFIG_FILE_PATH}";
                }
                return $"{Application.dataPath}{GeneralConfig.DynamicCode.GRU_CONFIG_FILE_PATH}";
            }
        }

        /// <summary>
        /// the same for all, regardless of 
        /// the scene we're currently in
        /// </summary>
        public static string PackagePath
        {
            get
            {
                return $"{Application.dataPath}{GeneralConfig.DynamicCode.PACKAGE_PATH}";
            }
        }

        public static string IntegrationFolderPath
        {
            get
            {
                bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                if (isInDemo)
                {
                    return $"{Application.dataPath}{GeneralConfig.DynamicCode.DEMO_INTEGRATION_FOLDER_PATH}";
                }
                return $"{Application.dataPath}{GeneralConfig.DynamicCode.INTEGRATION_FOLDER_PATH}";
            }
        }

        public static string RepoFilePath
        {
            get
            {
                bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                if (isInDemo)
                {
                    return $"{Application.dataPath}{GeneralConfig.DynamicCode.DEMO_REPO_FILE_PATH}";
                }
                return $"{Application.dataPath}{GeneralConfig.DynamicCode.REPO_FILE_PATH}";
            }
        }

        #endregion

        #region Private Static Properties



        private static string PackageConfigFilePath
        {
            get
            {
                bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                if (isInDemo)
                {
                    return $"{Application.dataPath}{GeneralConfig.DynamicCode.GRU_CONFIG_FILE_PATH}/{DEMO_PACKAGE_CONFIG_FILE}";
                }
                return $"{Application.dataPath}{GeneralConfig.DynamicCode.GRU_CONFIG_FILE_PATH}/{PACKAGE_CONFIG_FILE}";
            }
        }

        private static string DbContextFilePath
        {
            get
            {
                bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                if (isInDemo)
                {
                    return $"{Application.dataPath}{GeneralConfig.DynamicCode.DEMO_DB_CONTEXT_FILE_PATH}";
                }
                return $"{Application.dataPath}{GeneralConfig.DynamicCode.DB_CONTEXT_FILE_PATH}";
            }
        }



        private static string IRepoFilePath
        {
            get
            {
                bool isInDemo = SceneManager.GetActiveScene().IsGRUDemo();
                if (isInDemo)
                {
                    return $"{Application.dataPath}{GeneralConfig.DynamicCode.DEMO_IREPO_FILE_PATH}";
                }
                return $"{Application.dataPath}{GeneralConfig.DynamicCode.IREPO_FILE_PATH}";
            }
        }

        #endregion

#if UNITY_EDITOR

        #region API - Static Methods

        /// <summary>
        /// Creates a new db context script.
        /// <br/>
        /// Naming convention used: <i>[databaseName]DbContext.cs</i>
        /// <br/>
        /// <list type="bullet">
        /// <item>A new DbContext class is saved on this <see cref="GeneralConfig.DynamicCode.DB_CONTEXT_FILE_PATH">PATH</see></item>
        /// </list>
        /// </summary>
        public static string CreateNewDbContext(string databaseName, out string dbContextName)
        {
            string className = databaseName.NormalizeNaming().AddSuffix("DbContext");
            string scriptFileName = className.AddSuffix(GeneralConfig.Code.SCRIPT_FILE_SUFFIX);
            string classDeclaration = className.ToPublicClass().InheritClass(SQLITE_CONNECTION_CLASS).AndInheritInterface("IDbContext<" + className + ">");
            string constructorDeclaration = className.ToPublic().AddInParentheses(SQLITE_CONNECTION_CLASS + " connection").AddBaseInherits("connection.DatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create, connection.StoreDateTimeAsTicks").AddEmptyBody(indentLevel: 2);
            string contextPropDeclaration = className.ToPublicSelfPointingProp("Context");

            var dbContextBuilder = new StringBuilder();
            dbContextBuilder
                .Append(GeneralConfig.Code.DB_CONTEXT_CLASS_USINGS)
                .AppendLine(GeneralConfig.Code.EMPTY_LINE)
                .AppendLine(GeneralConfig.Code.DB_CONTEXT_CLASS_NAMESPACE)
                .AppendLine(GeneralConfig.Code.BODY_OPEN)
                    .AppendLine(GeneralConfig.Code.INDENT + classDeclaration)
                    .AppendLine(GeneralConfig.Code.INDENT + GeneralConfig.Code.BODY_OPEN)
                        .AppendLine(GeneralConfig.Code.DOUBLE_INDENT + constructorDeclaration)
                        .AppendLine()
                        .AppendLine(GeneralConfig.Code.DOUBLE_INDENT + contextPropDeclaration)
                    .AppendLine(GeneralConfig.Code.INDENT + GeneralConfig.Code.BODY_CLOSE)
                .AppendLine(GeneralConfig.Code.BODY_CLOSE);


            DbContextFilePath.ToPathOnDisk();
            string pathFile = $"{DbContextFilePath}/{scriptFileName}";
            pathFile.ToFileOnDisk(dbContextBuilder);

            AssetDatabase.Refresh();

            string debugMessage = MessageConfig.Debugging.Success.DB_CONTEXT_CREATED + pathFile;
            GRUDebugger.LogSuccess(debugMessage);

            dbContextName = className;

            return pathFile;
        }

        /// <summary>
        /// Creates both repo interface and repo class 
        /// scripts for passed list of database tables
        /// <br/>
        /// Naming convention used:  <i>[I][DatabaseTableName]Repository.cs</i>
        /// <br/>
        /// <br/>
        /// <list type="bullet">
        /// <item>Interface is saved on this  <see cref="GeneralConfig.DynamicCode.IREPO_FILE_PATH">PATH</see></item>
        /// <item>Class is saved on this <see cref="GeneralConfig.DynamicCode.REPO_FILE_PATH">PATH</see></item>
        /// </list>
        /// </summary>
        public static List<string> CreateNewRepositories
        (
            string databaseName,
            List<string> databaseTables,
            string dbContextClassName
        )
        {
            List<string> filePathsCreated = new List<string>();

            var dbConfig = GetDbConfigData();

            for (int i = 0; i < databaseTables.Count; i++)
            {
                var databaseTable = databaseTables[i];

                bool hasTableBeenUsed = dbConfig != null && dbConfig.DbMetaData
                                        .SelectMany(x => x.TableNames)
                                        .Any(t => t == databaseTable);

                bool useDefaultRepoNaming = !hasTableBeenUsed;

                //Creating Repo script
                string defaultRepoClassName = databaseTable.NormalizeNaming().AddSuffix("Repository");
                string repoClassName = useDefaultRepoNaming ? defaultRepoClassName : defaultRepoClassName.AddPrefix(databaseName.NormalizeNaming());
                string repoScriptFileName = repoClassName.AddSuffix(GeneralConfig.Code.SCRIPT_FILE_SUFFIX);
                string repoClassDeclaration = repoClassName.ToPublicClass() + " : " + "Repository<" + databaseTable + ", int, " + dbContextClassName + ">, " + "I" + repoClassName;
                string repoConstructorDeclaration = repoClassName.ToPublic().AddInParentheses("IDbContext<" + dbContextClassName + "> dbContext").AddBaseInherits("dbContext").AddEmptyBody(indentLevel: 2);

                var repoClassBuilder = new StringBuilder();
                repoClassBuilder
                    .Append(REPO_CLASS_USINGS)
                    .AppendLine(GeneralConfig.Code.EMPTY_LINE)
                    .AppendLine(REPO_CLASS_NAMESPACE)
                    .AppendLine(GeneralConfig.Code.BODY_OPEN)
                        .AppendLine(GeneralConfig.Code.INDENT + repoClassDeclaration)
                        .AppendLine(GeneralConfig.Code.INDENT + GeneralConfig.Code.BODY_OPEN)
                            .AppendLine(GeneralConfig.Code.DOUBLE_INDENT + repoConstructorDeclaration)
                        .AppendLine(GeneralConfig.Code.INDENT + GeneralConfig.Code.BODY_CLOSE)
                    .AppendLine(GeneralConfig.Code.BODY_CLOSE);


                RepoFilePath.ToPathOnDisk();

                string pathRepoFile = $"{RepoFilePath}/{repoScriptFileName}";
                pathRepoFile.ToFileOnDisk(repoClassBuilder);

                //Creating IRepo script
                string defaultIRepoInterfaceName = GeneralConfig.Code.INTERFACE_PREFIX + databaseTable.NormalizeNaming().AddSuffix("Repository");
                string irepoInterfaceName = useDefaultRepoNaming ? defaultIRepoInterfaceName : GeneralConfig.Code.INTERFACE_PREFIX.AddSuffix(databaseName.NormalizeNaming()).AddSuffix(databaseTable.NormalizeNaming().AddSuffix("Repository"));
                string irepoScriptFileName = irepoInterfaceName.AddSuffix(GeneralConfig.Code.SCRIPT_FILE_SUFFIX);
                string irepoInterfaceDeclaration = irepoInterfaceName.ToPublicInterface() + " : " + GeneralConfig.Code.INTERFACE_PREFIX + "Repository<" + databaseTable + ", int>";

                var repoInterfaceBuilder = new StringBuilder();
                repoInterfaceBuilder
                   .Append(IREPO_INTERFACE_USINGS)
                   .AppendLine(GeneralConfig.Code.EMPTY_LINE)
                   .AppendLine(IREPO_INTERFACE_NAMESPACE)
                   .AppendLine(GeneralConfig.Code.BODY_OPEN)
                       .AppendLine(GeneralConfig.Code.INDENT + irepoInterfaceDeclaration)
                       .AppendLine(GeneralConfig.Code.INDENT + GeneralConfig.Code.BODY_OPEN)
                           .AppendLine(GeneralConfig.Code.DOUBLE_INDENT)
                       .AppendLine(GeneralConfig.Code.INDENT + GeneralConfig.Code.BODY_CLOSE)
                   .AppendLine(GeneralConfig.Code.BODY_CLOSE);


                IRepoFilePath.ToPathOnDisk();

                string pathIRepoFile = $"{IRepoFilePath}/{irepoScriptFileName}";
                pathIRepoFile.ToFileOnDisk(repoInterfaceBuilder);

                filePathsCreated.Add(pathRepoFile);
                filePathsCreated.Add(pathIRepoFile);
            }

            AssetDatabase.Refresh();
            return filePathsCreated;
        }

        /// <summary>
        /// Checks if there's already a sqlite database 
        /// with the given name on the given path,
        /// <br/>
        /// if not => creates the new one and connects to it, 
        /// <br/>
        /// else => nothing
        /// </summary>
        public static DBHandler CreateNewOrUpdateExistingSQLiteDatabase
        (
            string databaseName,
            string databasePath,
            List<EntityBase<int>> databaseTables
        )
        {
            DBHandler dbHandler = new DBHandler();
            dbHandler.ConnectWithDatabase
            (
                databaseName,
                databasePath,
                databaseTables
            );

            AssetDatabase.Refresh();
            return dbHandler;
        }

        /// <summary>
        /// Deletes the database from the disk. 
        /// </summary>
        public static void DeleteDatabase(DbMetaData dbMetaData)
        {
            string filePath = dbMetaData.GetDbFilePathForEditor();

            if (!File.Exists(filePath)) return;

            if (filePath.IsNotNullOrWhiteSpace())
            {
                try
                {
                    File.Delete(filePath);
                    AssetDatabase.Refresh();
                }
                catch (Exception ex)
                {
                    GRUDebugger.LogError
                    (
                        MessageConfig.Debugging.Error.GET_PROGRAMS_BLOCKING_THREAD_ERR(filePath, ex)
                    );
                }
            }
            else
            {
                GRUDebugger.LogWarning
                (
                    MessageConfig.Debugging.Warning.GET_CANNOT_DELETE_DB_ON_PATH_WARNING(filePath)
                );
            }
        }

        public static void DeleteDbContext(DbMetaData dbMetaData)
        {
            string scriptFileName = dbMetaData.DbContextName.AddSuffix(GeneralConfig.Code.SCRIPT_FILE_SUFFIX);
            var filePath = $"{DbContextFilePath}/{scriptFileName}";
            if (filePath.IsNotNullOrWhiteSpace())
            {
                File.Delete(filePath);
                AssetDatabase.Refresh();
            }
            else
            {
                GRUDebugger.LogWarning
                (
                    MessageConfig.Debugging.Warning.GET_CANNOT_DELETE_DBCONTEXT_ON_PATH_WARNING(filePath, scriptFileName)
                );
            }
        }

        /// <summary>
        /// <u><b>IN PRODUCTION:</b></u>
        /// <br/>
        /// Creates a <i> <see cref="DB_CONFIG_FILE">db config json </see> </i> file on this 
        /// <see cref="GeneralConfig.DynamicCode.DB_CONFIG_FILE_PATH">PATH</see>
        /// <br/>
        /// <br/>
        /// <u><b>IN DEMO SCENE:</b></u>
        /// <br/>
        /// Creates a <i> <see cref="DEMO_DB_CONFIG_FILE">demo db config json </see> </i> file on this 
        /// <see cref="GeneralConfig.DynamicCode.DEMO_DB_CONFIG_FILE_PATH">PATH</see>
        /// <br/>
        /// <br/>
        /// It's a meta-file which keeps track of
        /// which db handler instance in the scene corresponds to which
        /// database name and path. 
        /// <br/>
        /// It also tracks database tables contained in each database
        /// </summary>
        public static void CreateDbConfig
        (
            string databaseName,
            string databasePath,
            List<string> tableNames,
            string dbHandlerGameObjectName,
            string dbContextName
        )
        {
            var metaDataEntry = new DbMetaData(databaseName, databasePath, tableNames, dbHandlerGameObjectName, dbContextName);
            var dbConfig = GetDbConfigData() ?? new DbMetaDataCollection();
            dbConfig.DbMetaData.Add(metaDataEntry);

            string json = JsonUtility.ToJson(dbConfig, true);
            string jsonConfigPath = DbConfigPath;
            jsonConfigPath.ToPathOnDisk();

            DbConfigFilePath.ToJsonOnDisk(json);

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// <u><b>IN PRODUCTION:</b></u>
        /// <br/>
        /// Creates an empty <i> <see cref="DB_CONFIG_FILE">db config json </see> </i> file on this 
        /// <see cref="GeneralConfig.DynamicCode.DB_CONFIG_FILE_PATH">PATH</see> unless <paramref name="path"/> param is passed
        /// <br/>
        /// <br/>
        /// <u><b>IN DEMO SCENE:</b></u>
        /// <br/>
        /// Creates an empty <i> <see cref="DEMO_DB_CONFIG_FILE">demo db config json </see> </i> file on this 
        /// <see cref="GeneralConfig.DynamicCode.DEMO_DB_CONFIG_FILE_PATH">PATH</see> unless <paramref name="path"/> param is passed
        /// <br/>
        /// <br/>
        /// It's a meta-file which keeps track of
        /// which db handler instance in the scene corresponds to which
        /// database name and path. 
        /// <br/>
        /// It also tracks database tables contained in each database
        /// </summary>
        public static void CreateEmptyDbConfig(string path = "")
        {
            if (string.IsNullOrEmpty(path))
            {
                path = DbConfigFilePath;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }

            var filePath = Path.Combine(path, DbConfigFile);
            string json = JsonUtility.ToJson(string.Empty, true);
            filePath.ToJsonOnDisk(json);

            // Set file permissions using cacls command-line tool (Windows)
            //string command = $"cacls \"{path}\" /E /G \"Everyone\":F";
            //RunCommand(command);

            AssetDatabase.Refresh();
        }

        private static void RunCommand(string command)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {command}";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// <u><b>IN PRODUCTION:</b></u>
        /// <br/>
        /// <see cref="DB_CONFIG_FILE">db config json used</see>
        /// <br/>
        /// <br/>
        /// <u><b>IN DEMO SCENE:</b></u>
        /// <br/>
        /// <see cref="DEMO_DB_CONFIG_FILE">demo db config json used</see>
        /// <br/>
        /// <br/>
        /// Updates the above-mentioned <i>db cnfig.json</i>'s entry by databaseName, 
        /// completly replacing the old one.
        /// </summary>
        public static void UpdateEntryInDbConfig(DbMetaData newDbMetaData)
        {
            var dbConfig = GetDbConfigData();
            for (int i = 0; i < dbConfig.DbMetaData.Count; i++)
            {
                var dbMetaData = dbConfig.DbMetaData[i];
                if (dbMetaData.DbName == newDbMetaData.DbName)
                {
                    dbConfig.DbMetaData[i] = newDbMetaData;
                    break;
                }
            }

            string json = JsonUtility.ToJson(dbConfig, true);
            DbConfigFilePath.ToJsonOnDisk(json);

            AssetDatabase.Refresh();
        }

        public static void RemoveEntryFromDbConfig(DbMetaData dbMetaDataEntry)
        {
            var dbConfig = GetDbConfigData(true);
            if (dbConfig != null && dbConfig.DbMetaData != null)
            {
                var entryToRemove = dbConfig.DbMetaData.SingleOrDefault(x => x.DbName == dbMetaDataEntry.DbName);
                if (entryToRemove != null)
                {
                    dbConfig.DbMetaData.Remove(entryToRemove);

                    string json = JsonUtility.ToJson(dbConfig, true);
                    DbConfigFilePath.ToJsonOnDisk(json);

                    AssetDatabase.Refresh();
                }
                else
                {
                    GRUDebugger.LogError
                    (
                        MessageConfig.Debugging.Warning.GET_CANNOT_REMOVE_ENTRY_FROM_DB_CONFIG_WARNING(dbMetaDataEntry.DbName)
                    );
                }
            }
        }

        public static DbMetaDataCollection GetDbConfigData(bool outputErrorIfNoneFound = false)
        {
            DbMetaDataCollection dbMetaDataCollection = null;

            if (File.Exists(DbConfigFilePath))
            {
                string fileContents = File.ReadAllText(DbConfigFilePath);
                dbMetaDataCollection = JsonUtility.FromJson<DbMetaDataCollection>(fileContents);
            }

            if (dbMetaDataCollection == null && outputErrorIfNoneFound)
            {
                GRUDebugger.LogError(MessageConfig.Debugging.Error.GET_DB_HANDLER_PRESENT_BUT_NO_DB_CONFIG_JSON_ERR());
            }

            return dbMetaDataCollection;
        }

        public static DbMetaData GetDbConfigEntry(DBHandler dbHandler)
        {
            DbMetaData dbMetaData = null;
            var dbConfig = GetDbConfigData(true);
            if (dbConfig != null)
            {
                try
                {
                    dbMetaData = dbConfig.DbMetaData
                                    .SingleOrDefault(x => x.DbHandlerGOName == dbHandler.gameObject.name);
                }
                catch
                {
                    GRUDebugger.LogError
                    (
                        MessageConfig.Debugging.Error.GET_FAILED_TO_LOAD_METADATA_VIA_DB_HANDLER_GO_ERR(dbHandler.gameObject.name)
                    );
                }
            }
            return dbMetaData;
        }

        public static DbMetaData GetDbConfigEntry(string databaseName)
        {
            DbMetaData dbMetaData = null;
            var dbConfig = GetDbConfigData();
            if (dbConfig != null)
            {
                try
                {
                    dbMetaData = dbConfig.DbMetaData.SingleOrDefault(x => x.DbName == databaseName);
                }
                catch
                {
                    GRUDebugger.LogError
                    (
                        MessageConfig.Debugging.Error.GET_FAILED_TO_LOAD_METADATA_VIA_DB_NAME_ERR(databaseName)
                    );
                }
            }
            return dbMetaData;
        }

        /// <summary>
        /// <u><b>IN PRODUCTION:</b></u>
        /// <br/>
        /// Creates <i> <see cref="PACKAGE_CONFIG_FILE">gru config json </see> </i> on this 
        /// <see cref="GeneralConfig.DynamicCode.GRU_CONFIG_FILE_PATH">PATH</see>
        /// <br/>
        /// <br/>
        /// <u><b>IN DEMO SCENE:</b></u>
        /// <br/>
        /// Creates <i> <see cref="DEMO_PACKAGE_CONFIG_FILE">gru demo config json </see> </i> on this 
        /// <see cref="GeneralConfig.DynamicCode.GRU_CONFIG_FILE_PATH">PATH</see>
        /// <br/>
        /// <br/>
        /// Contains configurations done on a package level
        /// </summary>
        /// <param name="metaData"></param>
        public static void CreatePackageConfig(PackageMetaData metaData)
        {
            string json = JsonUtility.ToJson(metaData, true);
            PackageConfigFilePath.ToJsonOnDisk(json);
            AssetDatabase.Refresh();
        }

        public static PackageMetaData GetPackageConfigData
        (
            bool outputErrorIfNoneFound = false,
            bool createConfigFolderIfNotFound = true
        )
        {
            PackageMetaData packageMetaData = null;

            var configPath = PackageConfigPath;
            if (!Directory.Exists(configPath))
            {
                if (createConfigFolderIfNotFound)
                {
                    Directory.CreateDirectory(configPath);
                    AssetDatabase.Refresh();
                }
            }

            if (File.Exists(PackageConfigFilePath))
            {
                string fileContents = File.ReadAllText(PackageConfigFilePath);
                packageMetaData = JsonUtility.FromJson<PackageMetaData>(fileContents);
            }

            if (packageMetaData == null && outputErrorIfNoneFound)
            {
                GRUDebugger.LogWarning
                (
                    MessageConfig.Debugging.Warning.GET_NO_GRU_CONFIG_JSON_WARNING(),
                    doItalic: true
                );
            }

            return packageMetaData;
        }

        public static void UpdateStringConstantVariableInCSharpScriptFile
        (
            string scriptPath,
            string variableName,
            string variableOldValue,
            string variableNewValue
        )
        {
            string scriptContent = File.ReadAllText(scriptPath);

            var oldVal = $"{variableName} = \"{variableOldValue}\"";
            var newVal = $"{variableName} = \"{variableNewValue}\"";

            string pattern = $@"public\s+const\s+string\s+{variableName}\s+=\s+"".*?"";";
            Match match = Regex.Match(scriptContent, pattern);
            if (match.Success)
            {
                // Replace the old value with the new value
                string oldValue = match.Value;
                string newAssignment = $"public const string {variableName} = \"{variableNewValue}\";";
                scriptContent = scriptContent.Replace(oldValue, newAssignment);

                // Write the modified content back to the file
                File.WriteAllText(scriptPath, scriptContent);
                AssetDatabase.Refresh();

                SpatiumDebuggerBase.LogSuccess($"String constant {nameof(GeneralConfig)}.{variableName} updated successfully!");
            }
            else
            {
                SpatiumDebuggerBase.LogError($"String constant {nameof(GeneralConfig)}.{variableName} not found!");
            }
        }

        public static void UpdateGeneralConfigCSharpScriptFile
        (
            string newGruIntegrationFolderLocation,
            string newGruPackageFolderLocation
        )
        {
            string GRUConfigsFolderPath = $"{Application.dataPath}{GeneralConfig.DynamicCode.PACKAGE_CONFIG_CLASSES_FOLDER_PATH}";
            string generalConfigScriptName = (nameof(GeneralConfig)).AddSuffix(".cs");
            string generalConfigScriptPath = $"{GRUConfigsFolderPath}/{generalConfigScriptName}";

            bool isInProduction = !SceneManager.GetActiveScene().IsGRUDemo();
            if (isInProduction)
            {
                UpdateStringConstantVariableInCSharpScriptFile
                (
                    generalConfigScriptPath,
                    nameof(GeneralConfig.DynamicCode.INTEGRATION_FOLDER_PATH),
                    GeneralConfig.DynamicCode.INTEGRATION_FOLDER_PATH,
                    newGruIntegrationFolderLocation
                );

                UpdateStringConstantVariableInCSharpScriptFile
                (
                    generalConfigScriptPath,
                    nameof(GeneralConfig.DynamicCode.DB_CONTEXT_FILE_PATH),
                    GeneralConfig.DynamicCode.DB_CONTEXT_FILE_PATH,
                    $"{newGruIntegrationFolderLocation}/DbContexts"
                );

                UpdateStringConstantVariableInCSharpScriptFile
                (
                    generalConfigScriptPath,
                    nameof(GeneralConfig.DynamicCode.REPO_FILE_PATH),
                    GeneralConfig.DynamicCode.REPO_FILE_PATH,
                    $"{newGruIntegrationFolderLocation}/Repositories"
                );

                UpdateStringConstantVariableInCSharpScriptFile
                (
                    generalConfigScriptPath,
                    nameof(GeneralConfig.DynamicCode.DB_CONFIG_FILE_PATH),
                    GeneralConfig.DynamicCode.DB_CONFIG_FILE_PATH,
                    $"{newGruIntegrationFolderLocation}/Configs"
                );

                UpdateStringConstantVariableInCSharpScriptFile
                (
                    generalConfigScriptPath,
                    nameof(GeneralConfig.DynamicCode.IREPO_FILE_PATH),
                    GeneralConfig.DynamicCode.IREPO_FILE_PATH,
                    $"{newGruIntegrationFolderLocation}/Repositories"
                );

                UpdateStringConstantVariableInCSharpScriptFile
                (
                    generalConfigScriptPath,
                    nameof(GeneralConfig.DynamicCode.PACKAGE_PATH),
                    GeneralConfig.DynamicCode.PACKAGE_PATH,
                    newGruPackageFolderLocation
                );
            }
        }

        #endregion

#endif
    }
}