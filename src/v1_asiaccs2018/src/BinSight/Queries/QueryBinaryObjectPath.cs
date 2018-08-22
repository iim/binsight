using APKInsight.Models;
using CsnowFramework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APKInsight.Models.DataBase;

namespace APKInsight.Queries
{
    class QueryBinaryObjectPath : QueryBase
    {
        public QueryBinaryObjectPath(string connectionString = null) : 
            base(connectionString)
        {
            // Do nothing
        }

        public List<BinaryObjectPath> SelectBinaryObjectPath(int? UId = null, int? parentUId = null, string name = "")
        {
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            string sSql = @"
SELECT *
FROM BinaryObjectPath
WHERE 1=1
";
            if (UId != null)
            {
                sSql += " AND bopUId = @bopUId";
                sqlParams.Add("@bopUId", UId);
            }
            if (parentUId != null)
            {
                sSql += " AND bopBopParentId = @bopBopParentId";
                sqlParams.Add("@bopBopParentId", parentUId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                sSql += " AND bopName = @bopName";
                sqlParams.Add("@bopName", name);
            }
            sSql += @" 
ORDER BY bopUId";
            return ExecSelectQuery<BinaryObjectPath>(sSql, sqlParams);
        }

        public int SelectInsertBinaryObjectPath(int parentUId, string name, string parentPath)
        {
            string sSql = @"
IF NOT EXISTS(SELECT * FROM BinaryObjectPath WHERE bopBopParentId = @bopBopParentId AND bopName = @bopName)
BEGIN
    INSERT INTO BinaryObjectPath (bopBopParentId, bopName, bopParentPath) VALUES (@bopBopParentId, @bopName, @bopParentPath)
    SELECT @@IDENTITY
END
ELSE
BEGIN
    SELECT bopUId FROM BinaryObjectPath WHERE bopBopParentId = @bopBopParentId AND bopName = @bopName
END
";
            var uid = ExecScalarSelectQuery(sSql, new Dictionary<string, object>
            {
                { "@bopBopParentId", parentUId},
                { "@bopName", name},
                { "@bopParentPath", parentPath},
            }, 1);

            return uid ?? -1;
        }

    }
}
