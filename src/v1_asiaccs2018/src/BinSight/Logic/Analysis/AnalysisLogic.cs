using System;
using System.Collections.Generic;
using System.Linq;
using APKInsight.Logic.Analysis.Data;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis
{
    /// <summary>
    /// A generic class to run the analysis. This class provives common functionality, e.g., program slicing.
    /// </summary>
    public abstract partial class AnalysisLogic
    {

        // Variables that describe APK
        public string ApplicationId { get; set; }
        public List<string> Files { get; set; }
        public string ApkOutDirectory { get; set; }
        public string Sha1 { get; set; }
        public string ApkFilename { get; set; }

        // The analysis state of the current case
        public AnalysisState AnalysisState { get; set; }
        
        // Entry point of the current Use-Case
        public CfgVertex EntryPoint { get; set; }
        public SmaliCfgInstruction EntryPointInstruction { get; set; }
        public CfgVertex EntryPointInstructionVertex { get; set; }

        public List<ProgramSliceState> CandidatePaths { get; set; }


        #region Public abstract functions

        /// <summary>
        /// Do the main processing
        /// </summary>
        public abstract bool Process();

        #endregion



        #region Backward slicing internal logic


        ///// <summary>
        ///// If we are tracking pN type of register (parameter), goes up stack, and finds where this parameter comes from.
        ///// </summary>
        ///// <param name="slice">The slice to extend with caller in-edge</param>
        ///// <param name="i">Index of the in-edge. Default value is 0.</param>
        //private static void ExtendSliceWithCallerInvokation(ref ProgramSliceState slice, int i = 0)
        //{
        //    try
        //    {
        //        var instr = slice.CurrentInstruction;
        //        slice.InjectVertexInPathHead(instr.ParentEntryPointVertex.EdgeIncomingVertex[i]);
        //        var instrIdx = instr.ParentEntryPointVertex.EdgeIncomingVertexInstruction[i];
        //        slice.CurrentInstruction =
        //            instr.ParentEntryPointVertex.EdgeIncomingVertex[i].Instructions[instrIdx];
        //        var regIdx = slice.TargerRegister.N;
        //        if (slice.CurrentInstruction.IsInstanceInvoke)
        //        {
        //            regIdx += 1; // Skip *this*
        //        }
        //        slice.TargerRegister = slice.CurrentInstruction.ArgsRegs[regIdx].Copy();
        //        slice.InstructionIndex = instrIdx;
        //        slice.Instructions.Add(slice.CurrentInstruction);
        //    }
        //    catch (Exception e)
        //    {
        //    }

        //}

        #endregion


        #region Need to be refactored

        public void AddPathsAsCurrentCandidate(ProgramSliceState current, List<CfgVertex> branches)
        {
            List<CfgVertex> path = new List<CfgVertex>(current.VertexPath.ToArray()) {current.CurrentVertex};

            foreach (var branch in branches)
            {
                CandidatePaths.Add(new ProgramSliceState
                {
                    CurrentVertex = branch,
                    InstructionIndex = branch.Instructions.Count,
                    TargerRegister = new DalvikRegister { IsReturnTracking = true },
                    VertexPath = path
                });
                
            }
        }

        public DalvikRegister ProcessExpectedReturnStatement(SmaliCfgInstruction instruction, DalvikRegister reg)
        {
            if (reg.IsReturnTracking)
            {
                if (instruction.IsReturn)
                {
                    return instruction.Src;
                }
            }
            return reg;
        }

        public SmaliCfgInstruction GetPreviousInstruction(ref ProgramSliceState pathState, out bool toBreak)
        {
            toBreak = false;
            pathState.InstructionIndex--;
            if (pathState.InstructionIndex >= 0)
            {
                // Simple case, just fetch the instruction
                return pathState.CurrentVertex.Instructions[pathState.InstructionIndex];
            }
            else
            {
                // More complex case
                if (pathState.CurrentVertex.EdgeIncomingVertex.Count == 0 && pathState.CurrentVertex.Predecessor != null)
                {
                    // Simple case, only one case
                    pathState.AddPredecessorVertex();
                    return GetPreviousInstruction(ref pathState, out toBreak);
                }
                if (pathState.CurrentVertex.EdgeIncomingVertex.Count > 0 && pathState.CurrentVertex.Predecessor != null)
                {
                    if (pathState.CurrentVertex.EdgeIncomingVertex.Count > 0)
                    {
                        var path = new List<CfgVertex>(pathState.VertexPath.ToArray()) { pathState.CurrentVertex };
                        for (int i = 0; i < pathState.CurrentVertex.EdgeIncomingVertex.Count; i++)
                        {
                            var inV = pathState.CurrentVertex.EdgeIncomingVertex[i];
                            var instrIdx = pathState.CurrentVertex.EdgeIncomingVertexInstruction[i];
                            CandidatePaths.Add(new ProgramSliceState()
                            {
                                InstructionIndex = instrIdx,
                                CurrentVertex = inV,
                                TargerRegister = pathState.TargerRegister,
                                VertexPath = path
                            });
                        }
                    }
                    // Simple case, only one case
                    pathState.AddPredecessorVertex();
                    return GetPreviousInstruction(ref pathState, out toBreak);
                }
                if (pathState.CurrentVertex.IsEntryPoint && pathState.TargerRegister.IsParameter)
                {
                    if (pathState.CurrentVertex == null)
                    {
                    }

                    ProcessFilesForInvokation(pathState.CurrentVertex.Name);
                    if (pathState.CurrentVertex.EdgeIncomingVertex.Count > 0)
                    {
                        toBreak = true;
                        var path = new List<CfgVertex>(pathState.VertexPath.ToArray()) { pathState.CurrentVertex };
                        for (int i = 0; i < pathState.CurrentVertex.EdgeIncomingVertex.Count; i++)
                        {
                            var inV = pathState.CurrentVertex.EdgeIncomingVertex[i];
                            var instrIdx = pathState.CurrentVertex.EdgeIncomingVertexInstruction[i];
                            CandidatePaths.Insert(0, new ProgramSliceState()
                            {
                                InstructionIndex = instrIdx,
                                CurrentVertex = inV,
                                TargerRegister = inV.Instructions[instrIdx].ArgsRegs[1 + pathState.TargerRegister.N],
                                VertexPath = path
                            });
                        }
                    }
                }
            }

            return null;
        }
        #endregion


        #region Public functions - Slicing Logic





        #endregion


        #region Private helper functions - Slicing Logic

        /// <summary>
        /// Checks if the instruction impacts currently tracked register
        /// </summary>
        /// <param name="instruction">The instruction under consideration</param>
        /// <param name="reg">Register that might be impacted</param>
        /// <returns>True if the op impacts the register, false otherwise</returns>
        protected bool IsRegisterImpactedByInstruction(SmaliCfgInstruction instruction, DalvikRegister reg)
        {
            // If we are tracking return, return true only if this is return statement
            if (reg.IsReturnTracking)
                return instruction.IsAnyReturn;

            // Otherwise compare Destination register
            if (instruction.Dest != null && instruction.Dest.CompareTo(reg) == 0) return true;

            // We invoke on an object
            if (instruction.IsInstanceInvoke && instruction.ArgsRegs[0].CompareTo(reg) == 0) return true;
            if (instruction.IsInvoke)
            {
                foreach (var dalvikRegister in instruction.ArgsRegs)
                {
                    if (dalvikRegister.CompareTo(reg) == 0)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the instruction impacts currently tracked register
        /// </summary>
        /// <param name="instruction">The instruction under consideration</param>
        /// <param name="reg">Register that might be impacted</param>
        /// <returns>True if the op impacts the register, false otherwise</returns>
        protected bool IsRegisterUsedForInvoke(SmaliCfgInstruction instruction, DalvikRegister reg)
        {
            // If we are tracking return, return true only if this is return statement
            if (!instruction.IsInstanceInvoke)
                return false;

            if (instruction.ArgsRegs.Length == 0)
            {
                // This should not happen, instance invoke is always on a method
                return false;
            }

            return instruction.ArgsRegs[0].CompareTo(reg) == 0;

        }


        private static void ModifySliceWithPropertySetterVertex(ref ProgramSliceState slice, CfgVertex field, int startIndex = 0)
        {
            slice.InjectVertexInPathHeadForBackwardWalk(field.EdgeIncomingVertex[startIndex]);

            // Set the instruction properly
            slice.InstructionIndex = field.EdgeIncomingVertexInstruction[startIndex];
            slice.CurrentInstruction = field.EdgeIncomingVertex[startIndex].Instructions[slice.InstructionIndex];
            slice.TargerRegister = slice.CurrentInstruction.Src.Copy();
            slice.Instructions.Add(slice.CurrentInstruction);
        }

        private static void CreateNewSlicesFromReturns(ref ProgramSliceState slice, ref List<ProgramSliceState> slicesToConsider, int startIndex = 0)
        {
            for (int i = startIndex; i < slice.CurrentVertex.EdgeIncomingVertex.Count; i++)
            {
                var inVertex = slice.CurrentVertex.EdgeIncomingVertex[i];
                var inIndex = slice.CurrentVertex.EdgeIncomingVertexInstruction[i];
                var newSlice = slice.CreateCopy();
                newSlice.InjectVertexInPathHeadForBackwardWalk(inVertex);

                // Fix the new slice so that it begins from a proper instruction, the instrucation that causes the jump
                newSlice.InstructionIndex = inIndex;
                newSlice.CurrentInstruction = inVertex.Instructions[inIndex];

                // Now add the slice for consideration
                slicesToConsider.Add(newSlice);
            }
        }

        #endregion

    }

}
