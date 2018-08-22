using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("JavaTypePath")]
    class JavaTypePath
    {

        [QueryColumn("jtpUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("jtpName", SqlDbType.NVarChar, 448)]
        public string Name { get; set; }
    }
}
