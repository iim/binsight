using System.Data;
using System.Text;
using APKInsight.Enums;
using CsnowFramework.Crypto;
using CsnowFramework.Database;
using CsnowFramework.Enum;

namespace APKInsight.Models.DataBase
{

    [QueryTable("StringValue")]
    class StringValue
    {
        [QueryColumn("strUId", SqlDbType.Int, isPrimaryKey: true)]
        public int? UId { get; set; }

        [QueryColumn("strValue", SqlDbType.NVarChar)]
        public string Value { get; set; }

        public void HashValue(Hash _hash = null)
        {
            if (_hash == null)
                Hash = CsnowFramework.Crypto.Hash.GetHashMd5Bytes(Encoding.UTF8.GetBytes(Value));
            else
            {
                Hash = _hash.ComputeHash(Value);
            }
        }

        [QueryColumn("strHash", SqlDbType.Binary, maxLen: 20)]
        public byte[] Hash { get; set; }

        [QueryColumn("strType", SqlDbType.VarChar, maxLen: 4)]
        public string StrType { get; set; }

        public StringValueType Type
        {
            get
            {
                return EnumExtension.GetEnumValue<StringValueType>(StrType);
            }
            set {
                StrType = value.GetStringValue();
            }
        }

    }

}
