#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SpatiumInteractive.Libraries.Unity.GRU.Base;
using SpatiumInteractive.Libraries.Unity.Platform.CustomEditor.Window;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Enums;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.Messages;

namespace SpatiumInteractive.Libraries.Unity.GRU.Helpers.Bootstrapping
{
    public abstract class GRUEditorWindowBase : EditorWindowBase
    {

        #region Protected Props

        protected float ScreenHeight
        {
            get
            {
                return Screen.currentResolution.height;
            }
        }

        protected float ScreenWidth
        {
            get
            {
                return Screen.currentResolution.width;
            }
        }

        #endregion

        #region Protected Methods 

        protected bool CheckForHookableProgramsRunning()
        {
            string[] hokablesRunning;
            if (AreAnyProblematicThirdPartyProgramsRunning(out hokablesRunning))
            {
                string listOfHookablesRunning = string.Empty;
                foreach (var hookable in hokablesRunning)
                {
                    listOfHookablesRunning += $"\n **{hookable}.exe";
                }

                EditorUtility.DisplayDialog
                (
                    MessageConfig.GRUConfigureWindow.Warning.CLOSE_RUNNING_PROGRAMS,
                    $"{MessageConfig.GRUConfigureWindow.Warning.RUNNING_PROGRAMS_DETECTED}\n {listOfHookablesRunning}",
                    MessageConfig.Generic.OK
                );

                return true;
            }

            return false;
        }

        /// <summary>
        /// returns list of table names of all tables that are in 
        /// newTableNames, but are not in currentTableNames
        /// </summary>
        /// <param name="currentTableNames"></param>
        /// <param name="newTableNames"></param>
        /// <returns></returns>
        protected List<string> GetNewTables(List<string> currentTableNames, List<string> newTableNames)
        {
            var newTables = new List<string>();
            foreach (var tableName in newTableNames)
            {
                if (!currentTableNames.Contains(tableName))
                {
                    newTables.Add(tableName);
                }
            }
            return newTables;
        }

        /// <summary>
        /// returns list of entitites (tables) which are visible and
        /// have been checked (selected) in the passed checkbox list 
        /// of database tables
        /// </summary>
        /// <param name="tablesCbxList">checkbox list of database tables</param>
        /// <returns></returns>
        protected List<EntityBase<int>> GetSelectedTables(List<Toggle> tablesCbxList)
        {
            var selectedTables = new List<EntityBase<int>>();
            List<Toggle> allSelectedCheckboxes =
                tablesCbxList
                    .Where(x => x.value && x.visible)
                    .ToList();
            foreach (var selectedTableCbx in allSelectedCheckboxes)
            {
                var tableEntity = (EntityBase<int>)selectedTableCbx.userData;
                selectedTables.Add(tableEntity);
            }
            return selectedTables;
        }

        /// <summary>
        /// returns string list of table names of tables that have been selected (checked)
        /// in the passed checkbox list of database tables
        /// </summary>
        /// <param name="tablesCbxList"></param>
        /// <param name="getPureNameOnly"></param>
        /// <returns></returns>
        protected List<string> GetSelectedTableNames
        (
            List<Toggle> tablesCbxList,
            bool getPureNameOnly = true
        )
        {
            List<string> selectedTableNames = new List<string>();
            var selectedTableEntities = GetSelectedTables(tablesCbxList);
            foreach (var entity in selectedTableEntities)
            {
                var table = entity.GetType().Name;
                string displaySuffix =
                    getPureNameOnly ?
                        string.Empty :
                        table.AddSuffix("Repository").WrapInParentheses();
                var tableName = $"{table}{displaySuffix}";
                selectedTableNames.Add(tableName);
            }
            return selectedTableNames;
        }

        /// <summary>
        /// returns list of table names extracted 
        /// from passed list of <i>entities </i> (tables basically)
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        protected List<string> GetTableNames
        (
            List<EntityBase<int>> entities,
            bool getPureNameOnly = true
        )
        {
            List<string> tableNames = new List<string>();
            foreach (var entity in entities)
            {
                var table = entity.GetType().Name;
                string displaySuffix =
                    getPureNameOnly ?
                        string.Empty :
                        table.AddSuffix("Repository").WrapInParentheses();
                var tableName = table + displaySuffix;
                tableNames.Add(tableName);
            }
            return tableNames;
        }

        /// <summary>
        /// scans the assembly looking for entity base classes, table 
        /// classes basically, and returns a string list of their names
        /// </summary>
        /// <param name="getPureNameOnly">
        /// <u>true</u>: returns a list of raw table names, <br/>
        /// <u>false</u>: returns a list of table names with <i>(Repository)</i> suffix added to each</param>
        /// <returns></returns>
        protected List<string> GetAllTableNames
        (
            bool getPureNameOnly = true,
            AssemblySelection assemblySelected = AssemblySelection.Everywhere
        )
        {
            List<string> allTableNames = new List<string>();
            var allTableEntities = GetAllTablesForDb(assemblySelected);
            foreach (var entity in allTableEntities)
            {
                var table = entity.GetType().Name;
                string displaySuffix =
                    getPureNameOnly ?
                        string.Empty :
                        table.AddSuffix("Repository").WrapInParentheses();
                var tableName = table + displaySuffix;
                allTableNames.Add(tableName);
            }
            return allTableNames;
        }

        protected List<string> GetAllTableNames(string databaseName)
        {
            var dbMetaData = FilesManager.GetDbConfigEntry(databaseName);
            return dbMetaData.TableNames;
        }

        /// <summary>
        /// scans the assembly looking for entity base classes, 
        /// table classes basically, and returns them as a list of objects
        /// </summary>
        /// <returns></returns>
        protected List<object> GetAllDbTablesAsObjects()
        {
            List<object> allPossibleDbTables = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(EntityBase<int>)))
                .Select(type => Activator.CreateInstance(type) as object).ToList();
            return allPossibleDbTables;
        }

        /// <summary>
        /// scans the assembly looking for entity base classes, 
        /// table classes basically, and returns them as a list of entity bases
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<EntityBase<int>> GetAllTablesForDb
        (
            AssemblySelection assemblySelected = AssemblySelection.Everywhere
        )
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            switch (assemblySelected)
            {
                case AssemblySelection.OutsideGRU:
                    assemblies = assemblies
                        .Where(x => !x.FullName.Contains("GRU"))
                        .ToArray();
                    break;
                case AssemblySelection.InsideGRU:
                    assemblies = assemblies
                        .Where(x => x.FullName.Contains("GRU"))
                        .ToArray();
                    break;
            }

            return assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(EntityBase<int>)))
                .Select(type => Activator.CreateInstance(type) as EntityBase<int>);
        }

        protected string GetNewDbHandlerGOName(string databaseName)
        {
            var dbConfig = FilesManager.GetDbConfigData();
            bool isFirst = dbConfig == null || dbConfig.DbMetaData == null;
            int id = isFirst ? 1 : dbConfig.DbMetaData.Count + 1;
            string goName = $"DbHandler_{databaseName.AddSuffix(id)}";

            GameObject instanceWithName = GameObject.Find(goName);
            if (instanceWithName != null)
            {
                goName += " (new)";
            }

            return goName;
        }

        protected bool ValidateIfOverrideRepoCanceled(Toggle overrideRepoCbx, List<string> selectedTableNames)
        {
            bool hasCanceledOverride = false;
            bool shouldOverrideRepos = overrideRepoCbx.value;

            if (shouldOverrideRepos)
            {
                bool hasSelectedInUseTable = CheckIfTablesAlreadyInSomeDatabase(selectedTableNames);
                if (hasSelectedInUseTable)
                {
                    bool doOverride = EditorUtility.DisplayDialog
                    (
                        MessageConfig.Generic.WARNING,
                        MessageConfig.GRUWindowBase.Warning.GET_SCRIPTS_WILL_GET_OVERRIDEN_WARN(),
                        MessageConfig.GRUWindowBase.Warning.CONFIRM_SCRIPTS_OVERRIDE,
                        MessageConfig.Generic.CANCEL
                    );
                    hasCanceledOverride = !doOverride;
                }
            }

            return hasCanceledOverride;
        }

        protected bool CheckIfTablesAlreadyInSomeDatabase(List<string> selectedTableNames)
        {
            var dbConfig = FilesManager.GetDbConfigData();
            if
            (
                dbConfig == null ||
                dbConfig.DbMetaData == null ||
                !dbConfig.DbMetaData.Any()
            )
            {
                return false;
            }
            else
            {
                foreach (var dbMetaDataEntry in dbConfig.DbMetaData)
                {
                    foreach (var table in dbMetaDataEntry.TableNames)
                    {
                        if (selectedTableNames.Contains(table))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// <u><b>IN PRODUCTION:</b></u>
        /// <br/>
        /// <see cref="FilesManager.DB_CONFIG_FILE">db config json used</see>
        /// <br/>
        /// <br/>
        /// <u><b>IN DEMO SCENE:</b></u>
        /// <br/>
        /// <see cref="FilesManager.DEMO_DB_CONFIG_FILE">demo db config json used</see>
        /// <br/>
        /// <br/>
        /// pulls db names from the above-mentioned <i>db config json</i> file
        /// </summary>
        /// <returns></returns>
        protected List<string> GetExistingDatabasesNames()
        {
            List<string> databaseNames = new List<string>();
            var dbConfig = FilesManager.GetDbConfigData();
            if (dbConfig != null && dbConfig.DbMetaData != null)
            {
                foreach (var dbMetaData in dbConfig.DbMetaData)
                {
                    databaseNames.Add(dbMetaData.DbName);
                }
            }

            return databaseNames;
        }

        #endregion

        #region Private Methods

        private bool AreAnyProblematicThirdPartyProgramsRunning(out string[] hookableProgramsRunning)
        {
            var hookablesRunning = new List<string>();

            try
            {
                Process[] processes = Process.GetProcesses();

                foreach (var process in processes)
                {
                    string processName = process.ProcessName.ToLower().Trim();

                    if (processName.Contains("microsoft.servicehub.controller"))
                    {
                        hookablesRunning.Add("Visual Studio");
                    }
                    else if (processName.Contains("db browser"))
                    {
                        hookablesRunning.Add("Db Browser SQLite");
                    }
                    else if (processName.Contains("dbschema"))
                    {
                        hookablesRunning.Add("Db Schema");
                    }
                    else if (processName.Contains("notepad"))
                    {
                        hookablesRunning.Add("Notepad");
                    }
                }
            }
            catch { }

            hookableProgramsRunning = hookablesRunning.ToArray();
            bool anyFound = hookablesRunning.Any();

            return anyFound;
        }

        #endregion

    }
}

#endif
