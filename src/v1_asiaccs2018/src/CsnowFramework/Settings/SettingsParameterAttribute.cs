using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsnowFramework.Settings
{
    /// <summary>
    /// An attribute that defines settings for an attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class SettingsParameterAttribute: System.Attribute
    {
        private string _xmlPath;
        private object _defaultValue;

        public SettingsParameterAttribute(string xmlPath, object defaultValue)
        {
            _xmlPath = xmlPath;
            _defaultValue = defaultValue;
        }
    }
}
