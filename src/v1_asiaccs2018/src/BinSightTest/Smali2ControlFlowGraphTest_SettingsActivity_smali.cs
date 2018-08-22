using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BinSightTest
{
    [TestFixture]
    class Smali2ControlFlowGraphTest_SettingsActivity_smali : SmaliParser
    {

        #region Testing BinSightTest.SmaliFiles.SettingsActivity.smali file

        [Test]
        public void ProcessMethod_BinSightTest_SmaliFiles_SettingsActivity_smali_getResIdFromAttribute()
        {
            string smaliContent = TestUtils.GetResource("BinSightTest.SmaliFiles.SettingsActivity.smali");
            var parser = new SmaliParser();
            parser.ProcessSmaliFile(smaliContent);
            var testMethod =
                parser.JavaType.Methods.FirstOrDefault(
                    m => m.SmaliName.EndsWith("getResIdFromAttribute(Landroid/app/Activity;I)I"));
            var cfg = new Cfg(new SmaliParser());
            ProcessMethod(ref cfg, testMethod);
            var v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getResIdFromAttribute(Landroid/app/Activity;I)I", "", false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 2);
            Assert.IsNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getResIdFromAttribute(Landroid/app/Activity;I)I", ":goto_0",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 2);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getResIdFromAttribute(Landroid/app/Activity;I)I", ":cond_0",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 3);
            Assert.IsNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);

            v = cfg.GetVertexByName(
                "Landroid/util/TypedValue;-><init>()V", "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getResIdFromAttribute(Landroid/app/Activity;I)I", "24",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 1);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);

            v = cfg.GetVertexByName(
                "Landroid/app/Activity;->getTheme()Landroid/content/res/Resources$Theme;", "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getResIdFromAttribute(Landroid/app/Activity;I)I", "26",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 3);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);

            v = cfg.GetVertexByName(
                "Landroid/content/res/Resources$Theme;->resolveAttribute(ILandroid/util/TypedValue;Z)Z", "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getResIdFromAttribute(Landroid/app/Activity;I)I", "33",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 2);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNull(v.Successor);
        }

        [Test]
        public void ProcessMethod_BinSightTest_SmaliFiles_SettingsActivity_smali_removeInternalSection()
        {
            string smaliContent = TestUtils.GetResource("BinSightTest.SmaliFiles.SettingsActivity.smali");
            var parser = new SmaliParser();
            parser.ProcessSmaliFile(smaliContent);
            var testMethod =
                parser.JavaType.Methods.FirstOrDefault(
                    m => m.SmaliName.EndsWith("removeInternalSection()V"));

            var cfg = new Cfg(new SmaliParser());
            ProcessMethod(ref cfg, testMethod);
            var v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V", ":try_start_0",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 2);
            Assert.IsNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            Assert.IsTrue(v.IsEntryPoint);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                 "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceManager()Landroid/preference/PreferenceManager;", "",
                 false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 1);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V", "8",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 3);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                 "Landroid/preference/PreferenceManager;->findPreference(Ljava/lang/CharSequence;)Landroid/preference/Preference;",
                 "",
                 false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 1);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V",
                "14",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 3);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getPreferenceScreen()Landroid/preference/PreferenceScreen;",
                "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 1);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V",
                "22",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 2);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                "Landroid/preference/PreferenceScreen;->removePreference(Landroid/preference/Preference;)Z",
                "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 1);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V",
                ":try_end_0",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 2);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V",
                ":goto_0",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 2);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V",
                ":catch_0",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 4);
            Assert.IsNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/util/Utils;->safeToast(Ljava/lang/String;Ljava/lang/Object;)V",
                "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->removeInternalSection()V",
                "43",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 1);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNull(v.Successor);

        }

        [Test]
        public void ProcessMethod_BinSightTest_SmaliFiles_SettingsActivity_smali_onCreate()
        {
            string smaliContent = TestUtils.GetResource("BinSightTest.SmaliFiles.SettingsActivity.smali");
            var parser = new SmaliParser();
            parser.ProcessSmaliFile(smaliContent);
            var testMethod =
                parser.JavaType.Methods.FirstOrDefault(
                    m => m.SmaliName.EndsWith("onCreate(Landroid/os/Bundle;)V"));

            var cfg = new Cfg(new SmaliParser());
            ProcessMethod(ref cfg, testMethod);
            var v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V", "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 1);
            Assert.IsNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            Assert.IsTrue(v.IsEntryPoint);

            v = cfg.GetVertexByName(
                 "Landroid/preference/PreferenceActivity;->onCreate(Landroid/os/Bundle;)V",
                 "",
                 false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V",
                "9",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 4);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V",
                "18",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 4);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);

            v = cfg.GetVertexByName(
                 "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->findViewById(I)Landroid/view/View;",
                 "",
                 false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 2);
            Assert.AreEqual(v.EdgeIncomingVertex.Count, 2);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V",
                "26",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 3);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            Assert.AreEqual(v.CatchVertices.Count, 0);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V",
                ":try_start_0",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 4);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            SmaliCfgInstruction e = (SmaliCfgInstruction)v.Instructions[0];
            Assert.AreEqual(e.InstructionType, ESmaliInstruction.LabelTryStart);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 0);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V",
                "205",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 3);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            Assert.AreEqual(v.CatchVertices.Count, 0);

            v = cfg.GetVertexByName(
                "Landroid/preference/Preference;->setOnPreferenceClickListener(Landroid/preference/Preference$OnPreferenceClickListener;)V",
                "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 5);
            Assert.AreEqual(v.CatchVertices.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V",
                ":try_end_1",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 2);
            Assert.AreEqual(v.EdgeIncomingVertex.Count, 40);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V",
                ":goto_1",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 2);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            Assert.AreEqual(v.EdgeIncomingVertex.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V",
                ":catch_1",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 6);
            Assert.IsNull(v.Predecessor);
            Assert.IsNotNull(v.Successor);
            Assert.AreEqual(v.EdgeIncomingVertex.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/util/Utils;->showOrGoneView(Landroid/view/View;Z)V",
                "",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 0);
            Assert.IsNull(v.Predecessor);
            Assert.IsNull(v.Successor);
            Assert.AreEqual(v.EdgeReturnVertex.Count, 1);

            v = cfg.GetVertexByName(
                "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->onCreate(Landroid/os/Bundle;)V",
                "655",
                false);
            Assert.NotNull(v);
            Assert.AreEqual(v.Instructions.Count, 1);
            Assert.IsNotNull(v.Predecessor);
            Assert.IsNull(v.Successor);

        }

        #endregion

        [Ignore("For dev only")]
        [Test]
        public void Test1()
        {
            var hash_sophos = new HashSet<string>(File.ReadAllLines(@"S:\binsight\dev\hashes\sophos_150K.txt"));
            string[] hash_ucsb = File.ReadAllLines(@"S:\binsight\dev\hashes\ucsb_hashes.txt");
            var match = hash_ucsb.Where(h => hash_sophos.Contains(h)).ToList();
            var notmatch = hash_ucsb.Where(h => !hash_sophos.Contains(h)).ToList();

            File.WriteAllLines(@"S:\binsight\dev\hashes\matched.txt", match);
            File.WriteAllLines(@"S:\binsight\dev\hashes\notmatched.txt", notmatch);

            string smaliContent = TestUtils.GetResource("BinSightTest.SmaliFiles.SettingsActivity.smali");
            var cfg = new Cfg(new SmaliParser());
            cfg.ProcessSourceFileContent(smaliContent);
            //var cfgManual = new Cfg(new SmaliParser());
            //var v =
            //    cfgManual.GetVertexByName(
            //        "Lcom/linkedin/android/jobs/jobseeker/activity/SettingsActivity;->getResIdFromAttribute(Landroid/app/Activity;I)I");
            //v.IsEntryPoint = true;
            //v.Instructions
        }

    }


}
