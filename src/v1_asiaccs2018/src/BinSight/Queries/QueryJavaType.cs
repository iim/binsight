using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APKInsight.Models.DataBase;
using CsnowFramework.Database;
using JavaType = APKInsight.Models.DataBase.JavaType;

namespace APKInsight.Queries
{
    class QueryJavaType : QueryBase
    {

        private readonly List<JavaType> _addedCache = new List<JavaType>();
        private readonly Dictionary<int, JavaType> _addedLookupCache = new Dictionary<int, JavaType>(32000);

        private readonly List<JavaTypeImplementedInterface> _addedCacheInterfce =
            new List<JavaTypeImplementedInterface>();

        private readonly List<JavaTypeField> _addedCacheField = new List<JavaTypeField>();

        private readonly Dictionary<int, JavaTypeField> _addedCacheFieldLookup =
            new Dictionary<int, JavaTypeField>(320000);

        private List<JavaTypeMethod> _addedCacheMethod = new List<JavaTypeMethod>();

        private readonly Dictionary<int, JavaTypeMethod> _addedCacheMethodLookup =
            new Dictionary<int, JavaTypeMethod>(320000);

        private static bool _currentMaxUIdInterfaceInitialized = false;
        private static int _currentMaxUIdInterface;
        private static object _currentMaxUIdInterfaceLock = new object();

        private static bool _currentMaxUIdFieldInitialized = false;
        private static int _currentMaxUIdField;
        private static object _currentMaxUIdFieldLock = new object();

        private static bool _currentMaxUIdMethodInitialized = false;
        private static int _currentMaxUIdMethod;
        private static object _currentMaxUIdMethodLock = new object();

        private static int _currentMaxUId = -1;
        private static bool _currentMaxUIdInitialized = false;
        private static object _currentMaxUIdLock = new object();

        protected int GetNextUId()
        {
            int nextId;
            lock (_currentMaxUIdLock)
            {
                nextId = _currentMaxUId + 1;
                _currentMaxUId++;
            }
            return nextId;
        }

        public QueryJavaType(string connectionString = null) :
            base(connectionString)
        {
            InitCurrentMaxUId("JavaTypeImplementedInterface", "jtiiUId", ref _currentMaxUIdInterface,
                ref _currentMaxUIdInterfaceInitialized, ref _currentMaxUIdInterfaceLock);
            InitCurrentMaxUId("JavaTypeField", "jtfUId", ref _currentMaxUIdField, ref _currentMaxUIdFieldInitialized,
                ref _currentMaxUIdFieldLock);
            InitCurrentMaxUId("JavaTypeMethod", "jtmUId", ref _currentMaxUIdMethod, ref _currentMaxUIdMethodInitialized,
                ref _currentMaxUIdMethodLock);
            InitCurrentMaxUId("JavaType", "jtypUId", ref _currentMaxUId, ref _currentMaxUIdInitialized,
                ref _currentMaxUIdLock);
        }

        public int GetMethodId(int smaliNameId)
        {
            if (_addedCacheMethodLookup.ContainsKey(smaliNameId))
                return _addedCacheMethodLookup[smaliNameId].UId.Value;
            return 0;
        }

        public int GetFieldId(int smaliNameId)
        {
            if (_addedCacheFieldLookup.ContainsKey(smaliNameId))
                return _addedCacheFieldLookup[smaliNameId].UId.Value;
            return 0;
        }

        #region Simple Selects

        public List<JavaType> SelectJavaTypes(int parentBioId)
        {
            var sSql = @"
SELECT *
FROM JavaType
WHERE 1=1
    AND jtypBioParentApkId = @jtypBioParentApkId
    AND jtypIsReferenceOnly = 0
";

            return ExecSelectQuery<JavaType>(sSql, new Dictionary<string, object>
            {
                {"@jtypBioParentApkId", parentBioId},
            });
        }

        #endregion


        public int InsertObjectIntoCache(JavaType jt)
        {
            if (jt.PackageNameId == 107 && !jt.IsReferenceOnly.Value)
            {
            }
            if (_addedLookupCache.ContainsKey(jt.SmaliFullNameId.Value))
            {
                var existingJt = _addedLookupCache[jt.SmaliFullNameId.Value];
                jt.UId = existingJt.UId;
                // Update Existing Jt only if jt is not re
                if (existingJt.IsReferenceOnly.Value && !jt.IsReferenceOnly.Value)
                {
                    existingJt.ParentApkId = jt.ParentApkId;
                    existingJt.ParentContentId = jt.ParentContentId;

                    existingJt.AccessControl = jt.AccessControl;
                    existingJt.DbgSourceNotFound = jt.DbgSourceNotFound;

                    existingJt.FileNameId = jt.FileNameId;
                    existingJt.PackageNameId = jt.PackageNameId;
                    existingJt.PathId = jt.PathId;

                    existingJt.OuterClassId = jt.OuterClassId;
                    existingJt.SuperClassId = jt.SuperClassId;

                    existingJt.IsAbstract = jt.IsAbstract;
                    existingJt.IsAnnotation = jt.IsAnnotation;
                    existingJt.IsClass = jt.IsClass;
                    existingJt.IsEnum = jt.IsEnum;
                    existingJt.IsFinal = jt.IsFinal;
                    existingJt.IsInterface = jt.IsInterface;
                    existingJt.IsReferenceOnly = jt.IsReferenceOnly;
                    existingJt.IsStatic = jt.IsStatic;
                }
            }
            else
            {
                jt.UId = GetNextUId();
                _addedCache.Add(jt);
                _addedLookupCache.Add(jt.SmaliFullNameId.Value, jt);
            }
            return jt.UId.Value;
        }

        protected int GetNextInterfaceUId()
        {
            int nextId;
            lock (_currentMaxUIdInterfaceLock)
            {
                nextId = _currentMaxUIdInterface + 1;
                _currentMaxUIdInterface++;
            }
            return nextId;
        }

        protected int GetNextFieldUId()
        {
            int nextId;
            lock (_currentMaxUIdFieldLock)
            {
                nextId = _currentMaxUIdField + 1;
                _currentMaxUIdField++;
            }
            return nextId;
        }

        protected int GetNextMethodUId()
        {
            int nextId;
            lock (_currentMaxUIdMethodLock)
            {
                nextId = _currentMaxUIdMethod + 1;
                _currentMaxUIdMethod++;
            }
            return nextId;
        }

        public int InsertObjectIntoCache(JavaTypeImplementedInterface jt)
        {
            jt.UId = GetNextInterfaceUId();
            _addedCacheInterfce.Add(jt);
            return jt.UId.Value;
        }

        public int InsertObjectIntoCache(JavaTypeField jt)
        {
            if (_addedCacheFieldLookup.ContainsKey(jt.SmaliNameId.Value))
            {
                var existingJt = _addedCacheFieldLookup[jt.SmaliNameId.Value];
                jt.UId = existingJt.UId;
            }
            else
            {
                jt.UId = GetNextFieldUId();
                _addedCacheField.Add(jt);
                _addedCacheFieldLookup.Add(jt.SmaliNameId.Value, jt);
            }
            return jt.UId.Value;

        }

        public int InsertObjectIntoCache(JavaTypeMethod jt)
        {
            if (_addedCacheMethodLookup.ContainsKey(jt.SmaliNameId.Value))
            {
                var existingJt = _addedCacheMethodLookup[jt.SmaliNameId.Value];
                jt.UId = existingJt.UId;
            }
            else
            {
                jt.UId = GetNextMethodUId();
                _addedCacheMethod.Add(jt);
                _addedCacheMethodLookup.Add(jt.SmaliNameId.Value, jt);
            }
            return jt.UId.Value;
        }

        public void SaveCache()
        {
            SaveJavaTypes();
            SaveImplementedInterfaces();
            SaveFields();
            SaveMethods();
        }

        private void SaveJavaTypes()
        {
            int indexBeg = 0;
            int count = 100;
            int remainingCount = _addedCache.Count;
            var toSave = _addedCache.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
            indexBeg += toSave.Count;
            remainingCount -= toSave.Count;
            while (toSave.Any())
            {
                SaveJavaTypesRange(toSave);
                toSave = _addedCache.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
                indexBeg += toSave.Count;
                remainingCount -= toSave.Count;
            }
        }

        private void SaveImplementedInterfaces()
        {
            int indexBeg = 0;
            int count = 200;
            int remainingCount = _addedCacheInterfce.Count;
            var toSave = _addedCacheInterfce.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
            indexBeg += toSave.Count;
            remainingCount -= toSave.Count;
            while (toSave.Any())
            {
                SaveJavaTypesInterfacesRange(toSave);
                toSave = _addedCacheInterfce.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
                indexBeg += toSave.Count;
                remainingCount -= toSave.Count;
            }
        }

        private void SaveFields()
        {
            int indexBeg = 0;
            int count = 200;
            int remainingCount = _addedCacheField.Count;
            var toSave = _addedCacheField.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
            indexBeg += toSave.Count;
            remainingCount -= toSave.Count;
            while (toSave.Any())
            {
                SaveJavaTypeFieldsRange(toSave);
                toSave = _addedCacheField.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
                indexBeg += toSave.Count;
                remainingCount -= toSave.Count;
            }
        }

        private void SaveMethods()
        {
            int indexBeg = 0;
            int count = 200;
            int remainingCount = _addedCacheMethod.Count;
            var toSave = _addedCacheMethod.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
            indexBeg += toSave.Count;
            remainingCount -= toSave.Count;
            while (toSave.Any())
            {
                SaveJavaTypeMethodsRange(toSave);
                toSave = _addedCacheMethod.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
                indexBeg += toSave.Count;
                remainingCount -= toSave.Count;
            }
        }

        private void SaveJavaTypesRange(List<JavaType> values)
        {
            var sSql = new StringBuilder(8000);
            sSql.Append(@"
SET IDENTITY_INSERT JavaType ON

INSERT INTO JavaType 
(
    jtypUId,

    jtypBioParentApkId, 
    jtypBocParentContentId, 
    jtypAccessControl,

    jtypStrPackageNameId, 
    jtypStrSmaliFullNameId,
    jtypStrPathId,
    jtypStrFileNameId,

    jtypIsClass,
    jtypIsInterface,
    jtypIsFinal,
    jtypIsEnum,

    jtypIsAbstract,
    jtypIsAnnotation,
    jtypIsStatic,
    jtypIsReferenceOnly,

    jtypJtypOuterClassId,
    jtypJtypSuperClassId,
    jtypDbgSourceNotFound
)
VALUES
");
            var parameters = new Dictionary<string, object>();
            foreach (var type in values)
            {
                if (type.IsReferenceOnly.Value)
                {
                    type.ParentApkId = 0;
                }
                sSql.Append(
                    $@" (
{type.UId},
{type.ParentApkId},
{type.ParentContentId},
{type.AccessControl},
{type.PackageNameId},
{type.SmaliFullNameId},
{type.PathId},
{type.FileNameId},
{Convert.ToInt32(type.IsClass)},
{Convert.ToInt32(type.IsInterface)},
{Convert.ToInt32(type.IsFinal)},
{Convert.ToInt32(type.IsEnum)},
{Convert.ToInt32(type.IsAbstract)},
{Convert.ToInt32(type.IsAnnotation)},
{Convert.ToInt32(type.IsStatic)},
{Convert.ToInt32(type.IsReferenceOnly)},
{type.OuterClassId},
{type.SuperClassId},
{Convert.ToInt32(type.DbgSourceNotFound)}),");
            }

            sSql = sSql.Remove(sSql.Length - 1, 1);
            sSql.Append($" SET IDENTITY_INSERT JavaType OFF");

            var inserted = ExecNonQuery(sSql.ToString(), parameters);
            if (inserted != values.Count)
            {
            }

        }

        private void SaveJavaTypesInterfacesRange(List<JavaTypeImplementedInterface> values)
        {
            var sSql = new StringBuilder(8000);
            sSql.Append(@"
SET IDENTITY_INSERT JavaTypeImplementedInterface ON

INSERT INTO JavaTypeImplementedInterface (jtiiUId, jtiiJtypClassId, jtiiJtypInterfaceId)
VALUES
");
            var parameters = new Dictionary<string, object>();
            foreach (var type in values)
            {
                sSql.Append($@" ({type.UId},{type.ClassId}, {type.InterfaceId}),");
            }

            sSql = sSql.Remove(sSql.Length - 1, 1);
            sSql.Append($" SET IDENTITY_INSERT JavaTypeImplementedInterface OFF");

            var inserted = ExecNonQuery(sSql.ToString(), parameters);
            if (inserted != values.Count)
            {
            }

        }

        private void SaveJavaTypeFieldsRange(List<JavaTypeField> values)
        {
            var sSql = new StringBuilder(8000);
            sSql.Append(@"
SET IDENTITY_INSERT JavaTypeField ON

INSERT INTO JavaTypeField 
(
    jtfUId,

    jtfJtypInTypeId, 
    jtfStrSmaliNameId, 
    jtfJtypOfTypeId,

    jtfAccessControl, 
    jtfIsArray,
    jtfIsStatic,
    jtfIsFinal,

    jtfIsSynthetic,
    jtfIsEnum,

    jtfSourceCodeIndex
)
VALUES
");
            var parameters = new Dictionary<string, object>();
            foreach (var type in values)
            {
                sSql.Append(
                    $@" ({type.UId},{type.InTypeId}, {type.SmaliNameId}, {type.OfTypeId}, 
{type.AccessControl}, {Convert
                        .ToInt32(type.IsArray)}, {Convert.ToInt32(type.IsStatic)}, {Convert.ToInt32(type.IsFinal)},
{Convert
                            .ToInt32(type.IsSynthetic)},{Convert.ToInt32(type.IsIsEnum)}, {type.SourceCodeIndex}),");
            }

            sSql = sSql.Remove(sSql.Length - 1, 1);
            sSql.Append($" SET IDENTITY_INSERT JavaTypeField OFF");

            var inserted = ExecNonQuery(sSql.ToString(), parameters);
            if (inserted != values.Count)
            {
            }

        }

        private void SaveJavaTypeMethodsRange(List<JavaTypeMethod> values)
        {
            var sSql = new StringBuilder(8000);
            sSql.Append(@"
SET IDENTITY_INSERT JavaTypeMethod ON

INSERT INTO JavaTypeMethod 
(
    jtmUId,

    jtmJtypInTypeId, 
    jtmStrSmaliNameId, 
    jtmJtypReturnTypeId,

    jtmAccessControl, 
    jtmIsAbstract,
    jtmIsConstructor,
    jtmIsStatic,

    jtmSourceCodeIndexBeg,
    jtmSourceCodeIndexEnd
)
VALUES
");
            var parameters = new Dictionary<string, object>();
            foreach (var type in values)
            {
                sSql.Append(
                    $@" ({type.UId},{type.InTypeId}, {type.SmaliNameId}, {type.ReturnTypeId}, 
{type.AccessControl}, {Convert
                        .ToInt32(type.IsAbstract)}, {Convert.ToInt32(type.IsConstructor)}, {Convert.ToInt32(
                            type.IsStatic)},
{type.SourceCodeIndexBeg}, {type.SourceCodeIndexEnd}),");
            }

            sSql = sSql.Remove(sSql.Length - 1, 1);
            sSql.Append($" SET IDENTITY_INSERT JavaTypeMethod OFF");

            var inserted = ExecNonQuery(sSql.ToString(), parameters);
            if (inserted != values.Count)
            {
            }
        }

        public int SelectTypeIdForBinaryObject(int bioUId)
        {
            var sSql = @"
                    SELECT jtypUId
                    FROM JavaType
                    WHERE 1=1
                        AND jtypBocParentContentId = @jtypBocParentContentId";

            var parameters = new Dictionary<string, object>()
            {
                {"@jtypBocParentContentId ", bioUId}
            };
            var resId = ExecScalarSelectQuery(sSql, parameters);
            if (resId.HasValue)
                return resId.Value;
            return -1;
        }

        public int SelectTypeIdForInApkReference(JavaType jt)
        {
            var sSql = @"
IF EXISTS 
                (
                    SELECT jtypUId
                    FROM JavaType
                    WHERE 1=1
                        AND jtypBioParentApkId = @bioUId
                        AND jtypStrPackageNameId = @jtypStrPackageNameId
                        AND jtypStrProcessedFullNameId = @jtypStrProcessedFullNameId
                        AND jtypStrSmaliFullNameId = @jtypStrSmaliFullNameId
                        AND jtypStrShortNameId = @jtypStrShortNameId
                        AND jtypStrShortShortNameId = @jtypStrShortShortNameId
                )
BEGIN
    SELECT jtypUId
    FROM JavaType
    WHERE 1=1
        AND jtypBioParentApkId = @bioUId
        AND jtypStrPackageNameId = @jtypStrPackageNameId
        AND jtypStrProcessedFullNameId = @jtypStrProcessedFullNameId
        AND jtypStrSmaliFullNameId = @jtypStrSmaliFullNameId
        AND jtypStrShortNameId = @jtypStrShortNameId
        AND jtypStrShortShortNameId = @jtypStrShortShortNameId
END
ELSE
BEGIN
    SELECT 0
END
";

            var parameters = new Dictionary<string, object>()
            {
                {"@bioUId", jt.ParentApkId},
                {"@jtypStrPackageNameId", jt.PackageNameId},
                {"@jtypStrSmaliFullNameId", jt.SmaliFullNameId},
            };
            var resId = ExecScalarSelectQuery(sSql, parameters);
            if (resId.HasValue)
                return resId.Value;
            return -1;
        }


        public JavaType SelectJavaType(int typeId)
        {
            var sSql = @"
SELECT *
FROM JavaType
WHERE 1=1
    AND jtypUId = @jtypUId
";

            var results = ExecSelectQuery<JavaType>(sSql, new Dictionary<string, object>
            {
                {"@jtypUId ", typeId},
            });

            return results.Any() ? results[0] : null;
        }

        public List<JavaType> SelectAllJavaTypes()
        {
            var sSql = @"
SELECT *
FROM JavaType
";

            return ExecSelectQuery<JavaType>(sSql);
        }

        public int SelectTypeIdForNotInApkShallowReference(JavaType jt)
        {
            var sSql = @"
IF EXISTS 
                (
                    SELECT jtypUId
                    FROM JavaType
                    WHERE 1=1
                        AND jtypBioParentApkId = 0
                        AND jtypBocParentContentId = 0
                        AND jtypIsReferenceOnly = 1
                        AND jtypStrPackageNameId = @jtypStrPackageNameId
                        AND jtypStrProcessedFullNameId = @jtypStrProcessedFullNameId
                        AND jtypStrSmaliFullNameId = @jtypStrSmaliFullNameId
                        AND jtypStrShortNameId = @jtypStrShortNameId
                        AND jtypStrShortShortNameId = @jtypStrShortShortNameId
                )
BEGIN
    SELECT jtypUId
    FROM JavaType
    WHERE 1=1
        AND jtypBioParentApkId = 0
        AND jtypBocParentContentId = 0
        AND jtypIsReferenceOnly = 1
        AND jtypStrPackageNameId = @jtypStrPackageNameId
        AND jtypStrProcessedFullNameId = @jtypStrProcessedFullNameId
        AND jtypStrSmaliFullNameId = @jtypStrSmaliFullNameId
        AND jtypStrShortNameId = @jtypStrShortNameId
        AND jtypStrShortShortNameId = @jtypStrShortShortNameId
END
ELSE
BEGIN
    SELECT 0
END
";

            var parameters = new Dictionary<string, object>()
            {
                {"@jtypStrPackageNameId", jt.PackageNameId},
                {"@jtypStrSmaliFullNameId", jt.SmaliFullNameId},
            };
            var resId = ExecScalarSelectQuery(sSql, parameters);
            if (resId.HasValue)
                return resId.Value;
            return -1;
        }

        public int TryUpdatingTypeIdForInApk(int typeId, JavaType jt)
        {
            /*
            The logic is as follows:
            - Check if we have refs to that type from types from other apks, if so, then fail to reassign it
            - If there are no such links, then update the JavaType
            */
            var sSql = @"
IF NOT EXISTS (
        SELECT jtypUId
        FROM JavaType
        WHERE 1=1
            AND (jtypJtypOuterClassId = @typeId OR jtypJtypSuperClassId = @typeId)
            AND jtypBioParentApkId <> @jtypBioParentApkId
)
BEGIN
    UPDATE JavaType
    SET jtypBioParentApkId = @jtypBioParentApkId, jtypBocParentContentId = @jtypBocParentContentId
    WHERE jtypUId = @typeId
    SELECT @typeId
END
ELSE
BEGIN
    SELECT 0
END
";

            var parameters = new Dictionary<string, object>()
            {
                {"@typeId", typeId},
                {"@jtypBioParentApkId", jt.ParentApkId},
                {"@jtypBocParentContentId", jt.ParentContentId}
            };
            var resId = ExecScalarSelectQuery(sSql, parameters);
            if (resId.HasValue)
                return resId.Value;
            return -1;
        }

        public void ReReferenceSuperClassInApkToNewType(int apkId, int declaredTypeId, int definedTypeId)
        {
            var sSql = @"
UPDATE JavaType
SET jtypJtypSuperClassId = @definedTypeId
WHERE jtypUId IN
    (
        SELECT jtypUId
        FROM JavaType
        WHERE 1=1
            AND jtypBioParentApkId = @apkId
            AND jtypJtypSuperClassId = @declaredTypeId
    )
";
            ExecNonQuery(sSql, new Dictionary<string, object>()
            {
                {"@apkId", apkId},
                {"@declaredTypeId", declaredTypeId},
                {"@definedTypeId", definedTypeId}
            });
        }

        public void ReReferenceOuterClassInApkToNewType(int apkId, int declaredTypeId, int definedTypeId)
        {
            var sSql = @"
UPDATE JavaType
SET jtypJtypOuterClassId = @definedTypeId
WHERE jtypUId IN
    (
        SELECT jtypUId
        FROM JavaType
        WHERE 1=1
            AND jtypBioParentApkId = @apkId
            AND jtypJtypOuterClassId = @declaredTypeId
    )";

            ExecNonQuery(sSql, new Dictionary<string, object>()
            {
                {"@apkId", apkId},
                {"@declaredTypeId", declaredTypeId},
                {"@definedTypeId", definedTypeId}
            });
        }

        public void ReReferenceImplementedInterfaceInApkToNewType(int apkId, int declaredTypeId, int definedTypeId)
        {
            var sSql = @"
UPDATE JavaTypeImplementedInterface
SET jtiiJtypInterfaceId = @definedTypeId
WHERE jtiiUId IN
    (
        SELECT DISTINCT jtiiUId
        FROM JavaTypeImplementedInterface
            INNER JOIN JavaType ON (jtiiJtypClassId = jtypUId)
        WHERE 1=1
            AND jtypBioParentApkId = @apkId
            AND jtiiJtypInterfaceId = @declaredTypeId
    )";

            ExecNonQuery(sSql, new Dictionary<string, object>()
            {
                {"@apkId", apkId},
                {"@declaredTypeId", declaredTypeId},
                {"@definedTypeId", definedTypeId}
            });
        }

        #region Index handling queries

        public void DropAllIndices()
        {
            var sql = @"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='UQ_jtypBioParentApkId_jtypStrSmaliFullNameId' AND object_id = OBJECT_ID('JavaType'))
BEGIN
DROP INDEX UQ_jtypBioParentApkId_jtypStrSmaliFullNameId ON JavaType
END
";
            ExecNonQuery(sql);

            sql = @"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_UQ_JavaTypeImplementedInterface' AND object_id = OBJECT_ID('JavaTypeImplementedInterface'))
BEGIN
DROP INDEX IDX_UQ_JavaTypeImplementedInterface ON JavaTypeImplementedInterface
END
";
            ExecNonQuery(sql);

            sql = @"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_JavaTypeField' AND object_id = OBJECT_ID('JavaTypeField'))
BEGIN
DROP INDEX IDX_JavaTypeField ON JavaTypeField
END
";
            ExecNonQuery(sql);

            sql = @"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_JavaTypeMethod' AND object_id = OBJECT_ID('JavaTypeMethod'))
BEGIN
DROP INDEX IDX_JavaTypeMethod ON JavaTypeMethod
END
";
            ExecNonQuery(sql);
            sql = @"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_JavaTypeUsedInType_Destination' AND object_id = OBJECT_ID('JavaTypeUsedInType'))
BEGIN
DROP INDEX IDX_JavaTypeUsedInType_Destination ON JavaTypeUsedInType
END
";
            ExecNonQuery(sql);
            sql = @"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_JavaTypeUsedInType_Source' AND object_id = OBJECT_ID('JavaTypeUsedInType'))
BEGIN
DROP INDEX IDX_JavaTypeUsedInType_Source ON JavaTypeUsedInType
END
";
            ExecNonQuery(sql);

        }

        public void CreateAllIndices()
        {
            var sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='UQ_jtypBioParentApkId_jtypStrSmaliFullNameId' AND object_id = OBJECT_ID('JavaType'))
BEGIN
    CREATE UNIQUE INDEX UQ_jtypBioParentApkId_jtypStrSmaliFullNameId ON JavaType (jtypBioParentApkId, jtypStrSmaliFullNameId)
END
";
            ExecNonQuery(sql);

            sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='UQ_JavaTypeImplementedInterface' AND object_id = OBJECT_ID('JavaTypeImplementedInterface'))
BEGIN
    CREATE UNIQUE INDEX UQ_JavaTypeImplementedInterface
    ON JavaTypeImplementedInterface (jtiiJtypClassId, jtiiJtypInterfaceId)
END
";
            ExecNonQuery(sql);

            sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_JavaTypeField' AND object_id = OBJECT_ID('JavaTypeField'))
BEGIN
    CREATE NONCLUSTERED INDEX IDX_JavaTypeField
    ON JavaTypeField (jtfJtypInTypeId, jtfStrSmaliNameId)
END
";
            ExecNonQuery(sql);

            sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_JavaTypeMethod' AND object_id = OBJECT_ID('JavaTypeMethod'))
BEGIN
    CREATE NONCLUSTERED INDEX IDX_JavaTypeMethod
    ON JavaTypeMethod (jtmJtypInTypeId, jtmStrSmaliNameId)
END
";
            ExecNonQuery(sql);
            sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_JavaTypeUsedInType_Destination' AND object_id = OBJECT_ID('JavaTypeUsedInType'))
BEGIN
    CREATE NONCLUSTERED INDEX IDX_JavaTypeUsedInType_Destination
    ON JavaTypeUsedInType (jtuJtmDestinationMethodId)
END
";
            ExecNonQuery(sql);
            sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_JavaTypeUsedInType_Source' AND object_id = OBJECT_ID('JavaTypeUsedInType'))
BEGIN
    CREATE NONCLUSTERED INDEX IDX_JavaTypeUsedInType_Source
    ON JavaTypeUsedInType (jtuJtmSourceMethodId)
END
";
            ExecNonQuery(sql);

        }

        public void DisableSelfKeys()
        {
            var sql = @"ALTER TABLE JavaType NOCHECK CONSTRAINT SK_jtypJtypOuterClassId";
            ExecNonQuery(sql);

            sql = @"ALTER TABLE JavaType NOCHECK CONSTRAINT SK_jtypJtypSuperClassId";
            ExecNonQuery(sql);

        }

        public void EnableSelfKeys()
        {
            var sql = @"ALTER TABLE JavaType WITH CHECK CHECK  CONSTRAINT SK_jtypJtypOuterClassId";
            ExecNonQuery(sql);

            sql = @"ALTER TABLE JavaType WITH CHECK CHECK  CONSTRAINT SK_jtypJtypSuperClassId";
            ExecNonQuery(sql);

        }

        public void AddCompressionColumn()
        {
            var sql = @"
IF NOT EXISTS(
 SELECT *
    FROM sys.columns 
    WHERE Name      = N'jtypCompressionId'
      AND Object_ID = Object_ID(N'JavaType'))
BEGIN
ALTER TABLE JavaType ADD jtypCompressionId INT NULL
END
";

            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeIdsToMinIds()
        {
            var sql = @"
UPDATE JavaType
SET JavaType.jtypCompressionId = TmpTable.minUId
FROM
(
  SELECT minUId = MIN(jtypUId), jtypBioParentApkId, jtypStrSmaliFullNameId, cnt = COUNT(*)
  FROM JavaType
  WHERE jtypUId > 0
  GROUP BY jtypBioParentApkId, jtypStrSmaliFullNameId
  HAVING COUNT(*) > 1
) AS TmpTable
WHERE JavaType.jtypBioParentApkId = TmpTable.jtypBioParentApkId AND JavaType.jtypStrSmaliFullNameId = TmpTable.jtypStrSmaliFullNameId
";
            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeToCompressIds()
        {
            var sql = @"
UPDATE JavaType
SET jtypJtypOuterClassId = shouldBeId
FROM 
(
  SELECT usedToBeId = jtypUId, shouldBeId =jtypCompressionId
  FROM JavaType
  WHERE jtypCompressionId IS NOT NULL AND jtypUId <> jtypCompressionId 
) As TmpTable
WHERE jtypJtypOuterClassId = usedToBeId

";
            ExecNonQuery(sql);

            sql = @"
UPDATE JavaType
SET jtypJtypSuperClassId = shouldBeId
FROM 
(
  SELECT usedToBeId = jtypUId, shouldBeId =jtypCompressionId
  FROM JavaType
  WHERE jtypCompressionId IS NOT NULL AND jtypUId <> jtypCompressionId 
) As TmpTable
WHERE jtypJtypSuperClassId = usedToBeId
";
            ExecNonQuery(sql);

            sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_TMP_JavaType' AND object_id = OBJECT_ID('JavaType'))
BEGIN
    CREATE NONCLUSTERED INDEX IDX_TMP_JavaType
    ON JavaType (jtypCompressionId, jtypUId)
END
";
            ExecNonQuery(sql);

        }

        public void CompressDuplicateFields()
        {
            var sql = @"
UPDATE JavaTypeField
SET jtfJtypInTypeId = JavaType.jtypCompressionId
FROM JavaType
WHERE jtfJtypInTypeId = JavaType.jtypUId AND jtypCompressionId IS NOT NULL AND jtypCompressionId < jtypUId

";
            ExecNonQuery(sql);

            sql = @"
UPDATE JavaTypeField
SET jtfJtypOfTypeId = JavaType.jtypCompressionId
FROM JavaType
WHERE jtfJtypOfTypeId = JavaType.jtypUId AND jtypCompressionId IS NOT NULL AND jtypCompressionId < jtypUId
";
            ExecNonQuery(sql);

            sql = @"
IF NOT EXISTS(
 SELECT *
    FROM sys.columns 
    WHERE Name      = N'jtfCompressionId'
      AND Object_ID = Object_ID(N'JavaTypeField'))
BEGIN
ALTER TABLE JavaTypeField ADD jtfCompressionId INT NULL
END
";
            ExecNonQuery(sql);

            sql = @"
UPDATE JavaTypeField
SET JavaTypeField.jtfCompressionId = TmpTable.minUId
FROM
(
  SELECT minUId = MIN(jtfUId), jtfJtypInTypeId, jtfStrSmaliNameId, cnt = COUNT(*)
  FROM JavaTypeField
  WHERE jtfUId > 0
  GROUP BY jtfJtypInTypeId, jtfStrSmaliNameId
  HAVING COUNT(*) > 1
) AS TmpTable
WHERE JavaTypeField.jtfJtypInTypeId = TmpTable.jtfJtypInTypeId AND JavaTypeField.jtfStrSmaliNameId = TmpTable.jtfStrSmaliNameId
";
            ExecNonQuery(sql);

        }

        public void RemoveDuplicateFields()
        {
            var sql = @"
DELETE
FROM JavaTypeField
WHERE jtfCompressionId IS NOT NULL AND jtfCompressionId <> jtfUId
";
            ExecNonQuery(sql);

            sql = @"
IF EXISTS(
 SELECT *
    FROM sys.columns 
    WHERE Name      = N'jtfCompressionId'
      AND Object_ID = Object_ID(N'JavaTypeField'))
BEGIN
ALTER TABLE JavaTypeField DROP COLUMN jtfCompressionId
END
";
            ExecNonQuery(sql);

        }

        public void CompressDuplicateMethods()
        {
            var sql = @"
UPDATE JavaTypeMethod
SET jtmJtypInTypeId = JavaType.jtypCompressionId
FROM JavaType
WHERE jtmJtypInTypeId = JavaType.jtypUId AND jtypCompressionId IS NOT NULL AND jtypCompressionId < jtypUId

";
            ExecNonQuery(sql);

            sql = @"
UPDATE JavaTypeMethod
SET jtmJtypReturnTypeId = JavaType.jtypCompressionId
FROM JavaType
WHERE jtmJtypReturnTypeId = JavaType.jtypUId AND jtypCompressionId IS NOT NULL AND jtypCompressionId < jtypUId
";
            ExecNonQuery(sql);

            sql = @"
IF NOT EXISTS(
 SELECT *
    FROM sys.columns 
    WHERE Name      = N'jtmCompressionId'
      AND Object_ID = Object_ID(N'JavaTypeMethod'))
BEGIN
ALTER TABLE JavaTypeMethod ADD jtmCompressionId INT NULL
END
";
            ExecNonQuery(sql);

            sql = @"
UPDATE JavaTypeMethod
SET JavaTypeMethod.jtmCompressionId = TmpTable.minUId
FROM
(
  SELECT minUId = MIN(jtmUId), jtmJtypInTypeId, jtmStrSmaliNameId, cnt = COUNT(*)
  FROM JavaTypeMethod
  WHERE jtmUId > 0
  GROUP BY jtmJtypInTypeId, jtmStrSmaliNameId
  HAVING COUNT(*) > 1
) AS TmpTable
WHERE JavaTypeMethod.jtmJtypInTypeId = TmpTable.jtmJtypInTypeId AND JavaTypeMethod.jtmStrSmaliNameId = TmpTable.jtmStrSmaliNameId
";
            ExecNonQuery(sql);

        }

        public void RemoveDuplicateMethods()
        {
            var sql = @"
DELETE
FROM JavaTypeMethod
WHERE jtmCompressionId IS NOT NULL AND jtmCompressionId <> jtmUId
";
            ExecNonQuery(sql);

            sql = @"
IF EXISTS(
 SELECT *
    FROM sys.columns 
    WHERE Name      = N'jtmCompressionId'
      AND Object_ID = Object_ID(N'JavaTypeMethod'))
BEGIN
ALTER TABLE JavaTypeMethod DROP COLUMN jtmCompressionId
END
";
            ExecNonQuery(sql);

        }

        public void RemoveDuplicatesFromJavaTypeImplementedInterface()
        {
            var sql = @"
DELETE
FROM JavaTypeImplementedInterface
WHERE jtiiJtypClassId IN
(
    SELECT jtypUId
    FROM JavaType
    WHERE jtypCompressionId IS NOT NULL AND jtypCompressionId < jtypUId
)
";
            ExecNonQuery(sql);

            sql = @"
DELETE
FROM JavaTypeImplementedInterface
WHERE jtiiJtypInterfaceId IN
(
    SELECT jtypUId
    FROM JavaType
    WHERE jtypCompressionId IS NOT NULL AND jtypCompressionId < jtypUId
)
";
            ExecNonQuery(sql);
        }

        public void RemoveDuplicatesFromJavaTypeMethod()
        {

            var sql = @"
DELETE
FROM JavaTypeMethod
WHERE jtmJtypInTypeId IN
(
    SELECT jtypUId
    FROM JavaType
    WHERE jtypCompressionId IS NOT NULL AND jtypCompressionId < jtypUId
)
";
            ExecNonQuery(sql);

            sql = @"
DELETE
FROM JavaTypeMethod
WHERE jtmJtypReturnTypeId IN
(
    SELECT jtypUId
    FROM JavaType
    WHERE jtypCompressionId IS NOT NULL AND jtypCompressionId < jtypUId
)
";
            ExecNonQuery(sql);
        }

        public void UpdateJavaTypeUsedInTypeWithCompressionIds()
        {
            var sql = @"
UPDATE JavaTypeUsedInType
SET jtuJtmDestinationMethodId = JavaTypeMethod.jtmCompressionId
FROM JavaTypeMethod
WHERE jtuJtmDestinationMethodId = JavaTypeMethod.jtmUId AND jtmCompressionId IS NOT NULL AND jtmCompressionId < jtmUId

";
            ExecNonQuery(sql);

            sql = @"
UPDATE JavaTypeUsedInType
SET jtuJtfDestinationFieldId = JavaTypeField.jtfCompressionId
FROM JavaTypeField
WHERE jtuJtfDestinationFieldId = JavaTypeField.jtfUId AND jtfCompressionId IS NOT NULL AND jtfCompressionId < jtfUId
";
            ExecNonQuery(sql);
        }

        public void RemoveDuplicates()
        {
            var sql = @"
DELETE 
FROM JavaType
WHERE jtypCompressionId IS NOT NULL AND jtypUId <> jtypCompressionId 
";
            ExecNonQuery(sql);
        }

        public void DropCompressionColumn()
        {

            var sql = @"
IF EXISTS(SELECT * FROM sys.indexes WHERE name='IDX_TMP_JavaType' AND object_id = OBJECT_ID('JavaType'))
BEGIN
    DROP INDEX IDX_TMP_JavaType ON JavaType
END
";
            ExecNonQuery(sql);

            sql = @"
IF EXISTS(
 SELECT *
    FROM sys.columns 
    WHERE Name      = N'jtypCompressionId'
      AND Object_ID = Object_ID(N'JavaType'))
BEGIN
ALTER TABLE JavaType DROP COLUMN jtypCompressionId
END
";

            ExecNonQuery(sql);
        }

        public void CreateUniqueIndexOnJavaType()
        {
            var sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='UQ_jtypBioParentApkId_jtypStrSmaliFullNameId' AND object_id = OBJECT_ID('JavaType'))
BEGIN
    CREATE UNIQUE INDEX UQ_jtypBioParentApkId_jtypStrSmaliFullNameId ON JavaType (jtypBioParentApkId, jtypStrSmaliFullNameId)
END
";
            ExecNonQuery(sql);
        }

        public void CreateUniqueIndexOnJavaTypeImplementedInterface()
        {
            var sql = @"
IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='UQ_JavaTypeImplementedInterface' AND object_id = OBJECT_ID('JavaTypeImplementedInterface'))
BEGIN
    CREATE UNIQUE INDEX UQ_JavaTypeImplementedInterface ON JavaTypeImplementedInterface (jtiiJtypClassId, jtiiJtypInterfaceId)
END
";
            ExecNonQuery(sql);
        }

        #endregion
    }

}