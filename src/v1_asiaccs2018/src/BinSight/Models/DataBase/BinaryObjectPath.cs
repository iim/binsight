using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("BinaryObjectPath")]
    class BinaryObjectPath
    {
        [QueryColumn("bopUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }
        [QueryColumn("bopBopParentId", SqlDbType.Int)]
        public int? ParentPathId { get; set; }
        [QueryColumn("bopName", SqlDbType.NVarChar, maxLen: 400)]
        public string Name { get; set; }
        [QueryColumn("bopParentPath", SqlDbType.NVarChar, maxLen: 2048)]
        public string ParentPath { get; set; }
    }
}
