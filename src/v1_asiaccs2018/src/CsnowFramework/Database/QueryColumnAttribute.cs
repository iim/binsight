using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CsnowFramework.Database
{
    /// <summary>
    /// The key attribute that maps a property in a model to a column in SQL DB.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryColumnAttribute: System.Attribute
    {
        public QueryColumnAttribute(string columnName, SqlDbType sqlType, int maxLen = -1, bool isNullable = false, bool isPrimaryKey = false)
        {
            ColumnName = columnName;
            SqlType = sqlType;
            MaxLen = maxLen;
            IsPrimaryKey = isPrimaryKey;
            IsNullable = isNullable;
        }

        public string ColumnName { get; }

        public bool IsPrimaryKey { get; }

        public bool IsNullable { get; }

        public SqlDbType SqlType { get; }

        public int MaxLen { get; }

    }
}
