using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APKInsight.Configs
{
    internal static class ApplicationConfiguration
    {
        private const string _kTempDriveLocationKeyName = "TempDriveLocation";
        private const string _kApkToolLocationKeyName = "ApkToolLocation";

        /// <summary>
        /// The location of the apktool.bat file (including the name of the file)
        /// </summary>
        public static string TempDriveLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(k => k == _kTempDriveLocationKeyName))
                {
                    return ConfigurationManager.AppSettings[_kTempDriveLocationKeyName];
                }
                else
                {
                    return @"T:\";
                }
            }
        }

        /// <summary>
        /// The location of the apktool.bat file (including the name of the file)
        /// </summary>
        public static string ApkToolLocation
        {
            get
            {
                if (ConfigurationManager.AppSettings.AllKeys.Any(k => k == _kApkToolLocationKeyName))
                {
                    return ConfigurationManager.AppSettings[_kApkToolLocationKeyName];
                }
                else
                {
                    return @"S:\csnowcode\apkinsight\scripts\apktool.bat";
                }
            }
        }
    }
}
