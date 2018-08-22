using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("Library")]
    class Library
    {
        [QueryColumn("libUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("libStrPackageNameId", SqlDbType.Int)]
        public int? PackageNameId { get; set; }

        [QueryColumn("libName", SqlDbType.NVarChar, 256)]
        public string Name { get; set; }

        [QueryColumn("libUrl", SqlDbType.NVarChar, 256)]
        public string Url { get; set; }

        [QueryColumn("libDescription", SqlDbType.NVarChar, 256)]
        public string Description { get; set; }
    }
}
