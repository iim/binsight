using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("JavaTypeUsedInType")]
    public class JavaTypeUsedInType
    {
        [QueryColumn("jtuUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("jtuJtmDestinationMethodId", SqlDbType.Int)]
        public int? DestinationMethodId { get; set; }

        [QueryColumn("jtuStrDestinationMethodSmaliNameId", SqlDbType.Int)]
        public int? DestinationMethodSmaliNameId { get; set; }


        [QueryColumn("jtuJtfDestinationFieldId", SqlDbType.Int)]
        public int? DestinationFieldId { get; set; }

        [QueryColumn("jtuStrDestinationFieldSmaliNameId", SqlDbType.Int)]
        public int? DestinationFieldSmaliNameId { get; set; }

        [QueryColumn("jtuJtmSourceMethodId", SqlDbType.Int)]
        public int? SourceMethodId { get; set; }

        [QueryColumn("jtuSourceLineIndex", SqlDbType.Int)]
        public int? SourceLineIndex { get; set; }

        [QueryColumn("jtuSourceWithinLineIndex", SqlDbType.Int)]
        public int? SourceWithinLineIndex { get; set; }

        [QueryColumn("jtuIsReturnType", SqlDbType.Int)]
        public bool? IsReturnType { get; set; }

        [QueryColumn("jtuIsParameter", SqlDbType.Int)]
        public bool? IsParameter { get; set; }

        [QueryColumn("jtuIsGetFieldAccessor", SqlDbType.Int)]
        public bool? IsGetFieldAccessor { get; set; }

        [QueryColumn("jtuIsPutFieldAccessor", SqlDbType.Int)]
        public bool? IsPutFieldAccessor { get; set; }
    }
}
