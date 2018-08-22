using System.Data;
using APKInsight.Enums;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("JavaType")]
    public class JavaType
    {
        [QueryColumn("jtypUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("jtypBioParentApkId", SqlDbType.Int)]
        public int? ParentApkId { get; set; }

        [QueryColumn("jtypBocParentContentId", SqlDbType.Int)]
        public int? ParentContentId { get; set; }

        [QueryColumn("jtypAccessControl", SqlDbType.Int)]
        public int? AccessControl { get; set; }

        public JavaAccessControl JavaAccessControl
        {
            get
            {
                if (AccessControl != null)
                    return (JavaAccessControl)AccessControl.Value;
                else
                    return JavaAccessControl.Undefined;
            }
            set { AccessControl = (int)value; }
        }

        [QueryColumn("jtypStrPackageNameId", SqlDbType.Int)]
        public int? PackageNameId { get; set; }

        [QueryColumn("jtypStrSmaliFullNameId", SqlDbType.Int)]
        public int? SmaliFullNameId { get; set; }

        [QueryColumn("jtypStrFileNameId", SqlDbType.Int)]
        public int? FileNameId { get; set; }

        [QueryColumn("jtypStrPathId", SqlDbType.Int)]
        public int? PathId { get; set; }

        [QueryColumn("jtypIsClass", SqlDbType.Bit)]
        public bool? IsClass { get; set; }

        [QueryColumn("jtypIsInterface", SqlDbType.Bit)]
        public bool? IsInterface { get; set; }

        [QueryColumn("jtypIsFinal", SqlDbType.Bit)]
        public bool? IsFinal { get; set; }

        [QueryColumn("jtypIsEnum", SqlDbType.Bit)]
        public bool? IsEnum { get; set; }

        [QueryColumn("jtypIsAbstract", SqlDbType.Bit)]
        public bool? IsAbstract { get; set; }

        [QueryColumn("jtypIsAnnotation", SqlDbType.Bit)]
        public bool? IsAnnotation { get; set; }

        [QueryColumn("jtypIsStatic", SqlDbType.Bit)]
        public bool? IsStatic { get; set; }

        [QueryColumn("jtypIsReferenceOnly", SqlDbType.Bit)]
        public bool? IsReferenceOnly { get; set; }

        [QueryColumn("jtypJtypOuterClassId", SqlDbType.Int)]
        public int? OuterClassId { get; set; }

        [QueryColumn("jtypJtypSuperClassId", SqlDbType.Int)]
        public int? SuperClassId { get; set; }

        [QueryColumn("jtypDbgSourceNotFound", SqlDbType.Bit)]
        public bool? DbgSourceNotFound { get; set; }

    }
}
