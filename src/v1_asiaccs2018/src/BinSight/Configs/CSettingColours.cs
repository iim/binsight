using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace APKInsight.Configs
{
    internal static class CSettingColours
    {
        public static Color PackagePathLibraryOnly { get; private set; } = Color.ForestGreen;
        public static Color PackagePathOwnCodeOnly { get; private set; } = Color.Black;
        public static Color PackagePathLibraryAndOwnCode { get; private set; } = Color.RoyalBlue;
    }
}
