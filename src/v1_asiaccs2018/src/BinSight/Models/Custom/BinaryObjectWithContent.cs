using System.Collections.Generic;
using System.Data;
using System.IO;
using CsnowFramework.Database;

namespace APKInsight.Models.Custom
{

    [QueryTable("BinaryObject")]
    public class BinaryObjectWithContent : BinaryObject
    {
        [QueryColumn("bocContent", SqlDbType.VarBinary)]
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
