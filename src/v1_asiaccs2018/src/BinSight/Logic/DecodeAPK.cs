using System;
using System.Collections.Generic;
using System.Text;

using APKInsight.Models;
using APKInsight.Queries;

using CsnowFramework.InputOutput;
using System.IO;
using System.Diagnostics;
using System.Linq;
using APKInsight.Enums;
using APKInsight.Logic.Analysis;
using APKInsight.Logic.Analysis.Data;
using APKInsight.Models.DataBase;
using CsnowFramework.Crypto;

namespace APKInsight.Logic
{
    internal class DecodeApk
    {
        private readonly string _ramDiskDrive;
        private readonly string _apkToolCmdLine;
        private const string ApkDecodeCommand = "{0} d -f -o {1} {2}";
        private string _apkFilepath;
        private string _apkOutDir;

        #region Processing Switches

        public static bool FindAllUseCases { get; set; } = false;
        public static bool EvalRule1FromCcs13 { get; set; } = false;
        public static bool EvalRule2FromCcs13 { get; set; } = false;
        public static bool EvalRule3FromCcs13 { get; set; } = false;
        public static bool EvalRule4FromCcs13 { get; set; } = false;
        public static bool EvalRule5FromCcs13 { get; set; } = false;
        public static bool EvalRule6FromCcs13 { get; set; } = false;
        public static bool EvalDataFlowAnalysis { get; set; } = false;

        public static bool ReadInAllSmaliFiles => 
            EvalRule1FromCcs13 || 
            EvalRule2FromCcs13 || 
            EvalRule3FromCcs13 || 
            EvalRule4FromCcs13 ||
            EvalRule5FromCcs13 ||
            EvalRule6FromCcs13 ||
            EvalDataFlowAnalysis;

        #endregion


        #region Public properties

        /// <summary>
        /// Filename of the report to use for all CFG based analysis steps
        /// </summary>
        public static List<ApkInfo> UseCases { get; set; }

        #endregion

        public  List<string> Files { get; set; }
        public bool Failed { get; set; }
        private static AnalysisReport _failedToDecode = new AnalysisReport();
        

        public string ApkFileSha1 { get; set; }

        #region Constructor

        public DecodeApk(string ramDrivePath, string apkToolCmpLine)
        {
            _ramDiskDrive = ramDrivePath;
            _apkToolCmdLine = apkToolCmpLine;
        }

        public static void InitReport()
        {
            _failedToDecode = new AnalysisReport();
            _failedToDecode.AddLineWithoutCounter("N,sha1,filename");
        }

        public static void SaveFailedReport(string filename)
        {
            _failedToDecode.SaveReport(filename);
        }

        #endregion

        [Obsolete("This function is not maintained")]
        public bool DecodeApkFile(BinaryObject apkFileObject)
        {
            InitDirNames(apkFileObject);
            QueryBinaryObjectContent bocQuery = new QueryBinaryObjectContent();
            var boc = bocQuery.SelectBinaryObjectContent(apkFileObject.ContentId.Value);
            if (boc.Count == 1)
            {
                if (Utilities.SaveAsFile(boc[0].Content, _apkFilepath))
                {
                    if (DecodeApkFileIntoDir())
                    {
                        QueryBinaryObject bioQuery = new QueryBinaryObject();
                        bioQuery.UpdateBinaryObjectProcessState(apkFileObject.UId.Value, (int) BinaryObjectApkProcessingStage.ExtractingAndUploadingInternals);
                    }

                }
            }
            return false;
        }

        public bool DecodeApkFile(string filename)
        {
            _apkFilepath = filename;
            var rnd = new Random((int)DateTime.Now.Ticks);
            _apkOutDir = Path.Combine(_ramDiskDrive, Path.GetFileName(filename) + $"_{rnd.Next()}_out");
            var sha1 = Hash.HashSha1(File.ReadAllBytes(filename)).ToLower();
            if (DecodeApkFileIntoDir())
            {
                if (FindAllUseCases)
                {
                    var logic = new AnalysisLogicFindAllUseCases
                    {
                        ApkFilename = filename,
                        ApkOutDirectory = _apkOutDir,
                        Files = Files,
                        Sha1 = sha1
                    };
                    logic.Process();
                }
                AnalysisState analysisState = null;
                if (ReadInAllSmaliFiles)
                {
                    analysisState = new AnalysisState
                    {
                        FilesToProcess = new HashSet<string>(Files.Where(fn => Path.GetExtension(fn) == ".smali").Select(fn => fn.Replace(_apkOutDir, "")).ToList()),
                        FilesProcessed = new HashSet<string>(),
                        FileContents = new Dictionary<string, string>(),
                        //FullToShortFilenamesMap = new Dictionary<string, string>(),
                        //ShortToFullFilenamesMap = new Dictionary<string, string>()
                    };
                    foreach (var smaliFile in Files.Where(fn => Path.GetExtension(fn) == ".smali"))
                    {
                        analysisState.FileContents.Add(smaliFile.Replace(_apkOutDir, ""), Encoding.UTF8.GetString(Utilities.ReadAllBytes(smaliFile)));
                        //analysisState.FullToShortFilenamesMap.Add(smaliFile.Replace(_apkOutDir, ""), smaliFile);
                        //analysisState.ShortToFullFilenamesMap.Add(smaliFile, smaliFile.Replace(_apkOutDir, ""));
                    }

                }
                if (EvalRule1FromCcs13)
                {
                    var info = UseCases.FirstOrDefault(uc => Path.GetFileName(uc.Filename) == Path.GetFileName(filename));
                    var logic = new AnalysisLogicRule1Ccs13()
                    {
                        ApkFilename = filename,
                        ApkOutDirectory = _apkOutDir,
                        Files = Files,
                        Sha1 = sha1,
                        ApkInfo = info,
                        AnalysisState = analysisState
                    };
                    logic.Process();
                }

                if (EvalRule2FromCcs13)
                {
                    var info = UseCases.FirstOrDefault(uc => Path.GetFileName(uc.Filename) == Path.GetFileName(filename));
                    var logic = new AnalysisLogicRule2Ccs13()
                    {
                        ApkFilename = filename,
                        ApkOutDirectory = _apkOutDir,
                        Files = Files,
                        Sha1 = sha1,
                        ApkInfo = info,
                        AnalysisState = analysisState
                    };
                    logic.Process();
                }
                if (EvalRule3FromCcs13)
                {
                    var info = UseCases.FirstOrDefault(uc => Path.GetFileName(uc.Filename) == Path.GetFileName(filename));
                    var logic = new AnalysisLogicRule3Ccs13()
                    {
                        ApkFilename = filename,
                        ApkOutDirectory = _apkOutDir,
                        Files = Files,
                        Sha1 = sha1,
                        ApkInfo = info,
                        AnalysisState = analysisState
                    };
                    logic.Process();
                }
                if (EvalRule4FromCcs13)
                {
                    var info = UseCases.FirstOrDefault(uc => Path.GetFileName(uc.Filename) == Path.GetFileName(filename));
                    var logic = new AnalysisLogicRule4Ccs13()
                    {
                        ApkFilename = filename,
                        ApkOutDirectory = _apkOutDir,
                        Files = Files,
                        Sha1 = sha1,
                        ApkInfo = info,
                        AnalysisState = analysisState
                    };
                    logic.Process();
                }
                if (EvalRule5FromCcs13)
                {
                    var info = UseCases.FirstOrDefault(uc => Path.GetFileName(uc.Filename) == Path.GetFileName(filename));
                    var logic = new AnalysisLogicRule5Ccs13()
                    {
                        ApkFilename = filename,
                        ApkOutDirectory = _apkOutDir,
                        Files = Files,
                        Sha1 = sha1,
                        ApkInfo = info,
                        AnalysisState = analysisState
                    };
                    logic.Process();
                }
                if (EvalRule6FromCcs13)
                {
                    var info = UseCases.FirstOrDefault(uc => Path.GetFileName(uc.Filename) == Path.GetFileName(filename));
                    var logic = new AnalysisLogicRule6Ccs13()
                    {
                        ApkFilename = filename,
                        ApkOutDirectory = _apkOutDir,
                        Files = Files,
                        Sha1 = sha1,
                        ApkInfo = info,
                        AnalysisState = analysisState
                    };
                    logic.Process();
                }
                if (EvalDataFlowAnalysis)
                {
                    var info = UseCases.FirstOrDefault(uc => Path.GetFileName(uc.Filename) == Path.GetFileName(filename));
                    var logic = new AnalysisLogicDataFlowForCipher()
                    {
                        ApkFilename = filename,
                        ApkOutDirectory = _apkOutDir,
                        Files = Files,
                        Sha1 = sha1,
                        ApkInfo = info,
                        AnalysisState = analysisState
                    };
                    logic.Process();
                }
                Utilities.RemoveDir(_apkOutDir);
                return true;
            }
            _failedToDecode.AddLineWithCounter("{0}," + $"{sha1},{filename}");
            Utilities.RemoveDir(_apkOutDir);
            return false;
        }

        private bool DecodeApkFileIntoDir()
        {
            var result = DecodeIntoDirectory(_apkFilepath, _apkOutDir);
            if (result)
            {
                Files = Utilities.GetChildFiles(_apkOutDir);
                return true;
            }
            return false;
        }

        private void InitDirNames(BinaryObject apkFileObject)
        {
            if (string.IsNullOrEmpty(_apkFilepath))
            {
                _apkFilepath = Path.Combine(_ramDiskDrive, apkFileObject.UId.ToString() +"." + apkFileObject.FileName);
                _apkOutDir = Path.Combine(_ramDiskDrive, apkFileObject.UId.ToString() + "." + apkFileObject.FileName + "_out");
            }
        }

        public void UploadAnInternalFile(BinaryObject apkFileObject, string filepath)
        {

            if (!apkFileObject.UId.HasValue)
                return;
            InitDirNames(apkFileObject);

            int maxRepeats = 10;
            int failed = 0;
            for (int attempt = 1; attempt <= maxRepeats; attempt++)
            {

                try
                {
                    UploadFile(filepath, _apkOutDir, apkFileObject.UId.Value, apkFileObject.DataSetApplicationCategoryId.Value);
                    attempt = maxRepeats;
                }
                catch (Exception exp)
                {
                    try
                    {
                        UploadFile(filepath, _apkOutDir, apkFileObject.UId.Value, apkFileObject.DataSetApplicationCategoryId.Value);
                        attempt = maxRepeats;
                    }
                    catch (Exception exp2)
                    {
                        if (attempt == maxRepeats)
                        {
                            if (failed == 1)
                            {
                                throw;
                            }
                            failed++;
                            attempt = 0;
                        }
                    }
                }
            }

        }

        public void FinalizeDecoding(BinaryObject apkFileObject)
        {
            var bioQuery = new QueryBinaryObject();
            InitDirNames(apkFileObject);
            CleanupApkDecode(_apkFilepath, _apkOutDir);
            bioQuery.UpdateBinaryObjectProcessState((int)apkFileObject.UId, (int)BinaryObjectApkProcessingStage.InternalsExtracted);
        }

        private bool DecodeIntoDirectory(string apkFilepath, string outputDirPath)
        {
            if (Utilities.CreateDir(outputDirPath))
            {
                string command = string.Format(ApkDecodeCommand, _apkToolCmdLine, outputDirPath, apkFilepath);
                var result = ExecuteCommand(command);
                return result == 0;
            }
            return false;
        }

        private int ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardError = false,
                RedirectStandardOutput = false
            };

            var process = Process.Start(processInfo);

            if (process != null)
            {
                process.WaitForExit();
                int result = process.ExitCode;
                process.Close();

                return result;
            }
            return -1;
        }

        private void CleanupApkDecode(string apkFilepath, string outputDirPath)
        {
            Utilities.RemoveFile(apkFilepath);
            Utilities.RemoveDir(outputDirPath);
        }

        /// <summary>
        /// Uploads an APK and returns newly created BinaryObject model.
        /// </summary>
        /// <param name="filepath">Path to the APK to be uploaded</param>
        /// <returns>Inserted BinaryObject</returns>
        private void UploadFile(string filepath, string outPath, int parentId, int parentCategoryId)
        {
            QueryBinaryObject bioQuery = new QueryBinaryObject();

            var result = new BinaryObject
            {
                DataSetApplicationCategoryId = parentCategoryId,
                RankInCategory = 0,
                PathId = GetBinaryObjectPath(filepath, outPath),
                ParentApkId = parentId,
                FileName = Path.GetFileName(filepath),
                IsRoot = 0
            };
            result.ContentId = UploadContent(filepath, ref result);
            // No matter what type the file is, 0 ALWAYS means Unprocessed
            result.ProcessingStage = 0;
            var existingObject = bioQuery.SelectBinaryObject(result);
            if (existingObject.Count > 0)
            {
                result.UId = existingObject[0].UId;
            }
            else
            {
                bioQuery.AddObject(ref result);
            }
        }

        private int GetBinaryObjectPath(string filepath, string outdir)
        {
            QueryBinaryObjectPath bopQuery = new QueryBinaryObjectPath();

            string simplePath = filepath.Replace(outdir, "");
            int index = simplePath.LastIndexOf("\\");
            string path = simplePath.Substring(0, index + 1);
            StringBuilder currentPath = new StringBuilder();

            string[] parentPaths = path.Split('\\'); // It must return at least 2 items, where item 0 should be empty.
            int parentPathId = 0; // The root parent.

            currentPath.Append("\\");
            // -1 in length since we ignore the last path
            for (int i = 1; i < parentPaths.Length - 1; i++)
            {
                var currentParentId = bopQuery.SelectInsertBinaryObjectPath(parentPathId, parentPaths[i], currentPath.ToString());
                parentPathId = currentParentId;
                currentPath.Append(parentPaths[i] + "\\");
            }

            return parentPathId;
        }

        private int UploadContent(string filename, ref BinaryObject bio)
        {
            QueryBinaryObjectContent bocQuery = new QueryBinaryObjectContent();

            BinaryObjectContent content = new BinaryObjectContent();
            content.Content = Utilities.ReadAllBytes(filename);
            content.Length = content.Content.Length;
            content.Hash = Hash.GetHashSha1Bytes(content.Content);
            bio.Hash = content.Hash;
            content.UId = bocQuery.AddObject(content, Path.GetFileName(filename));
            if (content.UId != null) return content.UId.Value;
            return -1;
        }

    }
}
