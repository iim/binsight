using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKInsight.Logic.ContentParsing.JavaObjects
{
    public class JavaTypeBaseInfo
    {
        public int UId { get; set; }
        public string PackageName { get; set; }
        public string NameFullSmali { get; set; }
        public JavaTypeBaseInfo OuterType { get; set; }
        public JavaTypeBaseInfo InnerType { get; set; }
    }
}
