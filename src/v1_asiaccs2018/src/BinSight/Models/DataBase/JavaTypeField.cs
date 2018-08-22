using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("JavaTypeField")]
    class JavaTypeField
    {

        [QueryColumn("jtfUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("jtfJtypInTypeId", SqlDbType.Int)]
        public int? InTypeId { get; set; }

        [QueryColumn("jtfStrSmaliNameId", SqlDbType.Int)]
        public int? SmaliNameId { get; set; }

        [QueryColumn("jtfJtypOfTypeId", SqlDbType.Int)]
        public int? OfTypeId { get; set; }

        [QueryColumn("jtfAccessControl", SqlDbType.Int)]
        public int? AccessControl { get; set; }

        [QueryColumn("jtfIsArray", SqlDbType.Bit)]
        public bool? IsArray { get; set; }

        [QueryColumn("jtfIsStatic", SqlDbType.Bit)]
        public bool? IsStatic { get; set; }

        [QueryColumn("jtfIsFinal", SqlDbType.Bit)]
        public bool? IsFinal { get; set; }

        [QueryColumn("jtfIsSynthetic", SqlDbType.Bit)]
        public bool? IsSynthetic { get; set; }

        [QueryColumn("jtfIsEnum", SqlDbType.Bit)]
        public bool? IsIsEnum { get; set; }

        [QueryColumn("jtfSourceCodeIndex", SqlDbType.Int)]
        public int? SourceCodeIndex { get; set; }
    }
}
