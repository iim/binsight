using CsnowFramework.Enum;

namespace APKInsight.Enums
{
    internal enum StringValueType
    {
        Unknown = 0,
        [EnumValue("SMLN")]
        JavaTypeSmaliFullName,
        [EnumValue("SRCN")]
        JavaTypeSourceFileName,
        [EnumValue("PKGN")]
        JavaPackageName,
        [EnumValue("PATH")]
        JavaPath,
        [EnumValue("MTHD")]
        JavaTypeMethodSmaliFullName,
        [EnumValue("FILD")]
        JavaTypeFieldSmaliFullName,
        [EnumValue("LBPK")]
        LibraryBasePackageName,
    }
}
