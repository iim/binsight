using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    public class JavaTypeUsedInTypeExtended: JavaTypeUsedInType
    {

        [QueryColumn("StrMethodSmaliName", SqlDbType.NVarChar)]
        public string SourceMethodSmaliName { get; set; }

        [QueryColumn("SourcejtmJtypInTypeId", SqlDbType.Int)]
        public int SourceInTypeId { get; set; }
    }
}
