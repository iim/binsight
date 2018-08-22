using System;
using System.Collections.Generic;
using System.Linq;
using APKInsight.Logic.Analysis.Data;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis
{
    public abstract partial class AnalysisLogic
    {
        private List<ProgramSliceState> _slicesToConsider = null;
        private List<ProgramSliceState> _slicingResults = null;
        private ProgramSliceState _currentSlice;

        #region Public functions

        /// <summary>
        /// Returns a slice of a program and returns slices of all paths to set the value for the specific register
        /// </summary>
        /// <param name="registerToTrack">Index of the register to track</param>
        /// <returns>List of all programm slices. Each slice is a set of instructions that impact the </returns>
        public List<ProgramSliceState> SliceProgramBack(int registerToTrack)
        {
            _slicingResults = new List<ProgramSliceState>();
            // Contains names of all instructions we have visited, to avoid loops

            InitializeVisitedVertexCache();

            // Maybe have a class or a structure for this one
            InitializeSlicesToConsider(registerToTrack);

            // Repeat until we explored all the slices
            while (_slicesToConsider.Count > 0)
            {
                //Get current register and slice to process
                DequeueNextSlice();
                //Now analyze backward the current slice
                SliceProgramSliceBack();
            }

            return _slicingResults;
        }

        #endregion


        #region Tracking a single program slice logic

        /// <summary>
        /// Processes current slice backward.
        /// </summary>
        private void SliceProgramSliceBack()
        {
            // This is the main flag that represent a case, where there is not point of searching farther
            bool searchFinished = false;
            while (!searchFinished)
            {
                var instructionFound = FindPreviousInstruction(ref searchFinished);

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

                if (HasCurrentInstructionBeenVisited(_currentSlice))
                {
                    searchFinished = true;
                    continue;
                }

                // We are only interested in instructions that modify register
                if (IsRegisterImpactedByInstruction(_currentSlice.CurrentInstruction, _currentSlice.TargerRegister))
                {
                    // Add the instruction to the list
                    _currentSlice.Instructions.Add(_currentSlice.CurrentInstruction);

                    // Depending what is the instruction is, branch out as needed
                    if (_currentSlice.CurrentInstruction.IsMoveResult || _currentSlice.CurrentInstruction.IsMoveResultObject)
                    {
                        // This is a move-result opcode
                        ProcessMoveResult(ref searchFinished);
                    }
                    else if (_currentSlice.CurrentInstruction.IsReturn)
                    {
                        // This is a return statement, we need to swap the monitoring register
                        _currentSlice.TargerRegister = _currentSlice.CurrentInstruction.Src.Copy();
                    }
                    else if (_currentSlice.CurrentInstruction.IsConst)
                    {
                        // This is a const definition, this is the end for this slice :)
                        _slicingResults.Add(_currentSlice);
                        searchFinished = true;
                    }
                    else if (_currentSlice.CurrentInstruction.InstructionType == ESmaliInstruction.NewInstance)
                    {
                        // New instance of something is created, end of slice
                        _slicingResults.Add(_currentSlice);
                        searchFinished = true;
                    }
                    else if (_currentSlice.CurrentInstruction.InstructionType == ESmaliInstruction.FillArrayData)
                    {
                        // The array is being filled from a statically defined array
                        _slicingResults.Add(_currentSlice);
                        searchFinished = true;
                    }
                    else if (_currentSlice.CurrentInstruction.InstructionType == ESmaliInstruction.ArrayPutByte)
                    {
                        // We now tracking another register
                        _currentSlice.TargerRegister = _currentSlice.CurrentInstruction.Src.Copy();
                    }
                    else if (_currentSlice.CurrentInstruction.IsGetObject)
                    {
                        ProcessFilesForGettersSetters(_currentSlice.CurrentInstruction.Field);
                        var fieldVertex = AnalysisState.ControlFlowGraph.GetFieldVertexByName(_currentSlice.CurrentInstruction.Field, false);
                        if (fieldVertex == null || fieldVertex.EdgeIncomingVertex.Count == 0)
                        {
                            // This is a useless slice, we did not find the property
                            searchFinished = true;
                            _slicingResults.Add(_currentSlice);
                        }
                        else
                        {
                            // We found the vertex with setter
                            // We will consider all but first InVertices later
                            for (int i = 1; i < fieldVertex.EdgeIncomingVertex.Count; i++)
                            {
                                var newSlice = _currentSlice.CreateCopy();
                                ModifySliceWithPropertySetterVertex(ref newSlice, fieldVertex, i);
                                _slicesToConsider.Add(newSlice);
                            }

                            // Use the first InVertex as the continuation
                            ModifySliceWithPropertySetterVertex(ref _currentSlice, fieldVertex);
                        }
                    }
                    else if (_currentSlice.CurrentInstruction.IsMoveObject)
                    {
                        _currentSlice.TargerRegister = new DalvikRegister(_currentSlice.CurrentInstruction.Src);
                    }
                }
                else if (IsRegisterUsedForInvoke(_currentSlice.CurrentInstruction, _currentSlice.TargerRegister))
                {
                    _currentSlice.Instructions.Add(_currentSlice.CurrentInstruction);
                }
            }
        }

        #endregion


        #region Finding previous instruction for backward slicing

        /// <summary>
        /// Tries to get previous instruction, fails if there is no simple way to get one
        /// </summary>
        /// <returns>Next instruction</returns>
        private bool FindPreviousInstruction(ref bool searchFinished)
        {
            //First, check if the case is simple, i.e., there is an instruction available in the current vertex
            _currentSlice.InstructionIndex--;
            if (_currentSlice.InstructionIndex >= 0)
            {
                _currentSlice.CurrentInstruction = _currentSlice.CurrentVertex.Instructions[_currentSlice.InstructionIndex];
                return true;
            }

            if (IsTryCatchInNotEntryPointCase)
                return MoveToPredecessorVertex();

            if (IsBranchingLavelInNotEntryPointCase)
                return ProcessingBranchingInstruction();

            if (IsEntryPointCaseWithTrackingItsParameter)
                return ExpandToInEdgesForEntryPoint(_currentSlice.CurrentVertex, ref searchFinished);

            // We do not deal with entry points types of instruction here, they need special attention.
            return false;
        }

        /// <summary>
        /// Returns true if the current instruction is Try/Catch case, which are not an entry point
        /// </summary>
        private bool IsTryCatchInNotEntryPointCase =>
            _currentSlice.CurrentInstruction != null &&
            !_currentSlice.CurrentVertex.IsEntryPoint &&
            (
                _currentSlice.CurrentInstruction.InstructionType == ESmaliInstruction.LabelTryEnd ||
                _currentSlice.CurrentInstruction.InstructionType == ESmaliInstruction.LabelTryStart
            );

        /// <summary>
        /// Returns true if the current instruction is a branching label or is not an entry point.
        /// </summary>
        private bool IsBranchingLavelInNotEntryPointCase =>
            !_currentSlice.CurrentVertex.IsEntryPoint || 
            (
                _currentSlice.CurrentInstruction != null && 
                _currentSlice.CurrentInstruction.IsBranchingLabel
            );

        /// <summary>
        /// Returns tru if the current vertex is an entry point and we are tracking a parameter.
        /// </summary>
        private bool IsEntryPointCaseWithTrackingItsParameter =>
            _currentSlice.CurrentVertex.IsEntryPoint &&
            _currentSlice.TargerRegister.IsParameter;

        /// <summary>
        /// Moves the current slice to predecessor
        /// </summary>
        /// <returns>true if move was successful, false otherwise</returns>
        private bool MoveToPredecessorVertex()
        {
            // If we meet try-start/end labels, then just use the predecessor
            if (_currentSlice.CurrentVertex.Predecessor != null)
            {
                _currentSlice.AddPredecessorVertex();
                return true;
            }
            return false;

        }

        /// <summary>
        /// Process branching logic, so that we after
        /// </summary>
        /// <returns></returns>
        private bool ProcessingBranchingInstruction()
        {
            if (_currentSlice.CurrentVertex.Predecessor == null && _currentSlice.CurrentVertex.EdgeIncomingVertex.Count == 0)
                return false;

            // Simple case, only predecessor exist, or only one in-edge
            if (_currentSlice.CurrentVertex.Predecessor != null && _currentSlice.CurrentVertex.EdgeIncomingVertex.Count == 0)
            {
                _currentSlice.AddPredecessorVertex();
                return true;
            }
            if (_currentSlice.CurrentVertex.Predecessor != null && _currentSlice.CurrentVertex.EdgeIncomingVertex.Count > 0)
            {
                // We will consider all InVertices later
                CreateNewSlicesToConsiderFromInEdgesForCurrentVertex(_currentSlice.CurrentVertex, false);
                _currentSlice.AddPredecessorVertex();
                return true;
            }
            if (_currentSlice.CurrentVertex.Predecessor == null && _currentSlice.CurrentVertex.EdgeIncomingVertex.Count >= 1)
            {
                // We will consider all but first InVertices later
                CreateNewSlicesToConsiderFromInEdgesForCurrentVertex(_currentSlice.CurrentVertex, true);
                // Use the first InVertex as the continuation
                ModifySliceWithInEdgeVertex(ref _currentSlice, _currentSlice.CurrentVertex, 0);
                // We should be good with instruction
                return true;
            }
            return false;
        }

        /// <summary>
        /// Expands to all in-edges that we met
        /// </summary>
        /// <param name="searchFinished">Shows whether or not the search is finished</param>
        /// <param name="vertex">The vertex for which we consider the in-degree edges</param>
        /// <returns>true if we managed to find the next instruction</returns>
        private bool ExpandToInEdgesForEntryPoint(CfgVertex vertex, ref bool searchFinished)
        {
            // First, if we do not have any in-edges, process all files that have calls into that entry point
            // in order to make sure that we uncovered all in-edges
            if (vertex.EdgeIncomingVertex.Count == 0)
            {
                ProcessFilesForInvokation(vertex.Name);
                // If we still came empty handed, then this is a dead-code, i.e., no one actually calls it.
                if (vertex.EdgeIncomingVertex.Count == 0)
                {
                    // We done with search
                    searchFinished = true;
                    // We failed to find an instruction
                    return false;
                }
            }

            // Create new slices to consider for future (all they get into _slicesToConsider array)
            CreateNewSlicesToConsiderFromInEdgesForCurrentVertex(vertex, true);

            // Modify the current slice with continuation point
            if (!ModifySliceWithInEdgeVertex(ref _currentSlice, vertex, 0))
            {
                searchFinished = true;
                return false;
            }

            // If we are here, this means we found the instruction
            return true;

        }

        #endregion


        #region Slice Initialization

        /// <summary>
        /// Initializes the array of slices to consider.
        /// </summary>
        /// <param name="registerToTrack">An index of the register to track. -1 stands for tracking return value of current invoke function</param>
        private void InitializeSlicesToConsider(int registerToTrack)
        {
            var slice =
                new ProgramSliceState
                {
                    CurrentVertex = EntryPointInstruction.ParentVertex,
                    InstructionIndex = EntryPointInstruction.ParentIndex,
                    Instructions = new List<SmaliCfgInstruction> {EntryPointInstruction}
                };
            if (registerToTrack >= 0)
            {
                slice.TargerRegister = EntryPointInstruction.ArgsRegs[registerToTrack].Copy();
            }
            else
            {
                slice.TargerRegister = new DalvikRegister();
                slice.IsTrackingNextMoveResultInstructionRegister = true;
            }
            slice.CurrentInstruction = EntryPointInstruction;
            _slicesToConsider = new List<ProgramSliceState> { slice };
        }

        /// <summary>
        /// Dequeues the next program slice for processing
        /// </summary>
        private void DequeueNextSlice()
        {
            if (_slicesToConsider.Count == 0)
            {
                _currentSlice = null;
                return;
            }
            _currentSlice = _slicesToConsider.First();
            _slicesToConsider.RemoveAt(0);
        }

        #endregion


        #region Slice expansions functions

        /// <summary>
        /// Expands the slices to consider array with all additional in-edges, skipping the first one
        /// since it will be used as a continuation point for current slice.
        /// </summary>
        private void CreateNewSlicesToConsiderFromInEdgesForCurrentVertex(CfgVertex vertex, bool skipFirstOne)
        {
            var iMax = skipFirstOne ? 1 : 0;
            for (var i = iMax; i < vertex.EdgeIncomingVertex.Count; i++)
            {
                var newSlice = _currentSlice.CreateCopy();
                // Now add the slice for consideration
                if (ModifySliceWithInEdgeVertex(ref newSlice, vertex, i))
                {
                    _slicesToConsider.Add(newSlice);
                }
            }
        }

        /// <summary>
        /// Modifies provided slice with expansion on in the specific in-edge.
        /// </summary>
        /// <param name="slice">The slice to modify</param>
        /// <param name="inEdgeIndex">The index of the in-edge</param>
        /// <param name="vertex">The vertex for which we consider in-edge</param>
        /// <returns>True if modification was successful, false otherwise</returns>
        private bool ModifySliceWithInEdgeVertex(ref ProgramSliceState slice, CfgVertex vertex, int inEdgeIndex)
        {
            // Get in Vertext and instruction
            var inVertex = vertex.EdgeIncomingVertex[inEdgeIndex];
            var inIndex = vertex.EdgeIncomingVertexInstruction[inEdgeIndex];

            // This is try/catch case, which we do not follow as of now
            if (inIndex == -1 || (inVertex.Instructions.Count <= inIndex && inIndex >= 0))
            {
                return false;
            }

            // Check if the vertext is an entry-point, if so, then it means we invoked a funciton in predecessor
            slice.InjectVertexInPathHeadForBackwardWalk(inVertex);

            // Fix the new slice so that it begins from a proper instruction, the instrucation that causes the jump
            slice.InstructionIndex = inIndex;
            slice.CurrentInstruction = inVertex.Instructions[inIndex];

            // Setup which registor we need to track now
            var argIndex = _currentSlice.TargerRegister.N;

            // This means we need to replace TargerRegister with a Args
            if (_currentSlice.TargerRegister.IsParameter && vertex.IsEntryPoint)
            {
                // Modify argIndex for the case of static invokation
                if (slice.CurrentInstruction.IsInstanceInvoke)
                    argIndex += 1; // Skip the *this* parameter, which is usually p0

                if (argIndex >= slice.CurrentInstruction.ArgsRegs.Length)
                {
                    //TODO(ildarm): We will fix this bug later, seems to be rare
                    return false;
                }
                slice.TargerRegister = slice.CurrentInstruction.ArgsRegs[argIndex].Copy();
            }

            // Add current instruction, since we are going to skip it on FindPrevInstruction
            slice.Instructions.Add(slice.CurrentInstruction);

            // Now add the slice for consideration
            return true;

        }

        /// <summary>
        /// Modifies provided slice with specific return point for an entry point
        /// </summary>
        /// <param name="slice">The slice to modify</param>
        /// <param name="vertex">Entry-point vertex</param>
        /// <param name="inEdgeIndex">Index of the return point</param>
        /// <returns></returns>
        private void ModifySliceWithReturnVertex(ref ProgramSliceState slice, CfgVertex vertex, int inEdgeIndex)
        {
            // Get in Vertext and instruction
            var retVertex = vertex.ReturnVertices[inEdgeIndex];

            // Check if the vertext is an entry-point, if so, then it means we invoked a funciton in predecessor
            slice.InjectVertexInPathHeadForBackwardWalk(retVertex);
            slice.Instructions.Add(slice.CurrentInstruction);

            if (slice.CurrentInstruction.IsReturn)
            {
                // We are focusing on return values now
                slice.TargerRegister = slice.CurrentInstruction.Src.Copy();
            }
            else
            {
                throw new Exception("Return op without return???");
            }
        }

        #endregion


        #region Slicing single slice

        /// <summary>
        /// Processes MoveResult instruction, which must be paired with an invoke instruction
        /// </summary>
        /// <param name="searchFinished">Flag that shows whether the current slice is done</param>
        private void ProcessMoveResult(ref bool searchFinished)
        {
            // Find previous instruction, make sure it is an invoke one
            bool foundInvoke = false;
            while (!foundInvoke)
            {
                bool instructionFound = FindPreviousInstruction(ref searchFinished);
                if (!instructionFound)
                {
                    // This should not happen since 
                    throw new Exception("Should not happen.");
                }
                // It might be that our call is wrapped around try/catch, so that we need to back-track a bit more

                foundInvoke = _currentSlice.CurrentInstruction.IsInvoke ||
                              _currentSlice.CurrentInstruction.IsInvokeRange;
            }

            // Don't forget to add invokation to slice
            if (_currentSlice.CurrentInstruction.IsInvoke)
                _currentSlice.Instructions.Add(_currentSlice.CurrentInstruction);
            
            // If the function is worthy, then lets try to find its body, but ignore a wellknown Java APIs
            if (_currentSlice.CurrentInstruction.IsWellKnownApi)
            {
                ProcessMoveResultForWellKnownApi(ref searchFinished);
            }
            else
            {
                ProcessMoveResultForInternalCode(ref searchFinished);
            }

        }

        /// <summary>
        /// Processes move result operation further into the function we call.
        /// </summary>
        /// <param name="searchFinished">Flag that shows whether we can't search anymore</param>
        private void ProcessMoveResultForInternalCode(ref bool searchFinished)
        {
            // Process all files for the function we are stepping in
            ProcessFilesForInvokation(_currentSlice.CurrentInstruction.Function);
            // Try to locate the entry point for the function
            var functionEntryPoint = 
                AnalysisState.ControlFlowGraph.GetEntryPointVertexByName(_currentSlice.CurrentInstruction.Function, false);
            // Sanity check, should not really happen if this is an internal code, but sometimes does.
            if (functionEntryPoint == null || functionEntryPoint.Instructions.Count == 0)
            {
                // Well, we did the best we can, can't find the function definition
                searchFinished = true;
                // Add current slice to results
                _slicingResults.Add(_currentSlice);
            }
            else // If we do have the entry point
            {
                // Make sure we have something that we return
                if (functionEntryPoint.ReturnVertices.Count == 0)
                {
                    // This is totally legit, since there are functions that only throw
                    // TODO(ildarm): Consider adding support to throwing functions
                    searchFinished = true;
                    _slicingResults.Add(_currentSlice);
                    return;
                }
                
                // If we have proper return statements, add all after the first as possible routes for extension
                for (int i = 1; i < functionEntryPoint.ReturnVertices.Count; i++)
                {
                    var newSlice = _currentSlice.CreateCopy();
                    ModifySliceWithReturnVertex(ref newSlice, functionEntryPoint, i);
                    _slicesToConsider.Add(newSlice);
                }
                
                // Use the first one as continuation point
                ModifySliceWithReturnVertex(ref _currentSlice, functionEntryPoint, 0);
            }

        }

        /// <summary>
        /// Processes move result operation for a well-known API.
        /// A well-known API is an API which belongs to Java standard, but we are still interested into where the data for it came from.
        /// </summary>
        /// <param name="searchFinished">Flag that shows whether we can't search anymore</param>
        private void ProcessMoveResultForWellKnownApi(ref bool searchFinished)
        {
            if (_currentSlice.CurrentInstruction.ShouldFollowTheObject && _currentSlice.CurrentInstruction.IsInvoke)
            {
                // Basically, now we observing how the objects behave
                // Get the *this* object pointer
                _currentSlice.TargerRegister = _currentSlice.CurrentInstruction.ArgsRegs[0].Copy(); 
                
                // If the current target register a parameter? If so, then we need to go up the stack to get where it came from
                if (_currentSlice.TargerRegister.IsParameter)
                {
                    // Get current EP and instruction
                    var instr = _currentSlice.CurrentInstruction;
                    var entryPointVertex = instr.ParentEntryPointVertex;

                    // Try to process all files where our EP is referred to
                    ProcessFilesForInvokation(entryPointVertex.Name);
                    // If we weren't able to find one, then stop search, since our EP is not called.
                    if (entryPointVertex.EdgeIncomingVertex.Count == 0)
                    {
                        // TODO(ildarm): Should be considered a dead end.
                        searchFinished = true;
                        _slicingResults.Add(_currentSlice);
                    }
                    else
                    {
                        // If we fonud proper in-edges, then expand.
                        CreateNewSlicesToConsiderFromInEdgesForCurrentVertex(entryPointVertex, true);
                        if (!ModifySliceWithInEdgeVertex(ref _currentSlice, entryPointVertex, 0))
                        {
                            searchFinished = true;
                            _slicesToConsider.Add(_currentSlice);
                        }
                    }
                }
            }
            else
            {
                // Yes, this is a wellknown API, but we are not going to follow it.
                searchFinished = true;
                _slicingResults.Add(_currentSlice);
            }
        }

        #endregion

    }
}
