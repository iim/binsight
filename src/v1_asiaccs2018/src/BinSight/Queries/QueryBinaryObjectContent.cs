using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsnowFramework.Database;
using APKInsight.Models;
using APKInsight.Models.DataBase;

namespace APKInsight.Queries
{
    class QueryBinaryObjectContent : QueryBase
    {
        public QueryBinaryObjectContent(string connectionString = null) : 
            base(connectionString)
        {
            // Do nothing
        }

        public List<BinaryObjectContent> SelectBinaryObjectContent(int Id)
        {
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            string sSql = @"
SELECT *
FROM BinaryObjectContent
WHERE 1=1
    AND bocUId = @bocUId
";
            sqlParams.Add("@bocUId", Id);

            var result = ExecSelectQuery<BinaryObjectContent>(sSql, sqlParams);
            if (result.Count == 1 && result.Where(c => c.Content == null).ToList().Any())
            {
                ConnectionString = AlternativeConnectionString["boc"];
                var contentResult = ExecSelectQuery<BinaryObjectContent>(sSql, sqlParams);
                if (contentResult.Any())
                    result[0].Content = contentResult[0].Content;
                ConnectionString = DefaultConnectionString;
            }

            return result;
        }

        public int? AddObject(BinaryObjectContent boc, string filename)
        {
            var sql = @"
IF NOT EXISTS (SELECT bocUId FROM BinaryObjectContent WHERE bocHash = @bocHash)
BEGIN
INSERT INTO BinaryObjectContent (bocHash, bocContent, bocLength) VALUES (@bocHash, @bocContent, @bocLength)
SELECT @@IDENTITY
END
ELSE
BEGIN
SELECT bocUId FROM BinaryObjectContent WHERE bocHash = @bocHash
END
";
            // Put only smali  and XML files in the main DB
            var extension = Path.GetExtension(filename);
            if (extension != null && 
                        (extension.ToLower() == ".smali" ||
                         extension.ToLower() == ".xml"))
            {
                return ExecScalarSelectQuery(sql, new Dictionary<string, object>
                {
                    {"@bocHash", boc.Hash },
                    {"@bocContent", boc.Content },
                    {"@bocLength", boc.Length }
                }, 1);
            }
            // Otherwise, put content into a separate DB
            //First add to the usual DB, but without content.

            boc.UId = ExecScalarSelectQuery(sql, new Dictionary<string, object>
                {
                    {"@bocHash", boc.Hash },
                    {"@bocContent", null },
                    {"@bocLength", boc.Length }
                }, 1);
            // Insert the content
            ConnectionString = AlternativeConnectionString["boc"];
            ExecScalarSelectQuery(sql, new Dictionary<string, object>
                {
                    {"@bocHash", boc.Hash },
                    {"@bocContent", boc.Content },
                    {"@bocLength", boc.Length }
                }, 1);
            ConnectionString = DefaultConnectionString;
            return boc.UId;
        }

    }
}
