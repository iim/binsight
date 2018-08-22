using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsnowFramework.Database
{

    /// <summary>
    /// Returns the name of the table the query class suppose to insert into.
    /// </summary>
    public class QueryTableAttribute: Attribute
    {
        private string _tableName;
        public QueryTableAttribute(string tableName)
        {
            _tableName = tableName;
        }

        public string TableName
        {
            get
            {
                return _tableName;
            }
        }

    }
}
