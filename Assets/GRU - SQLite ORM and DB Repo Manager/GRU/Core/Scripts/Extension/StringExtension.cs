using System.Globalization;
using System.Linq;
using SpatiumInteractive.Libraries.Unity.GRU.Configs.General;
using SpatiumInteractive.Libraries.Unity.GRU.Scripts.Helpers.Enums;
using SpatiumInteractive.Libraries.Unity.Platform.Extensions;

namespace SpatiumInteractive.Libraries.Unity.GRU.Extensions
{
    public static class StringExtension
    {
        public static string NormalizeNaming(this string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.Trim().Replace(" ", "").ToLower());
        }

        public static string AddBaseInherits(this string str, string classesToInherit)
        {
            return str + " : base".AddInParentheses(classesToInherit);
        }

        public static string AddEmptyBody(this string str, bool newBodyInOneLine = false, int indentLevel = 0)
        {
            var totalTabs = string.Empty;
            if (newBodyInOneLine)
            {
                return str.AddInCurlyBrackets(string.Empty);
            }
            else
            {
                string indent = GetTotalTabs(indentLevel);
                return str + "\n" + indent + "{" + "\n" + indent + "}";
            }
        }

        private static string GetTotalTabs(int indentLevel = 0)
        {
            string tabs = string.Empty;
            for (int i = 0; i < indentLevel; i++)
            {
                tabs += "\t";
            }
            return tabs;
        }

        /// <summary>
        /// e.g.:
        /// <br/>
        /// <b>INPUT</b>: <i>StudentsDatabase</i>
        /// <br/>
        /// <b>OUTPUT</b>: <i>StudentsDatabase.db</i>
        /// </summary>
        /// <param name="databaseNameRaw"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">exception thrown in case databaseNameRaw is not a valid db name (i.e. contains non alpha-numeric characters, for example some special characters) </exception>
        public static string ToDatabaseName(this string databaseNameRaw)
        {
            string dbName = "";
            if (!databaseNameRaw.All(char.IsLetterOrDigit))
                throw new System.ArgumentException
                (
                    "Database name can contain alpha numeric characters only. Please edit database name."
                );
            else
            {
                string dbFileExtension = GeneralConfig.Defaults.DATABASE_FILE_EXTENSION;
                if (!databaseNameRaw.EndsWith(dbFileExtension))
                {
                    dbName = databaseNameRaw.AddSuffix(dbFileExtension);
                }
            }
            return dbName;
        }

        public static string ToPublic(this string objName)
        {
            return $"public {objName} ";
        }

        public static string ToPublicClass(this string className)
        {
            return $"public class {className} ";
        }

        public static string ToPublicInterface(this string interfaceName)
        {
            return $"public interface {interfaceName} ";
        }

        public static string ToPublicSelfPointingProp(this string propTypeName, string propName)
        {
            string publicProp = propTypeName.ToPublic();
            string propDeclaration = $"{publicProp} {propName}";
            return $"{propDeclaration} => this;";
        }

        public static string InheritClass(this string baseString, string className)
        {
            return $"{baseString} : {className}";
        }

        public static string AndInheritInterface(this string baseString, string interfaceName)
        {
            return $"{baseString}, {interfaceName}";
        }

        public static string ToRawTableName(this string tableName)
        {
            string rawTableName = tableName.Contains("(") ? tableName.Split("(")[0] : tableName;
            return rawTableName;
        }

        public static AssemblySelection GetAssemblySelected(this string assemblySelectionTxt)
        {
            if (assemblySelectionTxt == "Outside GRU")
            {
                return AssemblySelection.OutsideGRU;
            }
            else if (assemblySelectionTxt == "Inside GRU")
            {
                return AssemblySelection.InsideGRU;
            }

            return AssemblySelection.Everywhere;
        }

        public static bool IsValidDatabaseName(this string rawDbName)
        {
            bool isValid = rawDbName.All(char.IsLetterOrDigit) && !string.IsNullOrWhiteSpace(rawDbName);
            return isValid;
        }
    }
}
