using System.Collections.Generic;
using System.Linq;
using APKInsight.Logic;
using APKInsight.Models;
using APKInsight.Models.DataBase;
using CsnowFramework.Database;
using JavaType = APKInsight.Models.DataBase.JavaType;

namespace APKInsight.Queries
{
    class QueryJavaTypeMethod : QueryBase
    {

        public QueryJavaTypeMethod(string connectionString = null) :
            base(connectionString)
        {
            // Do nothing
        }

        public List<JavaTypeInternals> SelectMethodsInType(int typeId)
        {
            var sSql = @"
SELECT *
FROM JavaTypeMethod
    INNER JOIN StringValueMTHD ON (strUId = jtmStrSmaliNameId)
WHERE jtmJtypInTypeId = @jtmJtypInTypeId
";
            return ExecSelectQuery<JavaTypeInternals>(sSql, new Dictionary<string, object>()
            {
                {"@jtmJtypInTypeId", typeId}
            });
        }

        public List<JavaTypeUsedInTypeExtended> SelectUseCasesForMethodsInType(int typeId)
        {
            var sSql = @"
SELECT 
JavaTypeUsedInType.*,
StrMethodSmaliName = StringValueMTHD.strValue,
SourcejtmJtypInTypeId = SourceJavaTypeMethod.jtmJtypInTypeId
FROM JavaTypeUsedInType
  INNER JOIN JavaTypeMethod AS DestinationJavaTypeMethod ON (DestinationJavaTypeMethod.jtmUId = jtuJtmDestinationMethodId)
  INNER JOIN JavaTypeMethod AS SourceJavaTypeMethod ON (SourceJavaTypeMethod.jtmUId = jtuJtmSourceMethodId)
  INNER JOIN StringValueMTHD ON (StringValueMTHD.strUId = SourceJavaTypeMethod.jtmStrSmaliNameId)
WHERE DestinationJavaTypeMethod.jtmJtypInTypeId = @jtmJtypInTypeId
";
            return ExecSelectQuery<JavaTypeUsedInTypeExtended>(sSql, new Dictionary<string, object>()
            {
                {"@jtmJtypInTypeId", typeId}
            });
        }

        public Models.DataBase.JavaTypeMethod SelectMethod(int nameId, int typeId)
        {
            var sSql = @"
SELECT *
FROM JavaTypeMethod
WHERE jtmJtypInTypeId = @jtmJtypInTypeId AND jtmStrSmaliNameId = @jtmStrSmaliNameId
";
            var result = ExecSelectQuery<Models.DataBase.JavaTypeMethod>(sSql, new Dictionary<string, object>()
            {
                {"@jtmJtypInTypeId", typeId},
                {"@jtmStrSmaliNameId", nameId}
            });

            return result.Any() ? result[0] : null;
        }

        public Models.DataBase.JavaTypeField SelectField(int nameId, int typeId)
        {
            var sSql = @"
SELECT *
FROM JavaTypeField
WHERE jtfJtypInTypeId = @jtfJtypInTypeId AND jtfStrSmaliNameId = @jtfStrSmaliNameId
";
            var result = ExecSelectQuery<Models.DataBase.JavaTypeField>(sSql, new Dictionary<string, object>()
            {
                {"@jtfJtypInTypeId", typeId},
                {"@jtfStrSmaliNameId", nameId}
            });

            return result.Any() ? result[0] : null;
        }

    }
}