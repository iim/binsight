using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APKInsight.Enums;
using APKInsight.Logic.ContentParsing.JavaObjects;

namespace APKInsight.Logic
{
    public class JavaTypeMethod
    {
        public JavaAccessControl AccessControl { get; set; }
        public bool IsConstructor { get; set; }
        public bool IsStatic { get; set; }
        public bool IsAbstract { get; set; }
        public string SmaliName { get; set; }
        public string Name { get; set; }
        public JavaTypeBaseInfo ReturnType { get; set; }
        public List<JavaTypeBaseInfo> InputTypeNames { get; set; }
        public List<string> InputParamNames { get; set; }
        public List<JavaTypeInvokedMethod> InvokedMethods { get; set; }
        public List<JavaTypeFieldAccessors> FieldAccessors { get; set; }
        public List<string> CodeLines { get; set; }
        public int SourceCodeIndexBeg { get; set; }
        public int SourceCodeIndexEnd { get; set; }
    }
}
