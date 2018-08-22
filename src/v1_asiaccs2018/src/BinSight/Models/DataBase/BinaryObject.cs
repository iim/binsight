using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using CsnowFramework.Database;

namespace APKInsight.Models
{
    [QueryTable("BinaryObject")]
    public class BinaryObject
    {
        [QueryColumn("bioUId", SqlDbType.Int, isPrimaryKey:true)]
        public int? UId { get; set; }

        [QueryColumn("bioBocContentId", SqlDbType.Int)]
        public int? ContentId { get; set; }

        [QueryColumn("bioDstcatDataSetApplicationCategoryId", SqlDbType.Int)]
        public int? DataSetApplicationCategoryId { get; set; }

        [QueryColumn("bioBopPathId", SqlDbType.Int)]
        public int? PathId { get; set; }

        [QueryColumn("bioBioParentApkId", SqlDbType.Int)]
        public int? ParentApkId { get; set; }

        [QueryColumn("bioHash", SqlDbType.VarBinary, maxLen:20)]
        public byte[] Hash { get; set; }

        [QueryColumn("bioFileName", SqlDbType.NVarChar, maxLen:256)]
        public string FileName { get; set; }

        [QueryColumn("bioRankInCategory", SqlDbType.Int)]
        public int? RankInCategory { get; set; }

        [QueryColumn("bioIsRoot", SqlDbType.Int)]
        public int? IsRoot { get; set; }

        [QueryColumn("bioProcessingStage", SqlDbType.Int)]
        public int? ProcessingStage { get; set; }
    }
}
