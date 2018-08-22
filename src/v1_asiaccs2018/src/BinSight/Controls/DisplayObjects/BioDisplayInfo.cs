using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using APKInsight.Models;
using APKInsight.Models.DataBase;
using APKInsight.Queries;

namespace APKInsight.Controls.DisplayObjects
{
    /// <summary>
    /// Main logic for organizing displaying of a Binary Object
    /// </summary>
    internal class BioDisplayInfo
    {
        // The main BinaryObject we are viewing
        public BinaryObject BinaryObject;
        // Types defined in BinaryObject
        public List<JavaType> InternalJavaTypes = null;

        // Collection of package nodes
        public Dictionary<string, TreeNode> PackageNameNodesCache = new Dictionary<string, TreeNode>();
        // Collection of filenames
        public Dictionary<string, TreeNode> FileNameNodesCache = new Dictionary<string, TreeNode>();
        // Links to JavaTypes based on their IDs
        public Dictionary<int, TreeNode> JavaTypesInBinaryObjectCache = new Dictionary<int, TreeNode>();

        public void GetSourceCodeLineIndex(int charIndex, string text, out string line, out int inlineIndex)
        {
            var search = text.Substring(0, charIndex);
            var lines = search.Split('\n');
            var allLines = text.Split('\n');
            line = allLines[lines.Length - 1];
            inlineIndex = lines.Last().Length;
        }

        public JavaType GetJavaType(int id)
        {
            // First see if that type is already added
            if (JavaTypesInBinaryObjectCache.ContainsKey(id))
                return ((JavaTypeDisplayInfo)JavaTypesInBinaryObjectCache[id].Tag).JavaType;

            var foundType = InternalJavaTypes.FirstOrDefault(jt => jt.UId.Value == id);
            if (foundType != null)
                return foundType;

            var query = new QueryJavaType();
            return query.SelectJavaType(id);
        }

    }
}
