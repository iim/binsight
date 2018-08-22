using System.Collections.Generic;
using System.Data;
using System.IO;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{

    [QueryTable("BinaryObjectContent")]
    class BinaryObjectContent
    {
        [QueryColumn("bocUId", SqlDbType.Int, isPrimaryKey:true)]
        public int? UId { get; set; }
        [QueryColumn("bocHash", SqlDbType.VarBinary, maxLen:20)]
        public byte[] Hash { get; set; }
        [QueryColumn("bocContent", SqlDbType.VarBinary, isNullable: true )]
        public byte[] Content { get; set; }
        [QueryColumn("bocLength", SqlDbType.Int)]
        public int? Length { get; set; }

        public List<string> ContentAsListOfStrings()
        {
            StreamReader reader = new StreamReader(new MemoryStream(Content));
            var contentLines = new List<string>();
            while (!reader.EndOfStream)
            {
                contentLines.Add(reader.ReadLine());
            }
            return contentLines;
        }

        public string ContentAsString()
        {
            StreamReader reader = new StreamReader(new MemoryStream(Content));
            return reader.ReadToEnd();
        }
    }
}
