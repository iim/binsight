using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APKInsight.Enums;
using APKInsight.Models.DataBase;
using APKInsight.Queries;

namespace APKInsight.Logic
{
    static class StringValueUtils
    {
        public static int SaveUniqueStringValue(string value, StringValueType type)
        {
            var query = new QueryStringValue();
            var stringValue = new StringValue
            {
                Type = type,
                Value = value,
            };
            stringValue.HashValue();
            if (query.AddUniqueStringValue(ref stringValue))
            {
                return stringValue.UId.Value;
            }
            return -1;
        }

        // Simply insert values without checking if such record already exists.
        public static int SaveStringValueWithNoSearch(QueryStringValue query, string value, StringValueType type)
        {
            int? uid = null;
            if (query.AddStringValueNoSearch(value, type, ref uid))
            {
                return uid.Value;
            }
            return -1;
        }


    }
}
