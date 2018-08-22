using System;
using System.Collections.Generic;
using System.Linq;
using APKInsight.Logic.Analysis.Data;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis
{
    /// <summary>
    /// Logic to evaluate rule 2 from CCS 2013 paper
    /// </summary>
    public class AnalysisLogicRule2Ccs13: AnalysisLogic
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
            var useCases = ApkInfo.UseCases.Where(uc => uc.IsCipherUseCaseRule2).ToList();
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
                if (EntryPointInstruction.Function == "Ljavax/crypto/Cipher;->init(ILjava/security/Key;)V")
                {
                    _report.AddLineWithoutCounter(
                        $"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},IvNotSet,");
                    return;
                }
                if (EntryPointInstruction.Function ==
                        "Ljavax/crypto/Cipher;->init(ILjava/security/cert/Certificate;Ljava/security/SecureRandom;)V" ||
                        EntryPointInstruction.Function ==
                        "Ljavax/crypto/Cipher;->init(ILjava/security/cert/Certificate;)V")
                {
                    _report.AddLineWithoutCounter(
                        $"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},PublicCrypto,");
                    return;
                }
                if (EntryPointInstruction.Function ==
                        "Ljavax/crypto/Cipher;->init(ILjava/security/Key;Ljava/security/SecureRandom;)V")
                {
                    _report.AddLineWithoutCounter(
                        $"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},SecRandCase,");
                    return;
                }

                var modeSlices = SliceProgramBack(3);

                foreach (var programSliceState in modeSlices)
                {
                    if (
                        programSliceState.Instructions.Any(
                            i =>
                                i != null &&
                                i.IsInvoke && i.Function == "Ljavax/crypto/spec/IvParameterSpec;-><init>([B)V" &&
                                i.ArgsRegs.Length == 2))
                    {
                        var instruction = programSliceState.Instructions.FirstOrDefault(
                            i =>
                                i != null &&
                                i.IsInvoke && i.Function == "Ljavax/crypto/spec/IvParameterSpec;-><init>([B)V" &&
                                i.ArgsRegs.Length == 2);

                        EntryPointInstruction = instruction;
                        EntryPoint = instruction.ParentEntryPointVertex;
                        EntryPointInstructionVertex = instruction.ParentVertex;
                        var slicesForIv = SliceProgramBack(1);
                        if (slicesForIv.Count > 0)
                        {
                            foreach (var ivSlice in slicesForIv)
                            {
                                var lastInstruction = ivSlice.Instructions.LastOrDefault();
                                if (lastInstruction == null)
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},DeadCode,");
                                }
                                else if (lastInstruction.InstructionType == ESmaliInstruction.ArrayLength)
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},NotFound-AL,{lastInstruction.ParentEntryPointVertex.UniqueName}");
                                }
                                else if (lastInstruction.InstructionType == ESmaliInstruction.FillArrayData)
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},StaticArrayFill,{lastInstruction.Label}");
                                }
                                else if (lastInstruction.IsConst)
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},StaticArray,");
                                }
                                else if (lastInstruction.IsInvoke)
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},FunctionCall,{lastInstruction.Function}");
                                }
                                else if (lastInstruction.IsMoveObject)
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},MoveObject,{lastInstruction.ParentEntryPointVertex.UniqueName}");
                                }
                                else if (lastInstruction.IsMoveResultObject)
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},MoveResultObject,{lastInstruction.ParentEntryPointVertex.UniqueName}");
                                }
                                else if (lastInstruction.InstructionType == ESmaliInstruction.NewInstance)
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},NewInstance,{lastInstruction.TypeName}");
                                }
                                else if (lastInstruction.InstructionType == ESmaliInstruction.NewArray)
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},NewArray,{lastInstruction.ParentEntryPointVertex.UniqueName}");
                                }
                                else
                                {
                                    _report.AddLineWithoutCounter($"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},NotFound,");
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

    }
}
