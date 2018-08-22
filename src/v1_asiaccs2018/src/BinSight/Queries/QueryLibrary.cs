using System.Collections.Generic;
using APKInsight.Models.DataBase;
using CsnowFramework.Database;

namespace APKInsight.Queries
{
    class QueryLibrary: QueryBase
    {
        #region SELECT Queries

        public List<LibraryCandidate> SelectAllPendingCandidates()
        {
            var sSql = @"
SELECT *
FROM
(
    SELECT 
        jtypStrPackageNameId, 
        strValue, 
        NumberOfBinaries = COUNT(*)
    FROM 
    (
        SELECT DISTINCT 
                jtypBioParentApkId, 
                jtypStrPackageNameId
        FROM JavaType
    ) AS TmpTable1
        INNER JOIN StringValuePKGN ON (strUId = jtypStrPackageNameId)
    WHERE 1 = 1
        AND jtypStrPackageNameId NOT IN (SELECT lalStrPackageNameId FROM LibraryAliases)
    GROUP BY jtypStrPackageNameId, strValue
    HAVING COUNT(*) > 1
) AS TmpTable2
ORDER BY NumberOfBinaries DESC";
            return ExecSelectQuery<LibraryCandidate>(sSql);
        }

        public List<LibraryPropertyTypes> SelectAllLibraryPropertyTypes()
        {
            var sSql = @"
SELECT *
FROM LibraryPropertyTypes
ORDER BY lptName
";
            return ExecSelectQuery<LibraryPropertyTypes>(sSql);
        }

        public List<LibraryProperties> SelectAllLibraryProperties(int libId)
        {
            var sSql = @"
SELECT *
FROM LibraryProperties
WHERE lprLibLibraryId = @lprLibLibraryId
";
            return ExecSelectQuery<LibraryProperties>(sSql, new Dictionary<string, object>
            {
                {"@lprLibLibraryId", libId }
            });
        }

        public bool DeleteLibraryProperty(int propertyId)
        {
            var sSql = @"
DELETE
FROM LibraryProperties
WHERE lprUId = @lprUId
";
            return ExecNonQuery(sSql, new Dictionary<string, object>
            {
                {"@lprUId", propertyId }
            }) > 0;
        }

        public List<LibraryCandidate> SelectAllPackagesInLibrary(int libraryId)
        {
            var sSql = @"
SELECT *
FROM
(
    SELECT 
        jtypStrPackageNameId, 
        strValue, 
        NumberOfBinaries = COUNT(*)
    FROM 
    (
        SELECT DISTINCT 
                jtypBioParentApkId, 
                jtypStrPackageNameId
        FROM JavaType
    ) AS TmpTable1
        INNER JOIN StringValuePKGN ON (strUId = jtypStrPackageNameId)
        INNER JOIN LibraryAliases ON (lalStrPackageNameId = strUId)
    WHERE 1 = 1
        AND lalLibLibraryId = @lalLibLibraryId
    GROUP BY jtypStrPackageNameId, strValue
) AS TmpTable2
ORDER BY NumberOfBinaries DESC";
            return ExecSelectQuery<LibraryCandidate>(sSql, new Dictionary<string, object>
            {
                {"@lalLibLibraryId", libraryId }
            });
        }

        public List<LibraryAliases> SelectAllLibraryAliases()
        {
            var sSql = @"
SELECT *
FROM LibraryAliases";
            return ExecSelectQuery<LibraryAliases>(sSql);
        }

        /// <summary>
        /// Selects all libraries as a 
        /// </summary>
        /// <returns></returns>
        public List<Library> SelectAllDefinedLibraries()
        {
            var sSql = @"
SELECT *
FROM Library
ORDER BY libName";
            return ExecSelectQuery<Library>(sSql);

        }

        public List<Library> SelectLibraryById(int id)
        {
            var sSql = @"
SELECT *
FROM Library
WHERE libUId = @libUId
";
            return ExecSelectQuery<Library>(sSql, new Dictionary<string, object>
            {
                {"@libUId", id }
            });

        }
        #endregion

        public bool InsertLinkBetweenLibraryAndPackage(int libraryId, int packageId)
        {
            var sSql = @"
IF NOT EXISTS (SELECT lalUId FROM LibraryAliases WHERE lalLibLibraryId = @lalLibLibraryId AND lalStrPackageNameId = @lalStrPackageNameId)
BEGIN
    INSERT INTO LibraryAliases (lalLibLibraryId, lalStrPackageNameId)
    VALUES (@lalLibLibraryId, @lalStrPackageNameId)
    SELECT @@IDENTITY
END
ELSE
BEGIN
    SELECT lalUId
    FROM LibraryAliases 
    WHERE 1=1
        AND lalLibLibraryId = @lalLibLibraryId
        AND lalStrPackageNameId = @lalStrPackageNameId
END

";
            var id = ExecScalarSelectQuery(sSql, new Dictionary<string, object>
            {
                { "@lalLibLibraryId", libraryId},
                { "@lalStrPackageNameId", packageId}

            }, 1);

            return id.HasValue && id.Value > 0;
        }

        public bool RemoveLinkBetweenLibraryAndPackage(int libraryId, int packageId)
        {
            var sSql = @"
DELETE
FROM LibraryAliases
WHERE 1 = 1
    AND lalLibLibraryId = @lalLibLibraryId
    AND lalStrPackageNameId = @lalStrPackageNameId
";
            return ExecNonQuery(sSql, new Dictionary<string, object>
                {
                    { "@lalLibLibraryId", libraryId},
                    { "@lalStrPackageNameId", packageId}

                }) > 0;

        }
    }
}
