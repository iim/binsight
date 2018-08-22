using System.Collections.Generic;
using APKInsight.Logic;
using APKInsight.Models;
using APKInsight.Models.DataBase;
using CsnowFramework.Database;

namespace APKInsight.Queries
{
    class QueryDataSet : QueryBase
    {
        public QueryDataSet(string connectionString = null) :
            base(connectionString)
        {
            // Do nothing
        }

        public List<DataSet> SelectDataSets()
        {
            var sqlQuery = @"
SELECT *
FROM DataSet
WHERE dstUId > 0
";
            return ExecSelectQuery<DataSet>(sqlQuery);
        }

        public void UpdateBinaryCountInDataSetCategory(int dataSetId)
        {
            var sqlQuery = @"
UPDATE DataSetApplicationCategories
SET dstcatBioCount = (SELECT COUNT(*) FROM BinaryObject WHERE bioDstcatDataSetApplicationCategoryId = dstcatUId AND bioUId > 0)
WHERE dstcatDstDataSetId = @dstcatDstDataSetId
";
            ExecNonQuery(sqlQuery, new Dictionary<string, object>
            {
                {"@dstcatDstDataSetId", dataSetId },
            });
        }
        public void UpdateBinaryCountInDataSet(int dataSetId)
        {
            var sqlQuery = @"
UPDATE DataSet
SET dstBioCount = (SELECT SUM(dstcatBioCount) FROM DataSetApplicationCategories WHERE dstcatDstDataSetId = @dstcatDstDataSetId)
WHERE dstUId = @dstUId
";
            ExecNonQuery(sqlQuery, new Dictionary<string, object>
            {
                {"@dstcatDstDataSetId", dataSetId },
                {"@dstUId", dataSetId },
            });
        }
    }
}