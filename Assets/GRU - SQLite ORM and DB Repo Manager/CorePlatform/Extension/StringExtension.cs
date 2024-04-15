using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SpatiumInteractive.Libraries.Unity.Platform.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return str == null || str.Length == 0;
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return str == null || string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotNullOrEmpty(this string str)
        {
            return str != null && str.Length > 0;
        }

        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return str != null && !string.IsNullOrWhiteSpace(str);
        }

        public static string AddPrefix(this string str, string prefix)
        {
            return $"{prefix}{str}";
        }

        public static string AddSuffix(this string str, string suffix)
        {
            return $"{str}{suffix}";
        }

        public static string AddSuffix(this string str, int suffix)
        {
            return str.AddSuffix(suffix.ToString());
        }

        public static string AddSpace(this string str)
        {
            return str + " ";
        }

        /// <summary>
        /// wraps the content in (...) brackets
        /// and appends to the end of str
        /// </summary>
        /// <param name="str"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AddInParentheses(this string str, string content)
        {
            return $"{str}({content})";
        }

        /// <summary>
        /// wraps the content in {...} brackets
        /// and appends to the end of str
        /// </summary>
        /// <param name="str"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AddInCurlyBrackets(this string str, string content)
        {
            return str + "{" + content + "}";
        }

        /// <summary>
        /// wraps the content in <...> brackets
        /// and appends to the end of str
        /// </summary>
        /// <param name="str"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AddInAngleBrackets(this string str, string content)
        {
            return $"{str}<{content}>";
        }

        /// <summary>
        /// wraps the content in [...] brackets
        /// and appends to the end of str
        /// </summary>
        /// <param name="str"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string AddInSquareBrackets(this string str, string content)
        {
            return $"{str}[{content}]";
        }

        public static string WrapInParentheses(this string str)
        {
            return $"({str})";
        }

        public static string WrapInCurlyBrackets(this string str)
        {
            return "{" + str + "}";
        }

        public static string WrapInAngleBrackets(this string str)
        {
            return $"<{str}>";
        }

        public static string WrapInSquareBrackets(this string str)
        {
            return $"[{str}]";
        }

        public static string WrapInItalicTags(this string str)
        {
            return $"<i>{str}</i>";
        }

        public static string WrapInBoldTags(this string str)
        {
            return $"<b>{str}</b>";
        }

        public static string WrapInSpaces(this string str)
        {
            return $" {str} ";
        }

        public static DirectoryInfo ToPathOnDisk(this string path)
        {
            var directoryInfo = Directory.CreateDirectory(path);
            return directoryInfo;
        }

        public static TextWriter ToFileOnDisk(this string pathWithFileName, StringBuilder textBuild)
        {
            using (StreamWriter f = new StreamWriter(pathWithFileName))
            {
                f.WriteLine(textBuild.ToString());
                return f;
            }
        }

        public static void ToJsonOnDisk(this string pathWithFileName, string json)
        {
            File.WriteAllText(pathWithFileName, json);
        }

        public static bool IsAllUpper(this string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }

        public static bool IsAllLower(this string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsLower(input[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// converts datetime string to date. Supported formats currently :
        /// dd/MM/yyyy 
        /// dd.MM.yyyy
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime? ToDate(this string str)
        {
            DateTime? date = null;
            str = str.Trim();
            str = str.Replace(" ", "");
            string[] splitted = new string[] { };
            if (str.Contains('.'))
            {
                splitted = str.Split('.');

            }
            else if (str.Contains('/'))
            {
                splitted = str.Split('/');
            }

            if (splitted.Length > 0)
            {
                int day = Convert.ToInt32(splitted[0]);
                int month = Convert.ToInt32(splitted[1]);
                int year = Convert.ToInt32(splitted[2].Substring(0, 4));
                date = new DateTime(year, month, day);
            }
            return date;
        }

        public static T? GetValueOrNull<T>(this string valueAsString)
            where T : struct
        {
            if (string.IsNullOrEmpty(valueAsString))
                return null;
            return (T)Convert.ChangeType(valueAsString, typeof(T));
        }

        public static string SafeReplace(this string input, string find, string replace, bool matchWholeWord = true)
        {
            string textToFind = matchWholeWord ? string.Format(@"\b{0}\b", find) : find;
            string res = Regex.Replace(input, textToFind, replace);
            return res;
        }

        /// <summary>
        /// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
        /// The comparison is case-insensitive, handles / and \ slashes as folder separators and
        /// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
        /// </summary>
        public static bool IsSubPathOf(this string path, string baseDirPath)
        {
            string normalizedPath = Path.GetFullPath(path.Replace('/', '\\')
                .WithEnding("\\"));

            string normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\')
                .WithEnding("\\"));

            return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
        /// results in satisfying .EndsWith(ending).
        /// </summary>
        /// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
        public static string WithEnding(this string str, string ending)
        {
            if (str == null)
                return ending;

            string result = str;

            // Right() is 1-indexed, so include these cases
            // * Append no characters
            // * Append up to N characters, where N is ending length
            for (int i = 0; i <= ending.Length; i++)
            {
                string tmp = result + ending.Right(i);
                if (tmp.EndsWith(ending))
                    return tmp;
            }

            return result;
        }

        /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
        /// <param name="value">The string to retrieve the substring from.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        public static string Right(this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException
                (
                    "length", 
                    length, 
                    "Length is less than zero"
                );
            }

            return (length < value.Length) ? value.Substring(value.Length - length) : value;
        }
    }
}

