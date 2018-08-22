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
    public class AnalysisLogicRule3Ccs13 : AnalysisLogic
    {
        private static AnalysisReport _report;
        public ApkInfo ApkInfo { get; set; }

        public static void InitReport()
        {
            _report = new AnalysisReport();
            _report.AddLineWithoutCounter("N,inFileLoc,fileName,result,labels,mode");
        }

        public static void SaveReport(string filename)
        {
            _report.SaveReport(filename);
        }

        public override bool Process()
        {
            var useCases = ApkInfo.UseCases.Where(uc => uc.IsCipherUseCaseRule3).ToList();
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

                var modeSlices = SliceProgramBack(2);
                var keySlices = SliceProgramBack(1);

                var mode = "";
                var key = "";
                var labels = "";
                foreach (var programSliceState in modeSlices)
                {
                    var lastInstr = programSliceState.Instructions.LastOrDefault();
                    if (lastInstr == null)
                    {
                        mode += "|";
                    }
                    else if (lastInstr.IsStrConst)
                    {
                        if (mode.Length > 0) mode += "|";
                        mode += lastInstr.ConstStrValue;
                    }
                    else if (lastInstr.IsInvoke || lastInstr.IsInvokeRange)
                    {
                        if (mode.Length > 0) mode += "|";
                        mode += lastInstr.Function;
                    }
                }

                foreach (var programSliceState in keySlices)
                {
                    var lastInstr = programSliceState.Instructions.LastOrDefault();

                    if (lastInstr == null)
                    {
                        key += "DeadCode|";
                        labels += "|";
                    }
                    else if (lastInstr.InstructionType == ESmaliInstruction.FillArrayData)
                    {
                        key += "StaticLabel|";
                        labels += lastInstr.Label + "|";
                    }
                    else if (lastInstr.IsArrayGet)
                    {
                        key += "ArrayGet|";
                        labels += lastInstr.ParentEntryPointVertex.UniqueName + "|";
                    }
                    else if (lastInstr.InstructionType == ESmaliInstruction.ArrayGet)
                    {
                        key += "NewInstance|";
                        labels += lastInstr.TypeName + "|";
                    }
                    else if (lastInstr.InstructionType == ESmaliInstruction.NewInstance)
                    {
                        key += "NewInstance|";
                        labels += lastInstr.TypeName + "|";
                    }
                    else if (lastInstr.InstructionType == ESmaliInstruction.ArrayLength)
                    {
                        key += "NotFound-AL|";
                        labels += lastInstr.ParentEntryPointVertex.UniqueName+ "|";
                    }
                    else if (lastInstr.InstructionType == ESmaliInstruction.NewArray)
                    {
                        key += "NewArray|";
                        labels += lastInstr.ParentEntryPointVertex.UniqueName + "|";
                    }
                    else if (lastInstr.IsConst)
                    {
                        key += "StaticVale|";
                        labels += lastInstr.ParentEntryPointVertex.UniqueName + "|";
                        break;
                    }
                    else if (lastInstr.IsMoveObject)
                    {
                        key += "MoveObject|";
                        labels += lastInstr.ParentEntryPointVertex.UniqueName + "|";
                        break;
                    }
                    else if (lastInstr.IsMoveResultObject)
                    {
                        key += "MoveResultObject|";
                        labels += lastInstr.ParentEntryPointVertex.UniqueName + "|";
                        break;
                    }
                    else if (lastInstr.IsInvoke || lastInstr.IsInvokeRange)
                    {
                        key += lastInstr.Function;
                    }
                    else
                    {
                        
                    }
                }

                if (key.Length > 0)
                {
                    _report.AddLineWithoutCounter(
                        $"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},{key},{labels},{mode}");
                }
                else
                {
                    
                }


                //                    if (prevInstruction.Function == "Ljava/lang/String;->getBytes()[B")
                //                    {
                //                        // We just follow the string now
                //                        currentCandidate.TargerRegister = prevInstruction.ArgsRegs[0];
                //                        continue;
                //                    }
                //                    if (IsInvoke(prevInstruction) || IsInvokeRange(prevInstruction))
                //                    {
                //                        currentInstruction = prevInstruction;
                //                        // Value is provided as a return value from a function call, but first we need to jump to invoke function
                //                        ProcessFilesForInvokation(currentInstruction.Function);
                //                        var functionVertex =
                //                            AnalysisState.ControlFlowGraph.GetEntryPointVertexByName(
                //                                currentInstruction.Function, false);
                //                        if (functionVertex == null || functionVertex.Instructions.Count == 0)
                //                        {
                //                            // We were not able to find the vertex, or the vertex was found, but does not have a body
                //                            _report.AddLineWithoutCounter(
                //                                $"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},ReachedAPIs,{currentInstruction.Function},{mode}");
                //                            return;
                //                        }
                //                        // If we are here, we found proper function definition
                //                        AddPathsAsCurrentCandidate(currentCandidate, functionVertex.ReturnVertices);
                //                        currentCandidate.AddPredecessorVertex();
                //                    }
                //                }
                //                if (IsMoveObject(currentInstruction))
                //                {
                //                    // Moving object, change the target register
                //                    currentCandidate.TargerRegister = currentInstruction.Src;
                //                    continue;
                //                }

                //                if (currentInstruction.InstructionType == ESmaliInstruction.StaticGetObject ||
                //                            currentInstruction.InstructionType == ESmaliInstruction.InstanceGetObject)
                //                {
                //                    ProcessFilesForGettersSetters(currentInstruction.Field);
                //                    var fieldVertex = AnalysisState.ControlFlowGraph.GetFieldVertexByName(currentInstruction.Field, false);
                //                    if (fieldVertex == null)
                //                    {
                //                        result = "CannotFindSetter";
                //                        _report.AddLineWithoutCounter(
                //                            $"{ApkInfo.Id},{useCase.InMethodPos},{useCase.Filename},{result},{labels}");
                //                        return;
                //                    }
                //                    result = "";
                //                    for (int i = 0; i < fieldVertex.EdgeIncomingVertex.Count; i++)
                //                    {
                //                        var v = fieldVertex.EdgeIncomingVertex[i];
                //                        var opCodeIdx = fieldVertex.EdgeIncomingVertexInstruction[i];
                //                        var instruction = (SmaliCfgInstruction)v.Instructions[opCodeIdx];
                //                        CandidatePaths.Add(new ProgramSliceState
                //                        {
                //                            CurrentVertex = fieldVertex.EdgeIncomingVertex[i],
                //                            TargerRegister = instruction.Src,
                //                            InstructionIndex = opCodeIdx,
                //                            VertexPath = new List<CfgVertex>(currentCandidate.VertexPath.ToArray()) { currentCandidate.CurrentVertex }
                //                        });
                //                    }
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //    if (modeFound && labelsFound)
                //    {
                //        break;
                //    }
                //}
            }
        }

    }
}
