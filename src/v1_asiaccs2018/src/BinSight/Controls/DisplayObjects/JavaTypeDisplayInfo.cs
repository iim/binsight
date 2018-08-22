using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Models.DataBase;

namespace APKInsight.Controls.DisplayObjects
{
    /// <summary>
    /// Class that contains all needed info for dislpaying a java type
    /// </summary>
    public class JavaTypeDisplayInfo
    {
        public JavaType JavaType { get; set; }
        public bool InternalsLoaded { get; set; }
        public TreeNode TreeNode { get; set; }
        public string SourceCode { get; set; }
        public string RtfSourceCode { get; set; }
        public bool SourceCodeLoaded { get; set; }
        public List<JavaTypeInternals> Internals { get; set; }
        public List<JavaTypeUsedInTypeExtended> MethodUseCases { get; set; }
        public Dictionary<int, ContextMenuStrip> MethodMenus { get; set; } = new Dictionary<int, ContextMenuStrip>();

    }
}
