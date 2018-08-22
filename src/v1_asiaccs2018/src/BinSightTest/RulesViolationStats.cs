using APKInsight.Logic.Analysis.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinSightTest
{
    public class RulesViolationStats
    {
        private static string _dataPath = @"E:\eurosp2018\final_data";
        private static string _dataPathForLibInfo = @"S:\binsightdata\data\libraries";

        private bool _loadR12 = true;
        private bool _loadR16 = true;
        private bool _loadT15 = true;

        private HashSet<int> _uniqueAppsIdsR12;
        private HashSet<int> _uniqueAppsIdsR16;
        private HashSet<int> _uniqueAppsIdsT15;

        private List<UseCase> _sitesR12;
        private List<UseCase> _sitesR16;
        private List<UseCase> _sitesT15;

        private List<List<UseCase>> _sitesR12perRule;
        private List<List<UseCase>> _sitesR16perRule;
        private List<List<UseCase>> _sitesT15perRule;

        private List<UseCase> _sitesR12_cryptolintWL;
        private List<UseCase> _sitesR16_cryptolintWL;
        private List<UseCase> _sitesT15_cryptolintWL;

        private List<List<UseCase>> _sitesR12perRule_cryptolintWL;
        private List<List<UseCase>> _sitesR16perRule_cryptolintWL;
        private List<List<UseCase>> _sitesT15perRule_cryptolintWL;

        private List<List<UseCaseResult>> _resultsR12;
        private List<List<UseCaseResult>> _resultsR16;
        private List<List<UseCaseResult>> _resultsT15;

        private List<List<UseCaseResult>> _resultsR12WithViolation;
        private List<List<UseCaseResult>> _resultsR16WithViolation;
        private List<List<UseCaseResult>> _resultsT15WithViolation;

        public RulesViolationStats()
        {

        }

        #region Reading all results in

        /// <summary>
        /// Loads libraries definitions for source attribution
        /// </summary>
        private void LoadLibrariesDefinitions()
        {
            UseCase.Libraries = Reader.LoadLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "libraries.csv"));
            UseCase.PossibleLibraries = Reader.LoadPossibleLibraryDefinitions(Path.Combine(_dataPathForLibInfo, "possible_libraries.csv"));
        }

        /// <summary>
        /// Load all identified call-sites
        /// </summary>
        private void ReadAllCallSitesData()
        {
            // R12
            if (_sitesR12 == null && _loadR12)
            {
                var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), "ccs13-11k.report.csv")));
                _uniqueAppsIdsR12 = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
                _sitesR12 = uniqueApps
                        .Where(a => _uniqueAppsIdsR12.Contains(a.Id))
                        .SelectMany(a => a.UseCases)
                        .ToList();
                _sitesR12_cryptolintWL = _sitesR12.Where(uc => !DevScriptsTests.IsCCS13ExcludedLibrary(uc)).ToList();
                _sitesR12perRule = new List<List<UseCase>>();
                _sitesR12perRule_cryptolintWL = new List<List<UseCase>>();
            }

            // R16
            if (_sitesR16 == null && _loadR16)
            {
                var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), "sophos150K.report.csv")));
                _uniqueAppsIdsR16 = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
                _sitesR16 = uniqueApps
                        .Where(a => _uniqueAppsIdsR16.Contains(a.Id))
                        .SelectMany(a => a.UseCases)
                        .ToList();
                _sitesR16_cryptolintWL = _sitesR16.Where(uc => !DevScriptsTests.IsCCS13ExcludedLibrary(uc)).ToList();
                _sitesR16perRule = new List<List<UseCase>>();
                _sitesR16perRule_cryptolintWL = new List<List<UseCase>>();
            }

            // T15
            if (_sitesT15 == null && _loadT15)
            {
                var uniqueApps = DevScriptsTests.GetUniqueApkInfo(Reader.GetApkinfo(Path.Combine(Path.Combine(_dataPath, "stage1"), "top100.report.csv")));
                _uniqueAppsIdsT15 = new HashSet<int>(uniqueApps.Select(apk => apk.Id).ToArray());
                _sitesT15 = uniqueApps
                        .Where(a => _uniqueAppsIdsT15.Contains(a.Id))
                        .SelectMany(a => a.UseCases)
                        .ToList();
                _sitesT15_cryptolintWL = _sitesT15.Where(uc => !DevScriptsTests.IsCCS13ExcludedLibrary(uc)).ToList();
                _sitesT15perRule = new List<List<UseCase>>();
                _sitesT15perRule_cryptolintWL = new List<List<UseCase>>();
            }

        }

        /// <summary>
        /// Load all results of the analysis for each of the dataset for each rule
        /// </summary>
        private void ReadAllResultsData()
        {
            if (_resultsR12 == null && _loadR12)
            {
                _resultsR12 = ReadResultData("r12.rule{0}.csv");
                _resultsR12WithViolation = SelectResultsWithViolations(_sitesR12, _resultsR12, _uniqueAppsIdsR12, ref _sitesR12perRule, ref _sitesR12perRule_cryptolintWL);
            }
            if (_resultsR16 == null && _loadR16)
            {
                _resultsR16 = ReadResultData("r16.rule{0}.csv");
                _resultsR16WithViolation = SelectResultsWithViolations(_sitesR16, _resultsR16, _uniqueAppsIdsR16, ref _sitesR16perRule, ref _sitesR16perRule_cryptolintWL);
            }
            if (_resultsT15 == null && _loadT15)
            {
                _resultsT15 = ReadResultData("t15.rule{0}.csv");
                _resultsT15WithViolation = SelectResultsWithViolations(_sitesT15, _resultsT15, _uniqueAppsIdsT15, ref _sitesT15perRule, ref _sitesT15perRule_cryptolintWL);
            }
        }

        private List<List<UseCaseResult>> SelectResultsWithViolations(
                    List<UseCase> allUseCases,
                    List<List<UseCaseResult>> allResults,
                    HashSet<int> uniqueAppsIds,
                    ref List<List<UseCase>> allUseCasesPerRule,
                    ref List<List<UseCase>> allUseCasesPerRule_cryptolintWL)
        {
            var result = new List<List<UseCaseResult>>();

            // Rule 1 - ECB Mode
            var resultsForRule1 = allResults[0].Where(
                uc =>
                    !uc.IsAsymmetric &&
                    !uc.IsModeNotFound &&
                    uniqueAppsIds.Contains(uc.Id))
                .ToList();
            result.Add(resultsForRule1.Where(ucr => DevScriptsTests.IsExplicitECBMode(ucr.Result) || DevScriptsTests.IsImplicitECBMode(ucr.Result)).ToList());
            allUseCasesPerRule.Add(allUseCases.Where(uc => uc.IsCipherUseCaseRule1).ToList());

            // Rule 2 - Static IV with CBC mode
            var rule2ResultsAll = allResults[1].Where(
                uc =>
                    uc.Rule2StaticIV &&
                    uniqueAppsIds.Contains(uc.Id))
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

            result.Add(rule2Results);

            // Rule 2 is special in that we only check static IV for CBC modes, hence we need results from rule 1 validation.
            var rule1CbcModeCasesResults = resultsForRule1.Where(uc => DevScriptsTests.IsCBCMode(uc.Result)).ToList();
            allUseCasesPerRule.Add(allUseCases
                .Where(uc =>
                    uc.IsCipherUseCaseRule2 &&
                    rule1CbcModeCasesResults.Any(cbc =>
                        cbc.Id == uc.ApplicationInfo.Id &&
                        cbc.Filename == uc.Filename))
                .ToList());


            // Rule 3 - Static Key
            result.Add(allResults[2].Where(
                uc =>
                    !uc.IsAsymmetric &&
                    uc.Rule3StaticKey &&
                    uniqueAppsIds.Contains(uc.Id))
                .ToList());
            allUseCasesPerRule.Add(allUseCases.Where(uc => uc.IsCipherUseCaseRule3).ToList());

            // Rule 4 - Static Salt
            result.Add(allResults[3].Where(
                uc =>
                    !uc.IsAsymmetric &&
                    uc.Rule4StaticSalt &&
                    uniqueAppsIds.Contains(uc.Id))
                .ToList());
            allUseCasesPerRule.Add(allUseCases.Where(uc => uc.IsCipherUseCaseRule4And5).ToList());

            // Rule 5 - Static Salt
            result.Add(allResults[4].Where(
                uc =>
                    !uc.IsAsymmetric &&
                    uc.Rule5LessThan1000Iterations &&
                    uniqueAppsIds.Contains(uc.Id))
                .ToList());
            allUseCasesPerRule.Add(allUseCases.Where(uc => uc.IsCipherUseCaseRule4And5).ToList());

            // Rule 6 - Static Seed for SecureRandom
            result.Add(allResults[5].Where(
                uc =>
                    !uc.IsAsymmetric &&
                    uc.Rule6StaticSeed &&
                    uniqueAppsIds.Contains(uc.Id))
                .ToList());
            allUseCasesPerRule.Add(allUseCases.Where(uc => uc.IsCipherUseCaseRule6).ToList());

            // Add filtered with CryptoLint rule use cases
            foreach (var allUseCasesForRule in allUseCasesPerRule)
            {
                allUseCasesPerRule_cryptolintWL
                    .Add(allUseCasesForRule.Where(uc => !DevScriptsTests.IsCCS13ExcludedLibrary(uc)).ToList());

            }

            return result;
        }

        /// <summary>
        /// Load results for a dataset for all rules.
        /// </summary>
        /// <param name="resultsFilenameTemplate">Template of the filename with results for all rules</param>
        /// <returns>Returns a list of lists with results</returns>
        private List<List<UseCaseResult>> ReadResultData(string resultsFilenameTemplate)
        {
            var result = new List<List<UseCaseResult>>();

            result = new List<List<UseCaseResult>>();
            var rule1Filename = Path.Combine(_dataPath, string.Format(resultsFilenameTemplate, "1"));
            result.Add(Reader.GetUseCaseResultsForRule1(rule1Filename));

            var rule2Filename = Path.Combine(_dataPath, string.Format(resultsFilenameTemplate, "2"));
            result.Add(Reader.GetUseCaseResultsForRule2(rule2Filename));

            var rule3Filename = Path.Combine(_dataPath, string.Format(resultsFilenameTemplate, "3"));
            result.Add(Reader.GetUseCaseResultsForRule3(rule3Filename));

            var rule4Filename = Path.Combine(_dataPath, string.Format(resultsFilenameTemplate, "4"));
            result.Add(Reader.GetUseCaseResultsForRule4To6(rule4Filename));

            var rule5Filename = Path.Combine(_dataPath, string.Format(resultsFilenameTemplate, "5"));
            result.Add(Reader.GetUseCaseResultsForRule4To6(rule5Filename));

            var rule6Filename = Path.Combine(_dataPath, string.Format(resultsFilenameTemplate, "6"));
            result.Add(Reader.GetUseCaseResultsForRule4To6(rule6Filename));

            return result;
        }

        #endregion

        #region All rules combined violation computation logic

        [Test]
        public string ComputeViolationOfAllRules()
        {
            LoadLibrariesDefinitions();
            ReadAllCallSitesData();
            ReadAllResultsData();

            string res = "";
            if (_loadR12)
            {
                res += ComputeViolationOfAllRuleForDataSet("R12*", _sitesR12_cryptolintWL, _resultsR12WithViolation);
                res += ComputeViolationOfAllRuleForDataSet("R12", _sitesR12, _resultsR12WithViolation);
            }
            if (_loadR16)
            {
                res += ComputeViolationOfAllRuleForDataSet("R16*", _sitesR16_cryptolintWL, _resultsR16WithViolation);
                res += ComputeViolationOfAllRuleForDataSet("R16", _sitesR16, _resultsR16WithViolation);
            }
            if (_loadT15)
            {
                res += ComputeViolationOfAllRuleForDataSet("T15", _sitesT15, _resultsT15WithViolation);
            }

            res += "Per Rule Analysis" + Environment.NewLine;
            res += ComputeViolationOfAllRuleSeparately();

            return res;
        }

        private string ComputeViolationOfAllRuleForDataSet(
                    string dsName,
                    List<UseCase> allUseCasesCombined,
                    List<List<UseCaseResult>> allUseCaseResultsWithViolations)
        {

            // Select all use case results that belong to our unique application set
            var results = allUseCaseResultsWithViolations[0]
                .Union(allUseCaseResultsWithViolations[1])
                .Union(allUseCaseResultsWithViolations[2])
                .Union(allUseCaseResultsWithViolations[3])
                .Union(allUseCaseResultsWithViolations[4])
                .Union(allUseCaseResultsWithViolations[5])
                .ToList();

            return
                ComputeResultDistributionbyAppsLibs(
                        $"{dsName} - All",
                        results,
                        allUseCasesCombined) + Environment.NewLine;

        }

        #endregion

        #region Rule 1 - 3 violation computation logic

        public string ComputeViolationOfAllRuleSeparately()
        {
            var str = "";

            str += ComputeViolationOfSingleRule("Rule 1 (ECB)", 0);
            //str += ComputeViolationOfSingleRule("Rule 2 (Static IV)", 1);
            //str += ComputeViolationOfSingleRule("Rule 3 (Static Key)", 2);
            //str += ComputeViolationOfSingleRule("Rule 4 (Static Salt)", 3);
            //str += ComputeViolationOfSingleRule("Rule 5 (1000 iterations)", 4);
            //str += ComputeViolationOfSingleRule("Rule 6 (Static Seed", 5);

            return str;
        }

        private string ComputeViolationOfSingleRule(string rule, int idx)
        {
            return Environment.NewLine +
                ComputeResultDistributionbyAppsLibs($"R12* - {rule}", _resultsR12WithViolation[idx], _sitesR12perRule_cryptolintWL[idx]) +
                ComputeResultDistributionbyAppsLibs($"R12 - {rule}", _resultsR12WithViolation[idx], _sitesR12perRule[idx]) +
                ComputeResultDistributionbyAppsLibs($"R16* - {rule}", _resultsR16WithViolation[idx], _sitesR16perRule_cryptolintWL[idx]) +
                ComputeResultDistributionbyAppsLibs($"R16 - {rule}", _resultsR16WithViolation[idx], _sitesR16perRule[idx]) +
                ComputeResultDistributionbyAppsLibs($"T15 - {rule}", _resultsT15WithViolation[idx], _sitesT15perRule[idx]) +
                Environment.NewLine;
        }

        #endregion


        #region Helper functions that compute averages

        /// <summary>
        /// Computes call-sites and APKs violations by origin category.
        /// </summary>
        /// <param name="name">Name of the dataset</param>
        /// <param name="results">Results with violation</param>
        /// <param name="allUseCases">All use cases for the rule validation</param>
        /// <returns>A string that provides data for CS and APK</returns>
        private string ComputeResultDistributionbyAppsLibs(string name, List<UseCaseResult> results, List<UseCase> allUseCases)
        {
            // Call-site analysis
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

            var ruleResults_InObfus = allUseCases
                .Where(uc =>
                    uc.IsPackageNameFullyObfuscated &&
                    results.Any(
                        ucr =>
                            ucr.Id == uc.ApplicationInfo.Id &&
                            uc.Filename == ucr.Filename &&
                            (uc.InClassPos == ucr.InFileLoc || uc.InMethodPos == ucr.InFileLoc))
                ).ToList();
            // Calculating the total number of call-sites with violations is easy, just a simple sum will work.
            var totalViolationInCalls = ruleResults_InApps.Count + ruleResults_InLibraries.Count + ruleResults_InObfus.Count + ruleResults_InPossibleLibraries.Count;

            // APK file number analysis with violations
            var ruleResults_InAppsApks = new HashSet<string>(ruleResults_InApps.Select(r => r.ApplicationInfo.ApplicationId));
            var ruleResults_InLibrariesApks = new HashSet<string>(ruleResults_InLibraries.Select(r => r.ApplicationInfo.ApplicationId));
            var ruleResults_InPossibleLibrariesApks = new HashSet<string>(ruleResults_InPossibleLibraries.Select(r => r.ApplicationInfo.ApplicationId));
            var ruleResults_InObfusApks = new HashSet<string>(ruleResults_InObfus.Select(r => r.ApplicationInfo.ApplicationId));
            var ruleAllTypesResults = new HashSet<string>(
                ruleResults_InAppsApks.ToList()
                .Union(ruleResults_InLibrariesApks)
                .Union(ruleResults_InPossibleLibrariesApks)
                .Union(ruleResults_InObfusApks));

            // Totals analysis (without violations)
            // Call-sites
            var apps_cs = allUseCases.Where(cs => !cs.IsPackageNameFullyObfuscated && !cs.IsLibrary && !cs.IsPossibleLibrary);
            var lib_cs = allUseCases.Where(cs => cs.IsLibrary);
            var posLib_cs = allUseCases.Where(cs => cs.IsPossibleLibrary);
            var full_cs = allUseCases.Where(cs => cs.IsPackageNameFullyObfuscated);

            // Android Apps
            var allApps = new HashSet<string>(allUseCases.Select(r => r.ApplicationInfo.ApplicationId));
            var apps_apk = new HashSet<string>(apps_cs.Select(r => r.ApplicationInfo.ApplicationId));
            var lib_apk = new HashSet<string>(lib_cs.Select(r => r.ApplicationInfo.ApplicationId));
            var posLib_apk = new HashSet<string>(posLib_cs.Select(r => r.ApplicationInfo.ApplicationId));
            var full_apk = new HashSet<string>(full_cs.Select(r => r.ApplicationInfo.ApplicationId));

            return
                $"{name} - APK All\t{allApps.Count}\t{lib_apk.Count()}\t{posLib_apk.Count()}\t{apps_apk.Count()}\t{full_apk.Count()}" + Environment.NewLine +
                $"{name} - APK V\t{ruleAllTypesResults.Count()}\t{ruleResults_InLibrariesApks.Count}\t{ruleResults_InPossibleLibrariesApks.Count}\t{ruleResults_InAppsApks.Count}\t{ruleResults_InObfusApks.Count}" + Environment.NewLine +

                $"{name} - CS All\t{allUseCases.Count}\t{lib_cs.Count()}\t{posLib_cs.Count()}\t{apps_cs.Count()}\t{full_cs.Count()}" + Environment.NewLine +
                $"{name} - CS V\t{totalViolationInCalls}\t{ruleResults_InLibraries.Count}\t{ruleResults_InPossibleLibraries.Count}\t{ruleResults_InApps.Count}\t{ruleResults_InObfus.Count}" + Environment.NewLine;

        }

        #endregion

    }
}
