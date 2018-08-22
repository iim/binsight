using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace APKInsight.Logic.ContentParsing.SmaliParser
{
    /// <summary>
    /// Class that actually does paring of a string of smali code
    /// </summary>
    public partial class SmaliParser
    {

        private const string _methodParsingRegEx = @"(?<=\s)[^\s]+\([^\s]*\)[^\s]+(?=($|\r|\n|\s))";

        #region Line type validation functions

        /// <summary>
        /// Returns true if the line is an invokation line
        /// </summary>
        public static bool IsInvokeLine(string line)
        {
            var m = Regex.Match(line, @"[\s]*invoke-(direct|interface|static|virtual|super)(\s|/range\s)");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line is a get line
        /// </summary>
        public static bool IsGetLine(string line)
        {
            var m = Regex.Match(line, @"^[\s]*[is]get[^\s]+\s");
            var m2 = Regex.Match(line, @"\sL[^\s]+;->[^\s]+:[^\s]+(?:$|\r|\n)");
            return m.Success && m2.Success;
        }

        /// <summary>
        /// Returns true if the line is a put line
        /// </summary>
        public static bool IsPutLine(string line)
        {
            var m = Regex.Match(line, @"^[\s]*[is]put[^\s]+\s");
            var m2 = Regex.Match(line, @"\sL[^\s]+;->[^\s]+:[^\s]+(?:$|\r|\n)");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line is a class name definition line
        /// </summary>
        public static bool IsClassNameLine(string line)
        {
            var m = Regex.Match(line, @"^\.class\s");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line is a source filename definition line
        /// </summary>
        public static bool IsSourceFileNameLine(string line)
        {
            var m = Regex.Match(line, @"^\.source\s");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line is a super class name definition line
        /// </summary>
        public static bool IsSuperClassNameLine(string line)
        {
            var m = Regex.Match(line, @"^\.super\s");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line is a implemented interface definition line
        /// </summary>
        public static bool IsImplementsInterfaceNameLine(string line)
        {
            var m = Regex.Match(line, @"^\.implements\s");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line is a field definition line
        /// </summary>
        public static bool IsFieldLine(string line)
        {
            var m = Regex.Match(line, @"^\.field\s");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line is a field definition line
        /// </summary>
        public static bool IsMethodStartLine(string line)
        {
            var m = Regex.Match(line, @"^\.method\s");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line is a field definition line
        /// </summary>
        public static bool IsMethodEndLine(string line)
        {
            var m = Regex.Match(line, @"^\.end method(?=$|\r|\n)");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line is a class name definition line
        /// </summary>
        public static bool IsInOuterClass(string line)
        {
            var m = Regex.Match(line, @"(?<=/)[^\s/]+\$[^\s/]+(?=;($|\r|\n))");
            return m.Success;
        }

        /// <summary>
        /// Returns true if the line contains "abstract" option
        /// </summary>
        public static bool IsAbstractOptionPresent(string line)
        {
            return IsOptionPresent(line, "abstract");
        }

        /// <summary>
        /// Returns true if the line contains "interface" option
        /// </summary>
        public static bool IsInterfaceOptionPresent(string line)
        {
            return IsOptionPresent(line, "interface");
        }

        /// <summary>
        /// Returns true if the line contains "enum" option
        /// </summary>
        public static bool IsEnumOptionPresent(string line)
        {
            return IsOptionPresent(line, "enum");
        }

        /// <summary>
        /// Returns true if the line contains "final" option
        /// </summary>
        public static bool IsFinalOptionPresent(string line)
        {
            return IsOptionPresent(line, "final");
        }

        /// <summary>
        /// Returns true if the line contains "annotation" option
        /// </summary>
        public static bool IsAnnotationOptionPresent(string line)
        {
            return IsOptionPresent(line, "annotation");
        }

        /// <summary>
        /// Returns true if the line contains "static" option
        /// </summary>
        public static bool IsStaticOptionPresent(string line)
        {
            return IsOptionPresent(line, "static");
        }

        /// <summary>
        /// Returns true if the line contains "public" option
        /// </summary>
        public static bool IsPublicOptionPresent(string line)
        {
            return IsOptionPresent(line, "public");
        }

        /// <summary>
        /// Returns true if the line contains "private" option
        /// </summary>
        public static bool IsPrivateOptionPresent(string line)
        {
            return IsOptionPresent(line, "private");
        }

        /// <summary>
        /// Returns true if the line contains "protected" option
        /// </summary>
        public static bool IsProtectedOptionPresent(string line)
        {
            return IsOptionPresent(line, "protected");
        }

        /// <summary>
        /// Returns true if the line contains "volatile" option
        /// </summary>
        public static bool IsVolatileOptionPresent(string line)
        {
            return IsOptionPresent(line, "volatile");
        }

        /// <summary>
        /// Returns true if the line contains "synthetic" option
        /// </summary>
        public static bool IsSyntheticOptionPresent(string line)
        {
            return IsOptionPresent(line, "synthetic");
        }

        /// <summary>
        /// Returns true if the line contains "constructor" option
        /// </summary>
        public static bool IsConstructorOptionPresent(string line)
        {
            return IsOptionPresent(line, "constructor");
        }

        /// <summary>
        /// Checks if an option is present in a smali code line
        /// </summary>
        /// <param name="line">The line to check in</param>
        /// <param name="option">The name of the option</param>
        /// <returns>True if present, false otherwise</returns>
        private static bool IsOptionPresent(string line, string option)
        {
            var m = Regex.Match(line, $@"\s{option}\s");
            return m.Success;
        }

        #endregion


        #region Line data extraction functions

        /// <summary>
        /// Returns the name of the method called
        /// </summary>
        public static string GetInvokedMethodFullName(string line)
        {
            var m = Regex.Match(line, @"(?<=\s)[^\s]+(?=$|\r|\n)");
            return m.Value;
        }

        /// <summary>
        /// Returns the name of the type in which the called method is defined
        /// </summary>
        public static string GetInvokedMethodTypeInFullName(string line)
        {
            var m = Regex.Match(line, @"(?<=\s)L[^\s]+;(?=->)");
            return m.Value;
        }

        /// <summary>
        /// Returns the name of the method called
        /// </summary>
        public static string GetAccessedMemberName(string line)
        {
            var m = Regex.Match(line, @"(?<=\s)L[^\s]+;->[^\s]+:[^\s]+(?=$|\r|\n)");
            return m.Value;
        }

        /// <summary>
        /// Returns the name of the type in which the accessed member is defined
        /// </summary>
        public static string GetAccessedMemberTypeInName(string line)
        {
            var m = Regex.Match(line, @"(?<=\s)L[^\s]+;(?=->)");
            return m.Value;
        }


        /// <summary>
        /// Returns the name of the class
        /// </summary>
        public static string GetClassName(string line)
        {
            var m = Regex.Match(line, @"(?<=\s)[^\s]+(?=$|\r|\n)");
            return m.Value;
        }

        /// <summary>
        /// Returns the filename defined in the line
        /// </summary>
        public static string GetFileName(string line)
        {
            var m = Regex.Match(line, @"(?<="")[^\s]+(?="")");
            return m.Value;
        }

        /// <summary>
        /// Get names of all outer classes
        /// </summary>
        /// <param name="innerClassName">The most inner class (i.e, the leaf child)</param>
        /// <returns>List of all class names that are outer</returns>
        public static List<string> GetAllOuterClassNames(string innerClassName)
        {
            var result = new List<string>();
            var remaining = innerClassName;
            while (IsInOuterClass(remaining))
            {
                remaining = remaining.Substring(0, remaining.LastIndexOf("$", StringComparison.Ordinal)).TrimEnd('$') + ";";
                result.Add(remaining);
            }

            return result;
        }

        /// <summary>
        /// Get the name of a field
        /// </summary>
        /// <param name="line">Field definition line</param>
        /// <returns>Extracted name of the field</returns>
        public static string GetFieldName(string line)
        {
            var m = Regex.Match(line, @"(?<=\s)[^\s]+(?=:)");
            return m.Value;
        }

        /// <summary>
        /// Gets the full name of a field (including name of the class and accessor symbol)
        /// </summary>
        /// <param name="fieldDefLine">Field definition line</param>
        /// <param name="className">Name of the class</param>
        public static string GetFullFieldName(string fieldDefLine, string className)
        {
            return className + "->" + GetFieldName(fieldDefLine) + ":" + GetFieldTypeName(fieldDefLine);
        }

        /// <summary>
        /// Get the name of a field's type
        /// </summary>
        /// <param name="line">Field definition line</param>
        /// <returns>Extracted type name of the field</returns>
        public static string GetFieldTypeName(string line)
        {
            var m = Regex.Match(line, @"(?<=:)[^\s]+(?=(\s|$|\r|\n))");
            return m.Value;
        }

        /// <summary>
        /// Get field's initialization value
        /// </summary>
        /// <param name="line">Field definition line</param>
        /// <returns>Extracted type name of the field</returns>
        public static string GetFieldInitValue(string line)
        {
            var m = Regex.Match(line, @"(?<==\s)[^\r\n]+");
            return m.Success? m.Value : "";
        }

        /// <summary>
        /// Get the name of a method
        /// </summary>
        /// <param name="line">Method definition line</param>
        /// <returns>Extracted name of the method</returns>
        public static string GetMethodName(string line)
        {
            var m = Regex.Match(line, _methodParsingRegEx);
            return m.Value;
        }

        /// <summary>
        /// Get the index at the line where the method line ends
        /// </summary>
        /// <param name="line">Method definition line</param>
        /// <returns>Extracted name of the method</returns>
        public static int GetMethodNameEndIndex(string line)
        {
            var m = Regex.Match(line, _methodParsingRegEx);
            return m.Index + m.Length;
        }

        /// <summary>
        /// Get the name of a method
        /// </summary>
        /// <param name="line">Method definition line</param>
        /// <param name="className">Full class name the method is defined in</param>
        /// <returns>Extracted name of the method</returns>
        public static string GetFullMethodName(string line, string className)
        {
            return className + "->" + GetMethodName(line);
        }

        /// <summary>
        /// Get the short name of a method
        /// </summary>
        /// <param name="line">Method definition line</param>
        /// <returns>Extracted name of the method</returns>
        public static string GetShortMethodName(string line)
        {
            var m = Regex.Match(line, @"(?<=\s)[^\s]+(?=\()");
            return m.Value;
        }

        /// <summary>
        /// Get the method's display name of a method
        /// </summary>
        /// <param name="line">Method definition line</param>
        /// <returns>Extracted name of the method</returns>
        public static string GetMethodDisplayNameFromCallLine(string line)
        {
            var m = Regex.Match(line, @"(?<=->)[^\s]+(?=\r|\n|$)");
            return m.Value;
        }

        /// <summary>
        /// Get the name of the return type for a method
        /// </summary>
        /// <param name="line">Method definition line</param>
        /// <returns>Extracted name of the method</returns>
        public static string GetReturnTypeName(string line)
        {
            var m = Regex.Match(line, @"(?<=\))[^\s]+(?=($|\r|\n))");
            return m.Value;
        }

        /// <summary>
        /// Gets the names of all input parameters from a method definition line
        /// </summary>
        /// <param name="line">Method definition line</param>
        /// <returns>List of all parameters</returns>
        public static List<string> GetInputParametersTypes(string line)
        {
            var result = new List<string>();
            var m = Regex.Match(line, @"(?:\()(?<param>[\[]*([BCDFIJSVZ]|L[^;]+;))*(?:\))");
            var grp = m.Groups["param"];
            foreach (Capture c in grp.Captures)
            {
                result.Add(c.Value);
            }
            return result;
        }

        #endregion


        #region String conversion


        /// <summary>
        /// Converts a smali name into a package name.
        /// </summary>
        /// <param name="smaliName">Smali formatted name.</param>
        /// <returns>A string with package name.</returns>
        public static string GetPackageName(string smaliName)
        {
            // Lock for the last "/" char, only if found that we have a package name
            int idx = smaliName.LastIndexOf("/", StringComparison.Ordinal);
            if (idx > 0)
            {
                return smaliName
                    .Substring(0, idx)
                    .Replace("/", ".")
                    .Substring(1);
            }
            // If we don't have that symbol, then it means we have only object name.
            return "";
        }

        #endregion
    }
}
