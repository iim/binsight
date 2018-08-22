using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APKInsight.Enums;
using CsnowFramework.Database;
using APKInsight.Models.DataBase;
using CsnowFramework.Crypto;
using CsnowFramework.Enum;

namespace APKInsight.Queries
{
    class QueryStringValue: QueryBase
    {
        private readonly Dictionary<StringValueType, Dictionary<string, int>> _stringValueIdsCache = new Dictionary<StringValueType, Dictionary<string, int>>();

        // Managing unique IDs
        private static readonly Dictionary<StringValueType, int> _currentStrMaxUId = new Dictionary<StringValueType, int>();
        private static bool _currentStrMaxUIdInitialized = false;
        private static readonly object _currentStrMaxUIdLock = new object();

        // Cache
        private readonly Dictionary<StringValueType, List<StringValue>> _addedStringsCache = new Dictionary<StringValueType, List<StringValue>>();

        // Hash
        private Hash _hash = new Hash("md5");

        private static bool _currentMaxUIdInitialized = false;
        private static object _currentMaxUIdLock = new object();

        public QueryStringValue(string connectionString = null): 
            base(connectionString)
        {
            _addedStringsCache.Add(StringValueType.JavaPackageName, new List<StringValue>());
            _addedStringsCache.Add(StringValueType.JavaPath, new List<StringValue>());
            _addedStringsCache.Add(StringValueType.JavaTypeFieldSmaliFullName, new List<StringValue>());
            _addedStringsCache.Add(StringValueType.JavaTypeMethodSmaliFullName, new List<StringValue>());
            _addedStringsCache.Add(StringValueType.JavaTypeSmaliFullName, new List<StringValue>());
            _addedStringsCache.Add(StringValueType.JavaTypeSourceFileName, new List<StringValue>());
            InitCurrentMaxUId();
        }

        public void InitCurrentMaxUId()
        {
            lock (_currentMaxUIdLock)
            {
                if (!_currentMaxUIdInitialized)
                {
                    _currentMaxUIdInitialized = true;

                    var query = @"
    SELECT MAX(strUId)
    FROM StringValue{0}";

                    var uid = ExecScalarSelectQuery(string.Format(query, StringValueType.JavaPackageName.GetStringValue()));
                    _currentStrMaxUId.Add(StringValueType.JavaPackageName, uid.Value + 1);

                    uid = ExecScalarSelectQuery(string.Format(query, StringValueType.JavaPath.GetStringValue()));
                    _currentStrMaxUId.Add(StringValueType.JavaPath, uid.Value + 1);

                    uid = ExecScalarSelectQuery(string.Format(query, StringValueType.JavaTypeFieldSmaliFullName.GetStringValue()));
                    _currentStrMaxUId.Add(StringValueType.JavaTypeFieldSmaliFullName, uid.Value + 1);

                    uid = ExecScalarSelectQuery(string.Format(query, StringValueType.JavaTypeMethodSmaliFullName.GetStringValue()));
                    _currentStrMaxUId.Add(StringValueType.JavaTypeMethodSmaliFullName, uid.Value + 1);

                    uid = ExecScalarSelectQuery(string.Format(query, StringValueType.JavaTypeSmaliFullName.GetStringValue()));
                    _currentStrMaxUId.Add(StringValueType.JavaTypeSmaliFullName, uid.Value + 1);

                    uid = ExecScalarSelectQuery(string.Format(query, StringValueType.JavaTypeSourceFileName.GetStringValue()));
                    _currentStrMaxUId.Add(StringValueType.JavaTypeSourceFileName, uid.Value + 1);
                }
            }
        }

        private int GetIdFromCache(StringValueType type, byte[] hash)
        {
            int result = -1;
            if (_stringValueIdsCache.ContainsKey(type))
            {
                string strHash = Convert.ToBase64String(hash);
                if (_stringValueIdsCache[type].ContainsKey(strHash))
                {
                    result = _stringValueIdsCache[type][strHash];
                }
            }
            return result;
        }

        private void AddIdToCache(StringValueType type, byte[] hash, int value)
        {
            if (!_stringValueIdsCache.ContainsKey(type))
                _stringValueIdsCache.Add(type, new Dictionary<string, int>(256000));
            string strHash = Convert.ToBase64String(hash);
            _stringValueIdsCache[type].Add(strHash, value);
        }


        public bool AddUniqueStringValue(ref StringValue strValue)
        {
            var cachedId = GetIdFromCache(strValue.Type, strValue.Hash);
            if (cachedId != -1)
            {
                strValue.UId = cachedId;
                return true;
            }
            const string sqlQuery = @"
IF NOT EXISTS (SELECT strUId FROM StringValue WHERE strType = @strType AND strHash = @strHash)
BEGIN
    INSERT INTO StringValue (strValue, strHash, strType)
    VALUES (@strValue, @strHash, @strType)
    SELECT @@IDENTITY
END
ELSE
BEGIN
    SELECT TOP 1 strUId
    FROM StringValue 
    WHERE 1=1
        AND strType = @strType
        AND strHash = @strHash
    ORDER BY strUId ASC
END
";
            strValue.UId = ExecScalarSelectQuery(sqlQuery, new Dictionary<string, object>
            {
                { "@strValue", strValue.Value},
                { "@strHash", strValue.Hash},
                { "@strType", strValue.StrType},

            }, 1);

            if (strValue.UId.HasValue)
            {
                AddIdToCache(strValue.Type, strValue.Hash, strValue.UId.Value);
            }
            return strValue.UId.HasValue && strValue.UId.Value > 0;
        }

        public bool AddStringValueNoSearch(string value, StringValueType type, ref int? uid)
        {
            var strVal = new StringValue {Value = value};
            strVal.HashValue(_hash);
            uid = GetIdFromCache(type, strVal.Hash);
            if (uid == -1)
            {
                lock (_currentStrMaxUIdLock)
                {
                    uid = _currentStrMaxUId[type];
                    _currentStrMaxUId[type] += 1;
                }
                strVal.UId = uid;
                AddIdToCache(type, strVal.Hash, strVal.UId.Value);
                _addedStringsCache[type].Add(strVal);
            }

            return uid.HasValue && uid.Value > 0;
        }

        public void SaveCache()
        {
            SaveCacheOfType(StringValueType.JavaPackageName);
            SaveCacheOfType(StringValueType.JavaPath);
            SaveCacheOfType(StringValueType.JavaTypeFieldSmaliFullName);
            SaveCacheOfType(StringValueType.JavaTypeMethodSmaliFullName);
            SaveCacheOfType(StringValueType.JavaTypeSmaliFullName);
            SaveCacheOfType(StringValueType.JavaTypeSourceFileName);
        }

        private void SaveCacheOfType(StringValueType type)
        {
            int indexBeg = 0;
            int count = 200;
            int remainingCount = _addedStringsCache[type].Count;
            var toSave = _addedStringsCache[type].GetRange(indexBeg, count > remainingCount ? remainingCount : count);
            indexBeg += toSave.Count;
            remainingCount -= toSave.Count;
            while (toSave.Any())
            {
                SaveCacheOfTypeRange(type, toSave);
                toSave = _addedStringsCache[type].GetRange(indexBeg, count > remainingCount ? remainingCount : count);
                indexBeg += toSave.Count;
                remainingCount -= toSave.Count;
            }
        }

        private void SaveCacheOfTypeRange(StringValueType type, List<StringValue> values)
        {
            var parameters = new Dictionary<string, object>();

            StringBuilder sqlQuery = new StringBuilder(8000);
            sqlQuery.Append($" SET IDENTITY_INSERT StringValue{type.GetStringValue()} ON");
            sqlQuery.Append($" INSERT INTO StringValue{type.GetStringValue()} (strUId, strValue, strHash) VALUES");
            int pId = 0;
            foreach (var stringValue in values)
            {
                sqlQuery.Append($" ({stringValue.UId.Value}, @p{pId}, @p{pId + 1}),");
                parameters.Add($"@p{pId}", stringValue.Value);
                parameters.Add($"@p{pId + 1}", stringValue.Hash);
                pId += 2;
            }

            sqlQuery = sqlQuery.Remove(sqlQuery.Length - 1, 1);
            sqlQuery.Append($" SET IDENTITY_INSERT StringValue{type.GetStringValue()} OFF");

            var inserted = ExecNonQuery(sqlQuery.ToString(), parameters);
            if (inserted != values.Count)
            {
            }
        }

        public List<StringValue> SelectStringValuesOfType()
        {
            const string sqlQuery = @"

SELECT 
    strUId,
    strValue
FROM StringValue
";
            return  ExecSelectQuery<StringValue>(sqlQuery);
        }
        public List<StringValue> SelectStringValuesOfType(StringValueType type)
        {
            string sqlQuery = $@"

SELECT 
    strUId,
    strValue
FROM StringValue{type.GetStringValue()}
";
            return ExecSelectQuery<StringValue>(sqlQuery);
        }

        public StringValue SelectStringValue(string type, byte[] hash)
        {
            const string sqlQuery = @"

SELECT 
    strUId,
    strValue
FROM StringValue 
WHERE strType = @strType AND strHash = @strHash
";
            var result = ExecSelectQuery<StringValue>(
                        sqlQuery,
                        new Dictionary<string, object>
                        {
                            { "@strType", type },
                            { "@strHash", hash}
                        });
            return result.Any() ? result[0] : null;

        }

        public StringValue SelectStringValueById(int id)
        {
            const string sqlQuery = @"

SELECT 
    strUId,
    strValue
FROM StringValue 
WHERE strUId = @strUId
";
            var result = ExecSelectQuery<StringValue>(
                        sqlQuery,
                        new Dictionary<string, object>
                        {
                            { "@strUId", id },
                        });
            return result.Any() ? result[0] : null;

        }


        #region Index handling queries

        public void CreateUniqueIndex()
        {
            var sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='UQX_strType_strHash' AND object_id = OBJECT_ID('StringValue'))
BEGIN
CREATE UNIQUE INDEX UQX_strType_strHash ON StringValue(strType, strHash)
END
";
            ExecNonQuery(sql);
        }

        public void CreateNonclusteredIndex()
        {
            var sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_strType_strHash' AND object_id = OBJECT_ID('StringValue'))
BEGIN
CREATE NONCLUSTERED INDEX IDX_strType_strHash ON StringValue(strType, strHash)
END
";
            ExecNonQuery(sql);
        }

        public void DropUniqueIndex()
        {
            var sql = @"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='UQX_strType_strHash' AND object_id = OBJECT_ID('StringValue'))
BEGIN
DROP INDEX UQX_strType_strHash ON StringValue
END
";
            ExecNonQuery(sql);
        }

        public void DropAllIndices()
        {
            var sql = @"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_strType_strHash' AND object_id = OBJECT_ID('StringValue'))
BEGIN
DROP INDEX IDX_strType_strHash ON StringValue
END
";
            ExecNonQuery(sql);

            DropNonclusturedIndex(StringValueType.JavaTypeFieldSmaliFullName);
            DropNonclusturedIndex(StringValueType.JavaTypeMethodSmaliFullName);
            DropNonclusturedIndex(StringValueType.JavaPath);
            DropNonclusturedIndex(StringValueType.JavaPackageName);
            DropNonclusturedIndex(StringValueType.JavaTypeSmaliFullName);
            DropNonclusturedIndex(StringValueType.JavaTypeSourceFileName);
        }

        #endregion


        #region Data Base Compression

        public bool IsCompressionRequired()
        {

            var sql = @"IF EXISTS(SELECT * FROM sys.indexes WHERE name='UQX_strHash_{0}' AND object_id = OBJECT_ID('StringValue{0}'))
BEGIN
SELECT 1
END
ELSE
BEGIN
SELECT 0
END";
            var result = ExecScalarSelectQuery(string.Format(sql, StringValueType.JavaTypeFieldSmaliFullName.GetStringValue()));
            if (result.Value == 0)
                return true;
            result = ExecScalarSelectQuery(string.Format(sql, StringValueType.JavaTypeMethodSmaliFullName.GetStringValue()));
            if (result.Value == 0)
                return true;
            result = ExecScalarSelectQuery(string.Format(sql, StringValueType.JavaPath.GetStringValue()));
            if (result.Value == 0)
                return true;
            result = ExecScalarSelectQuery(string.Format(sql, StringValueType.JavaPackageName.GetStringValue()));
            if (result.Value == 0)
                return true;
            result = ExecScalarSelectQuery(string.Format(sql, StringValueType.JavaTypeSmaliFullName.GetStringValue()));
            if (result.Value == 0)
                return true;
            result = ExecScalarSelectQuery(string.Format(sql, StringValueType.JavaTypeSourceFileName.GetStringValue()));
            if (result.Value == 0)
                return true;

            sql = @"IF EXISTS(SELECT * FROM sys.indexes WHERE name='UQ_jtypBioParentApkId_jtypStrSmaliFullNameId' AND object_id = OBJECT_ID('JavaType'))
BEGIN
SELECT 1
END
ELSE
BEGIN
SELECT 0
END";
            result = ExecScalarSelectQuery(sql);
            if (result.Value == 0)
                return true;

            sql = @"IF EXISTS(SELECT * FROM sys.indexes WHERE name='UQ_JavaTypeImplementedInterface' AND object_id = OBJECT_ID('JavaTypeImplementedInterface'))
BEGIN
SELECT 1
END
ELSE
BEGIN
SELECT 0
END";
            result = ExecScalarSelectQuery(sql);
            if (result.Value == 0)
                return true;

            return false;
        }

        public void AddCompressionColumnToStringValueTable(StringValueType type)
        {
            var sql = $@"
IF NOT EXISTS(
 SELECT *
    FROM sys.columns 
    WHERE Name      = N'strStrCompressId'
      AND Object_ID = Object_ID(N'StringValue{type.GetStringValue()}'))
BEGIN
ALTER TABLE StringValue{type.GetStringValue()} ADD strStrCompressId INT NULL
END
";

            ExecNonQuery(sql);
        }

        public void CreateNonclusteredIndex(StringValueType type)
        {
            var sql = $@"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_strHash_{type.GetStringValue()}' AND object_id = OBJECT_ID('StringValue{type.GetStringValue()}'))
BEGIN
CREATE NONCLUSTERED INDEX IDX_strHash_{type.GetStringValue()} ON StringValue{type.GetStringValue()}(strHash)
END
";
            ExecNonQuery(sql);
        }

        public void DropNonclusturedIndex(StringValueType type)
        {
            var sql = $@"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_strHash_{type.GetStringValue()}' AND object_id = OBJECT_ID('StringValue{type.GetStringValue()}'))
BEGIN
DROP INDEX IDX_strHash_{type.GetStringValue()} ON StringValue{type.GetStringValue()}
END
";
            ExecNonQuery(sql);
        }

        public void SetCompressionIdColumnValue(StringValueType type)
        {
            var sql = $@"
UPDATE StringValue{type.GetStringValue()}
SET StringValue{type.GetStringValue()}.strStrCompressId = TmpTable.strMinUId
FROM
(
  SELECT strMinUId = MIN(strUId), strHash, cnt = COUNT(*)
  FROM StringValue{type.GetStringValue()}
  WHERE strUId > 0
  GROUP BY strHash
  HAVING COUNT(*) > 1
) AS TmpTable
WHERE StringValue{type.GetStringValue()}.strHash = TmpTable.strHash
";
            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeIdsToMinIds(StringValueType type, string columnName)
        {
            var sql = $@"
UPDATE JavaType
SET {columnName} = TmpTable.strStrCompressId
FROM 
(
  SELECT *
  FROM JavaType
    INNER JOIN StringValue{type.GetStringValue()} ON (strUId = {columnName})
  WHERE strStrCompressId IS NOT NULL AND strUId > 0
) AS TmpTable
WHERE JavaType.jtypUId = TmpTable.jtypUId
";
            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeFieldIdsToMinIds()
        {
            var sql = $@"
UPDATE JavaTypeField
SET jtfStrSmaliNameId = TmpTable.strStrCompressId
FROM 
(
  SELECT *
  FROM JavaTypeField
    INNER JOIN StringValue{StringValueType.JavaTypeFieldSmaliFullName.GetStringValue()} ON (strUId = jtfStrSmaliNameId)
  WHERE strStrCompressId IS NOT NULL AND strUId > 0
) AS TmpTable
WHERE JavaTypeField.jtfUId = TmpTable.jtfUId
";
            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeFieldUseIdsToMinIds()
        {
            var sql = $@"
UPDATE JavaTypeUsedInType
SET jtuStrDestinationFieldSmaliNameId = TmpTable.strStrCompressId
FROM 
(
  SELECT *
  FROM JavaTypeUsedInType
    INNER JOIN StringValue{StringValueType.JavaTypeFieldSmaliFullName.GetStringValue()} ON (strUId = jtuStrDestinationFieldSmaliNameId)
  WHERE strStrCompressId IS NOT NULL AND strUId > 0
) AS TmpTable
WHERE JavaTypeUsedInType.jtuUId = TmpTable.jtuUId
";
            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeFieldUseWithFieldId()
        {
            var sql = @"
UPDATE JavaTypeUsedInType
SET 
  JavaTypeUsedInType.jtuJtfDestinationFieldId = TmpTable.jtfUId
FROM 
(
    SELECT *
    FROM JavaTypeUsedInType
      INNER JOIN JavaTypeField ON (jtfStrSmaliNameId = jtuStrDestinationFieldSmaliNameId)
    WHERE jtuStrDestinationFieldSmaliNameId > 0 AND jtuJtfDestinationFieldId = 0
) AS TmpTable
WHERE TmpTable.jtuUId = JavaTypeUsedInType.jtuUId
";
            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeMethodIdsToMinIds()
        {
            var sql = $@"
UPDATE JavaTypeMethod
SET jtmStrSmaliNameId = TmpTable.strStrCompressId
FROM 
(
  SELECT *
  FROM JavaTypeMethod
    INNER JOIN StringValue{StringValueType.JavaTypeMethodSmaliFullName.GetStringValue()} ON (strUId = jtmStrSmaliNameId)
  WHERE strStrCompressId IS NOT NULL AND strUId > 0
) AS TmpTable
WHERE JavaTypeMethod.jtmUId = TmpTable.jtmUId
";
            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeMethodUseIdsToMinIds()
        {
            var sql = $@"
UPDATE JavaTypeUsedInType
SET jtuStrDestinationMethodSmaliNameId = TmpTable.strStrCompressId
FROM 
(
  SELECT *
  FROM JavaTypeUsedInType
    INNER JOIN StringValue{StringValueType.JavaTypeMethodSmaliFullName.GetStringValue()} ON (strUId = jtuStrDestinationMethodSmaliNameId)
  WHERE strStrCompressId IS NOT NULL AND strUId > 0
) AS TmpTable
WHERE JavaTypeUsedInType.jtuUId = TmpTable.jtuUId
";
            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeMethodUseWithMethodId()
        {
            var sql = @"
UPDATE JavaTypeUsedInType
SET 
  JavaTypeUsedInType.jtuJtmDestinationMethodId = TmpTable.jtmUId
FROM 
(
SELECT *
FROM JavaTypeUsedInType
  INNER JOIN JavaTypeMethod ON (jtmStrSmaliNameId = jtuStrDestinationMethodSmaliNameId)
WHERE jtuStrDestinationMethodSmaliNameId > 0 AND jtuJtmDestinationMethodId = 0
) AS TmpTable
WHERE TmpTable.jtuUId = JavaTypeUsedInType.jtuUId
";
            ExecNonQuery(sql);
        }

        public void DeleteDuplicatedStrings(StringValueType type)
        {
            var sql = $@"
DELETE
FROM StringValue{type.GetStringValue()}
WHERE strStrCompressId IS NOT NULL AND strStrCompressId <> strUId";
            ExecNonQuery(sql);
        }

        public void CreateUniqueIndex(StringValueType type)
        {
            var sql = $@"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='UQX_strHash_{type.GetStringValue()}' AND object_id = OBJECT_ID('StringValue{type.GetStringValue()}'))
BEGIN
CREATE UNIQUE INDEX UQX_strHash_{type.GetStringValue()} ON StringValue{type.GetStringValue()}(strHash)
END
";
            ExecNonQuery(sql);
        }
        public void DropCompressColumn(StringValueType type)
        {
            var sql = $@"
IF EXISTS(
 SELECT *
    FROM sys.columns 
    WHERE Name      = N'strStrCompressId'
      AND Object_ID = Object_ID(N'StringValue{type.GetStringValue()}'))
BEGIN
ALTER TABLE StringValue{type.GetStringValue()} DROP COLUMN strStrCompressId
END
";
            ExecNonQuery(sql);
        }

        public void DropUniqueIndex(StringValueType type)
        {
            var sql = $@"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='UQX_strHash_{type.GetStringValue()}' AND object_id = OBJECT_ID('StringValue{type.GetStringValue()}'))
BEGIN
DROP INDEX UQX_strHash_{type.GetStringValue()} ON StringValue{type.GetStringValue()}
END
";
            ExecNonQuery(sql);
        }

        #endregion
    }
}
