using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsnowFramework.Enum
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValueAttribute: System.Attribute
    {
        public EnumValueAttribute(string strValue)
        {
            StringValue = strValue;
        }

        public string StringValue { get; set; }
    }
}
