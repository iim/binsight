using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using APKInsight.Logic.Analysis.Data;
using CsnowFramework.InputOutput;
using NUnit.Framework;

namespace BinSightTest
{
    //[TestFixture]
    public class UsenixSubmissionScripts
    {
        private static string _dataPath = @"E:\eurosp2018\final_data";
        private static string _dataPathForLibInfo = @"S:\binsightdata\data\libraries";


        #region Splitting apps into different categories of Crypto they use

        // Gives a generic report on three datasets
        [Test]
        public void ComputeDuplicateCallSiteStatsForAllSets()
        {
            ComputeDuplicateCallSiteStatsForSet(Path.Combine(_dataPath, @"stage1\ccs13-11k.report.csv"), "R12");
            ComputeDuplicateCallSiteStatsForSet(Path.Combine(_dataPath, @"stage1\top100.report.csv"), "T15");
            ComputeDuplicateCallSiteStatsForSet(Path.Combine(_dataPath, @"stage1\sophos150K.report.csv"), "R16");
        }

        private static void ComputeDuplicateCallSiteStatsForSet(string fullPath, string datasetName)
        {
            var apksInfo = Reader.GetApkinfo(fullPath);
            var uniqueApksInfo = DevScriptsTests.GetUniqueApkInfo(apksInfo);
            string report = $"{datasetName} & {apksInfo.Count}/{uniqueApksInfo.Count} &\n";

            report += $"Q1 - How many apps use Cipher, SecureRandom and PBKDF2?\n";
            var secRandomUseCases = uniqueApksInfo.Where(apk => apk.UseCases.Any(uc => uc.IsSecRandUseCase)).ToList();
            var cipherUseCases = uniqueApksInfo.Where(apk => apk.UseCases.Any(uc => uc.IsCipherUseCase)).ToList();
            var pbkdfUseCases = uniqueApksInfo.Where(apk => apk.UseCases.Any(uc => uc.IsPbkdf2UseCase)).ToList();
            report += ReportOnCryptoAPIsUse(secRandomUseCases, cipherUseCases, pbkdfUseCases);

            //report += "\n\n";
            //report += $"Q2 - How many apps can be excluded by CCS'13 rules?\n";

            //// Obfuscated class name
            //var allUseCases = apkInfo.SelectMany(a => a.UseCases).ToList();
            //var temp = allUseCases.Where(uc => uc.ApiSig.Contains("Ljava/security/SecureRandom;->")).ToList();
            //report += "\n\n";
            //report += $"Q3 - How many use cases do we have in total?\n";
            //report += $"\tTotal: {allUseCases.Count}\n";
            //report += $"\tSecRand: {allUseCases.Count(uc => uc.IsSecRandUseCase)}\n";
            //report += $"\tCipher: {allUseCases.Count(uc => uc.IsCipherUseCaseRule1)}\n";
            //report += $"\tPBKDF: {allUseCases.Count(uc => uc.IsPbkdf2UseCase)}\n";

            //var nonObfuscatedClassNames =
            //    allUseCases.Where(uc => !uc.IsClassNameObfuscated).Select(uc => uc.ClassName).Distinct().ToList();
            //var nonObfuscatedPackageNames =
            //    allUseCases.Where(uc => uc.IsPackageNameReadable && uc.PackageName.Length != 0)
            //        .Select(uc => uc.PackageName)
            //        .Distinct()
            //        .ToList();
            //var obfuscatedPackageNames =
            //    allUseCases.Where(uc => uc.IsPackageNamePartiallyObfuscated).Select(uc => uc.PackageName).Distinct().ToList();
            //var obfuscatedPackageNames2 =
            //    allUseCases.Where(uc => uc.IsPackageNameFullyObfuscated).Select(uc => uc.PackageName).Distinct().ToList();

            //report += "\n\n";
            //report +=
            //    $"Q4 - How many Crypto APIs entry-points/apps in classes with obfuscated class name BUT readable package name?\n";
            //report += $"\t{allUseCases.Count(uc => uc.IsClassNameObfuscated && uc.IsPackageNameReadable)}\n";

            //report +=
            //    $"Q5 - How many Crypto APIs entry-points/apps in classes with obfuscated class name and partially obfuscated package name (e.g., com.google.a.b)?\n";
            //report += $"\t{allUseCases.Count(uc => uc.IsClassNameObfuscated && uc.IsPackageNamePartiallyObfuscated)}\n";

            //report +=
            //    $"Q6 - How many Crypto APIs entry-points/apps in classes with obfuscated class name and fully obfuscated package name (e.g., a.a.a.b)?\n";
            //report += $"\t{allUseCases.Count(uc => uc.IsClassNameObfuscated && uc.IsPackageNameFullyObfuscated)}\n";

            //report +=
            //    $"Q7 - How many Crypto APIs entry-points/apps in classes with obfuscated class name and with an empty package name?\n";
            //report += $"\t{allUseCases.Count(uc => uc.IsClassNameObfuscated && uc.PackageName.Length == 0)}\n";

            Console.WriteLine(report);
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

        #endregion


        #region Provide data for stacked barplots for call-sites by rule

        // Gives a generic report on three datasets
        [Test]
        public void ComputeCallSiteNumbersForEachRule()
        {
            ComputeCallSiteNumbersForEachRuleForDataSet(Path.Combine(_dataPath, @"stage1\ccs13-11k.report.csv"), "R12");
            ComputeCallSiteNumbersForEachRuleForDataSet(Path.Combine(_dataPath, @"stage1\top100.report.csv"), "T15");
            ComputeCallSiteNumbersForEachRuleForDataSet(Path.Combine(_dataPath, @"stage1\sophos150K.report.csv"), "R16");
        }

        private static void ComputeCallSiteNumbersForEachRuleForDataSet(string fullPath, string datasetName)
        {
            var apksInfo = Reader.GetApkinfo(fullPath);
            var uniqueApksInfo = DevScriptsTests.GetUniqueApkInfo(apksInfo);
            var allCallSites = uniqueApksInfo.SelectMany(app => app.UseCases).Where(cs => cs.IsCipherUseCaseRule1to6).ToList();
            var r1 = allCallSites.Count(cs => cs.IsCipherUseCaseRule1);
            var r2 = allCallSites.Count(cs => cs.IsCipherUseCaseRule2);
            var r3 = allCallSites.Count(cs => cs.IsCipherUseCaseRule3);
            var r4 = allCallSites.Count(cs => cs.IsCipherUseCaseRule4And5);
            var r6 = allCallSites.Count(cs => cs.IsCipherUseCaseRule6);
            Console.WriteLine($"{datasetName}\t{allCallSites.Count}\t{r1}\t{r2}\t{r3}\t{r4}\t{r6}");
        }

        #endregion


        #region Provide data for stacked barplots for call-sutes by obfuscation

        // Gives a generic report on three datasets
        [Test]
        public void ComputeCallSiteNumbersForEachObfuscationTechnique()
        {
            ComputeCallSiteNumbersForEachObfuscationTechniqueForDataSet(Path.Combine(_dataPath, @"stage1\ccs13-11k.report.csv"), "R12");
            ComputeCallSiteNumbersForEachObfuscationTechniqueForDataSet(Path.Combine(_dataPath, @"stage1\top100.report.csv"), "T15");
            ComputeCallSiteNumbersForEachObfuscationTechniqueForDataSet(Path.Combine(_dataPath, @"stage1\sophos150K.report.csv"), "R16");
        }

        private static void ComputeCallSiteNumbersForEachObfuscationTechniqueForDataSet(string fullPath, string datasetName)
        {
            var apksInfo = Reader.GetApkinfo(fullPath);
            var uniqueApksInfo = DevScriptsTests.GetUniqueApkInfo(apksInfo);
            var allCallSites = uniqueApksInfo.SelectMany(app => app.UseCases).Where(cs => cs.IsCipherUseCaseRule1to6).ToList();
            var none = allCallSites.Count(cs => cs.IsPackageNameReadable && !cs.IsClassNameObfuscated);
            var cn = allCallSites.Count(cs => cs.IsPackageNameReadable && cs.IsClassNameObfuscated);
            var ppn = allCallSites.Count(cs => cs.IsPackageNamePartiallyObfuscated);
            var full = allCallSites.Count(cs => cs.IsPackageNameFullyObfuscated);
            Console.WriteLine($"{datasetName}\t{allCallSites.Count}\t{none}\t{cn}\t{ppn}\t{full}");
        }

        #endregion


        #region Provide data for stacked barplots for call-sutes by app vs library

        // Gives a generic report on three datasets
        [Test]
        public void ComputeCallSiteNumbersForAppVsLib()
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            ComputeCallSiteNumbersForAppVsLibForDataSet(Path.Combine(_dataPath, @"stage1\ccs13-11k.report.csv"), "R12");
            ComputeCallSiteNumbersForAppVsLibForDataSet(Path.Combine(_dataPath, @"stage1\top100.report.csv"), "T15");
            ComputeCallSiteNumbersForAppVsLibForDataSet(Path.Combine(_dataPath, @"stage1\sophos150K.report.csv"), "R16");
        }

        private static void ComputeCallSiteNumbersForAppVsLibForDataSet(string fullPath, string datasetName)
        {
            var apksInfo = Reader.GetApkinfo(fullPath);
            var uniqueApksInfo = DevScriptsTests.GetUniqueApkInfo(apksInfo);
            var allCallSites = uniqueApksInfo.SelectMany(app => app.UseCases).Where(cs => cs.IsCipherUseCaseRule1to6).ToList();
            var lib = allCallSites.Count(cs => cs.IsLibrary);
            var posLib = allCallSites.Count(cs => cs.IsPossibleLibrary);
            var apps = allCallSites.Count(cs => !cs.IsPackageNameFullyObfuscated && !cs.IsLibrary && !cs.IsPossibleLibrary);
            var full = allCallSites.Count(cs => cs.IsPackageNameFullyObfuscated);
            Console.WriteLine($"{datasetName}\t{allCallSites.Count}\t{lib}\t{posLib}\t{apps}\t{full}");
        }

        [Test]
        public void ComputeUniquePackageNames()
        {
            ComputeUniquePackageNamesDataSet(@"ccs13-11k.report.csv", "R12");
            ComputeUniquePackageNamesDataSet(@"sophos150K.report.csv", "R16");
            ComputeUniquePackageNamesDataSet(@"top100.report.csv", "T15");
        }

        private void ComputeUniquePackageNamesDataSet(string useCaseReport, string dsName)
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries =
                Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueApps =
                DevScriptsTests.GetUniqueApkInfo(
                    Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), useCaseReport)));
            var uniqueAppsIds = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
            var allUseCases = uniqueApps.Where(a => uniqueAppsIds.Contains(a.Id)).SelectMany(a => a.UseCases).ToList();

            var packageStats = new Dictionary<string, int>();
            foreach (var allUseCase in allUseCases)
            {
                if (!packageStats.ContainsKey(allUseCase.PackageName))
                {
                    packageStats.Add(allUseCase.PackageName, 0);
                }
                packageStats[allUseCase.PackageName]++;
            }

            Console.WriteLine($"{dsName}&{packageStats.Count(kv => kv.Value == 1)}&{packageStats.Count(kv => kv.Value > 1)}&{packageStats.Count(kv => kv.Value >= 5)}");

            allUseCases = allUseCases.Where(uc => uc.IsLibrary).ToList();
            packageStats = new Dictionary<string, int>();
            foreach (var allUseCase in allUseCases)
            {
                if (!packageStats.ContainsKey(allUseCase.PackageName))
                {
                    packageStats.Add(allUseCase.PackageName, 0);
                }
                packageStats[allUseCase.PackageName]++;
            }
            Console.WriteLine($"{dsName}&{packageStats.Count()}");

        }

        [Test]
        public void ComputeLibrariesIntersection()
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries =
                Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueAppsR12 =
                DevScriptsTests.GetUniqueApkInfo(
                    Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), @"ccs13-11k.report.csv")));
            var uniqueAppsR16 =
                DevScriptsTests.GetUniqueApkInfo(
                    Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), @"sophos150K.report.csv")));
            var uniqueAppsT15 =
                DevScriptsTests.GetUniqueApkInfo(
                    Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), @"top100.report.csv")));
            var allUseCasesR12 = uniqueAppsR12.SelectMany(a => a.UseCases).Where(cs => cs.IsLibrary).ToList();
            var allUseCasesR16 = uniqueAppsR16.SelectMany(a => a.UseCases).Where(cs => cs.IsLibrary).ToList();
            var allUseCasesT15 = uniqueAppsT15.SelectMany(a => a.UseCases).Where(cs => cs.IsLibrary).ToList();

            var packageStats12 = new Dictionary<string, int>();
            var packageStats16 = new Dictionary<string, int>();
            var packageStats15 = new Dictionary<string, int>();
            foreach (var allUseCase in allUseCasesR12)
            {
                if (!packageStats12.ContainsKey(allUseCase.PackageName))
                {
                    packageStats12.Add(allUseCase.PackageName, 0);
                }
                packageStats12[allUseCase.PackageName]++;
            }
            foreach (var allUseCase in allUseCasesR16)
            {
                if (!packageStats16.ContainsKey(allUseCase.PackageName))
                {
                    packageStats16.Add(allUseCase.PackageName, 0);
                }
                packageStats16[allUseCase.PackageName]++;
            }
            foreach (var allUseCase in allUseCasesT15)
            {
                if (!packageStats15.ContainsKey(allUseCase.PackageName))
                {
                    packageStats15.Add(allUseCase.PackageName, 0);
                }
                packageStats15[allUseCase.PackageName]++;
            }

            Console.WriteLine($"R12&{packageStats12.Count}");
            Console.WriteLine($"R16&{packageStats16.Count}");
            Console.WriteLine($"T15&{packageStats15.Count}");

            // How many libs in R12 and R16
            var r12Ranked = packageStats12.OrderByDescending(kv => kv.Value).ToList();
            var r16Ranked = packageStats16.OrderByDescending(kv => kv.Value).ToList();
            var t15Ranked = packageStats15.OrderByDescending(kv => kv.Value).ToList();

            // Output R12 top 10
            Console.WriteLine("R12 dataset");
            for (int i = 0; i < 10; i++)
            {
                var libPackageName = r12Ranked[i];
                int r12 = i + 1;
                var r16 = GetInidexOfPackage(libPackageName.Key, r16Ranked);
                var t15 = GetInidexOfPackage(libPackageName.Key, t15Ranked);
                Console.WriteLine($@"&{libPackageName.Key} & {r12} & {r16} & {t15}\\\hline");
            }

            // Output R16 top 10
            Console.WriteLine("R16 dataset");
            for (int i = 0; i < 11; i++)
            {
                var libPackageName = r16Ranked[i];
                var r12 = GetInidexOfPackage(libPackageName.Key, r12Ranked);
                int r16 = i + 1;
                var t15 = GetInidexOfPackage(libPackageName.Key, t15Ranked);
                Console.WriteLine($@"&{libPackageName.Key} & {r12} & {r16} & {t15}\\\hline");
            }
            // Output T15 top 10
            Console.WriteLine("T15 dataset");
            for (int i = 0; i < 11; i++)
            {
                var libPackageName = t15Ranked[i];
                var r12 = GetInidexOfPackage(libPackageName.Key, r12Ranked);
                var r16 = GetInidexOfPackage(libPackageName.Key, r16Ranked);
                int t15 = i + 1;
                Console.WriteLine($@"&{libPackageName.Key} & {r12} & {r16} & {t15}\\\hline");
            }
        }

        private int GetInidexOfPackage(string name, List<KeyValuePair<string, int>> libs)
        {
            for (int j = 0; j < libs.Count; j++)
            {
                if (libs[j].Key == name)
                {
                    return j + 1;
                }
            }
            return -1;
        }

        [Test]
        public void SelectApkFilesForLibraryAnalysis()
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries =
                Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueAppsR12 =
                DevScriptsTests.GetUniqueApkInfo(
                    Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), @"ccs13-11k.report.csv")));
            var uniqueAppsR16 =
                DevScriptsTests.GetUniqueApkInfo(
                    Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), @"sophos150K.report.csv")));
            var uniqueAppsT15 =
                DevScriptsTests.GetUniqueApkInfo(
                    Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), @"top100.report.csv")));
            var allUseCasesR12 = uniqueAppsR12.SelectMany(a => a.UseCases).Where(
                cs =>
                    cs.IsLibrary &&
                    (
                    //cs.PackageName == "com.google.ads.util"
                    cs.PackageName == "com.vpon.adon.android.utils"
                    ))
                .Select(cs => cs.ApplicationInfo).ToList();
            var allUseCasesR16 = uniqueAppsR16.SelectMany(a => a.UseCases).Where(
                cs =>
                    cs.IsLibrary &&
                    (
                    //cs.PackageName == "com.google.android.gms.internal"
                    cs.PackageName == "org.apache.http.impl.auth"
                    )
                    )
                    .Select(cs => cs.ApplicationInfo).ToList();
            var allUseCasesT15 = uniqueAppsT15.SelectMany(a => a.UseCases).Where(
                cs =>
                    cs.IsLibrary &&
                    (
                    //cs.PackageName == "com.google.android.gms.internal"
                    cs.PackageName.StartsWith("com.inmobi.commons.")
                    ))
                    .Select(cs => cs.ApplicationInfo).ToList();


        }

        #endregion


        #region Provide data for stacked plot for rule 1-6 violation

        [Test]
        public void ComputeCipherDistributions()
        {
            ComputeCipherDistributionsForDataSet(@"ccs13-11k.report.csv", "r12.rule1.csv", "R12");
            ComputeCipherDistributionsForDataSet(@"sophos150K.report.csv", "r16.rule1.csv", "R16");
            //ComputeCipherDistributionsForDataSet(@"top100.report.csv", "t15.rule1.csv", "T15");
        }

        private void ComputeCipherDistributionsForDataSet(string useCaseReport, string rulefn, string dsName)
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), useCaseReport)));
            var uniqueAppsIds = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
            var allUseCases = uniqueApps.Where(a => uniqueAppsIds.Contains(a.Id)).SelectMany(a => a.UseCases).ToList();

            var ruleFilename = Path.Combine(_dataPath, rulefn);

            // Select all use case results that belong to our unique application set
            var rule1Results = Reader.GetUseCaseResultsForRule1(ruleFilename).Where(uc => !uc.IsAsymmetric && !uc.IsModeNotFound && uniqueAppsIds.Contains(uc.Id)).ToList();
            var frequency = new Dictionary<string, int>();
            foreach (var useCaseResult in rule1Results)
            {
                if (!frequency.ContainsKey(DevScriptsTests.GetCipherName(useCaseResult.Result)))
                {
                    frequency.Add(DevScriptsTests.GetCipherName(useCaseResult.Result), 0);
                }
                frequency[DevScriptsTests.GetCipherName(useCaseResult.Result)]++;
            }

            var selectedCiphers = new List<string>(new []{"AES", "DES", "DESEDE", "PBEWITHMD5ANDDES", "BLOWFISH", "RC4"});
            var top5ciphers = frequency.OrderByDescending(kv => kv.Value).Where(kv => selectedCiphers.Contains(kv.Key)).ToList();
            var topNcipherNames = top5ciphers.Select(kv => kv.Key).ToList();
            var filteredRule1Results = rule1Results.Where(r => topNcipherNames.Contains(DevScriptsTests.GetCipherName(r.Result))).ToList();

            var allCallSitesForTheRule = allUseCases.Where(uc => uc.IsCipherUseCaseRule1).ToList();

            var lib = allCallSitesForTheRule.Count(cs => cs.IsLibrary);
            var posLib = allCallSitesForTheRule.Count(cs => cs.IsPossibleLibrary);
            var apps = allCallSitesForTheRule.Count(cs => !cs.IsPackageNameFullyObfuscated && !cs.IsLibrary && !cs.IsPossibleLibrary);
            var full = allCallSitesForTheRule.Count(cs => cs.IsPackageNameFullyObfuscated);

            Console.WriteLine($"{dsName}\t{allCallSitesForTheRule.Count}\t{lib}\t{posLib}\t{apps}\t{full}");

            foreach (var keyValuePair in top5ciphers)
            {
                Console.WriteLine($"{keyValuePair.Key}\t{keyValuePair.Value}");
                var rule1SpecificCipherResults =
                    filteredRule1Results.Where(
                        r => DevScriptsTests.GetCipherName(r.Result.ToUpper()) == keyValuePair.Key).ToList();
                Console.WriteLine(
                    ComputeResultDistributionbyAppsLibs(
                            $"{keyValuePair.Key}",
                            rule1SpecificCipherResults,
                            allCallSitesForTheRule));
            }


            Console.WriteLine();

        }

        [Test]
        public void ComputeViolationOfRule1()
        {
            ComputeViolationOfRule1ForDataSet(@"ccs13-11k.report.csv", "r12.rule1.csv", "R12", true);
            ComputeViolationOfRule1ForDataSet(@"sophos150K.report.csv", "r16.rule1.csv", "R16*", true);
            ComputeViolationOfRule1ForDataSet(@"sophos150K.report.csv", "r16.rule1.csv", "R16", false);
            ComputeViolationOfRule1ForDataSet(@"top100.report.csv", "t15.rule1.csv", "T15", false);
        }

        private void ComputeViolationOfRule1ForDataSet(string useCaseReport, string rulefn, string dsName, bool applyCryptoLintWhiteListing)
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), useCaseReport)));
            var uniqueAppsIds = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
            var allUseCases = uniqueApps
                    .Where(a => uniqueAppsIds.Contains(a.Id))
                    .SelectMany(a => a.UseCases)
                    .ToList();

            var ruleFilename = Path.Combine(_dataPath, rulefn);

            // Select all use case results that belong to our unique application set
            var rule1Results = Reader.GetUseCaseResultsForRule1(ruleFilename).Where(uc => !uc.IsAsymmetric && !uc.IsModeNotFound && uniqueAppsIds.Contains(uc.Id)).ToList();

            var rule1ResultsImplicit = rule1Results.Where(uc => DevScriptsTests.IsImplicitECBMode(uc.Result)).ToList();
            var rule1ResultsExplicit = rule1Results.Where(uc => DevScriptsTests.IsExplicitECBMode(uc.Result)).ToList();
            var allCallSitesForTheRule = allUseCases.Where(uc => uc.IsCipherUseCaseRule1)
                .Where(uc => !applyCryptoLintWhiteListing || !DevScriptsTests.IsCCS13ExcludedLibrary(uc))
                .ToList();

            var lib = allCallSitesForTheRule.Count(cs => cs.IsLibrary);
            var posLib = allCallSitesForTheRule.Count(cs => cs.IsPossibleLibrary);
            var apps = allCallSitesForTheRule.Count(cs => !cs.IsPackageNameFullyObfuscated && !cs.IsLibrary && !cs.IsPossibleLibrary);
            var full = allCallSitesForTheRule.Count(cs => cs.IsPackageNameFullyObfuscated);

            Console.WriteLine($"{dsName}\t{allCallSitesForTheRule.Count}\t{lib}\t{posLib}\t{apps}\t{full}");

            Console.WriteLine(
                ComputeResultDistributionbyAppsLibs(
                        $"{dsName} - Rule 1 (D-ECB)",
                        rule1ResultsImplicit,
                        allCallSitesForTheRule));

            Console.WriteLine(
                ComputeResultDistributionbyAppsLibs(
                        $"{dsName} - Rule 1 (E-ECB)",
                        rule1ResultsExplicit,
                        allCallSitesForTheRule));

            Console.WriteLine();

        }

        [Test]
        public void ComputeViolationOfRule2()
        {
            ComputeViolationOfRule2ForDataSet(@"ccs13-11k.report.csv", "r12.rule1.csv", "r12.rule2.csv", "R12", true);
            ComputeViolationOfRule2ForDataSet(@"sophos150K.report.csv", "r16.rule1.csv", "r16.rule2.csv", "R16*", true);
            ComputeViolationOfRule2ForDataSet(@"sophos150K.report.csv", "r16.rule1.csv", "r16.rule2.csv", "R16", false);
            ComputeViolationOfRule2ForDataSet(@"top100.report.csv", "t15.rule1.csv", "t15.rule2.csv", "T15", false);
        }

        private void ComputeViolationOfRule2ForDataSet(string useCaseReport, string rule1fn, string rule2fn, string dsName, bool applyCryptoLintWhiteListing)
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), useCaseReport)));
            var uniqueAppsIds = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
            var allUseCases = uniqueApps.Where(a => uniqueAppsIds.Contains(a.Id)).SelectMany(a => a.UseCases).ToList();

            var rule1Ffn = Path.Combine(_dataPath, rule1fn);
            var rule2Ffn = Path.Combine(_dataPath, rule2fn);

            // Select all use case results that belong to our unique application set
            var rule1Results = Reader.GetUseCaseResultsForRule1(rule1Ffn).Where(uc => !uc.IsAsymmetric && !uc.IsModeNotFound && uniqueAppsIds.Contains(uc.Id)).ToList();

            var allCallSitesForTheRule2 = allUseCases.Where(uc => uc.IsCipherUseCaseRule2)
                .Where(uc => !applyCryptoLintWhiteListing || !DevScriptsTests.IsCCS13ExcludedLibrary(uc))
                .ToList();

            var rule2ResultsAll = Reader.GetUseCaseResultsForRule2(rule2Ffn).Where(uc => uc.Rule2StaticIV && uniqueAppsIds.Contains(uc.Id)).ToList();
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

            var rule1CbcModeCasesResults = rule1Results.Where(uc => DevScriptsTests.IsCBCMode(uc.Result)).ToList();
            var allUseCasesRule2 = allUseCases
                .Where(uc =>
                    uc.IsCipherUseCaseRule2 &&
                    rule1CbcModeCasesResults.Any(cbc =>
                        cbc.Id == uc.ApplicationInfo.Id &&
                        cbc.Filename == uc.Filename))
                .ToList();

            // Report totals
            var lib = allCallSitesForTheRule2.Count(cs => cs.IsLibrary);
            var posLib = allCallSitesForTheRule2.Count(cs => cs.IsPossibleLibrary);
            var apps = allCallSitesForTheRule2.Count(cs => !cs.IsPackageNameFullyObfuscated && !cs.IsLibrary && !cs.IsPossibleLibrary);
            var full = allCallSitesForTheRule2.Count(cs => cs.IsPackageNameFullyObfuscated);
            Console.WriteLine($"{dsName}\t{allCallSitesForTheRule2.Count}\t{lib}\t{posLib}\t{apps}\t{full}");

            Console.WriteLine(ComputeResultDistributionbyAppsLibs(
                        $"{dsName} - Rule 2 (Static IV)",
                        rule2Results,
                        allUseCasesRule2));

            Console.WriteLine();

        }

        [Test]
        public void ComputeViolationOfRule3()
        {
            ComputeViolationOfRule3ForDataSet(@"ccs13-11k.report.csv", "r12.rule3.csv", "R12", true);
            ComputeViolationOfRule3ForDataSet(@"sophos150K.report.csv", "r16.rule3.csv", "R16*", true);
            //ComputeViolationOfRule3ForDataSet(@"top100.report.csv", "t15.rule3.csv", "T15", false);
        }

        private void ComputeViolationOfRule3ForDataSet(string useCaseReport, string rulefn, string dsName, bool applyCryptoLintWhiteListing)
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), useCaseReport)));
            var uniqueAppsIds = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
            var allUseCases = uniqueApps.Where(a => uniqueAppsIds.Contains(a.Id)).SelectMany(a => a.UseCases).ToList();

            var ruleFfn = Path.Combine(_dataPath, rulefn);

            var rule3Results = Reader.GetUseCaseResultsForRule3(ruleFfn).Where(uc => !uc.IsAsymmetric && uc.Rule3StaticKey && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule3 = allUseCases.Where(uc => uc.IsCipherUseCaseRule3)
                .Where(uc => !applyCryptoLintWhiteListing || !DevScriptsTests.IsCCS13ExcludedLibrary(uc))
                .ToList();

            var lib = allUseCasesRule3.Count(cs => cs.IsLibrary);
            var posLib = allUseCasesRule3.Count(cs => cs.IsPossibleLibrary);
            var apps = allUseCasesRule3.Count(cs => !cs.IsPackageNameFullyObfuscated && !cs.IsLibrary && !cs.IsPossibleLibrary);
            var full = allUseCasesRule3.Count(cs => cs.IsPackageNameFullyObfuscated);
            Console.WriteLine($"{dsName}\t{allUseCasesRule3.Count}\t{lib}\t{posLib}\t{apps}\t{full}");

            Console.WriteLine(ComputeResultDistributionbyAppsLibs(
                        $"{dsName} - Rule 2 (Static Key)",
                        rule3Results,
                        allUseCasesRule3));

            Console.WriteLine();

        }

        [Test]
        public void ComputeViolationOfRule4()
        {
            ComputeViolationOfRule4ForDataSet(@"ccs13-11k.report.csv", "r12.rule4.csv", "R12", true);
            ComputeViolationOfRule4ForDataSet(@"sophos150K.report.csv", "r16.rule4.csv", "R16*", true);
            ComputeViolationOfRule4ForDataSet(@"sophos150K.report.csv", "r16.rule4.csv", "R16", false);
            ComputeViolationOfRule4ForDataSet(@"top100.report.csv", "t15.rule4.csv", "T15", false);
        }

        private void ComputeViolationOfRule4ForDataSet(string useCaseReport, string rulefn, string dsName, bool applyCryptoLintWhiteListing)
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), useCaseReport)));
            var uniqueAppsIds = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
            var allUseCases = uniqueApps.Where(a => uniqueAppsIds.Contains(a.Id)).SelectMany(a => a.UseCases).ToList();

            var ruleFfn = Path.Combine(_dataPath, rulefn);

            var rule4Results = Reader.GetUseCaseResultsForRule4To6(ruleFfn).Where(uc => !uc.IsAsymmetric && uc.Rule4StaticSalt && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule4 = allUseCases.Where(uc => uc.IsCipherUseCaseRule4And5)
                .Where(uc => !applyCryptoLintWhiteListing || !DevScriptsTests.IsCCS13ExcludedLibrary(uc))
                .ToList();

            var lib = allUseCasesRule4.Count(cs => cs.IsLibrary);
            var posLib = allUseCasesRule4.Count(cs => cs.IsPossibleLibrary);
            var apps = allUseCasesRule4.Count(cs => !cs.IsPackageNameFullyObfuscated && !cs.IsLibrary && !cs.IsPossibleLibrary);
            var full = allUseCasesRule4.Count(cs => cs.IsPackageNameFullyObfuscated);
            Console.WriteLine($"{dsName}\t{allUseCasesRule4.Count}\t{lib}\t{posLib}\t{apps}\t{full}");

            Console.WriteLine(ComputeResultDistributionbyAppsLibs(
                        $"{dsName} - Rule 4 (Static Salt)",
                        rule4Results,
                        allUseCasesRule4));

            Console.WriteLine();

        }

        [Test]
        public void ComputeViolationOfRule5()
        {
            ComputeViolationOfRule5ForDataSet(@"ccs13-11k.report.csv", "r12.rule5.csv", "R12", true);
            ComputeViolationOfRule5ForDataSet(@"sophos150K.report.csv", "r16.rule5.csv", "R16", true);
            ComputeViolationOfRule5ForDataSet(@"top100.report.csv", "t15.rule5.csv", "T15", false);
        }

        private void ComputeViolationOfRule5ForDataSet(string useCaseReport, string rulefn, string dsName, bool applyCryptoLintWhiteListing)
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), useCaseReport)));
            var uniqueAppsIds = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
            var allUseCases = uniqueApps.Where(a => uniqueAppsIds.Contains(a.Id)).SelectMany(a => a.UseCases).ToList();

            var ruleFfn = Path.Combine(_dataPath, rulefn);

            var rule5Results = Reader.GetUseCaseResultsForRule4To6(ruleFfn).Where(uc => !uc.IsAsymmetric && uc.Rule5LessThan1000Iterations && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule5 = allUseCases.Where(uc => uc.IsCipherUseCaseRule4And5)
                .Where(uc => !applyCryptoLintWhiteListing || !DevScriptsTests.IsCCS13ExcludedLibrary(uc))
                .ToList();

            var lib = allUseCasesRule5.Count(cs => cs.IsLibrary);
            var posLib = allUseCasesRule5.Count(cs => cs.IsPossibleLibrary);
            var apps = allUseCasesRule5.Count(cs => !cs.IsPackageNameFullyObfuscated && !cs.IsLibrary && !cs.IsPossibleLibrary);
            var full = allUseCasesRule5.Count(cs => cs.IsPackageNameFullyObfuscated);
            Console.WriteLine($"{dsName}\t{allUseCasesRule5.Count}\t{lib}\t{posLib}\t{apps}\t{full}");

            Console.WriteLine(ComputeResultDistributionbyAppsLibs(
                        $"{dsName} - Rule 5 (Less than 1,000 iterations)",
                        rule5Results,
                        allUseCasesRule5));

            Console.WriteLine();

        }

        [Test]
        public void ComputeViolationOfRule6()
        {
            ComputeViolationOfRule6ForDataSet(@"ccs13-11k.report.csv", "r12.rule6.csv", "R12", true);
            ComputeViolationOfRule6ForDataSet(@"sophos150K.report.csv", "r16.rule6.csv", "R16", true);
            ComputeViolationOfRule6ForDataSet(@"top100.report.csv", "t15.rule6.csv", "T15", false);
        }

        private void ComputeViolationOfRule6ForDataSet(string useCaseReport, string rulefn, string dsName, bool applyCryptoLintWhiteListing)
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            // Data preparation
            var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), useCaseReport)));
            var uniqueAppsIds = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
            var allUseCases = uniqueApps.Where(a => uniqueAppsIds.Contains(a.Id)).SelectMany(a => a.UseCases).ToList();

            var ruleFfn = Path.Combine(_dataPath, rulefn);

            var rule6Results = Reader.GetUseCaseResultsForRule4To6(ruleFfn).Where(uc => !uc.IsAsymmetric && uc.Rule6StaticSeed && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule6 = allUseCases.Where(uc => uc.IsCipherUseCaseRule6)
                .Where(uc => !applyCryptoLintWhiteListing || !DevScriptsTests.IsCCS13ExcludedLibrary(uc))
                .ToList();

            var lib = allUseCasesRule6.Count(cs => cs.IsLibrary);
            var posLib = allUseCasesRule6.Count(cs => cs.IsPossibleLibrary);
            var apps = allUseCasesRule6.Count(cs => !cs.IsPackageNameFullyObfuscated && !cs.IsLibrary && !cs.IsPossibleLibrary);
            var full = allUseCasesRule6.Count(cs => cs.IsPackageNameFullyObfuscated);
            Console.WriteLine($"{dsName}\t{allUseCasesRule6.Count}\t{lib}\t{posLib}\t{apps}\t{full}");

            Console.WriteLine(ComputeResultDistributionbyAppsLibs(
                        $"{dsName} - Rule 6 (Static Seed)",
                        rule6Results,
                        allUseCasesRule6));

            Console.WriteLine();

        }

        private string ComputeResultDistributionbyAppsLibs(string name, List<UseCaseResult> results, List<UseCase> allUseCases)
        {
            var ruleResults_InLibraries = allUseCases
                .Where(uc =>
                    !uc.IsPackageNameFullyObfuscated &&
                    (uc.IsLibrary) &&
                    results.Any(
                        ucr =>
                            ucr.Id == uc.ApplicationInfo.Id &&
                            uc.Filename == ucr.Filename &&
                            (uc.InClassPos == ucr.InFileLoc || uc.InMethodPos == ucr.InFileLoc))
                ).ToList();

            var ruleResults_InPossibleLibraries = allUseCases
                .Where(uc =>
                    !uc.IsPackageNameFullyObfuscated &&
                    (uc.IsPossibleLibrary) &&
                    results.Any(
                        ucr =>
                            ucr.Id == uc.ApplicationInfo.Id &&
                            uc.Filename == ucr.Filename &&
                            (uc.InClassPos == ucr.InFileLoc || uc.InMethodPos == ucr.InFileLoc))
                ).ToList();

            var ruleResults_InApps = allUseCases
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
            var ruleResults_InObfus = allUseCases
                .Where(uc =>
                    uc.IsPackageNameFullyObfuscated &&
                    results.Any(
                        ucr =>
                            ucr.Id == uc.ApplicationInfo.Id &&
                            uc.Filename == ucr.Filename &&
                            (uc.InClassPos == ucr.InFileLoc || uc.InMethodPos == ucr.InFileLoc))
                ).ToList();

            return $"{name}\t\t{ruleResults_InLibraries.Count}\t{ruleResults_InPossibleLibraries.Count}\t{ruleResults_InApps.Count}\t{ruleResults_InObfus.Count}";
        }

        #endregion


        #region Compute libraries frequencies and number of apps that are impacted by libs' misuse

        /// <summary>
        /// This script asnwers the following research question.
        /// What is the frequency of use for libraries that misuse crypto?
        /// </summary>
        [Test]
        public void ComputeFrequencyOfLibrariesInAppsThatHaveMisuse()
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));

            ComputeFrequencyOfLibrariesInAppsThatHaveMisuseForDataSet(
                Path.Combine(_dataPath, @"stage1\ccs13-11k.report.csv"),
                Path.Combine(_dataPath, "r12.rule{0}.csv"),
                @"e:\",
                "R12");
            ComputeFrequencyOfLibrariesInAppsThatHaveMisuseForDataSet(
                Path.Combine(_dataPath, @"stage1\top100.report.csv"),
                Path.Combine(_dataPath, "t15.rule{0}.csv"),
                @"e:\",
                "T15");
            ComputeFrequencyOfLibrariesInAppsThatHaveMisuseForDataSet(
                Path.Combine(_dataPath, @"stage1\sophos150K.report.csv"),
                Path.Combine(_dataPath, "r16.rule{0}.csv"),
                @"e:\",
                "R16");
        }

        private void ComputeFrequencyOfLibrariesInAppsThatHaveMisuseForDataSet(
                    string fullPath,
                    string resultTemplateFullPath,
                    string reportPath,
                    string datasetName)
        {
            var apksInfo = Reader.GetApkinfo(fullPath);
            var uniqueApksInfo = DevScriptsTests.GetUniqueApkInfo(apksInfo);
            var uniqueAppsIds = new HashSet<int>(uniqueApksInfo.Select(apk => apk.Id).ToArray());
            var allCallSites = uniqueApksInfo.SelectMany(app => app.UseCases).Where(cs => cs.IsCipherUseCaseRule1to6).ToList();

            // This variable holds dictionary of library of appIds that it is used in
            var appsWithExplicitMisuse = new HashSet<int>();
            var libsWithMisuseInApps = new Dictionary<string, HashSet<int>>();
            var allLibsInApps = new Dictionary<string, HashSet<int>>();

            var rule1Results = Reader.GetUseCaseResultsForRule1(string.Format(resultTemplateFullPath, 1)).Where(uc => !uc.IsAsymmetric && !uc.IsModeNotFound && uniqueAppsIds.Contains(uc.Id)).ToList();
            var rule1ResultsEcb = rule1Results.Where(uc => DevScriptsTests.IsImplicitECBMode(uc.Result) || DevScriptsTests.IsExplicitECBMode(uc.Result)).ToList();
            var allCallSitesForTheRule1 = allCallSites.Where(uc => uc.IsCipherUseCaseRule1).ToList();

            CountLibraryFrequency(rule1ResultsEcb, allCallSitesForTheRule1, ref libsWithMisuseInApps, ref appsWithExplicitMisuse);
            CountLibraryFrequency(allCallSitesForTheRule1, ref allLibsInApps);
            var tmpMisuse = libsWithMisuseInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();
            var tmpAll = allLibsInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();

            var rule2ResultsAll = Reader.GetUseCaseResultsForRule2(string.Format(resultTemplateFullPath, 2)).Where(uc => uc.Rule2StaticIV && uniqueAppsIds.Contains(uc.Id)).ToList();
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

            var rule1CbcModeCasesResults = rule1Results.Where(uc => DevScriptsTests.IsCBCMode(uc.Result)).ToList();
            var allUseCasesRule2 = allCallSites
                .Where(uc =>
                    uc.IsCipherUseCaseRule2 &&
                    rule1CbcModeCasesResults.Any(cbc =>
                        cbc.Id == uc.ApplicationInfo.Id &&
                        cbc.Filename == uc.Filename))
                .ToList();

            CountLibraryFrequency(rule2Results, allUseCasesRule2, ref libsWithMisuseInApps, ref appsWithExplicitMisuse);
            CountLibraryFrequency(allUseCasesRule2, ref allLibsInApps);
            tmpMisuse = libsWithMisuseInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();
            tmpAll = allLibsInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();

            var rule3Results = Reader.GetUseCaseResultsForRule3(string.Format(resultTemplateFullPath, 3)).Where(uc => !uc.IsAsymmetric && uc.Rule3StaticKey && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule3 = allCallSites.Where(uc => uc.IsCipherUseCaseRule3).ToList();

            CountLibraryFrequency(rule3Results, allUseCasesRule3, ref libsWithMisuseInApps, ref appsWithExplicitMisuse);
            CountLibraryFrequency(allUseCasesRule3, ref allLibsInApps);
            tmpMisuse = libsWithMisuseInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();
            tmpAll = allLibsInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();

            var rule4Results = Reader.GetUseCaseResultsForRule4To6(string.Format(resultTemplateFullPath, 4)).Where(uc => !uc.IsAsymmetric && uc.Rule4StaticSalt && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule4 = allCallSites.Where(uc => uc.IsCipherUseCaseRule4And5).ToList();
            CountLibraryFrequency(rule4Results, allUseCasesRule4, ref libsWithMisuseInApps, ref appsWithExplicitMisuse);
            CountLibraryFrequency(allUseCasesRule4, ref allLibsInApps);
            tmpMisuse = libsWithMisuseInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();
            tmpAll = allLibsInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();

            var rule5Results = Reader.GetUseCaseResultsForRule4To6(string.Format(resultTemplateFullPath, 5)).Where(uc => !uc.IsAsymmetric && uc.Rule5LessThan1000Iterations && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule5 = allCallSites.Where(uc => uc.IsCipherUseCaseRule4And5).ToList();
            CountLibraryFrequency(rule5Results, allUseCasesRule5, ref libsWithMisuseInApps, ref appsWithExplicitMisuse);
            CountLibraryFrequency(allUseCasesRule5, ref allLibsInApps);
            tmpMisuse = libsWithMisuseInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();
            tmpAll = allLibsInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();

            var rule6Results = Reader.GetUseCaseResultsForRule4To6(string.Format(resultTemplateFullPath, 6)).Where(uc => !uc.IsAsymmetric && uc.Rule6StaticSeed && uniqueAppsIds.Contains(uc.Id)).ToList();
            var allUseCasesRule6 = allCallSites.Where(uc => uc.IsCipherUseCaseRule6).ToList();
            CountLibraryFrequency(rule6Results, allUseCasesRule6, ref libsWithMisuseInApps, ref appsWithExplicitMisuse);
            CountLibraryFrequency(allUseCasesRule6, ref allLibsInApps);
            tmpMisuse = libsWithMisuseInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();
            tmpAll = allLibsInApps.OrderByDescending(kv => kv.Value.Count).Select(kv => new { Name = kv.Key, kv.Value.Count }).ToList();

            var orderedMisuseViaLibraries = libsWithMisuseInApps.OrderByDescending(kv => kv.Value.Count).ToList();

            // Get all unique applications that has misuse
            var appsWithImplicitMisuse = new HashSet<int>(orderedMisuseViaLibraries.SelectMany(mu => mu.Value.ToArray()));
            var t = appsWithImplicitMisuse.ToList();
            t.AddRange(appsWithExplicitMisuse);

            // Get all unique apps with any kind of misuse
            var appsWithAllMisuses = new HashSet<int>(t);
            Console.WriteLine($"Dataset: {datasetName}");
            Console.WriteLine($"Total Libs: {tmpAll.Count}, with misuse: {tmpMisuse.Count} ({tmpMisuse.Count * 100.0 / tmpAll.Count})");
            Console.WriteLine($"Total with misuse: {appsWithAllMisuses.Count}, with explicit: {appsWithExplicitMisuse.Count}, with implicit {appsWithImplicitMisuse.Count}");

            // Frequency of libs data
            var freq = new List<string>();
            freq.Add($"libRank,libPrefix,usedInApps,wouldFixApps,cdfOfFixedApps,{appsWithAllMisuses.Count}");
            int i = 1;
            int totalFixedSoFar = 0;
            foreach (var libWithMisuse in orderedMisuseViaLibraries)
            {
                var subRange = orderedMisuseViaLibraries.Skip(i).ToList();
                var appsWithImplicitMisuseAfterFixingNLibs = new HashSet<int>(subRange.SelectMany(mu => mu.Value.ToArray()));
                var allAppsAfterFix = appsWithExplicitMisuse.ToList();
                allAppsAfterFix.AddRange(appsWithImplicitMisuseAfterFixingNLibs);
                var appsNotFixed = new HashSet<int>(allAppsAfterFix);
                freq.Add($"{i},{libWithMisuse.Key},{libWithMisuse.Value.Count}, {appsWithAllMisuses.Count - appsNotFixed.Count - totalFixedSoFar},{appsWithAllMisuses.Count - appsNotFixed.Count}");
                totalFixedSoFar = appsWithAllMisuses.Count - appsNotFixed.Count;
                i++;
            }
            File.WriteAllLines(Path.Combine(reportPath, $"{datasetName}-libraries-misuse.csv"), freq);
        }

        private void CountLibraryFrequency(
                    List<UseCaseResult> ruleResults,
                    List<UseCase> allCallSitesForTheRule,
                    ref Dictionary<string, HashSet<int>> libsWithMisuseInApps,
                    ref HashSet<int> appsWithExplicitMisuse)
        {
            foreach (var useCaseResult in ruleResults)
            {
                var callSite = allCallSitesForTheRule
                    .FirstOrDefault(
                        cs => useCaseResult.Id == cs.ApplicationInfo.Id &&
                            useCaseResult.Filename == cs.Filename &&
                            (useCaseResult.InFileLoc == cs.InClassPos || useCaseResult.InFileLoc == cs.InMethodPos));

                if (callSite != null)
                {
                    if (callSite.IsLibrary)
                    {
                        var libName = callSite.PackageName;
                        if (!libsWithMisuseInApps.ContainsKey(libName))
                        {
                            libsWithMisuseInApps.Add(libName, new HashSet<int>());
                        }
                        libsWithMisuseInApps[libName].Add(callSite.ApplicationInfo.Id);
                    }
                    else
                    {
                        appsWithExplicitMisuse.Add(callSite.ApplicationInfo.Id);
                    }
                }
            }
        }

        private void CountLibraryFrequency(
            List<UseCase> allCallSitesForTheRule,
            ref Dictionary<string, HashSet<int>> libsInApps)
        {
            foreach (var callSite in allCallSitesForTheRule)
            {
                if (callSite.IsLibrary)
                {
                    var libName = callSite.PackageName;
                    if (!libsInApps.ContainsKey(libName))
                    {
                        libsInApps.Add(libName, new HashSet<int>());
                    }
                    libsInApps[libName].Add(callSite.ApplicationInfo.Id);
                }
            }
        }

        #endregion
    }

}
