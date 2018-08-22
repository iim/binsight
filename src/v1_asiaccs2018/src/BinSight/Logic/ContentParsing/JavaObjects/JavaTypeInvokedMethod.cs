using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKInsight.Logic.ContentParsing.JavaObjects
{
    public class JavaTypeInvokedMethod: JavaTypeBaseInfo
    {
        public string MethodSmaliName { get; set; }
        public int SourceLineIndex { get; set; }
    }
}
