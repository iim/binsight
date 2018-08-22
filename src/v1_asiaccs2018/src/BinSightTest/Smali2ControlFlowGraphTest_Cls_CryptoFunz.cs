using System.Linq;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Logic.ControlFlowGraph;
using NUnit.Framework;

namespace BinSightTest
{
    [TestFixture]
    public class Smali2ControlFlowGraphTest_Cls_CryptoFunz : SmaliParser
    {

        [Test]
        public void Test_setMasterKey()
        {
            string smaliContent = TestUtils.GetResource("BinSightTest.SmaliFiles.Cls_CryptoFunz.smali");
            var parser = new SmaliParser();
            parser.ProcessSmaliFile(smaliContent);
            var testMethod =
                parser.JavaType.Methods.FirstOrDefault(
                    m => m.SmaliName.EndsWith("setMasterKey([B)V"));

            var cfg = new Cfg(new SmaliParser());
            ProcessMethod(ref cfg, testMethod);

            var v = cfg.GetVertexByName(
                "Lcom/lindberghapps/kidsboard/Cls_CryptoFunz;->setMasterKey([B)V",
                "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 1);
            Assert.AreEqual(v.AllInstructions.Count, 4);
            Assert.IsNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
        }
    }
}
