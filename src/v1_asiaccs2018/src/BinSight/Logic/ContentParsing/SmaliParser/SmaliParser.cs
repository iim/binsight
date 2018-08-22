using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using APKInsight.Logic.ContentParsing.JavaObjects;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Models.Custom;

namespace APKInsight.Logic.ContentParsing.SmaliParser
{
    /// <summary>
    /// Special calss that parses Smali files.
    /// </summary>
    public partial class SmaliParser: ICfgParser
    {
        // Contains all code lines
        private List<string> _contentLines;

        // Contains javatype describing structure.
        public JavaType _javaType;
        public JavaType JavaType {
            get
            {
                return _javaType;
            }
            private set
            {
                _javaType = value;
            } 
        }

        
        #region Public functions

        /// <summary>
        /// We work under assumption that each smali correspondes to a single java type.
        /// </summary>
        /// <param name="bio">A binary object we are going to parse</param>
        /// <remarks>Upon completion read JavaType for object details</remarks>
        public void ProcessSmaliFile(BinaryObjectWithContent bio)
        {
            InitFromBio(bio);
            ParseCodeLines();
        }

        /// <summary>
        /// We work under assumption that each smali correspondes to a single java type.
        /// </summary>
        /// <param name="bio">A binary object we are going to parse</param>
        /// <remarks>Upon completion read JavaType for object details</remarks>
        public void ProcessSmaliFile(string content)
        {
            InitFromContent(content);
            ParseCodeLines();
        }

        protected void ParseCodeLines()
        {
            // Parsing all required pieces
            ParseTypeName();
            ParseFileName();
            ParseOuterClasses();
            ParseSuperClassName();
            ParseAllMethods();
            ParseAllFields();
            ParseAllImplementedInterfaces();
        }

        #endregion


        #region Parsing logic

        /// <summary>
        /// Parses java type name from the current smali file content.
        /// Also sets flag properties found in the type defintion line.
        /// </summary>
        private void ParseTypeName()
        {
            var classNameLine = _contentLines.FirstOrDefault(IsClassNameLine);
            if (classNameLine == null)
                throw new Exception("Cannot find class name");

            JavaType.IsAbstract = IsAbstractOptionPresent(classNameLine);
            JavaType.IsInterface = IsInterfaceOptionPresent(classNameLine);
            JavaType.IsEnum = IsEnumOptionPresent(classNameLine);
            JavaType.IsFinal = IsFinalOptionPresent(classNameLine);
            JavaType.IsAnnotation = IsAnnotationOptionPresent(classNameLine);
            JavaType.IsStatic = IsStaticOptionPresent(classNameLine);
            JavaType.IsClass = !JavaType.IsInterface && !JavaType.IsEnum;

            JavaType.AccessControl = SmaliParserUtils.GetAccessControl(classNameLine);

            SmaliParserUtils.SetNameToJavaType(ref _javaType, GetClassName(classNameLine));
        }

        /// <summary>
        /// Parses file name where the java type is defined.
        /// </summary>
        private void ParseFileName()
        {
            var sourceLine = _contentLines.FirstOrDefault(IsSourceFileNameLine);
            JavaType.DbgSourceNotFound = string.IsNullOrWhiteSpace(sourceLine);
            if (JavaType.DbgSourceNotFound)
            {
                // We just fake the filename based on 
                JavaType.FileName = SmaliParserUtils.GetTypeNameShortShort(JavaType.NameFullSmali) + ".java";
            }
            else
            {
                JavaType.FileName = GetFileName(sourceLine);
            }
        }

        /// <summary>
        /// Parse name of the class that we inherit from.
        /// </summary>
        private void ParseSuperClassName()
        {
            var superSmaliName = _contentLines.FirstOrDefault(IsSuperClassNameLine);
            if (string.IsNullOrWhiteSpace(superSmaliName))
            {
                // This should never happen, since all classes must derived at least from java.Object
                throw new Exception("Cannot find source line, ohoh.");
            }

            var superType = new JavaTypeBaseInfo();
            SmaliParserUtils.SetNameToJavaType(ref superType, GetClassName(superSmaliName));
            JavaType.SuperClass = superType;
        }

        /// <summary>
        /// Parses outer classes if present and replaces filename if we haven't found one yet.
        /// </summary>
        private void ParseOuterClasses()
        { 
            if (IsInOuterClass(JavaType.NameFullSmali))
            {
                var allOuterClasses = GetAllOuterClassNames(JavaType.NameFullSmali);
                var outerType = new JavaTypeBaseInfo();
                JavaTypeBaseInfo newOuter = JavaType;

                foreach (var outerClass in allOuterClasses)
                {
                    outerType = new JavaTypeBaseInfo();
                    SmaliParserUtils.SetNameToJavaType(ref outerType, outerClass);
                    outerType.InnerType = newOuter;
                    newOuter.OuterType = outerType;
                    newOuter = outerType;
                }

                // We use the last outer type to define file path, since this is how it works.
                var nameFill = SmaliParserUtils.GetTypeNameFull(outerType.NameFullSmali);
                JavaType.FilePath = nameFill.LastIndexOf(".", StringComparison.Ordinal) == -1
                    ? "" 
                    : nameFill.Substring(0, nameFill.LastIndexOf(".", StringComparison.Ordinal)).Replace(".", "/");
            }
            else
            {
                JavaType.FilePath = "";
                if (JavaType.NameFullSmali.LastIndexOf("/", StringComparison.Ordinal) != -1)
                {
                    JavaType.FilePath = JavaType.NameFullSmali
                        .Substring(0, JavaType.NameFullSmali.LastIndexOf("/", StringComparison.Ordinal))
                        .Substring(1);
                }
            }

        }

        /// <summary>
        /// Parse all fields in the type definition
        /// </summary>
        private void ParseAllFields()
        {
            JavaType.Fields = new List<JavaTypeField>();

            for(var i = 0; i < _contentLines.Count; i++)
            {
                if (IsFieldLine(_contentLines[i]))
                {
                    ParseField(_contentLines[i], i);
                }
            }
        }

        /// <summary>
        /// Parses single field.
        /// </summary>
        /// <param name="field">Field to be parsed</param>
        /// <param name="index">Index within source code</param>
        private void ParseField(string field, int index)
        {
            var fieldObj = new JavaTypeField
            {
                AccessControl = SmaliParserUtils.GetAccessControl(field),
                IsFinal = IsFinalOptionPresent(field),
                IsStatic = IsStaticOptionPresent(field),
                IsVolatile = IsVolatileOptionPresent(field),
                IsSynthetic = IsSyntheticOptionPresent(field),
                IsEnum = IsEnumOptionPresent(field),
                SourceLineIndex = index,
                SmaliName = GetFullFieldName(field, JavaType.NameFullSmali),
                InitValue = GetFieldInitValue(field)
            };

            var fieldType = new JavaTypeBaseInfo();
            fieldObj.IsArray = GetFieldTypeName(field).StartsWith("[");
            SmaliParserUtils.SetNameToJavaType(ref fieldType, GetFieldTypeName(field));
            fieldObj.JavaType = fieldType;
            fieldObj.SourceWithinLineIndex = field.IndexOf(fieldObj.JavaType.NameFullSmali, StringComparison.OrdinalIgnoreCase);
            JavaType.Fields.Add(fieldObj);
        }

        /// <summary>
        /// Parses all implmeneted interfaces
        /// </summary>
        private void ParseAllImplementedInterfaces()
        {
            var interfaces = _contentLines.Where(IsImplementsInterfaceNameLine);

            JavaType.ImplementedInterfaces = new List<JavaTypeBaseInfo>();
            foreach (var implementedInterface in interfaces)
            {
                var interfaceType = new JavaTypeBaseInfo();
                SmaliParserUtils.SetNameToJavaType(ref interfaceType, GetClassName(implementedInterface));
                JavaType.ImplementedInterfaces.Add(interfaceType);
            }
        }

        #endregion

        #region Initialization logic

        private void InitFromBio(BinaryObjectWithContent bio)
        {
            // Get content and reinit JavaType
            _contentLines = bio.ContentAsListOfStrings();
            JavaType = new JavaType();
        }

        private void InitFromContent(string content)
        {
            // Get content and reinit JavaType
            _contentLines = new List<string>(content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
            JavaType = new JavaType();
        }
        #endregion
    }
}
