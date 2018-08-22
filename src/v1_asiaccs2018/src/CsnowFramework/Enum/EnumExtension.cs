using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsnowFramework.Enum
{
    public static class EnumExtension
    {
        public static string GetStringValue(this System.Enum enumValue)
        {
            var typ = enumValue.GetType();
            var member = typ.GetMember(enumValue.ToString())[0];
            // Check if we have the proper attribute set
            if (member.CustomAttributes.Any())
            {
                var valueAttr = member.GetCustomAttributes(typeof(EnumValueAttribute), false);
                if (valueAttr.Any())
                {
                    return (valueAttr[0] as EnumValueAttribute).StringValue;
                }
            }
            return "";
        }

        public static T GetEnumValue<T>(string value)
        {
            var type = typeof(T);
            foreach (var memberInfo in type.GetMembers())
            {
                var valueAttr = memberInfo.GetCustomAttributes(typeof(EnumValueAttribute), false);
                if (valueAttr.Any() && (valueAttr[0] as EnumValueAttribute).StringValue == value)
                {
                    return (T)System.Enum.Parse(type, memberInfo.Name);
                }
            }

            return default(T);
        }
    }
}
