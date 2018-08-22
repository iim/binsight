using System;
using System.Linq;
using APKInsight.Enums;
using APKInsight.Logic.ContentParsing.JavaObjects;

namespace APKInsight.Logic.ContentParsing.SmaliParser
{
    /// <summary>
    /// Static smali parsing logic
    /// </summary>
    internal static class SmaliParserUtils
    {

        /// <summary>
        /// Parses Java access control for a line that defines type name, method, field, property.
        /// </summary>
        /// <param name="line">Source code line</param>
        /// <returns>Access Control type enum value</returns>
        public static JavaAccessControl GetAccessControl(string line)
        {
            if (SmaliParser.IsPublicOptionPresent(line))
                return JavaAccessControl.Public;
            if (SmaliParser.IsPrivateOptionPresent(line))
                return JavaAccessControl.Private;
            if (SmaliParser.IsProtectedOptionPresent(line))
                return JavaAccessControl.Protected;
            return JavaAccessControl.PackagePrivate;
        }

        /// <summary>
        /// Get full name of the type.
        /// This function will also expand names of the basic types in Java (e.g., int) and highlights whenever or not something is an array.
        /// </summary>
        /// <param name="smaliTypeName">Smali type to be expanded</param>
        /// <returns>Name of the expanded class</returns>
        public static string GetTypeNameFull(string smaliTypeName)
        {
            int arraysBracketCount = 0;
            while (smaliTypeName[arraysBracketCount] == '[')
            {
                arraysBracketCount++;
            }
            var name = smaliTypeName;
            if (arraysBracketCount > 0)
            {
                name = name.Substring(arraysBracketCount);
            }
            // Special type
            if (name == "Z") return "bool";
            if (name == "C") return "char";
            if (name == "D") return "double";
            if (name == "F") return "float";
            if (name == "I") return "int";
            if (name == "J") return "long";
            if (name == "S") return "short";
            if (name == "V") return "void";
            if (name == "B") return "byte";
            if (name[0] == 'L') name = name.Substring(1);
            name = name.TrimEnd(';');
            for (int i = 0; i < arraysBracketCount; i++)
                name = name + "[]";

            return name.Replace("/", ".").Replace("$", ".");
        }

        /// <summary>
        /// Gets a short name of the type
        /// </summary>
        /// <param name="smaliTypeNameFull">Fullname of the type</param>
        /// <returns>Returns name of that type. If that type is declared inside another class, it will strip that name away too.</returns>
        public static string GetTypeNameShort(string smaliTypeNameFull)
        {
            if (smaliTypeNameFull.IndexOf("/", StringComparison.Ordinal) == -1)
                return smaliTypeNameFull.Substring(1).TrimEnd(';');

            return smaliTypeNameFull.Substring(smaliTypeNameFull.LastIndexOf("/", StringComparison.Ordinal) + 1).TrimEnd(';');
        }

        /// <summary>
        /// Gets a short name of the type
        /// </summary>
        /// <param name="smaliTypeNameFull">Fullname of the type</param>
        /// <returns>Returns name of that type. If that type is declared inside another class, it will strip that name away too.</returns>
        public static string GetTypeNameShortShort(string smaliTypeNameFull)
        {
            var name = smaliTypeNameFull;
            if (name.IndexOf("/", StringComparison.Ordinal) != -1)
            {
                name = name.Substring(name.LastIndexOf("/", StringComparison.Ordinal) + 1);
            }
            else
            {
                name = name.Substring(1); // Remove leading "L"
            }

            if (name.LastIndexOf("$", StringComparison.Ordinal) > -1)
                name = name.Substring(name.LastIndexOf("$", StringComparison.Ordinal) + 1);
            return name.TrimEnd(';');
        }

        public static void SetNameToJavaType(ref JavaType javaType, string smaliName)
        {
            javaType.NameFullSmali = smaliName.TrimStart('[');
            javaType.PackageName = SmaliParser.GetPackageName(smaliName.TrimStart('['));
        }

        public static void SetNameToJavaType(ref JavaTypeBaseInfo javaType, string smaliName)
        {
            javaType.NameFullSmali = smaliName.TrimStart('[');
            javaType.PackageName = SmaliParser.GetPackageName(smaliName.TrimStart('['));
        }

        public static JavaTypeBaseInfo GetShallowJavaType(string smaliName)
        {
            return new JavaTypeBaseInfo
            {
                NameFullSmali = smaliName.TrimStart('['),
                PackageName = SmaliParser.GetPackageName(smaliName.TrimStart('['))
            };
        }

    }
}
