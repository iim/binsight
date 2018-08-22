using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("LibraryProperties")]
    class LibraryProperties
    {
        [QueryColumn("lprUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("lprLibLibraryId", SqlDbType.Int)]
        public int? LibraryId { get; set; }

        [QueryColumn("lprLptPropertyTypeId", SqlDbType.Int)]
        public int? PropertyTypeId { get; set; }

        [QueryColumn("lprStrValue", SqlDbType.NVarChar)]
        public string StrValue { get; set; }

        [QueryColumn("lprIntValue", SqlDbType.Int)]
        public int IntValue { get; set; }

        [QueryColumn("lprBoolValue", SqlDbType.Bit)]
        public bool BoolValue { get; set; }
    }
}
