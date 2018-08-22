using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKInsight.Logic.ContentParsing.JavaObjects
{
    public class JavaTypeFieldAccessors : JavaTypeBaseInfo
    {
        public string FieldSmaliName { get; set; }
        public int SourceLineIndex { get; set; }
        public bool IsGet { get; set; }
        public bool IsPut { get; set; }
    }
}
