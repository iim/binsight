using System.Collections.Generic;
using APKInsight.Enums;
using CsnowFramework.Database;
using APKInsight.Models;
using APKInsight.Models.Custom;

namespace APKInsight.Queries
{
    class QueryBinaryObject: QueryBase
    {
        public QueryBinaryObject(string connectionString = null): 
            base(connectionString)
        {
            // Do nothing
        }

        public List<BinaryObject> SelectBinaryObject(BinaryObject bio, int? datasetId = null)
        {
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            string sSql = @"
SELECT *
FROM BinaryObject
    INNER JOIN DataSetApplicationCategories ON (bioDstcatDataSetApplicationCategoryId = dstcatUId)
WHERE 1=1
";
            if (datasetId.HasValue)
            {
                sSql += " AND dstcatDstDataSetId = @dstcatDstDataSetId";
                sqlParams.Add("@dstcatDstDataSetId", datasetId);
            }
            if (bio.UId != null)
            {
                sSql += " AND bioUId = @bioUId";
                sqlParams.Add("@bioUId", bio.UId);
            }
            else
            {
                if (bio.ContentId != null)
                {
                    sSql += " AND bioBocContentId = @bioBocContentId";
                    sqlParams.Add("@bioBocContentId", bio.ContentId);
                }
                if (bio.DataSetApplicationCategoryId != null)
                {
                    sSql += " AND bioDstcatDataSetApplicationCategoryId = @bioDstcatDataSetApplicationCategoryId";
                    sqlParams.Add("@bioDstcatDataSetApplicationCategoryId", bio.DataSetApplicationCategoryId);
                }
                if (bio.Hash != null)
                {
                    sSql += " AND bioHash = @bioHash";
                    sqlParams.Add("@bioHash", bio.Hash);
                }
                if (bio.FileName != null)
                {
                    sSql += " AND bioFileName = @bioFileName";
                    sqlParams.Add("@bioFileName", bio.FileName);
                }
                if (bio.RankInCategory != null)
                {
                    sSql += " AND bioRankInCategory = @bioRankInCategory";
                    sqlParams.Add("@bioRankInCategory", bio.RankInCategory);
                }
                if (bio.IsRoot != null)
                {
                    sSql += " AND bioIsRoot = @bioIsRoot";
                    sqlParams.Add("@bioIsRoot", bio.IsRoot);
                }
                if (bio.ProcessingStage != null)
                {
                    sSql += " AND bioProcessingStage = @bioProcessingStage";
                    sqlParams.Add("@bioProcessingStage", bio.ProcessingStage);
                }
                if (bio.PathId != null)
                {
                    sSql += " AND bioBopPathId = @bioBopPathId";
                    sqlParams.Add("@bioBopPathId", bio.PathId);
                }
                if (bio.ParentApkId != null)
                {
                    sSql += " AND bioBioParentApkId = @bioBioParentApkId";
                    sqlParams.Add("@bioBioParentApkId", bio.ParentApkId);
                }
            }
            sSql += @"
ORDER BY bioUId ASC
";

            return ExecSelectQuery<BinaryObject>(sSql, sqlParams);
        }

        public void UpdateBinaryObjectProcessState(int UId, int state)
        {
            Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            string sSql = @"
UPDATE BinaryObject
SET bioProcessingStage = @bioProcessingStage
WHERE bioUId = @bioUId
";
            sqlParams.Add("@bioUId", UId);
            sqlParams.Add("@bioProcessingStage", state);
            ExecNonQuery(sSql, sqlParams);
        }

        public int SelectSmaliFilesCount(int state, int dataSetId)
        {
            string sSql = @"
SELECT COUNT(*)
FROM BinaryObject
    INNER JOIN DataSetApplicationCategories ON (dstcatUId = bioDstcatDataSetApplicationCategoryId)
WHERE 1=1
    AND dstcatDstDataSetId = @dstcatDstDataSetId
    AND bioProcessingStage = @bioProcessingStage
    AND bioFileName LIKE '%.smali'
";
            var count = ExecScalarSelectQuery(sSql, new Dictionary<string, object>
            {
                { "@dstcatDstDataSetId", dataSetId },
                { "@bioProcessingStage", state}
            });
            return count ?? 0;
        }

        public int SelectRootBiosCount(int dataSetId)
        {
            string sSql = @"
SELECT COUNT(*)
FROM BinaryObject
    INNER JOIN DataSetApplicationCategories ON (dstcatUId = bioDstcatDataSetApplicationCategoryId)
WHERE 1=1
    AND dstcatDstDataSetId = @dstcatDstDataSetId
    AND bioProcessingStage = @bioProcessingStage
    AND bioIsRoot = 1
";

            var count = ExecScalarSelectQuery(sSql, new Dictionary<string, object>
            {
                { "@dstcatDstDataSetId", dataSetId },
                { "@bioProcessingStage", (int)BinaryObjectApkProcessingStage.InternalsExtracted},
            });
            return count ?? 0;
        }

        public int SelectSmaliFilesCount(int dataSetId)
        {
            string sSql = @"
SELECT COUNT(*)
FROM BinaryObject
    INNER JOIN DataSetApplicationCategories ON (dstcatUId = bioDstcatDataSetApplicationCategoryId)
WHERE 1=1
    AND dstcatDstDataSetId = @dstcatDstDataSetId
    AND bioFileName LIKE '%.smali'
";
            var count = ExecScalarSelectQuery(sSql, new Dictionary<string, object>
            {
                { "@dstcatDstDataSetId", dataSetId },
            });
            return count ?? 0;
        }

        public List<BinaryObject> SelectNextRootBio(int maxId, int dataSetId)
        {
            string sSql = @"
SELECT TOP 1 *
FROM BinaryObject
    INNER JOIN DataSetApplicationCategories ON (dstcatUId = bioDstcatDataSetApplicationCategoryId)
WHERE 1=1
    AND dstcatDstDataSetId = @dstcatDstDataSetId
    AND bioProcessingStage = @bioProcessingStage
    AND bioUId > @bioUId

";
            sSql += " ORDER BY bioUId ASC";
            return ExecSelectQuery<BinaryObject>(sSql, new Dictionary<string, object>
            {
                {"@dstcatDstDataSetId", dataSetId},
                {"@bioUId", maxId},
                {"@bioProcessingStage", (int)BinaryObjectApkProcessingStage.InternalsExtracted}
            });
        }

        public List<BinaryObjectWithContent> SelectSmaliFilesToProcess(int parentId)
        {
            string sSql = @"
SELECT  *
FROM BinaryObject
    INNER JOIN BinaryObjectContent ON (bocUId = bioBocContentId)
    INNER JOIN DataSetApplicationCategories ON (dstcatUId = bioDstcatDataSetApplicationCategoryId)
WHERE 1=1
    AND bioBioParentApkId = @bioBioParentApkId
    AND bioFileName LIKE '%.smali'
ORDER BY bioUId ASC
";
            return ExecSelectQuery<BinaryObjectWithContent>(sSql, new Dictionary<string, object>
            {
                { "@bioBioParentApkId", parentId }
            }
);
        }

        public List<BinaryObjectWithContentJavaTypeId> SelectSmaliFilesToProcessWithJavaType(
            int count,
            int maxId,
            int dataSetId,
            int processingStage)
        {
            string sSql = @"
SELECT TOP " + count.ToString() + @" *
FROM BinaryObject
    INNER JOIN BinaryObjectContent ON (bocUId = bioBocContentId)
    INNER JOIN DataSetApplicationCategories ON (dstcatUId = bioDstcatDataSetApplicationCategoryId)
    INNER JOIN JavaType ON (jtypBioParentApkId = bioBioParentApkId AND jtypBocParentContentId = bioBocContentId)
WHERE 1=1
    AND dstcatDstDataSetId = @dstcatDstDataSetId
    AND bioProcessingStage = @bioProcessingStage
    AND bioFileName LIKE '%.smali'
    AND bioUId > @bioUId
ORDER BY bioUId ASC
";
            return ExecSelectQuery<BinaryObjectWithContentJavaTypeId>(sSql, new Dictionary<string, object>
            {
                { "@dstcatDstDataSetId", dataSetId },
                { "@bioUId", maxId},
                { "@bioProcessingStage", processingStage}
            }
);
        }

    }
}
