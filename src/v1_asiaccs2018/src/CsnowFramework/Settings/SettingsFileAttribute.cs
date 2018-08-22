using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsnowFramework.Settings
{
    /// <summary>
    /// Defines key parameters for a settings file.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    class SettingsFileAttribute: System.Attribute
    {
        private string _settingsFilename;

        public SettingsFileAttribute(string settingsFilename)
        {
            _settingsFilename = settingsFilename;
        }

        public string SettingFilename
        {
            get
            {
                return _settingsFilename;
            }
        }
    }
}
