using System;
using System.Collections.Generic;
using System.Linq;
using APKInsight.Logic.Analysis.Data;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis
{
    /// <summary>
    /// Logic to evaluate rule 1 from CCS 2013 paper
    /// </summary>
    public class AnalysisLogicRule1Ccs13: AnalysisLogic
    {
        private static AnalysisReport _report;
        public ApkInfo ApkInfo { get; set; }

        public static void InitReport()
        {
            _report = new AnalysisReport();
            _report.AddLineWithoutCounter("N,inFileLoc,fileName,cipherModeString");
        }

        public static void SaveReport(string filename)
        {
            _report.SaveReport(filename);
        }

        public override bool Process()
        {
            var useCases = ApkInfo.UseCases.Where(uc => uc.IsCipherUseCaseRule1).ToList();
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

                var modeSlices = SliceProgramBack(0);

                foreach (var programSliceState in modeSlices)
                {
                    var instr = programSliceState.Instructions.LastOrDefault();
                    if (instr != null && instr.IsStrConst)
                    {
                        _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InClassPos},{useCase.Filename},{instr.ConstStrValue}");
                    }
                    else
                    {
                        if (instr == null)
                        {
                            _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InClassPos},{useCase.Filename},DeadCode");
                        }
                        else if (instr.IsInvoke || instr.IsInvokeRange)
                        {
                            _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InClassPos},{useCase.Filename},NotFound-Invoke:{instr.Function}");
                        }
                        else
                        {
                            _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InClassPos},{useCase.Filename},NotFound");
                        }
                    }
                }
            }
        }
    }
}
