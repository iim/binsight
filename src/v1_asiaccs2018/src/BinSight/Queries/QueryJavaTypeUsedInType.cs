using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APKInsight.Logic;
using APKInsight.Models;
using APKInsight.Models.DataBase;
using CsnowFramework.Database;
using JavaType = APKInsight.Models.DataBase.JavaType;

namespace APKInsight.Queries
{
    class QueryJavaTypeUsedInType : QueryBase
    {

        private List<JavaTypeUsedInType> _addedCacheUsed = new List<JavaTypeUsedInType>();

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

        public QueryJavaTypeUsedInType(string connectionString = null) :
            base(connectionString)
        {
            InitCurrentMaxUId("JavaTypeUsedInType", "jtuUId", ref _currentMaxUId, ref _currentMaxUIdInitialized, ref _currentMaxUIdLock);
        }

        public int InsertObjectIntoCache(JavaTypeUsedInType jt)
        {
            jt.UId = GetNextUId();
            _addedCacheUsed.Add(jt);
            return jt.UId.Value;
        }

        public void SaveCache()
        {
            SaveJavaTypesUsedIn();
        }

        private void SaveJavaTypesUsedIn()
        {
            int indexBeg = 0;
            int count = 100;
            int remainingCount = _addedCacheUsed.Count;
            var toSave = _addedCacheUsed.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
            indexBeg += toSave.Count;
            remainingCount -= toSave.Count;
            while (toSave.Any())
            {
                SaveJavaTypesUsedInRange(toSave);
                toSave = _addedCacheUsed.GetRange(indexBeg, count > remainingCount ? remainingCount : count);
                indexBeg += toSave.Count;
                remainingCount -= toSave.Count;
            }
        }

        private void SaveJavaTypesUsedInRange(List<JavaTypeUsedInType> values)
        {
            var sSql = new StringBuilder(8000);
            sSql.Append(@"
SET IDENTITY_INSERT JavaTypeUsedInType ON

INSERT INTO JavaTypeUsedInType 
(
    jtuUId,

    jtuJtmDestinationMethodId, 
    jtuStrDestinationMethodSmaliNameId,
    jtuJtfDestinationFieldId, 
    jtuStrDestinationFieldSmaliNameId,

    jtuJtmSourceMethodId,
    jtuSourceLineIndex,
    jtuSourceWithinLineIndex,

    jtuIsReturnType,
    jtuIsParameter,
    jtuIsGetFieldAccessor,
    jtuIsPutFieldAccessor
)
VALUES
");
            var parameters = new Dictionary<string, object>();
            foreach (var type in values)
            {
                sSql.Append(
$@" ({type.UId},
{type.DestinationMethodId}, {type.DestinationMethodSmaliNameId}, {type.DestinationFieldId}, {type.DestinationFieldSmaliNameId},
{type.SourceMethodId},{type.SourceLineIndex}, {type.SourceWithinLineIndex},
{Convert.ToInt32(type.IsReturnType)}, {Convert.ToInt32(type.IsParameter)}, {Convert.ToInt32(type.IsGetFieldAccessor)},{Convert.ToInt32(type.IsPutFieldAccessor)}),");
            }

            sSql = sSql.Remove(sSql.Length - 1, 1);
            sSql.Append($" SET IDENTITY_INSERT JavaTypeUsedInType OFF");

            var inserted = ExecNonQuery(sSql.ToString(), parameters);
            if (inserted != values.Count)
            {
            }

        }


    }
}