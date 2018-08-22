using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("LibraryPropertyTypes")]
    class LibraryPropertyTypes
    {
        [QueryColumn("lptUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("lptName", SqlDbType.NVarChar, maxLen: 256)]
        public string Name { get; set; }

        [QueryColumn("lptDescription", SqlDbType.NVarChar)]
        public string Description { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
