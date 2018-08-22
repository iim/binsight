using System.Linq;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Logic.ControlFlowGraph;
using NUnit.Framework;

namespace BinSightTest
{
    [TestFixture]
    public class Smali2ControlFlowGraphTest_vn_smali: SmaliParser
    {

        [Test]
        public void Test_j_Laeu_PackedSwitch()
        {
            string smaliContent = TestUtils.GetResource("BinSightTest.SmaliFiles.vn.smali");
            var parser = new SmaliParser();
            parser.ProcessSmaliFile(smaliContent);
            var testMethod =
                parser.JavaType.Methods.FirstOrDefault(
                    m => m.SmaliName.EndsWith("j(Laeu;)Lcom/snapchat/android/model/MediaMailingMetadata$SendStatus;"));

            var cfg = new Cfg(new SmaliParser());
            ProcessMethod(ref cfg, testMethod);

            var v = cfg.GetVertexByName(
                "Lvn;->j(Laeu;)Lcom/snapchat/android/model/MediaMailingMetadata$SendStatus;",
                ":pswitch_data_0",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 7);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
        }
    }
}
