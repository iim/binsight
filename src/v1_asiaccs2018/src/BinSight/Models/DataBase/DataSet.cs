using System;
using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("DataSet")]
    class DataSet
    {
        [QueryColumn("dstUId", SqlDbType.Int, isPrimaryKey:true)]
        public int? UId { get; set; }

        [QueryColumn("dstName", SqlDbType.NVarChar, maxLen:1024)]
        public string Name { get; set; }

        [QueryColumn("dstSource", SqlDbType.NVarChar, maxLen:128)]
        public string Source { get; set; }

        [QueryColumn("dstBioCount", SqlDbType.Int)]
        public int? BioCount { get; set; }

        [QueryColumn("dstDownloadDateBeg", SqlDbType.DateTime)]
        public DateTime? DownloadDateBeg { get; set; }

        [QueryColumn("dstDownloadDateEnd", SqlDbType.DateTime)]
        public DateTime? DownloadDateEnd { get; set; }

        public override string ToString() => $"{Name} from {Source} ({BioCount} binaries)";
    }
}
