using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APKInsight.Logic.Analysis.Data;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis
{
    public abstract partial class AnalysisLogic
    {

        #region Public functions

        public List<ProgramSliceState> SliceProgramForward(int registerToTrack)
        {
            _slicingResults = new List<ProgramSliceState>();
            // Contains names of all instructions we have visited, to avoid loops

            InitializeVisitedVertexCache();

            InitializeSlicesToConsider(registerToTrack);

            // Repeat until we explored all the slices
            while (_slicesToConsider.Count > 0)
            {
                //Get current register and slice to process
                DequeueNextSlice();
                //Now analyze backward the current slice
                SliceProgramSliceForward();
            }

            return _slicingResults;
        }

        #endregion


        #region Tracking a single program slice logic

        /// <summary>
        /// Processes current slice forward.
        /// </summary>
        private void SliceProgramSliceForward()
        {
            // This is the main flag that represent a case, where there is not point of searching farther
            bool searchFinished = false;
            while (!searchFinished)
            {
                var instructionFound = FindNextInstruction(ref searchFinished);
                if (!instructionFound)
                {
                    if (searchFinished)
                    {
                        // Not called
                        _currentSlice.Instructions.Add(null);
                    }
                    // We failed to find the next instruction (might have endup in system APIs)
                    searchFinished = true;
                    //TODO(ildarm): Implement checking if the slice is useful
                    _slicingResults.Add(_currentSlice);
                    continue;
                }

				// Handle cache
                if (HasCurrentInstructionBeenVisited(_currentSlice))
                {
                    searchFinished = true;
                    continue;
                }

				// Check if we are still tracking move-result object
                if (_currentSlice.IsTrackingNextMoveResultInstructionRegister &&
                    _currentSlice.CurrentInstruction.IsMoveAnyResult)
                {
					_currentSlice.Instructions.Add(_currentSlice.CurrentInstruction);
                    _currentSlice.TargerRegister = _currentSlice.CurrentInstruction.Dest.Copy();
                    _currentSlice.IsTrackingNextMoveResultInstructionRegister = false;
                    // Goto next instruction
                    continue;
                }

				// We should be here only if 
				if (!_currentSlice.IsTrackingNextMoveResultInstructionRegister && 
					IsRegisterImpactedByInstruction(_currentSlice.CurrentInstruction, _currentSlice.TargerRegister))
                {
                    // Add the instruction to the list
                    _currentSlice.Instructions.Add(_currentSlice.CurrentInstruction);

                    if (_currentSlice.CurrentInstruction.IsArrayCopyInstruction)
                    {
						// This is array copying instruction, just use destination array now as the target
                        _currentSlice.TargerRegister = _currentSlice.CurrentInstruction.ArgsRegs[2].Copy();
                    }
					else if (_currentSlice.CurrentInstruction.IsInvoke)
					{
                        // Check if we need to dig into the function
                        if (_currentSlice.CurrentInstruction.IsInstanceInvoke && _currentSlice.CurrentInstruction.ArgsRegs[0].CompareTo(_currentSlice.TargerRegister) == 0)
                        {
							// This is the case of *this* reference, just move on
							continue;
                        }

						// We need to step in
					    int pRegIndex;
					    for (pRegIndex = 0; pRegIndex < _currentSlice.CurrentInstruction.ArgsRegs.Length; pRegIndex++)
					    {
					        if (_currentSlice.CurrentInstruction.ArgsRegs[pRegIndex].CompareTo(_currentSlice.TargerRegister) == 0)
								break;
					    }

                        ProcessFilesForInvokation(_currentSlice.CurrentInstruction.Function);
                        var functionEntryPoint = AnalysisState.ControlFlowGraph.GetEntryPointVertexByName(_currentSlice.CurrentInstruction.Function, false);
					    if (functionEntryPoint != null)
					    {
					        _currentSlice.InjectVertexInPathHeadForForwardWalk(functionEntryPoint);
                            _currentSlice.TargerRegister = new DalvikRegister(pRegIndex, true);
                            // So that we start from the first instruction
                            _currentSlice.InstructionIndex = -1;

					    }
					    else
					    {
					        // What shall we do if we failed to find the function.
					    }

					}
					else if (_currentSlice.CurrentInstruction.IsReturn)
					{
					}
                }				

            }
        }

        #endregion


        #region Finding previous instruction for backward slicing

        /// <summary>
        /// Tries to get previous instruction, fails if there is no simple way to get one
        /// </summary>
        /// <returns>Next instruction</returns>
        private bool FindNextInstruction(ref bool searchFinished)
        {
            //First, check if the case is simple, i.e., there is an instruction available in the current vertex
            _currentSlice.InstructionIndex++;
            if (_currentSlice.InstructionIndex < _currentSlice.CurrentVertex.Instructions.Count)
            {
                if (_currentSlice.CurrentInstruction.IsBranchingStatement)
                    CreateNewSlicesToConsiderForBranchingStatemetn();
                _currentSlice.CurrentInstruction = _currentSlice.CurrentVertex.Instructions[_currentSlice.InstructionIndex];
                return true;
            }

            if (_currentSlice.CurrentInstruction.IsGotoStatement)
            {
                FollowGotoStatement();
				return true;
            }

            if (_currentSlice.CurrentInstruction.IsSwitchStatement)
            {
                BranchOutOnSwitch();
            }

            // Process case of when this is an invoke
            if (_currentSlice.CurrentVertex.Successor != null)
            {
                if (_currentSlice.CurrentInstruction.IsInvoke)
                {
					// Shall we go inside?
                }
                _currentSlice.AddSuccessorVertex();
                return true;
            }

            // We do not deal with entry points types of instruction here, they need special attention.
            return false;
        }

        private void BranchOutOnSwitch()
        {
            //TODO(ildarm): Implement
        }

        private void CreateNewSlicesToConsiderForBranchingStatemetn()
        {
            var branchingVertex = AnalysisState.ControlFlowGraph
				.GetVertexByName(_currentSlice.CurrentVertex.Name, _currentSlice.CurrentInstruction.Label, false);
            if (branchingVertex != null)
            {
                var newSlice = _currentSlice.CreateCopy();
                newSlice.Instructions.Add(_currentSlice.CurrentInstruction);
                newSlice.InjectVertexInPathHeadForForwardWalk(branchingVertex);
                newSlice.InstructionIndex = -1;
                _slicesToConsider.Add(newSlice);
            }
            else
            {
                throw new Exception("This should not happen (parser bug?)");
            }
        }

        private void FollowGotoStatement()
        {
            var gotoVertex = AnalysisState.ControlFlowGraph
				.GetVertexByName(_currentSlice.CurrentVertex.Name, _currentSlice.CurrentInstruction.Label, false);
            if (gotoVertex != null)
            {
                _currentSlice.Instructions.Add(_currentSlice.CurrentInstruction);
                _currentSlice.InjectVertexInPathHeadForForwardWalk(gotoVertex);
            }
            else
            {
                throw new Exception("This should not happen (parser bug?)");
            }
        }

        ///// <summary>
        ///// Returns true if the current instruction is Try/Catch case, which are not an entry point
        ///// </summary>
        //private bool IsTryCatchInNotEntryPointCase =>
        //    _currentSlice.CurrentInstruction != null &&
        //    !_currentSlice.CurrentVertex.IsEntryPoint &&
        //    (
        //        _currentSlice.CurrentInstruction.InstructionType == ESmaliInstruction.LabelTryEnd ||
        //        _currentSlice.CurrentInstruction.InstructionType == ESmaliInstruction.LabelTryStart
        //    );

        ///// <summary>
        ///// Returns true if the current instruction is a branching label or is not an entry point.
        ///// </summary>
        //private bool IsBranchingLavelInNotEntryPointCase =>
        //    !_currentSlice.CurrentVertex.IsEntryPoint ||
        //    (
        //        _currentSlice.CurrentInstruction != null &&
        //        _currentSlice.CurrentInstruction.IsBranchingLabel
        //    );

        ///// <summary>
        ///// Returns tru if the current vertex is an entry point and we are tracking a parameter.
        ///// </summary>
        //private bool IsEntryPointCaseWithTrackingItsParameter =>
        //    _currentSlice.CurrentVertex.IsEntryPoint &&
        //    _currentSlice.TargerRegister.IsParameter;

        ///// <summary>
        ///// Moves the current slice to predecessor
        ///// </summary>
        ///// <returns>true if move was successful, false otherwise</returns>
        //private bool MoveToPredecessorVertex()
        //{
        //    // If we meet try-start/end labels, then just use the predecessor
        //    if (_currentSlice.CurrentVertex.Predecessor != null)
        //    {
        //        _currentSlice.AddPredecessorVertex();
        //        return true;
        //    }
        //    return false;

        //}

        ///// <summary>
        ///// Process branching logic, so that we after
        ///// </summary>
        ///// <returns></returns>
        //private bool ProcessingBranchingInstruction()
        //{
        //    if (_currentSlice.CurrentVertex.Predecessor == null && _currentSlice.CurrentVertex.EdgeIncomingVertex.Count == 0)
        //        return false;

        //    // Simple case, only predecessor exist, or only one in-edge
        //    if (_currentSlice.CurrentVertex.Predecessor != null && _currentSlice.CurrentVertex.EdgeIncomingVertex.Count == 0)
        //    {
        //        _currentSlice.AddPredecessorVertex();
        //        return true;
        //    }
        //    if (_currentSlice.CurrentVertex.Predecessor != null && _currentSlice.CurrentVertex.EdgeIncomingVertex.Count > 0)
        //    {
        //        // We will consider all InVertices later
        //        CreateNewSlicesToConsiderFromInEdgesForCurrentVertex(_currentSlice.CurrentVertex, false);
        //        _currentSlice.AddPredecessorVertex();
        //        return true;
        //    }
        //    if (_currentSlice.CurrentVertex.Predecessor == null && _currentSlice.CurrentVertex.EdgeIncomingVertex.Count >= 1)
        //    {
        //        // We will consider all but first InVertices later
        //        CreateNewSlicesToConsiderFromInEdgesForCurrentVertex(_currentSlice.CurrentVertex, true);
        //        // Use the first InVertex as the continuation
        //        ModifySliceWithInEdgeVertex(ref _currentSlice, _currentSlice.CurrentVertex, 0);
        //        // We should be good with instruction
        //        return true;
        //    }
        //    return false;
        //}

        ///// <summary>
        ///// Expands to all in-edges that we met
        ///// </summary>
        ///// <param name="searchFinished">Shows whether or not the search is finished</param>
        ///// <param name="vertex">The vertex for which we consider the in-degree edges</param>
        ///// <returns>true if we managed to find the next instruction</returns>
        //private bool ExpandToInEdgesForEntryPoint(CfgVertex vertex, ref bool searchFinished)
        //{
        //    // First, if we do not have any in-edges, process all files that have calls into that entry point
        //    // in order to make sure that we uncovered all in-edges
        //    if (vertex.EdgeIncomingVertex.Count == 0)
        //    {
        //        ProcessFilesForInvokation(vertex.Name);
        //        // If we still came empty handed, then this is a dead-code, i.e., no one actually calls it.
        //        if (vertex.EdgeIncomingVertex.Count == 0)
        //        {
        //            // We done with search
        //            searchFinished = true;
        //            // We failed to find an instruction
        //            return false;
        //        }
        //    }

        //    // Create new slices to consider for future (all they get into _slicesToConsider array)
        //    CreateNewSlicesToConsiderFromInEdgesForCurrentVertex(vertex, true);

        //    // Modify the current slice with continuation point
        //    if (!ModifySliceWithInEdgeVertex(ref _currentSlice, vertex, 0))
        //    {
        //        searchFinished = true;
        //        return false;
        //    }

        //    // If we are here, this means we found the instruction
        //    return true;

        //}

        #endregion


    }
}
