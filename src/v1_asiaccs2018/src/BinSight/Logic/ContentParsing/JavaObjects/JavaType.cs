using System.Collections.Generic;
using APKInsight.Enums;

namespace APKInsight.Logic.ContentParsing.JavaObjects
{
    public class JavaType: JavaTypeBaseInfo
    {
        public JavaAccessControl AccessControl { get; set; }

        // Type flags
        public bool IsClass { get; set; }
        public bool IsInterface { get; set; }
        public bool IsFinal { get; set; }
        public bool IsEnum { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsAnnotation { get; set; }
        public bool IsStatic { get; set; }

        // Location of the definition.
        public string FilePath { get; set; }
        public string FileName { get; set; }

        // Super class info
        public JavaTypeBaseInfo SuperClass { get; set; }

        // Main pieces of the class
        public List<JavaTypeField> Fields { get; set; }
        public List<JavaTypeMethod> Methods { get; set; }
        public List<string> EnumOptionsSmali { get; set; }
        public List<string> EnumStrValue { get; set; }

        // Implemented interfaces
        public List<JavaTypeBaseInfo> ImplementedInterfaces { get; set; }

        // Debugging flags
        public bool DbgSourceNotFound { get; set; }
    }

}
