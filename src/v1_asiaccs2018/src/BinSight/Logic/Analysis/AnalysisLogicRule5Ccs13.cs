using System;
using System.Collections.Generic;
using System.Linq;
using APKInsight.Logic.Analysis.Data;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis
{
    /// <summary>
    /// Logic to evaluate rule 4 from CCS 2013 paper
    /// </summary>
    public class AnalysisLogicRule5Ccs13 : AnalysisLogic
    {
        private static AnalysisReport _report;
        public ApkInfo ApkInfo { get; set; }

        public static void InitReport()
        {
            _report = new AnalysisReport();
            _report.AddLineWithoutCounter("N,inFileLoc,fileName,result,labels");
        }

        public static void SaveReport(string filename)
        {
            _report.SaveReport(filename);
        }

        public override bool Process()
        {
            var useCases = ApkInfo.UseCases.Where(uc => uc.IsCipherUseCaseRule4And5).ToList();
            foreach (var useCase in useCases)
            {
                try
                {
                    ProcessUseCase(useCase);
                }
                catch(Exception exp)
                {
                    //TODO(ildarm): Report on failure
                }
            }
            return true;
        }

        private void ProcessUseCase(UseCase useCase)
        {
            // Init the control flow graph
            AnalysisState.InitCfg();
            if (ProcessFileForUseCase(useCase))
            {
                // Get the entry point details (instruction, method and vertex)
                SetupEntryPointForUseCase(useCase);

                // Setup slicing machine
                var result = "";
                var labels = "";
                var labelsFound = false;
                if (EntryPointInstruction.Function != "Ljavax/crypto/spec/PBEKeySpec;-><init>([C[BI)V" &&
                    EntryPointInstruction.Function != "Ljavax/crypto/spec/PBEKeySpec;-><init>([C[BII)V" &&
                    EntryPointInstruction.Function != "Ljavax/crypto/spec/PBEParameterSpec;-><init>([BI)V"
                    )
                {
                    _report.AddLineWithoutCounter(
                        $"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},Negative: {EntryPointInstruction.Function},{labels}");
                    return;

                }

                int saltRegIdx = 0;
                switch (EntryPointInstruction.Function)
                {
                    case "Ljavax/crypto/spec/PBEKeySpec;-><init>([C[BI)V":
                        saltRegIdx = 3;
                        break;
                    case "Ljavax/crypto/spec/PBEKeySpec;-><init>([C[BII)V":
                        saltRegIdx = 3;
                        break;
                    case "Ljavax/crypto/spec/PBEParameterSpec;-><init>([BI)V":
                        saltRegIdx = 2;
                        break;
                    default:
                        break;
                }
                var saltSlices = SliceProgramBack(saltRegIdx);

                if (saltSlices.Count > 0)
                {
                    foreach (var saltSlice in saltSlices)
                    {
                        var lastInstruction = saltSlice.Instructions.LastOrDefault();
                        if (lastInstruction == null)
                        {
                            _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},DeadCode,");
                        }
                        else if (lastInstruction.IsConst || lastInstruction.IsStrConst)
                        {
                            var res = lastInstruction.ConstLongValue.HasValue
                                ? lastInstruction.ConstLongValue.Value.ToString()
                                : "null";
                            var label = lastInstruction.ConstStrValue != null ? lastInstruction.ConstStrValue : "";
                            _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InClassPos},{useCase.Filename},{res}, {label} ");
                        }
                        else if (lastInstruction.IsInvoke || lastInstruction.IsInvokeRange)
                        {
                            _report.AddLineWithoutCounter(
                                $"{ApkInfo.Id},{useCase.InClassPos},{useCase.Filename},NotFound-Invoke:{lastInstruction.Function}");
                        }
                        else
                        {
                            _report.AddLineWithoutCounter(
                                $"{ApkInfo.Id},{useCase.InClassPos},{useCase.Filename},NotFound");
                        }
                    }
                }
                else
                {
                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},NotFound,");
                }

            }
        }

    }
}
