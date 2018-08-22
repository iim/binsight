using System;
using System.Collections.Generic;
using System.Linq;
using APKInsight.Logic.ContentParsing.JavaObjects;

namespace APKInsight.Logic.ContentParsing.SmaliParser
{
    /// <summary>
    /// Part of the SmaliParser which is responsible for Methods parsing
    /// </summary>
    public partial class SmaliParser
    {

        /// <summary>
        /// Parses all methods
        /// </summary>
        private void ParseAllMethods()
        {
            JavaType.Methods = new List<JavaTypeMethod>();
            var methodBegIndices = new List<int>();
            var methodEndIndices = new List<int>();

            GetBegEndIndicesForMethods(ref methodBegIndices, ref methodEndIndices);

            for (int i = 0; i < methodBegIndices.Count; i++)
            {
                // Get beg/end indices
                var methodBeg = methodBegIndices[i];
                var methodEnd = methodEndIndices[i];
                
                // Get generic info about method based on its prototype
                var methodPrototype = _contentLines[methodBeg];
                var method = new JavaTypeMethod()
                {
                    CodeLines = new List<string>(_contentLines.GetRange(methodBeg, methodEnd - methodBeg + 1)),
                    AccessControl = SmaliParserUtils.GetAccessControl(methodPrototype),
                    IsConstructor = IsConstructorOptionPresent(methodPrototype),
                    IsStatic = IsStaticOptionPresent(methodPrototype),
                    IsAbstract = IsAbstractOptionPresent(methodPrototype),
                    SmaliName = GetFullMethodName(methodPrototype, JavaType.NameFullSmali),
                    Name = GetShortMethodName(methodPrototype),
                    InputTypeNames = new List<JavaTypeBaseInfo>(),
                    InputParamNames = new List<string>(),
                    InvokedMethods = new List<JavaTypeInvokedMethod>(),
                    FieldAccessors = new List<JavaTypeFieldAccessors>(),
                    SourceCodeIndexBeg = methodBeg,
                    SourceCodeIndexEnd = methodEnd,
                    ReturnType = SmaliParserUtils.GetShallowJavaType(GetReturnTypeName(methodPrototype))
                };

                // Process input parameters
                ParseMethodParameters(methodPrototype, ref method);

                ParseMethodForMethodCalls(ref method);
            }

        }

        private void GetBegEndIndicesForMethods(ref List<int> begs, ref List<int> ends)
        {
            int methodBeg = -1, methodEnd = -1;
            int i = 0;

            while (i < _contentLines.Count)
            {

                if (methodBeg == -1 && methodEnd == -1 && IsMethodStartLine(_contentLines[i]))
                {
                    methodBeg = i;
                }
                else if (methodBeg != -1 && methodEnd == -1 && IsMethodEndLine(_contentLines[i]))
                {
                    methodEnd = i;
                }
                if (methodEnd != -1 && methodBeg != -1)
                {
                    begs.Add(methodBeg);
                    ends.Add(methodEnd);
                    methodEnd = -1;
                    methodBeg = -1;
                }
                i++;
            }

            if (methodEnd != -1 || methodBeg != -1)
            {
                throw new Exception("This should not happen");
            }

        }

        private void ParseMethodParameters(string methodNamePart, ref JavaTypeMethod method)
        {
            var parameters = GetInputParametersTypes(methodNamePart);
            foreach (var parameter in parameters)
            {
                method.InputTypeNames.Add(SmaliParserUtils.GetShallowJavaType(parameter));
            }
            JavaType.Methods.Add(method);
        }

        private void ParseMethodForMethodCalls(ref JavaTypeMethod method)
        {
            for (int i = 0; i < method.CodeLines.Count; i++)
            {
                var line = method.CodeLines[i].TrimStart(' ');
                if (IsInvokeLine(line))
                {
                    method.InvokedMethods.Add(new JavaTypeInvokedMethod
                    {
                        MethodSmaliName = GetInvokedMethodFullName(line),
                        NameFullSmali = GetInvokedMethodTypeInFullName(line),
                        PackageName = GetPackageName(GetInvokedMethodTypeInFullName(line)),
                        SourceLineIndex = i + method.SourceCodeIndexBeg
                    });
                }
                else if (IsPutLine(line) || IsGetLine(line))
                {
                    method.FieldAccessors.Add(new JavaTypeFieldAccessors
                    {
                        FieldSmaliName = GetAccessedMemberName(line),
                        NameFullSmali = GetAccessedMemberTypeInName(line),
                        PackageName = GetPackageName(GetAccessedMemberTypeInName(line)),
                        SourceLineIndex = i + method.SourceCodeIndexBeg,
                        IsGet = IsGetLine(line),
                        IsPut = IsPutLine(line)
                    });
                }
            }
        }
    }
}
