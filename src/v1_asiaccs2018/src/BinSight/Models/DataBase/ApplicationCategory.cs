using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using CsnowFramework.Database;

namespace APKInsight.Models
{
    internal class ApplicationCategory
    {
        [QueryColumn("apcUId", SqlDbType.Int, isPrimaryKey:true)]
        public int UId { get; set; }
        [QueryColumn("apcName", SqlDbType.NVarChar, maxLen:256)]
        public string Name { get; set; }
        [QueryColumn("apcDescription", SqlDbType.NVarChar, maxLen: 256)]
        public string Description { get; set; }
    }

}
