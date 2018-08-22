using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using APKInsight.Logic.Analysis.Data;
using NUnit.Framework;

namespace BinSightTest
{
    //[TestFixture]
    public class DevScriptsTests
    {
        /// <summary>
        /// The white listed libraries from CCS'13 paper
        /// </summary>
        private static string[] _ccs13excludedLibsRegEx =
        {
            "com.scoreloop",
            "com.google.android.vending",
            "com.android.vending",
            "com.urbanairship",
            "com.openfeint",
            "com.google.ads",
            "com.phonegap",
            "vpadn",
            "com.unity3d",
            "co.microad",
            "com.amazonaws",
            "org.apache.james"

        };

        // Gives a generic report on three datasets
        [TestCase(@"D:\data\ccs13-11k.report.csv")]
        [TestCase(@"D:\data\top100.report.csv")]
        [TestCase(@"D:\data\sophos150K.report.csv")]
        public void ComputeStats(string filename)
        {
            string report = $"Report for data in {filename}\n";
            var apkInfo = Reader.GetApkinfo(filename);
            report += $"Applications with at least one Crypto API use case: {apkInfo.Count}\n";

            report += $"Q1 - How many apps use Cipher, SecureRandom and PBKDF2?\n";
            var secRandomUseCases = FilterApksThatHaveSig( apkInfo, new[] { "Ljava/security/SecureRandom;->" });
            var cipherUseCases = FilterApksThatHaveSig(apkInfo, new[] { "Ljavax/crypto/Cipher;->getInstance(Ljava/lang/String;)Ljavax/crypto/Cipher;" });
            var pbkdfUseCases = FilterApksThatHaveSig(apkInfo, new[] { "Ljavax/crypto/SecretKeyFactory;->getInstance(Ljava/lang/String;)Ljavax/crypto/SecretKeyFactory;", "Ljavax/crypto/spec/PBEKeySpec;->" });
            report += ReportOnCryptoAPIsUse(secRandomUseCases, cipherUseCases, pbkdfUseCases);

            report += "\n\n";
            report += $"Q2 - How many apps can be excluded by CCS'13 rules?\n";
            var ccs13Filtered = ApplyCCS13Filter(apkInfo);
            report += $"\tDataset size after applying the rules: {ccs13Filtered.Count} (Removed: {apkInfo.Count - ccs13Filtered.Count})";

            // Obfuscated class name
            var allUseCases = apkInfo.SelectMany(a => a.UseCases).ToList();
            report += "\n\n";
            report += $"Q3 - How many use cases do we have in total?\n";
            report += $"\tTotal: {allUseCases.Count}\n";
            report += $"\tSecRand: {allUseCases.Count(uc => uc.IsSecRandUseCase)}\n";
            report += $"\tCipher: {allUseCases.Count(uc => uc.IsCipherUseCaseRule1)}\n";
            report += $"\tPBKDF: {allUseCases.Count(uc => uc.IsPbkdf2UseCase)}\n";

            var nonObfuscatedClassNames = allUseCases.Where(uc => !uc.IsClassNameObfuscated).Select(uc => uc.ClassName).Distinct().ToList();
            var nonObfuscatedPackageNames = allUseCases.Where(uc => uc.IsPackageNameReadable && uc.PackageName.Length != 0).Select(uc => uc.PackageName).Distinct().ToList();
            var obfuscatedPackageNames = allUseCases.Where(uc => uc.IsPackageNamePartiallyObfuscated).Select(uc => uc.PackageName).Distinct().ToList();
            var obfuscatedPackageNames2 = allUseCases.Where(uc => uc.IsPackageNameFullyObfuscated).Select(uc => uc.PackageName).Distinct().ToList();

            report += "\n\n";
            report += $"Q4 - How many Crypto APIs entry-points/apps in classes with obfuscated class name BUT readable package name?\n";
            report += $"\t{allUseCases.Count(uc => uc.IsClassNameObfuscated && uc.IsPackageNameReadable)}\n";

            report += $"Q5 - How many Crypto APIs entry-points/apps in classes with obfuscated class name and partially obfuscated package name (e.g., com.google.a.b)?\n";
            report += $"\t{allUseCases.Count(uc => uc.IsClassNameObfuscated && uc.IsPackageNamePartiallyObfuscated)}\n";

            report += $"Q6 - How many Crypto APIs entry-points/apps in classes with obfuscated class name and fully obfuscated package name (e.g., a.a.a.b)?\n";
            report += $"\t{allUseCases.Count(uc => uc.IsClassNameObfuscated && uc.IsPackageNameFullyObfuscated)}\n";

            report += $"Q7 - How many Crypto APIs entry-points/apps in classes with obfuscated class name and with an empty package name?\n";
            report += $"\t{allUseCases.Count(uc => uc.IsClassNameObfuscated && uc.PackageName.Length == 0)}\n";

            //var top10secrnd = GetTopN(secRandomUseCases, 10);
            //var top10cipher = GetTopN(cipherRandomUseCases, 10);
            //var top10pbkdf2 = GetTopN(pbkdfRandomUseCases, 10);

            Console.Write(report);
        }

        private static string ReportOnCryptoAPIsUse(List<ApkInfo> secRand, List<ApkInfo> cipher, List<ApkInfo> pbkdf)
        {
            var report = "";
            var secRandomHashes = new HashSet<string>(secRand.Select(uc => uc.Sha1));
            var cipherHashes = new HashSet<string>(cipher.Select(uc => uc.Sha1));
            var pbkdfHashes = new HashSet<string>(pbkdf.Select(uc => uc.Sha1));
            report += $"\tSecureRandom=\t\t\t\t\t{secRandomHashes.Count}\n";
            report += $"\tCipher=\t\t\t\t\t\t\t{cipherHashes.Count}\n";
            report += $"\tPBKDF2=\t\t\t\t\t\t\t{pbkdfHashes.Count}\n";
            report += $"\tSecureRandom&Cipher=\t\t\t{secRandomHashes.Count(h => cipherHashes.Contains(h))}\n";
            report += $"\tSecureRandom&tPBKDF2=\t\t\t{secRandomHashes.Count(h => pbkdfHashes.Contains(h))}\n";
            report += $"\tCipher&tPBKDF2=\t\t\t\t\t{cipherHashes.Count(h => pbkdfHashes.Contains(h))}\n";
            report += $"\tSecureRandom&Cipher&tPBKDF2=\t{cipherHashes.Count(h => pbkdfHashes.Contains(h) && secRandomHashes.Contains(h))}\n";
            return report;
        }

        [Test]
        public void ComputeSubSets()
        {
            var apkInfo11K_12 = Reader.GetApkinfo(@"D:\data\ccs13-11k.report.csv");
            var apkInfo118K_16 = Reader.GetApkinfo(@"D:\data\sophos150K.report.csv");
            var apkInfoTOP100_15 = Reader.GetApkinfo(@"D:\data\top100.report.csv");

            var appIds11K = new HashSet<string>(apkInfo11K_12.Select(o => o.ApplicationId).ToArray());
            var appIds118K = new HashSet<string>(apkInfo118K_16.Select(o => o.ApplicationId).ToArray());
            var appIdsTOP100 = new HashSet<string>(apkInfoTOP100_15.Select(o => o.ApplicationId).ToArray());

            var appIds11KEmpty = apkInfo11K_12.Where(o => o.ApplicationId.Length == 0).Select(o => o.ApplicationId).ToArray();
            var appIds118KEmpty = apkInfo118K_16.Where(o => o.ApplicationId.Length == 0).Select(o => o.ApplicationId).ToArray();
            var appIdsTOP100Empty = apkInfoTOP100_15.Where(o => o.ApplicationId.Length == 0).Select(o => o.ApplicationId).ToArray();

            var appIds11KDups = apkInfo11K_12.Where(o => apkInfo11K_12.Count(ai => ai.ApplicationId == o.ApplicationId) > 1).Select(o => o.ApplicationId).ToArray();
            var appIds118KDups = apkInfo118K_16.Where(o => apkInfo118K_16.Count(ai => ai.ApplicationId == o.ApplicationId) > 1).ToArray();
            var appIdsTOP100Dups = apkInfoTOP100_15.Where(o => apkInfoTOP100_15.Count(ai => ai.ApplicationId == o.ApplicationId) > 1).ToArray();

            //11K^118K
            var subSet11KAnd118K = appIds11K.Where(id => appIds118K.Contains(id)).ToList();
            File.WriteAllLines(@"d:\data\subSet11KAnd118k.csv",subSet11KAnd118K);

            //TOP100^118K
            var subSetTOP100And118K = appIdsTOP100.Where(id => appIds118K.Contains(id)).ToList();
            File.WriteAllLines(@"d:\data\subSetTOP100And118k.csv", subSetTOP100And118K);

            //TOP100^11K^118K
            var subSetTOP100And118KAnd11K = appIdsTOP100.Where(id => appIds118K.Contains(id) && appIds11K.Contains(id)).ToList();
            File.WriteAllLines(@"d:\data\subSetTOP100And118kAnd11K.csv", subSetTOP100And118KAnd11K);
        }

        [TestCase("11K-12", @"D:\data\ccs13-11k.report.csv", 10222)]
        [TestCase("T100-15", @"D:\data\top100.report.csv", 3645)]
        [TestCase("118K-16", @"D:\data\sophos150K.report.csv", 95775)]
        public void GetAllFullClassNames(string fn, string inputFn, int totalApps)
        {
            var apkInfoList = GetUniqueApkInfo(Reader.GetApkinfo(inputFn));

            var allUseCases =
                apkInfoList
                .SelectMany(o => o.UseCases).ToList();
            var allFullReadable = new HashSet<string>(allUseCases.Where(uc => uc.IsPackageNameReadable && !uc.IsClassNameObfuscated).Select(uc => uc.SmaliClassName).ToArray().OrderBy(o => o));
            File.WriteAllLines($@"d:\data\obfuscation\{fn}_readable.csv", allFullReadable);

            var allFullReadablePackageNameNotClass = new HashSet<string>(allUseCases.Where(uc => uc.IsPackageNameReadable && uc.IsClassNameObfuscated).Select(uc => uc.SmaliClassName).ToArray().OrderBy(o => o));
            File.WriteAllLines($@"d:\data\obfuscation\{fn}_readable_pkgName_obfs_class.csv", allFullReadablePackageNameNotClass);

            var allPartialPackageName = new HashSet<string>(allUseCases.Where(uc => uc.IsPackageNamePartiallyObfuscated).Select(uc => uc.SmaliClassName).ToArray().OrderBy(o => o));
            File.WriteAllLines($@"d:\data\obfuscation\{fn}_partial_pkgName.csv", allPartialPackageName);

            var allFullPackageName = new HashSet<string>(allUseCases.Where(uc => uc.IsPackageNameFullyObfuscated).Select(uc => uc.SmaliClassName).ToArray().OrderBy(o => o));
            File.WriteAllLines($@"d:\data\obfuscation\{fn}_full_pkgName.csv", allFullPackageName);

            Console.WriteLine($@"Data Set:{fn}");
            var rule1Apps = apkInfoList.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule1)).ToList();
            var r1UseCases = rule1Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule1)).ToList();

            var rule2Apps = apkInfoList.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule2)).ToList();
            var r2UseCases = rule2Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule2)).ToList();

            var rule3Apps = apkInfoList.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule3)).ToList();
            var r3UseCases = rule3Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule3)).ToList();

            var rule45Apps = apkInfoList.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule4And5)).ToList();
            var r45UseCases = rule45Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule4And5)).ToList();

            var rule6Apps = apkInfoList.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule6)).ToList();
            var r6UseCases = rule6Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule6)).ToList();

            allUseCases = r1UseCases
                .Union(r2UseCases)
                .Union(r3UseCases)
                .Union(r45UseCases)
                .Union(r6UseCases).ToList();

            var none =
                allUseCases.Where(uc => uc.IsPackageNameReadable && !uc.IsClassNameObfuscated)
                    .ToList();
            var cn =
                allUseCases.Where(uc => uc.IsPackageNameReadable && uc.IsClassNameObfuscated)
                    .ToList();
            var cn_ppn =
                allUseCases.Where(uc => uc.IsPackageNamePartiallyObfuscated && uc.IsClassNameObfuscated)
                    .ToList();
            var fpn =
                allUseCases.Where(uc => uc.IsPackageNameFullyObfuscated)
                    .ToList();

            var noneApps = new HashSet<string>(
                allUseCases.Where(uc => uc.IsPackageNameReadable && !uc.IsClassNameObfuscated).Select(o => o.ApplicationInfo.ApplicationId)
                    .ToList());
            var cnApps = new HashSet<string>(
                allUseCases.Where(uc => uc.IsPackageNameReadable && uc.IsClassNameObfuscated).Select(o => o.ApplicationInfo.ApplicationId)
                    .ToList());
            var cn_ppnApps = new HashSet<string>(
                allUseCases.Where(uc => uc.IsPackageNamePartiallyObfuscated && uc.IsClassNameObfuscated).Select(o => o.ApplicationInfo.ApplicationId)
                    .ToList());
            var fpnApps = new HashSet<string>(
                allUseCases.Where(uc => uc.IsPackageNameFullyObfuscated).Select(o => o.ApplicationInfo.ApplicationId)
                    .ToList());

            var allApps = new HashSet<string>(noneApps.Union(cnApps).Union(cn_ppnApps).Union(fpnApps).ToArray());

            Console.WriteLine($@"{none.Count}&{cn.Count}&{cn_ppn.Count}&{fpn.Count}&{none.Count + cn.Count + cn_ppn.Count + fpn.Count}");
            Console.WriteLine($@"&{Math.Round(none.Count * 100.0 / allUseCases.Count)}\%&{Math.Round(cn.Count * 100.0 / allUseCases.Count)}\%&{Math.Round(cn_ppn.Count * 100.0 / allUseCases.Count)}\%&{Math.Round(fpn.Count * 100.0 / allUseCases.Count)}\%&");

            Console.WriteLine($@"{noneApps.Count}&{cnApps.Count}&{cn_ppnApps.Count}&{fpnApps.Count}&{allApps.Count}");
            Console.WriteLine($@"&{Math.Round(noneApps.Count * 100.0 / totalApps, 1)}\%&{Math.Round(cnApps.Count * 100.0 / totalApps, 1)}\%&{Math.Round(cn_ppnApps.Count * 100.0 / totalApps, 1)}\%&{Math.Round(fpnApps.Count * 100.0 / totalApps, 1)}\%&");

        }

        [Test]
        public void ComputeSetsFor1KSet()
        {
            var apkInfo1K = Reader.GetApkinfo(@"D:\_testds-stage1-result.csv");

            var appIds1K = new HashSet<string>(apkInfo1K.Select(o => o.ApplicationId).ToArray());
            var appIds1KList = apkInfo1K.Select(o => o.ApplicationId).ToList();
            var appIds1KListEmpty = apkInfo1K.Where(o => o.ApplicationId.Length == 0).Select(o => o.ApplicationId).ToList();
        }

        #region Count Unique Applications

        [Test]
        public void ComputeUniqueApplicationSet()
        {
            //ComputeUniqueApplicationSetForDataSet(@"D:\data\ccs13-11k.report.csv", @"D:\data\ccs13-11k.report.notfound.csv");
            //ComputeUniqueApplicationSetForDataSet(@"D:\data\top100.report.csv", @"D:\data\top100.report.notfound.csv");
            ComputeUniqueApplicationSetForDataSet(@"D:\data\sophos150K.report.csv", @"D:\data\sophos150K.report.notfound.csv");
        }

        private void ComputeUniqueApplicationSetForDataSet(string foundName, string notFoundName)
        {
            Console.WriteLine($"Using {foundName} and {notFoundName} files as an input");
            var found = Reader.GetApkinfo(foundName);
            var notFound = Reader.GetApkinfo(notFoundName);
            var foundUnique = new HashSet<string>(found.Select(o => o.ApplicationId).ToArray());
            var foundUniqueForNotFound = new HashSet<string>(notFound.Select(o => o.ApplicationId).ToArray());
            Console.WriteLine($"Found {foundUnique.Count} unique applications in {found.Count} APKs analyzed.");
            Console.WriteLine($"In not-found set, found {foundUniqueForNotFound.Count} unique applications in {notFound.Count} APKs analyzed.");

            var totalUnique = new HashSet<string>(found.Select(o => o.ApplicationId).Union(notFound.Select(o => o.ApplicationId)).ToArray());
            Console.WriteLine($"Total Unique direct {totalUnique.Count} (indirect {foundUnique.Count + foundUniqueForNotFound.Count})");

            Console.WriteLine($"Crypto APIs Used in {foundUnique.Count} unique applications in {totalUnique.Count} APKs analyzed ({foundUnique.Count * 100.0/ totalUnique.Count}%).");

            Console.WriteLine(@"------------------------------------------------------------------------------------------------------------------");

        }

        #endregion


        #region Count Unique Applications Use Cases Sets

        [Test]
        public void ComputeUniqueApplicationSetUseCaseStats()
        {
            //ComputeUniqueApplicationSetUseCaseStatsForDataSet(@"D:\data\ccs13-11k.report.csv", @"D:\data\ccs13-11k.report.notfound.csv", 10222);
            //ComputeUniqueApplicationSetUseCaseStatsForDataSet(@"D:\data\top100.report.csv", @"D:\data\top100.report.notfound.csv", 4067);
            ComputeUniqueApplicationSetUseCaseStatsForDataSet(@"D:\data\sophos150K.report.csv", @"D:\data\sophos150K.report.notfound.csv", 115683);
        }

        private void ComputeUniqueApplicationSetUseCaseStatsForDataSet(string foundName, string notFoundName, int total)
        {
            Console.WriteLine($"Using {foundName} and {notFoundName} files as an input");
            var found = GetUniqueApkInfo(Reader.GetApkinfo(foundName));

            var rule1Apps = found.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule1)).ToList();
            var r1UseCases = rule1Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule1));

            var rule2Apps = found.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule2)).ToList();
            var r2UseCases = rule2Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule2));

            var rule3Apps = found.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule3)).ToList();
            var r3UseCases = rule3Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule3));

            var rule45Apps = found.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule4And5)).ToList();
            var r45UseCases = rule45Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule4And5));

            var rule6Apps = found.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCaseRule6)).ToList();
            var r6UseCases = rule6Apps.SelectMany(o => RemoveDups(o.UseCases, uc => uc.IsCipherUseCaseRule6));

            Console.WriteLine($"{rule1Apps.Count}&{rule2Apps.Count}&{rule3Apps.Count}" + @"&\multicolumn{2}{c|}{" + rule45Apps.Count + @"}" + $"&{rule6Apps.Count}");
            Console.WriteLine($"{Math.Round(rule1Apps.Count * 100.0 / total)}\\%&{Math.Round(rule2Apps.Count * 100.0 / total)}\\%&{Math.Round(rule3Apps.Count * 100.0 / total)}" + @"\%&\multicolumn{2}{c|}{" + Math.Round(rule45Apps.Count * 100.0 / total) + @"\%}" + $"&{Math.Round(rule6Apps.Count * 100.0 / total)}\\%");
            Console.WriteLine($"{r1UseCases.Count()}&{r2UseCases.Count()}&{r3UseCases.Count()}" + @"&\multicolumn{2}{c|}{" + r45UseCases.Count() + @"}" + $"&{r6UseCases.Count()}");
            Console.WriteLine($"{Math.Round(r1UseCases.Count() * 1.0 / total, 1)}&{Math.Round(r2UseCases.Count() * 1.0 / total, 1)}&{Math.Round(r3UseCases.Count() * 1.0 / total, 1)}" + @"&\multicolumn{2}{c|}{" + Math.Round(r45UseCases.Count() * 1.0/ total, 1) + @"}" + $"&{Math.Round(r6UseCases.Count() * 1.0 / total, 1)}");
            Console.WriteLine(@"------------------------------------------------------------------------------------------------------------------");
        }

        public List<UseCase> RemoveDups(List<UseCase> list, Func<UseCase,bool> predicate)
        {
            var cache = new HashSet<string>();
            var result = new List<UseCase>();
            foreach (var useCase in list.Where(predicate))
            {
                if (!cache.Contains(useCase.SmaliClassName + "->" + useCase.SmaliMethodName))
                {
                    cache.Add(useCase.SmaliClassName + "->" + useCase.SmaliMethodName);
                    result.Add(useCase);
                }
            }

            return result;
        }

        public static List<ApkInfo> GetUniqueApkInfo(List<ApkInfo> infoList)
        {
            var cache = new Dictionary<string, ApkInfo>();
            foreach (var apkInfo in infoList)
            {
                if (cache.ContainsKey(apkInfo.ApplicationId))
                {
                    if (cache[apkInfo.ApplicationId].UseCases.Count < apkInfo.UseCases.Count)
                    {
                        cache[apkInfo.ApplicationId] = apkInfo;
                    }
                }
                else
                {
                    cache.Add(apkInfo.ApplicationId, apkInfo);
                }

            }
            return cache.Values.ToList();
        }

        #endregion



        private static List<ApkInfo> ApplyCCS13Filter(List<ApkInfo> apkInfo)
        {
            var result = new List<ApkInfo>();
            foreach (var info in apkInfo)
            {
                foreach (var usecase in info.UseCases)
                {
                    bool isLib = IsCCS13ExcludedLibrary(usecase);
                    if (!isLib)
                    {
                        result.Add(info);
                        break;
                    }
                }
            }
            return result;
        }

        public static bool IsCCS13ExcludedLibrary(UseCase usecase)
        {
            bool isLib = false;
            foreach (var sig in _ccs13excludedLibsRegEx)
            {
                if (usecase.PackageName.StartsWith(sig))
                {
                    isLib = true;
                }
                if (isLib)
                {
                    break;
                }
            }
            return isLib;
        }

        #region Counting functions

        private static List<ApkInfo> FilterApksThatHaveSig(List<ApkInfo> apkInfo, string[] sigs)
        {
            var result = new List<ApkInfo>();
            foreach (var info in apkInfo)
            {
                foreach (var usecase in info.UseCases)
                {
                    bool shouldBeConsidered = false;
                    foreach (var sig in sigs)
                    {
                        if (usecase.ApiSig == sig)
                        {
                            shouldBeConsidered = true;
                            break;
                        }
                    }
                    if (shouldBeConsidered)
                    {
                        result.Add(info);
                    }
                }
            }
            return result;
        }

        #endregion


        #region Compute Stats table for rules 1-6

        [TestCase(@"ccs13-11k.report.csv", "r12.rule1.csv", "r12.rule2.csv", "r12.rule3.csv", "r12.rule4.csv", "r12.rule5.csv", "r12.rule6.csv", true)]
        [TestCase(@"ccs13-11k.report.csv", "r12.rule1.csv", "r12.rule2.csv", "r12.rule3.csv", "r12.rule4.csv", "r12.rule5.csv", "r12.rule6.csv", false)]
        [TestCase(@"top100.report.csv", "t15.rule1.csv", "t15.rule2.csv", "t15.rule3.csv", "t15.rule4.csv", "t15.rule5.csv", "t15.rule6.csv", false)]
        public void ComputeTable2(string useCaseReport, string rule1fn, string rule2fn, string rule3fn, string rule4fn, string rule5fn, string rule6fn, bool useWhileListing)
        {
            var pathData = @"e:\data";
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(pathData, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(pathData, "possible_libraries.csv"));

            // Data preparation
            var uniqueApps = GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(pathData, useCaseReport)));
            var uniqueAppsIds = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
            var allUseCases = uniqueApps.Where(a => uniqueAppsIds.Contains(a.Id)).SelectMany(a => a.UseCases).ToList();
            if (useWhileListing)
                allUseCases = allUseCases.Where(uc => !IsCCS13ExcludedLibrary(uc)).ToList();

            var path = @"S:\binsightdata\data\rules1_6";
            //var path = @"D:\source\binsightdata\data\rules1_6";
            var rule1ffn = Path.Combine(path, rule1fn);
            var rule2ffn = Path.Combine(path, rule2fn);
            var rule3ffn = Path.Combine(path, rule3fn);
            var rule4ffn = Path.Combine(path, rule4fn);
            var rule5ffn = Path.Combine(path, rule5fn);
            var rule6ffn = Path.Combine(path, rule6fn);

            var latexTable = @"\textbf{Rule \#}&" +
                             @"\multicolumn{3}{|c|}{\textbf{Use-Cases}}&" +
                             @"\multicolumn{4}{|c|}{\textbf{Applications}}&" +
                             @"\textbf{P. Lib \%}" +
                             @"\\\cline{2-8}" + Environment.NewLine;
            latexTable +=
                        @"&" +
                        @"App & Lib (\% Possible Lib) & Obf &" +
                        @"App Only & Lib Only & App+Lib & Obf" +
                        @"\\\hline" + Environment.NewLine + Environment.NewLine;

            // Select all use case results that belong to our unique application set
            var rule1Results = Reader.GetUseCaseResultsForRule1(rule1ffn).Where(uc => !uc.IsAsymmetric && !uc.IsModeNotFound && uniqueAppsIds.Contains(uc.Id)).ToList();

            var rule1ResultsImplicit = rule1Results.Where(uc => IsImplicitECBMode(uc.Result)).ToList();
            var rule1ResultsExplicit = rule1Results.Where(uc => IsExplicitECBMode(uc.Result)).ToList();
            var allUseCasesRule1 = allUseCases.Where(uc => uc.IsCipherUseCaseRule1).ToList();

            latexTable += ComputeStatsForRuleInUseCaseResults(
                        "Rule 1 (Default ECB)",
                        rule1ResultsImplicit,
                        allUseCasesRule1);

            latexTable += ComputeStatsForRuleInUseCaseResults(
                        "Rule 1 (Explicit ECB)",
                        rule1ResultsExplicit,
                        allUseCasesRule1);

            var rule2ResultsAll = Reader.GetUseCaseResultsForRule2(rule2ffn)
                .Where(uc => uc.Rule2StaticIV && uniqueAppsIds.Contains(uc.Id))
                .ToList();
            var rule2Results = new List<UseCaseResult>();
            var seenCases = new HashSet<string>();
            foreach (var useCaseResult in rule2ResultsAll)
            {
                if (!seenCases.Contains($"{useCaseResult.Id}.{useCaseResult.InFileLoc}"))
                {
                    seenCases.Add($"{useCaseResult.Id}.{useCaseResult.InFileLoc}");
                    rule2Results.Add(useCaseResult);
                }
            }

            var rule1CBCModeCasesResults= rule1Results.Where(uc => IsCBCMode(uc.Result)).ToList();
            var allUseCasesRule2 = allUseCases
                .Where(uc =>
                    uc.IsCipherUseCaseRule2 &&
                    rule1CBCModeCasesResults.Any(cbc =>
                        cbc.Id == uc.ApplicationInfo.Id &&
                        cbc.Filename == uc.Filename))
                .ToList();
            latexTable += ComputeStatsForRuleInUseCaseResults(
                        "Rule 2 (Static IV)",
                        rule2Results,
                        allUseCasesRule2);


            var rule3Results = Reader.GetUseCaseResultsForRule3(rule3ffn).Where(uc => !uc.IsAsymmetric && uc.Rule3StaticKey && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule3 = allUseCases.Where(uc => uc.IsCipherUseCaseRule3).ToList();
            latexTable += ComputeStatsForRuleInUseCaseResults(
                        "Rule 3 (Static Key)",
                        rule3Results,
                        allUseCasesRule3);

            var rule4Results = Reader.GetUseCaseResultsForRule4To6(rule4ffn).Where(uc => !uc.IsAsymmetric && uc.Rule4StaticSalt && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule4 = allUseCases.Where(uc => uc.IsCipherUseCaseRule4And5).ToList();
            latexTable += ComputeStatsForRuleInUseCaseResults(
                        "Rule 4 (Static Salt)",
                        rule4Results,
                        allUseCasesRule4);

            var rule5Results = Reader.GetUseCaseResultsForRule4To6(rule5ffn).Where(uc => !uc.IsAsymmetric && uc.Rule5LessThan1000Iterations && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule5 = allUseCases.Where(uc => uc.IsCipherUseCaseRule4And5).ToList();
            latexTable += ComputeStatsForRuleInUseCaseResults(
                        "Rule 5 (Less than 1,000 iterations)",
                        rule5Results,
                        allUseCasesRule5);

            var rule6Results = Reader.GetUseCaseResultsForRule4To6(rule6ffn).Where(uc => !uc.IsAsymmetric && uc.Rule6StaticSeed&& uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule6 = allUseCases.Where(uc => uc.IsCipherUseCaseRule6).ToList();
            latexTable += ComputeStatsForRuleInUseCaseResults(
                        "Rule 6 (Static Seed)",
                        rule6Results,
                        allUseCasesRule6);

            //var rule1ResultsExplicitApps = rule1ResultsExplicit.Select(uc => uc.Id).Distinct().ToArray();
            //latexTable += @"Rule 1 (Explicit ECB) & {rule1ResultsExplicit.Count} & {rule1ResultsExplicitApps.Length} \\\hline" + Environment.NewLine;

            //var modes = new HashSet<string>(rule1Results.Select(uc => uc.Result).ToArray());
            //var result = "";
            //foreach (var mode in modes)
            //{
            //    result += $@"Result.StartsWith(@""{mode.Replace("\"", "\"\"")}"") ||" + Environment.NewLine;
            //}

            //var packageNames = new Dictionary<string, HashSet<int>>();
            //var resultFn = "";
            //foreach (var uc in allUseCases.Where(uc => !uc.IsPackageNameFullyObfuscated))
            //{
            //    if (!packageNames.ContainsKey(uc.PackageName))
            //    {
            //        packageNames.Add(uc.PackageName, new HashSet<int>());
            //    }
            //    packageNames[uc.PackageName].Add(uc.ApplicationInfo.Id);
            //}
            //var sortedPns = packageNames.OrderByDescending(kv => kv.Value.Count);
            //var resultPn = "";
            //foreach (var pn in sortedPns)
            //{
            //    resultPn += $@"PackageName.StartsWith(@""{pn.Key.Replace("\"", "\"\"")}"") || // {pn.Value.Count}" + Environment.NewLine;
            //}

            Console.WriteLine(latexTable);

        }

        private string ComputeStatsForRuleInUseCaseResults(string ruleName, List<UseCaseResult> results, List<UseCase> allUseCases)
        {
            var ruleResults_Unique = new HashSet<string>();

            var ruleResults_InLibrariesAll = allUseCases
                .Where(uc =>
                    !uc.IsPackageNameFullyObfuscated &&
                    (uc.IsLibrary || uc.IsPossibleLibrary) &&
                    results.Any(
                        ucr =>
                            ucr.Id == uc.ApplicationInfo.Id &&
                            uc.Filename == ucr.Filename &&
                            (uc.InClassPos == ucr.InFileLoc || uc.InMethodPos == ucr.InFileLoc))
                ).ToList();
            var ruleResults_InLibraries = new List<UseCase>();
            foreach (var useCase in ruleResults_InLibrariesAll)
            {
                var ucId = $@"{useCase.ApplicationInfo.Id}:{useCase.InMethodPos}:{useCase.InClassPos}:{useCase.Filename}";
                if (!ruleResults_Unique.Contains(ucId))
                {
                    ruleResults_Unique.Add(ucId);
                    ruleResults_InLibraries.Add(useCase);
                }
            }
            var ruleResults_InAppsAll = allUseCases
                .Where(uc =>
                    !uc.IsPackageNameFullyObfuscated &&
                    !uc.IsLibrary &&
                    !uc.IsPossibleLibrary &&
                    results.Any(
                        ucr =>
                            ucr.Id == uc.ApplicationInfo.Id &&
                            uc.Filename == ucr.Filename &&
                            (uc.InClassPos == ucr.InFileLoc || uc.InMethodPos == ucr.InFileLoc))
                ).ToList();
            var ruleResults_InApps = new List<UseCase>();
            foreach (var useCase in ruleResults_InAppsAll)
            {
                var ucId = $@"{useCase.ApplicationInfo.Id}:{useCase.InMethodPos}:{useCase.InClassPos}:{useCase.Filename}";
                if (!ruleResults_Unique.Contains(ucId))
                {
                    ruleResults_Unique.Add(ucId);
                    ruleResults_InApps.Add(useCase);
                }
            }
            var ruleResults_ObfusAll = allUseCases
                .Where(uc =>
                    uc.IsPackageNameFullyObfuscated &&
                    results.Any(
                        ucr =>
                            ucr.Id == uc.ApplicationInfo.Id &&
                            uc.Filename == ucr.Filename &&
                            (uc.InClassPos == ucr.InFileLoc || uc.InMethodPos == ucr.InFileLoc))
                )
                .ToList();
            var ruleResults_Obfus = new List<UseCase>();
            foreach (var useCase in ruleResults_ObfusAll)
            {
                var ucId = $@"{useCase.ApplicationInfo.Id}:{useCase.InMethodPos}:{useCase.InClassPos}:{useCase.Filename}";
                if (!ruleResults_Unique.Contains(ucId))
                {
                    ruleResults_Unique.Add(ucId);
                    ruleResults_Obfus.Add(useCase);
                }
            }

            var appsList = new List<int>(ruleResults_InLibraries.Select(uc => uc.ApplicationInfo.Id));
            appsList.AddRange(ruleResults_InApps.Select(uc => uc.ApplicationInfo.Id));
            appsList.AddRange(ruleResults_Obfus.Select(uc => uc.ApplicationInfo.Id));
            var results_APPs = new HashSet<int>(appsList);
            var results_InLibs = new HashSet<int>(ruleResults_InLibraries.Select(uc => uc.ApplicationInfo.Id));
            var results_InApps = new HashSet<int>(ruleResults_InApps.Select(uc => uc.ApplicationInfo.Id));
            var results_Obfuscated = new HashSet<int>(ruleResults_Obfus.Select(uc => uc.ApplicationInfo.Id));
            var possibleLibraryRatio = ruleResults_InLibraries.Count(uc => uc.IsPossibleLibrary)*100.0D/Convert.ToDouble(ruleResults_InLibraries.Count);

            // Application
            var inLibsOnly = results_InLibs.Count(id => !results_InApps.Contains(id));
            var inLibsAndApp = results_InLibs.Count(id => results_InApps.Contains(id));
            var inAppOnly = results_InApps.Count(id => !results_InLibs.Contains(id));

            // Format LaTeX line for the table
            var latexTableRows = ruleName +
                                 @" &\multicolumn{3}{|c|}{" + $"{ruleResults_Unique.Count}" +
                                 @"}&\multicolumn{4}{|c|}{" + $"{results_APPs.Count}" + "}" +
                                 @"\\\cline{2-8}" + Environment.NewLine;

            // Use-Cases
            latexTableRows += $@"&{ruleResults_InApps.Count} & {ruleResults_InLibraries.Count} ({possibleLibraryRatio:N2}\%) & {ruleResults_Obfus.Count} & ";
            latexTableRows += $@"{inAppOnly} & {inLibsOnly} & {inLibsAndApp} & {results_Obfuscated.Count}";
            latexTableRows += @"\\\hline" + Environment.NewLine + Environment.NewLine;

            return latexTableRows;
        }

        public static string GetPackageName(string fullName)
        {
            var index = fullName.LastIndexOf("/");
            if (index > 0)
                return fullName.Substring(0, index);
            return "";
        }

        public static bool IsExplicitECBMode(string mode)
        {
            if (mode.ToUpper().Contains("/ECB/"))
                return true;

            var modeParts = mode.Split('/');
            if (modeParts.Length > 1 && modeParts[1].ToUpper() == "ECB")
                return true;

            return false;
        }

        public static bool IsImplicitECBMode(string mode)
        {
            if (IsExplicitECBMode(mode))
                return false;

            var modeParts = mode.Split('/');
            if (modeParts.Length == 1)
                return true;

            return false;
        }

        public static string GetCipherName(string mode)
        {
            var parts = mode.Split('/');
            if (parts.Length > 0)
                return parts[0].ToUpper();
            return mode;
        }

        public static bool IsCBCMode(string mode)
        {
            if (mode.ToUpper().Contains("/CBC/"))
                return true;

            var modeParts = mode.Split('/');
            if (modeParts.Length > 1 && modeParts[1].ToUpper() == "CBC")
                return true;

            return false;
        }

        [Test]
        public void ComputeStatsTableForRules1()
        {
            var tableData = new Dictionary<string, List<int>>();
            var tableDataForApps = new Dictionary<string, List<HashSet<int>>>();
            var totalsApps = new List<int>();
            var totalsUseCase = new List<int>();
            var totalsUseCaseNot = new List<int>();
            var totalsUseCasePub = new List<int>();
            var accuracy = 2;
            ComputeStatsTableForRules1_6ForAFile(
                0,
                tableData,
                tableDataForApps,
                totalsApps,
                totalsUseCase,
                totalsUseCaseNot,
                totalsUseCasePub,
                @"D:\data\ccs13-11k.report.csv",
                @"D:\source\binsightdata\data\rules1_6\ccs13-11k.rule1.csv");

            ComputeStatsTableForRules1_6ForAFile(
                1,
                tableData,
                tableDataForApps,
                totalsApps,
                totalsUseCase,
                totalsUseCaseNot,
                totalsUseCasePub,
                @"D:\data\top100.report.csv",
                @"D:\source\binsightdata\data\rules1_6\top100.rule1.csv");
            //ComputeUniqueApplicationSetUseCaseStatsForDataSet(@"D:\data\top100.report.csv", @"D:\data\top100.report.notfound.csv", 4067);
            //ComputeUniqueApplicationSetUseCaseStatsForDataSet(@"D:\data\sophos150K.report.csv", @"D:\data\sophos150K.report.notfound.csv", 115683);
            var totalsPerCase = new Dictionary<string, int>();
            foreach (var row in tableData)
            {
                totalsPerCase.Add(row.Key, row.Value.Sum());
            }

            for (int i = 0; i < totalsUseCase.Count; i++)
            {
                Console.WriteLine($"Index {i}: Symmetric Cases {totalsUseCase[i]}, Asymmetric Cases {totalsUseCasePub[i]}, NotFound {totalsUseCaseNot[i]}");
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");

            var orderedList = totalsPerCase.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
            var stringTable = new List<string>
            {
                @"\textbf{Mode} & \multicolumn{4}{|c|}{\textbf{R12}} & \multicolumn{4}{|c|}{\textbf{T15}}"  // + "& \multicolumn{4}{|c|}{\textbf{R16}}\\\hline"
                ,@"\\\hline"
                ,@"&UC-N&UC-\%&App-N&App-\%&UC-N&UC-\%&App-N&App-\%"
                ,@"\\\hline"
            };

            foreach (var encrMode in orderedList)
            {
                var line = encrMode
                    .Replace(">", @"\>")
                    .Replace("&", @"\&")
                    .Replace("$", @"\$")
                    .Replace("%", @"\%")
                    .Replace("_", @"\_")
                    .Replace("#", @"\#")
                    + "&";
                int index = 0;
                var totalAppsForMode = tableDataForApps[encrMode];
                foreach (var n in tableData[encrMode])
                {
                    line += $@"{n} & { Math.Round(n*100.0D / totalsUseCase[index], accuracy) }\% &";
                    line += $@"{ totalAppsForMode[index].Count} & {Math.Round(totalAppsForMode[index].Count * 100.0D / totalsApps[index], accuracy)} \%";
                    line += "&";
                    index++;
                }
                line = line.TrimEnd('&');
                stringTable.Add(line + @"\\\hline");
            }

            Console.WriteLine(string.Join("\n", stringTable));
        }

        private void ComputeStatsTableForRules1_6ForAFile(
                    int index,
                    Dictionary<string, List<int>> resultTable,
                    Dictionary<string, List<HashSet<int>>> resultTableForApps,
                    List<int> totalsApps,
                    List<int> totalsUseCases,
                    List<int> totalsUseCasesNot,
                    List<int> totalsUseCasesPub,
                    string foundName,
                    string ruleFileName)
        {
            var found = GetUniqueApkInfo(Reader.GetApkinfo(foundName));
            totalsApps.Add(found.Count);
            var appIntIds = found.Select(a => a.Id).ToList();
            var rule1Results = Reader.GetUseCaseResultsForRule1(ruleFileName);
            var rule1ResultsInUniqueApps = rule1Results.Where(ucr => appIntIds.Contains(ucr.Id)).ToList();
            totalsUseCasesNot.Add(rule1ResultsInUniqueApps.Count(ucr => ucr.IsModeNotFound));
            totalsUseCasesPub.Add(rule1ResultsInUniqueApps.Count(ucr => ucr.IsAsymmetric));

            rule1ResultsInUniqueApps = rule1ResultsInUniqueApps.Where(uc => !uc.IsAsymmetric && !uc.IsModeNotFound).ToList();
            totalsUseCases.Add(rule1ResultsInUniqueApps.Count);

            foreach (var rule1ResultsInUniqueApp in rule1ResultsInUniqueApps)
            {
                // This counts the total number of use-cases
                if (!resultTable.ContainsKey(rule1ResultsInUniqueApp.Result.ToUpper()))
                {
                    resultTable.Add(rule1ResultsInUniqueApp.Result.ToUpper(), new List<int>());
                }
                var lst = resultTable[rule1ResultsInUniqueApp.Result.ToUpper()];
                if (lst.Count <= index)
                {
                    for (int i = lst.Count; i <= index; i++)
                    {
                        lst.Add(0);
                    }
                }
                lst[index] = lst[index] + 1;

                // This counts the total number of apps (the same way CryptoLint did)
                if (!resultTableForApps.ContainsKey(rule1ResultsInUniqueApp.Result.ToUpper()))
                {
                    resultTableForApps.Add(rule1ResultsInUniqueApp.Result.ToUpper(), new List<HashSet<int>>());
                }
                var lsthset = resultTableForApps[rule1ResultsInUniqueApp.Result.ToUpper()];
                if (lsthset.Count <= index)
                {
                    for (int i = lsthset.Count; i <= index; i++)
                    {
                        lsthset.Add(new HashSet<int>());
                    }
                }
                lsthset[index].Add(rule1ResultsInUniqueApp.Id);
            }
        }

        #endregion


        #region Generate unique package names reused across more than one application

        [TestCase(@"e:\data")]
//        [TestCase(@"d:\data")]
        public void GenerateListOfReusedPackages(string path)
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(path, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(path, "possible_libraries.csv"));

            var r12path = Path.Combine(path, "ccs13-11k.report.csv");
            var uniqueAppsR12 = GetUniqueApkInfo(Reader.GetApkinfo(r12path));
            var packageNamesR12 = GetNameOrderedListOfAllPackageNames(uniqueAppsR12);
            File.WriteAllText(Path.Combine(path, "r12PackageNames.csv"), packageNamesR12);

            var t15path = Path.Combine(path, "top100.report.csv");
            var uniqueAppsT15 = GetUniqueApkInfo(Reader.GetApkinfo(t15path));
            var packageNamesT15 = GetNameOrderedListOfAllPackageNames(uniqueAppsT15);
            File.WriteAllText(Path.Combine(path, "t15PackageNames.csv"), packageNamesT15);

            var r16path = Path.Combine(path, "sophos150K.report.csv");
            var uniqueAppsR16 = GetUniqueApkInfo(Reader.GetApkinfo(r16path));
            var packageNamesR16 = GetNameOrderedListOfAllPackageNames(uniqueAppsR16);
            File.WriteAllText(Path.Combine(path, "r16PackageNames.csv"), packageNamesR16);
        }

        private string GetNameOrderedListOfAllPackageNames(List<ApkInfo> appInfo)
        {
            var uniquePackageNames =
                appInfo.SelectMany(uc => uc.UseCases).Where(uc => uc.IsCipherUseCaseRule1to6 && !uc.IsPackageNameFullyObfuscated && !uc.IsLibrary);
            var packageCounts = new Dictionary<string, int>();
            var packageKnownPerApp = new Dictionary<string, HashSet<string>>();
            foreach (var useCase in uniquePackageNames)
            {
                if (!packageKnownPerApp.ContainsKey(useCase.PackageName))
                {
                    packageKnownPerApp.Add(useCase.PackageName, new HashSet<string>());
                }
                if (!packageCounts.ContainsKey(useCase.PackageName))
                {
                    packageCounts.Add(useCase.PackageName, 0);
                }
                if (!packageKnownPerApp[useCase.PackageName].Contains(useCase.ApplicationInfo.ApplicationId))
                {
                    packageKnownPerApp[useCase.PackageName].Add(useCase.ApplicationInfo.ApplicationId);
                    packageCounts[useCase.PackageName] += 1;
                }
            }

            return "PackageName,Count\n" +
                string.Join("\n",
                packageCounts.Where(kv => kv.Value < 5 && kv.Value >= 2)
                    .OrderBy(kv => kv.Key)
                    .Select(kv => $@"{kv.Key},{kv.Value}")
                    .ToArray());
        }

        #endregion


        #region Reading report files


        public class SigUseCount
        {
            public string PackageName;
            public int UseCount = 0;
        }



        #endregion

    }

}
