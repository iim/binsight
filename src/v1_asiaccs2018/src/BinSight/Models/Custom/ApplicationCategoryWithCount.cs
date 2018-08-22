using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.Custom
{

    internal class ApplicationCategoryWithCount: ApplicationCategory
    {
        [QueryColumn("dstcatUId", SqlDbType.Int)]
        public int DataSetApplicationCategoryId { get; set; }

        [QueryColumn("dstcatBioCount", SqlDbType.Int)]
        public int BioCount { get; set; }

        // Other flags
        public bool IsLoaded { get; set; } = false;
    }
}
