using System.Collections.Generic;

using APKInsight.Models;
using APKInsight.Models.Custom;
using CsnowFramework.Database;

namespace APKInsight.Queries
{
    class QueryApplicationCategories: QueryBase
    {
        public QueryApplicationCategories(string connectionString = null) : 
            base(connectionString)
        {
            // Do nothing
        }

        public List<ApplicationCategoryWithCount> SelectAllApplicationCategories(int dataSetId)
        {
            string query = @"
SELECT 
    ApplicationCategory.*,
    dstcatUId,
    dstcatBioCount
FROM DataSetApplicationCategories
    INNER JOIN ApplicationCategory ON apcUId = dstcatApcApplicationCategoryId
WHERE dstcatDstDataSetId = @dstcatDstDataSetId
ORDER BY apcUId ASC
";
            return ExecSelectQuery<ApplicationCategoryWithCount>(query, 
                new Dictionary<string, object>
                {
                    { "@dstcatDstDataSetId", dataSetId}
                });
        }

    }

}
