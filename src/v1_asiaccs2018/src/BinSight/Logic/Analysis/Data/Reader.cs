using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace APKInsight.Logic.Analysis.Data
{

    /// <summary>
    /// Reader of CSV files into structure
    /// </summary>
    public static class Reader
    {
        /// <summary>
        /// Reads info into collection of classes that describe use cases.
        /// </summary>
        /// <param name="filename">Filename from where we read all the data</param>
        /// <returns>Collection with use cases definitions</returns>
        public static List<ApkInfo> GetApkinfo(string filename)
        {
            var result = new List<ApkInfo>();

            // Read all the content in as array of lines
            var content = File.ReadAllLines(filename);
            ApkInfo currentApkInfo = null;
            for (int i = 2; i < content.Length; i++)
            {
                if (content[i].Length == 0)
                    continue;
                var cells = content[i].Split(',');
                try
                {

                    if (cells[0].Length > 0)
                    {
                        // Apk def line
                        currentApkInfo = new ApkInfo
                        {
                            Id = int.Parse(cells[0]),
                            ApplicationId = cells[1],
                            Sha1 = cells[2],
                            Filename = cells[3]
                        };
                        result.Add(currentApkInfo);
                    }
                    else
                    {
                        var useCase = new UseCase
                        {
                            SmaliClassName = cells[3],
                            SmaliMethodName = cells[4],
                            InClassPos = int.Parse(cells[5]),
                            InMethodPos = int.Parse(cells[6]),
                            ApiSig = cells[7],
                            Filename = cells[8],
                            ApplicationInfo = currentApkInfo
                        };
                        var packageNameMatc = Regex.Match(useCase.SmaliClassName, @"^L(.)+(?=/[^/]+$)");
                        if (packageNameMatc.Success)
                        {
                            useCase.PackageName = packageNameMatc.Value.Substring(1).Replace("/", ".");
                            var match = Regex.Match(useCase.SmaliClassName, @"(?<=^L(.)+?/)[^/]+(?=;$)");
                            useCase.ClassName = match.Value;
                        }
                        else
                        {
                            useCase.PackageName = "";
                            useCase.ClassName = useCase.SmaliClassName.Substring(1).TrimEnd(';');
                        }
                        currentApkInfo.UseCases.Add(useCase);
                    }
                }
                catch (Exception exp)
                {
                }

            }

            return result;
        }

        public static List<UseCaseResult> GetUseCaseResultsForRule1(string filename)
        {
            var result = new List<UseCaseResult>();

            // Read all the content in as array of lines
            var content = File.ReadAllLines(filename);
            for (int i = 1; i < content.Length; i++)
            {
                if (content[i].Length == 0)
                    continue;
                var cells = content[i].Split(',');
                try
                {
                    result.Add(new UseCaseResult
                    {
                        Id = int.Parse(cells[0]),
                        InFileLoc = int.Parse(cells[1]),
                        Filename = cells[2],
                        Result = cells[3].ToUpper()
                    });
                }
                catch (Exception exp)
                {
                }

            }

            return result;
        }

        public static List<UseCaseResult> GetUseCaseResultsForRule2(string filename)
        {
            var result = new List<UseCaseResult>();

            // Read all the content in as array of lines
            var content = File.ReadAllLines(filename);
            for (int i = 1; i < content.Length; i++)
            {
                if (content[i].Length == 0)
                    continue;
                var cells = content[i].Split(',');
                try
                {
                    result.Add(new UseCaseResult
                    {
                        Id = int.Parse(cells[0]),
                        InFileLoc = int.Parse(cells[1]),
                        Filename = cells[2],
                        Result = cells[3],
                        ResultLabel = cells[4]
                    });
                }
                catch (Exception exp)
                {
                }

            }

            return result;
        }

        public static List<UseCaseResult> GetUseCaseResultsForRule3(string filename)
        {
            var result = new List<UseCaseResult>();

            // Read all the content in as array of lines
            var content = File.ReadAllLines(filename);
            for (int i = 1; i < content.Length; i++)
            {
                if (content[i].Length == 0)
                    continue;
                var cells = content[i].Split(',');
                try
                {
                    result.Add(new UseCaseResult
                    {
                        Id = int.Parse(cells[0]),
                        InFileLoc = int.Parse(cells[1]),
                        Filename = cells[2],
                        Result = cells[3],
                        ResultLabel = cells[4],
                        ResultMode = cells[5]
                    });
                }
                catch (Exception exp)
                {
                }

            }

            return result;
        }

        public static List<UseCaseResult> GetUseCaseResultsForRule4To6(string filename)
        {
            var result = new List<UseCaseResult>();

            // Read all the content in as array of lines
            var content = File.ReadAllLines(filename);
            for (int i = 1; i < content.Length; i++)
            {
                if (content[i].Length == 0)
                    continue;
                var cells = content[i].Split(',');
                try
                {
                    result.Add(new UseCaseResult
                    {
                        Id = int.Parse(cells[0]),
                        InFileLoc = int.Parse(cells[1]),
                        Filename = cells[2],
                        Result = cells[3],
                        ResultLabel = cells[4]
                    });
                }
                catch (Exception exp)
                {
                }

            }

            return result;
        }

        public static List<LibraryDefinition> LoadLibraryDefinitions(string filename)
        {
            var result = new List<LibraryDefinition>();
            var id = 1;
            // Read all the content in as array of lines
            var content = File.ReadAllLines(filename);
            for (int i = 1; i < content.Length; i++)
            {
                if (content[i].Length == 0)
                    continue;
                var cells = content[i].Split(',');
                try
                {
                    result.Add(new LibraryDefinition
                    {
                        Id = id,
                        Name = cells[0],
                        Prefix = cells[1],
                        RegExPattern = cells[2],
                        CloseLibrary = string.IsNullOrWhiteSpace(cells[3]) || bool.Parse(cells[3])
                    });
                    id++;
                }
                catch (Exception exp)
                {
                }

            }

            return result;
        }

        public static List<LibraryDefinition> LoadPossibleLibraryDefinitions(string filename)
        {
            var result = new List<LibraryDefinition>();
            var id = 1;
            // Read all the content in as array of lines
            var content = File.ReadAllLines(filename);
            for (int i = 1; i < content.Length; i++)
            {
                if (content[i].Length == 0)
                    continue;
                var cells = content[i].Split(',');
                try
                {
                    result.Add(new LibraryDefinition
                    {
                        Id = id,
                        Prefix = cells[0]
                    });
                    id++;
                }
                catch (Exception exp)
                {
                }

            }

            return result;
        }

    }
}
