using System.Data;
using CsnowFramework.Database;

namespace APKInsight.Models.DataBase
{
    [QueryTable("JavaTypeMethod")]
    class JavaTypeMethod
    {

        [QueryColumn("jtmUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("jtmJtypInTypeId", SqlDbType.Int)]
        public int? InTypeId { get; set; }

        [QueryColumn("jtmStrSmaliNameId", SqlDbType.Int)]
        public int? SmaliNameId { get; set; }

        [QueryColumn("jtmJtypReturnTypeId", SqlDbType.Int)]
        public int? ReturnTypeId { get; set; }

        [QueryColumn("jtmAccessControl", SqlDbType.Int)]
        public int? AccessControl { get; set; }

        [QueryColumn("jtmIsAbstract", SqlDbType.Bit)]
        public bool? IsAbstract { get; set; }

        [QueryColumn("jtmIsConstructor", SqlDbType.Bit)]
        public bool? IsConstructor { get; set; }

        [QueryColumn("jtmIsStatic", SqlDbType.Bit)]
        public bool? IsStatic { get; set; }

        [QueryColumn("jtmSourceCodeIndexBeg", SqlDbType.Int)]
        public int? SourceCodeIndexBeg { get; set; }
        [QueryColumn("jtmSourceCodeIndexEnd", SqlDbType.Int)]
        public int? SourceCodeIndexEnd { get; set; }
    }
}
