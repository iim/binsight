using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsnowFramework.InputOutput;

namespace APKInsight.Logic.Analysis
{

    /// <summary>
    /// Logic to find all "Use-Cases"
    /// A use case is a call to a Crypt API under investigation.
    /// </summary>
    public class AnalysisLogicFindAllUseCases : AnalysisLogic
    {
        private static AnalysisReport _report;
        private static AnalysisReport _reportNotFound;
        private static readonly object _reportLock = new object();

        public static void InitReport()
        {
            _report = new AnalysisReport();
            _report.AddLineWithoutCounter("N,apkId,sha1,apkFilename,smaliN");
            _report.AddLineWithoutCounter(",,,classname,functionName,inFileLoc,inFuncLoc,sig,fileName");
            _reportNotFound = new AnalysisReport();
            _reportNotFound.AddLineWithoutCounter("N,apkId,sha1,apkFilename,smaliN");
        }


        public static void SaveReport(string filename, string dupsFilename)
        {
            _report.SaveReport(filename);
            _reportNotFound.SaveReport(dupsFilename);
        }

        private readonly string[] _apkIsInterestingSigs =
        {
            "Ljavax/crypto/Cipher;->getInstance(Ljava/lang/String;)Ljavax/crypto/Cipher;", // Rule #1

            "Ljavax/crypto/Cipher;->init(I", // Rule #2

            "Ljavax/crypto/spec/SecretKeySpec;-><init>([BLjava/lang/String;)V", // Rule #3

            "Ljavax/crypto/spec/PBEParameterSpec;-><init>", // Rule #4&5
            "Ljavax/crypto/spec/PBEKeySpec;-><init>", // Rule #4&5

            "Ljava/security/SecureRandom;-><init>([B)", //Rule #6
            "Ljava/security/SecureRandom;->setSeed(", //Rule #6
            "Ljava/security/Signature;->getInstance", //Rule #6
            "Ljava/security/Signature;->getInstanceStrong", //Rule #6

            "Ljavax/crypto/Cipher;->update(", // Our eval
            "Ljavax/crypto/Cipher;->updateAAD(", // Our eval
            "Ljavax/crypto/Cipher;->doFinal(", // Our eval
        };

        public override bool Process()
        {
            string applicationHeader = "";
            string report = "";
            ApplicationId = null;
            foreach (var fileName in Files)
            {
                if (
                    string.Equals(Path.GetFileName(fileName), "AndroidManifest.xml",
                        StringComparison.CurrentCultureIgnoreCase) &&
                    !fileName.Replace(ApkOutDirectory, "").Contains("original"))
                {
                    var content = File.ReadAllText(fileName);
                    var match = Regex.Match(content, "<manifest[^>]+>", RegexOptions.Multiline);
                    if (!match.Success)
                    {
                    }
                    var match2 = Regex.Match(match.Value, @"(?<=package="")[a-zA-Z0-9-_.]+(?="")");
                    if (match2.Success)
                    {
                        ApplicationId = match2.Value;
                        applicationHeader = "{0}," +
                                            $"{ApplicationId},{Sha1},{ApkFilename},{Files.Count(fn => Path.GetExtension(fn) == ".smali")}";
                    }
                }
                else if (Path.GetExtension(fileName) == ".smali")
                {
                    var content = Encoding.UTF8.GetString(Utilities.ReadAllBytes(fileName)).Split('\n');
                    string className = null;
                    string methodName = null;
                    int methodIndex = 0;
                    foreach (var sig in _apkIsInterestingSigs)
                    {
                        for (int codeLineIndex = 0; codeLineIndex < content.Length; codeLineIndex++)
                        {
                            methodIndex++;
                            var line = content[codeLineIndex];
                            if (line.StartsWith(".class"))
                            {
                                // Class name line
                                var parts = line.Split(' ');
                                className = parts[parts.Length - 1].Replace("\r", "").Replace("\n", "");
                            }
                            if (line.StartsWith(".method"))
                            {
                                // Class name line
                                var parts = line.Split(' ');
                                methodName = parts[parts.Length - 1].Replace("\r", "").Replace("\n", "");
                                methodIndex = 0;
                            }
                            if (line.Contains(sig))
                            {
                                report +=
                                    $",,,{className},{methodName},{codeLineIndex},{methodIndex},{sig},{fileName.Replace(ApkOutDirectory, "")}" +
                                    Environment.NewLine;
                            }
                        }
                    }
                }

            }
            // Locking just to make sure noone interjects between two additions
            lock (_reportLock)
            {
                if (report.Length > 0)
                {
                    _report.AddLineWithCounter(applicationHeader);
                    _report.AddLineWithoutCounter(report);
                }
                else
                {
                    _reportNotFound.AddLineWithCounter(applicationHeader);
                }
            }
            return true;
        }

    }
}
