using System.Collections.Generic;
using System.Data;
using System.IO;
using CsnowFramework.Database;

namespace APKInsight.Models.Custom
{

    [QueryTable("BinaryObject")]
    internal class BinaryObjectWithContentJavaTypeId : BinaryObjectWithContent
    {
        [QueryColumn("jtypUId", SqlDbType.Int)]
        public int JavaTypeId { get; set; }
    }
}
