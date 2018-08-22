using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("LibraryAliases")]
    class LibraryAliases
    {
        [QueryColumn("lalUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("lalLibLibraryId", SqlDbType.Int)]
        public int? LibraryId { get; set; }

        [QueryColumn("lalStrPackageNameId", SqlDbType.Int)]
        public int? PackageNameId { get; set; }
    }
}
