using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("EnumOption")]
    class EnumOption
    {
        [QueryColumn("enoUId", SqlDbType.Int, isPrimaryKey:true)]
        public int? UId { get; set; }

        [QueryColumn("enoJtypEnumTypeId", SqlDbType.Int)]
        public int? EnumTypeId { get; set; }

        [QueryColumn("enoStrSmaliFullNameId", SqlDbType.Int)]
        public int? SmaliFullNameId { get; set; }

        [QueryColumn("enoValue", SqlDbType.NVarChar)]
        public string Value { get; set; }
    }
}
