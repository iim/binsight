using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using APKInsight.Controls.DisplayObjects;
using APKInsight.Globals;
using APKInsight.Logic.ContentParsing.SmaliParser;

namespace APKInsight.Syntaxis
{
    /// <summary>
    /// Formats source code text into  
    /// </summary>
    /// <remarks>More details are at http://latex2rtf.sourceforge.net/rtfspec.html </remarks>
    internal static class Smali2RtfFormatter
    {

        #region Public static functions

        public static string FormatSourceCode(BioDisplayInfo bioDisplayInfo, JavaTypeDisplayInfo displayInfo)
        {
            StringBuilder result = new StringBuilder(128000);
            result.Append(@"{\rtf {\fonttbl {\f0 Consolas;}}\f0\fs20 ");
            AddColourTable(result);

            var lines = displayInfo.SourceCode.Split('\n');
            bool firstLine = true;
            foreach (var line in lines)
            {
                if (!firstLine)
                    result.Append(@"\line");

                result.Append(FormatSoruceCodeLine(line, bioDisplayInfo, displayInfo));

                firstLine = false;
            }
            result.Append("}");
            return result.ToString();
        }

        #endregion


        #region Rtf Initialization functions

        private static void AddColourTable(StringBuilder builder)
        {
            builder.Append(@"{\colortbl ");
            builder.Append(@"\red0\green0\blue0;"); //cf0
            builder.Append(@"\red0\green0\blue255;"); // cf1
            builder.Append(@"\red0\green128\blue0;"); // cf2
            builder.Append(@"\red255\green51\blue0;"); // cf3
            builder.Append(@"\red255\green0\blue0;"); // cf4
            builder.Append(@"}");
        }

        #endregion


        #region Formatting logic

        private static string FormatSoruceCodeLine(string line, BioDisplayInfo bioDisplayInfo, JavaTypeDisplayInfo displayInfo)
        {
            
            if (IsComment(line))
                return FormatComment(line);

            var result = EscapeSpecialRtfSymbols(line);
            result = FormatKeywords(result);
            result = FormatStringLiterals(result);

            if (SmaliParser.IsFieldLine(line))
                result = FormatField(result);

            if (SmaliParser.IsInvokeLine(line))
                result = FormatInvoke(result);

            if (SmaliParser.IsMethodStartLine(line))
            {
                var className = PathResolver.GetJavaTypeSmaliName(displayInfo.JavaType.SmaliFullNameId.Value);
                var methodName = SmaliParser.GetFullMethodName(line, className.Value);
                var method = displayInfo.Internals.FirstOrDefault(i => i.SmaliName == methodName);
                if (method != null)
                {
                    var useCases = displayInfo.MethodUseCases.Where(uc => uc.DestinationMethodId == method.UId);
                    if (useCases.Any())
                    {
                        result += $@" - \cf4\ul Used in {useCases.Count()} case(s)\ul0\cf0";
                    }
                }
            }

            return result;
        }

        private static string EscapeSpecialRtfSymbols(string line)
        {
            return line.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}");
        }

        #endregion


        #region Coloring logic

        private static string FormatKeyword(Match m) => $@"\cf1{m.Value}\cf0 ";
        private static string FormatStringLiteral(Match m) => $@"\cf3{m.Value}\cf0 ";

        private static string FormatComment(string line) => $@"\cf2{line}\cf0 ";

        private static string FormatKeywords(string line)
        {
            return Regex.Replace(line, @"(?<=\s)(public|private|protected|abstract|final|annotation|interface|static|system|synthetic|constructor)(?=\s)", FormatKeyword);
        }

        private static string FormatStringLiterals(string line)
        {
            return Regex.Replace(line, @""".+?""", FormatStringLiteral);
        }

        private static bool IsComment(string line) => line.StartsWith("#");

        #endregion


        #region Formtting specific lines

        private static string FormatField(string line)
        {
            Match fieldName = Regex.Match(line, @"(?<=\s)[^\s]+(?=:)");
            StringBuilder result = new StringBuilder();
            result.Append(line.Substring(0, fieldName.Index));
            result.Append(fieldName.Value + ":");
            result.Append(@"\ul " + SmaliParser.GetFieldTypeName(line) + @"\ul0");
            return result.ToString();
        }

        private static string FormatInvoke(string line)
        {
            Match invoke = Regex.Match(line, @"(?<=\s)[^\s]+($|\r|\n)");
            StringBuilder result = new StringBuilder();
            result.Append(line.Substring(0, invoke.Index));
            result.Append(@"\ul " + invoke.Value + @"\ul0");
            return result.ToString();
        }

        #endregion


        public static bool IsClickable(string line, int inlineIdx)
        {
            if (SmaliParser.IsFieldLine(line))
            {
                Match fieldType = Regex.Match(line, @"(?<=:).+$");
                return (fieldType.Index <= inlineIdx) && 
                    (fieldType.Index + fieldType.Length >= inlineIdx);
            }
            if (SmaliParser.IsInvokeLine(line))
            {
                Match invokeType = Regex.Match(line, @"(?<=\s)[^\s]+$");
                return (invokeType.Index <= inlineIdx) &&
                    (invokeType.Index + invokeType.Length >= inlineIdx);
            }
            if (SmaliParser.IsMethodStartLine(line))
            {
                return SmaliParser.GetMethodNameEndIndex(line) < inlineIdx;
            }

            return false;
        }

    }
}
