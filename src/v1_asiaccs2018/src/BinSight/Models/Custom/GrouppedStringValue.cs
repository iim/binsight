using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsnowFramework.Database;

namespace APKInsight.Models.Custom
{
    class GrouppedStringValue
    {
        [QueryColumn("strHash", SqlDbType.Binary, maxLen: 20)]
        public byte[] Hash { get; set; }

        [QueryColumn("strType", SqlDbType.VarChar, maxLen: 4)]
        public string StrType { get; set; }

    }
}
