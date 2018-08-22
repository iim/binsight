using APKInsight.Enums;
using APKInsight.Logic.ContentParsing.JavaObjects;

namespace APKInsight.Logic
{
    public class JavaTypeField
    {
        public JavaAccessControl AccessControl { get; set; }
        public bool IsArray{ get; set; }
        public bool IsStatic { get; set; }
        public bool IsFinal { get; set; }
        public bool IsVolatile { get; set; }
        public bool IsSynthetic { get; set; }
        public bool IsEnum { get; set; }
        public string SmaliName { get; set; }
        public string InitValue { get; set; }
        public JavaTypeBaseInfo JavaType { get; set; }
        public int SourceLineIndex { get; set; }
        public int SourceWithinLineIndex{ get; set; }
    }
}
