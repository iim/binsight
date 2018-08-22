using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    class LibraryCandidate
    {

        [QueryColumn("jtypStrPackageNameId", SqlDbType.Int, isPrimaryKey: true)]
        public int? StrUId { get; set; }

        [QueryColumn("strValue", SqlDbType.Int)]
        public string PackageName { get; set; }

        [QueryColumn("NumberOfBinaries", SqlDbType.Int)]
        public int? NumberOfBinaries { get; set; }

        public override string ToString()
        {
            return $"{PackageName} ({NumberOfBinaries})";
        }

    }

}
