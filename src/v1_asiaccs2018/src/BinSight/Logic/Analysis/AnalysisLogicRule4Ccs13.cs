﻿using System;
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
    public class AnalysisLogicRule4Ccs13 : AnalysisLogic
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
                        saltRegIdx = 2;
                        break;
                    case "Ljavax/crypto/spec/PBEKeySpec;-><init>([C[BII)V":
                        saltRegIdx = 2;
                        break;
                    case "Ljavax/crypto/spec/PBEParameterSpec;-><init>([BI)V":
                        saltRegIdx = 1;
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
