using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("JavaTypeImplementedInterface")]
    class JavaTypeImplementedInterface
    {

        [QueryColumn("jtiiUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("jtiiJtypClassId", SqlDbType.Int)]
        public int? ClassId { get; set; }

        [QueryColumn("jtiiJtypInterfaceId", SqlDbType.Int)]
        public int? InterfaceId { get; set; }

    }
}
