using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.ContentParsing.SmaliParser
{
    /// <summary>
    /// Smali parser that converts smali instructions into structures and CFG
    /// </summary>
    public partial class SmaliParser
    {

        private const bool _printInfoMessages = false;

        #region Public functions

        public void ExtractAllVertices(Cfg cfg, string content)
        {
            ProcessSmaliFile(content);

            foreach (var javaTypeMethod in JavaType.Methods)
            {
                ProcessMethod(ref cfg, javaTypeMethod);
            }
        }

        #endregion


        #region Processing logic

        /// <summary>
        /// The main logic for processing a java function and converting it to a CFG.
        /// </summary>
        /// <param name="cfg">The CFG to which we will add the function graph</param>
        /// <param name="method">The function to be processed</param>
        protected void ProcessMethod(ref Cfg cfg, JavaTypeMethod method)
        {
            var methodEntryPoint = cfg.GetVertexByName(method.SmaliName, "", false);

            // No need to process the method, since we have it already
            if (methodEntryPoint != null && methodEntryPoint.AllInstructions.Count > 0) return;
            methodEntryPoint = cfg.GetVertexByName(method.SmaliName, "");
            methodEntryPoint.Predecessor = null;
            methodEntryPoint.IsEntryPoint = true;

            var currentVertex = methodEntryPoint;
            bool forceNewVertex = false;
            bool setPredecessor = true;
            CfgVertex invokeCall = null;
            var catchBlocks = new Dictionary<string, List<CfgVertex>>();
            var catchBlocksEndLabels = new Dictionary<string, string>();
            List<CfgVertex> currentIncomingCatchBlock = null;
            CfgVertex switchDataBlock = null;

            for (int lineIndex = 1; lineIndex < method.CodeLines.Count - 1; lineIndex++)
            {
                // Skip all empty lines (no need in them)
                if (string.IsNullOrWhiteSpace(method.CodeLines[lineIndex])) continue;

                // Convert line into an instruction object
                var instruction = GetInstruction(method.CodeLines[lineIndex]);

                // Set the entry point if needed
                if (currentVertex.EntryPointVertex == null)
                {
                    currentVertex.EntryPointVertex = methodEntryPoint;
                }

                // Keep this while we still not supporting all types of smali code,
                // some of the mnemonics are not parsed, since they have no impact on CFG, but might be useful in future.
                if (instruction == null)
                {
                    if (_printInfoMessages)
                    {
                        Console.WriteLine($"Cannot parse instruction: '{method.CodeLines[lineIndex]}'");
                    }
                    continue;
                }

                instruction.InstructionIndexInMethod = lineIndex;
                methodEntryPoint.AllInstructions.Add(lineIndex, instruction);

                // Check if we are forced to create a new vertex
                if (forceNewVertex)
                {
                    forceNewVertex = false;
                    CreateNewVertex(cfg, method, instruction, lineIndex, ref currentVertex, ref invokeCall, ref setPredecessor);
                }

                // Handle try catch labels (try_start/try_end)
                if (instruction.InstructionType == ESmaliInstruction.LabelTryStart)
                {
                    CreateNewCatchBlock(instruction, catchBlocks, catchBlocksEndLabels);
                }
                else if (instruction.InstructionType == ESmaliInstruction.LabelTryEnd)
                {
                    currentIncomingCatchBlock = GetIncomingCatchBlockForTryEndLabel(instruction, catchBlocks, catchBlocksEndLabels);
                }

                // Handle new block for packed/spare switches
                if (IsSwitchBlockBeginLabel(instruction))
                {
                    switchDataBlock = currentVertex;
                }
                else if (IsSwitchBlockEndLabel(instruction))
                {
                    switchDataBlock = null;
                }

                // Handle the instruction
                if (IsIfStatement(instruction) || IsSwitchStatement(instruction))
                {
                    // If and switch can both fall through, hence they are handled similarly
                    AddInstructionToVertext(currentVertex, instruction, method, lineIndex);
                    AddInstructionToCatchBlocks(ref catchBlocks, instruction, currentVertex);
                    CreateNewLabeledVertexForBranching(cfg, method, instruction, currentVertex);
                }
                else if (IsGotoStatement(instruction))
                {
                    forceNewVertex = true;
                    setPredecessor = false;
                    AddInstructionToVertext(currentVertex, instruction, method, lineIndex);
                    AddInstructionToCatchBlocks(ref catchBlocks, instruction, currentVertex);
                    CreateNewLabeledVertexForBranching(cfg, method, instruction, currentVertex);
                }
                else if (IsInvokeStatement(instruction))
                {
                    forceNewVertex = true;
                    AddInstructionToVertext(currentVertex, instruction, method, lineIndex);
                    AddInstructionToCatchBlocks(ref catchBlocks, instruction, currentVertex);
                    invokeCall = CreateNewLabeledVertexForInvoke(cfg, currentVertex, instruction);
                    AddInstructionToCatchBlocks(ref catchBlocks, instruction, invokeCall);
                }
                else if (IsLabelStatement(instruction) || IsSwitchDataLabel(instruction) ||
                    (switchDataBlock == null && IsSwitchLabel(instruction)))
                {
                    CreateNewLabelVertext(cfg, method, instruction, ref currentVertex);
                    // Add instruction after we swapped the vertices
                    AddInstructionToVertext(currentVertex, instruction, method, lineIndex);
                    AddInstructionToCatchBlocks(ref catchBlocks, instruction, currentVertex);
                    // Try catch specific stuff
                    if (instruction.InstructionType == ESmaliInstruction.LabelTryStart ||
                        instruction.InstructionType == ESmaliInstruction.LabelTryEnd)
                    {
                        ProcessCurrentIncomingCatchBlock(instruction, currentVertex, currentIncomingCatchBlock);
                        currentIncomingCatchBlock = null;
                    }
                }
                else if (IsReturnStatement(instruction))
                {
                    forceNewVertex = true;
                    setPredecessor = false;
                    AddInstructionToVertext(currentVertex, instruction, method, lineIndex);
                    AddInstructionToCatchBlocks(ref catchBlocks, instruction, currentVertex);
                    methodEntryPoint.ReturnVertices.Add(currentVertex);
                }
                else if (IsCatchStatement(instruction))
                {
                    // We forcing new vertex, since catch is branching opcode
                    forceNewVertex = true;
                    CreateNewLabeledVertexForBranching(cfg, method, instruction, currentVertex);
                    AddInstructionToVertext(currentVertex, instruction, method, lineIndex);
                    AddInstructionToCatchBlocks(ref catchBlocks, instruction, currentVertex);
                }
                else
                {
                    AddInstructionToVertext(currentVertex, instruction, method, lineIndex);
                    AddInstructionToCatchBlocks(ref catchBlocks, instruction, currentVertex);

                    if (switchDataBlock != null && IsSwitchLabel(instruction))
                    {
                        CreateNewLabeledVertexForBranching(cfg, method, instruction, currentVertex);
                    }
                    // Create Field (i.e., property) if needed
                    if (IsGetStatement(instruction) || IsPutStatement(instruction))
                    {
                        CreateNewFieldVertex(cfg, instruction, currentVertex);
                    }
                }

                methodEntryPoint.InstructionInVertex.Add(lineIndex, currentVertex);
                instruction.ParentEntryPointIndex = methodEntryPoint.AllInstructions.Count - 1;
                instruction.ParentEntryPointVertex = methodEntryPoint;

            }
        }

        /// <summary>
        /// Processes the current try/catch block and sets extra edge of possible jumps
        /// </summary>
        /// <param name="instruction">The current instruction (should be :try_endXYZ)</param>
        /// <param name="currentVertex">Current vertex (should be with a single label)</param>
        /// <param name="currentIncomingCatchBlock">The collection of all vertices in the current try/catch block</param>
        private static void ProcessCurrentIncomingCatchBlock(SmaliCfgInstruction instruction, CfgVertex currentVertex, List<CfgVertex> currentIncomingCatchBlock)
        {
            // Special treatment for try_end label, since we need to remove the catch block for it
            if (instruction.InstructionType == ESmaliInstruction.LabelTryEnd)
            {
                // We should have that array set, otherwise it means we saw try_end before try_start, which is illogical
                if (currentIncomingCatchBlock == null)
                {
                    throw new NullReferenceException("Do not have array of incoming edges to try_end label");
                }
                // Since we can jump to that label from any instruction in the block, add an edge from each vertex to the incoming edges
                foreach (var incomingVertex in currentIncomingCatchBlock)
                {
                    // If the vertex is in another function, then add only one link
                    if (incomingVertex.Name != currentVertex.Name)
                    {
                        currentVertex.AddIncomingEdge(incomingVertex, -1); // -1 simply means, any instructions
                    }
                    else
                    {
                        for (int i = 0; i < incomingVertex.Instructions.Count; i++)
                        {
                            currentVertex.AddIncomingEdge(incomingVertex, i);
                        }
                    }
                 
                    // Add all vertices where we might jump (at least we can)
                    incomingVertex.CatchVertices.Add(currentVertex);
                }
            }
            
        }

        /// <summary>
        /// Create a new invoke type of vertex (i.e., an entry point type where we call into)
        /// </summary>
        /// <param name="cfg">The CFG we are working on </param>
        /// <param name="currentVertex">Current vertex</param>
        /// <param name="instruction">Current instruction</param>
        /// <returns>Returns new invoke vertex</returns>
        private static CfgVertex CreateNewLabeledVertexForInvoke(Cfg cfg, CfgVertex currentVertex, SmaliCfgInstruction instruction)
        {
            var labeledVertex = cfg.GetEntryPointVertexByName(instruction.Function);
            labeledVertex.AddIncomingEdge(currentVertex, currentVertex.Instructions.Count - 1);
            return labeledVertex;
        }

        /// <summary>
        /// Add instruction to the vertext
        /// </summary>
        /// <param name="vertex">Vertext to add instruction to</param>
        /// <param name="instruction">The instruction to add</param>
        /// <param name="method">Method to which this instruction and vertex belongs</param>
        /// <param name="lineIndex">Line index within the method</param>
        private static void AddInstructionToVertext(CfgVertex vertex, SmaliCfgInstruction instruction, JavaTypeMethod method, int lineIndex)
        {
            vertex.Instructions.Add(instruction);
            vertex.InstructionsCode += method.CodeLines[lineIndex].Trim() + Environment.NewLine;
            instruction.ParentVertex = vertex;
            instruction.ParentIndex = vertex.Instructions.Count - 1;
        }

        /// <summary>
        /// Create new in-function label used for branching into (e.g., goto)
        /// </summary>
        /// <param name="cfg">The CFG</param>
        /// <param name="method">Method that we process</param>
        /// <param name="instruction">Current instruction</param>
        /// <param name="currentVertex">Current vertex</param>
        private static void CreateNewLabeledVertexForBranching(Cfg cfg, JavaTypeMethod method, SmaliCfgInstruction instruction, CfgVertex currentVertex)
        {
            var labeledVertex = cfg.GetVertexByName(method.SmaliName, instruction.Label);
            labeledVertex.AddIncomingEdge(currentVertex, currentVertex.Instructions.Count - 1);
        }

        /// <summary>
        /// Create new in class vertext for field to track get/set data flows
        /// </summary>
        /// <param name="cfg">The CFG</param>
        /// <param name="instruction">Current instruction</param>
        /// <param name="currentVertex">Current vertex</param>
        private static void CreateNewFieldVertex(Cfg cfg, SmaliCfgInstruction instruction, CfgVertex currentVertex)
        {
            var labeledVertex = cfg.GetFieldVertexByName(instruction.Field);
            if (IsGetStatement(instruction))
            {
                labeledVertex.AddOutgoingEdge(currentVertex, currentVertex.Instructions.Count - 1);
            }
            else
            {
                labeledVertex.AddIncomingEdge(currentVertex, currentVertex.Instructions.Count - 1);
            }
        }

        /// <summary>
        /// Creates a new label type of vertex
        /// </summary>
        /// <param name="cfg">The CFG</param>
        /// <param name="method">Method that we are processing</param>
        /// <param name="instruction">Instruction</param>
        /// <param name="currentVertex">Current vertext</param>
        private void CreateNewLabelVertext(
                    Cfg cfg, 
                    JavaTypeMethod method,
                    SmaliCfgInstruction instruction,
                    ref CfgVertex currentVertex)
        {
            CfgVertex labeledVertex = null;
            // Check if the current vertex is an empty entry point (i.e., this label is the first instruction of the function)
            if (currentVertex.Instructions.Count == 0)
            {
                labeledVertex = currentVertex;
                labeledVertex.Label = instruction.Label;
            }
            else
            {
                // Get the vertex, create new, if required
                labeledVertex = cfg.GetVertexByName(method.SmaliName, instruction.Label);
            }

            // We might have set this already, so skip setting these up
            // This happens if say a label is the first instruction of the method. Then, we should have created it already.
            // Another case, is if we were forced to create a new vertext, e.g., previous intruction was return.
            if (!labeledVertex.Equals(currentVertex))
            {
                currentVertex.Successor = labeledVertex;
                labeledVertex.Predecessor = currentVertex;
                currentVertex = labeledVertex;
            }
        }

        /// <summary>
        /// Retrives current incoming catch block for :try_end label. This block contains all vertices that might eventually jump to our :try_end label
        /// </summary>
        /// <param name="instruction">The instruction</param>
        /// <param name="catchBlocks">Catch blocks collection</param>
        /// <param name="catchBlocksEndLabels">Catch blocks :try_start labels collection</param>
        private static List<CfgVertex> GetIncomingCatchBlockForTryEndLabel(
                    SmaliCfgInstruction instruction, 
                    Dictionary<string, List<CfgVertex>> catchBlocks,
                    Dictionary<string, string> catchBlocksEndLabels)
        {
            // The logic is to remove the label and the array from current caches, yet, return the selected array.
            // NOTE: This function does not check if the key exists, since such case would be considered as we have a bogus smali code
            var catchBlockStartLabel = catchBlocksEndLabels[instruction.Label];
            catchBlocksEndLabels.Remove(instruction.Label);
            var currentIncomingCatchBlock = catchBlocks[catchBlockStartLabel];
            catchBlocks.Remove(catchBlockStartLabel);
            return currentIncomingCatchBlock;
        }

        /// <summary>
        /// Creates new catch block, where we put all vertices that might jump to the :try_end label, so we have to add that edge for each of them
        /// </summary>
        /// <param name="instruction">The instruction</param>
        /// <param name="catchBlocks">Catch blocks collection</param>
        /// <param name="catchBlocksEndLabels">Catch blocks :try_start labels collection</param>
        private static void CreateNewCatchBlock(
                    SmaliCfgInstruction instruction, 
                    Dictionary<string, List<CfgVertex>> catchBlocks,
                    Dictionary<string, string> catchBlocksEndLabels)
        {
            catchBlocks.Add(instruction.Label, new List<CfgVertex>());
            catchBlocksEndLabels.Add(instruction.Label.Replace("start", "end"), instruction.Label);
        }

        /// <summary>
        /// Creates new vertex
        /// </summary>
        /// <param name="cfg">The CFG</param>
        /// <param name="method">Method we are processing</param>
        /// <param name="instruction">Instruction</param>
        /// <param name="lineIndex">Instruction code line index within the method</param>
        /// <param name="currentVertex">Current vertex</param>
        /// <param name="invokeCall">The previous invoke-kind vertex</param>
        /// <param name="setPredecessor">Flag that shows whether we need set predecessor</param>
        /// <returns></returns>
        private void CreateNewVertex(
                    Cfg cfg,
                    JavaTypeMethod method,
                    SmaliCfgInstruction instruction,
                    int lineIndex,
                    ref CfgVertex currentVertex,
                    ref CfgVertex invokeCall,
                    ref bool setPredecessor)
        {
            // For all legit label types of instructions, we use label itself as within method location (e.g., :goto, :cond)
            // Otherwise we use codeline index as so called "label"
            string labeledName = IsLabelStatement(instruction)
                ? instruction.Label
                : $"{lineIndex}";
            // Get labeled vertext (create new vertext, if needed)
            var labeledVertex = cfg.GetVertexByName(method.SmaliName, labeledName);
            // Set predecessor only if needed. Some instructions, e.g., return(*), goto cannot be predecessor, since they exit the function and break "natural" code flow.
            if (setPredecessor)
            {
                currentVertex.Successor = labeledVertex;
                labeledVertex.Predecessor = currentVertex;
            }

            currentVertex = labeledVertex;
            setPredecessor = true;

            // If the last command was invoke-kind, then add return edge
            if (invokeCall != null)
            {
                invokeCall.EdgeReturnVertex.Add(currentVertex);
                invokeCall = null;
            }
        }

        /// <summary>
        /// Adds current instruction to all catch blocks that are being monitored
        /// </summary>
        /// <param name="catchBlocks">Collection of catch blocks</param>
        /// <param name="instruction">Instruction</param>
        /// <param name="vertex">Current vertex</param>
        private void AddInstructionToCatchBlocks(ref Dictionary<string, List<CfgVertex>> catchBlocks, SmaliCfgInstruction instruction, CfgVertex vertex)
        {
            if (instruction.InstructionType == ESmaliInstruction.LabelTryEnd)
            {
                return;
            }
            foreach (var catchBlock in catchBlocks)
            {
                // Check if the vertex has not been added to the block
                // NOTE: It could be added if a vertex has more than one instruction
                var existingVertex =
                    catchBlock.Value.FirstOrDefault(v => v.Name == vertex.Name && v.Label == vertex.Label);
                if (existingVertex == null)
                {
                    catchBlock.Value.Add(vertex);
                }
            }
        }

        #endregion


        #region Instruction type checking functions

        /// <summary>
        /// Check if the instruction is an if statement
        /// </summary>
        public static bool IsIfStatement(SmaliCfgInstruction instruction) => 
            (
                instruction.InstructionType == ESmaliInstruction.IfEqual ||
                instruction.InstructionType == ESmaliInstruction.IfNotEqual ||
                instruction.InstructionType == ESmaliInstruction.IfLessOrEqual ||
                instruction.InstructionType == ESmaliInstruction.IfLessThan ||
                instruction.InstructionType == ESmaliInstruction.IfGreaterOrEqual ||
                instruction.InstructionType == ESmaliInstruction.IfGreaterThan ||
                instruction.InstructionType == ESmaliInstruction.IfEqualToZero ||
                instruction.InstructionType == ESmaliInstruction.IfNotEqualToZero ||
                instruction.InstructionType == ESmaliInstruction.IfLessOrEqualToZero ||
                instruction.InstructionType == ESmaliInstruction.IfLessThanToZero ||
                instruction.InstructionType == ESmaliInstruction.IfGreaterOrEqualToZero||
                instruction.InstructionType == ESmaliInstruction.IfGreaterThanToZero
            );

        /// <summary>
        /// Check if the instruction is a goto statement
        /// </summary>
        public static bool IsGotoStatement(SmaliCfgInstruction instruction) =>
            (
                instruction.InstructionType == ESmaliInstruction.Goto ||
                instruction.InstructionType == ESmaliInstruction.Goto16 ||
                instruction.InstructionType == ESmaliInstruction.Goto32
            );

        /// <summary>
        /// Check if the instruction is an invoke statement
        /// </summary>
        public static bool IsInvokeStatement(SmaliCfgInstruction instruction) =>
            (
                instruction.InstructionType == ESmaliInstruction.InvokeVirtual ||
                instruction.InstructionType == ESmaliInstruction.InvokeSuper ||
                instruction.InstructionType == ESmaliInstruction.InvokeDirect ||
                instruction.InstructionType == ESmaliInstruction.InvokeStatic ||
                instruction.InstructionType == ESmaliInstruction.InvokeInterface ||
                instruction.InstructionType == ESmaliInstruction.InvokeVirtualRange ||
                instruction.InstructionType == ESmaliInstruction.InvokeSuperRange ||
                instruction.InstructionType == ESmaliInstruction.InvokeDirectRange ||
                instruction.InstructionType == ESmaliInstruction.InvokeStaticRange ||
                instruction.InstructionType == ESmaliInstruction.InvokeInterfaceRange
            );

        /// <summary>
        /// Check if the instruction is a label statement
        /// </summary>
        protected static bool IsLabelStatement(SmaliCfgInstruction instruction) =>
        (
            instruction.InstructionType == ESmaliInstruction.LabelGoto ||
            instruction.InstructionType == ESmaliInstruction.LabelCond ||
            instruction.InstructionType == ESmaliInstruction.LabelCatch ||
            instruction.InstructionType == ESmaliInstruction.LabelCatchAll ||
            instruction.InstructionType == ESmaliInstruction.LabelTryStart ||
            instruction.InstructionType == ESmaliInstruction.LabelTryEnd ||
            instruction.InstructionType == ESmaliInstruction.LabelPSwitchData ||
            instruction.InstructionType == ESmaliInstruction.LabelSSwitchData
        );

        /// <summary>
        /// Check if the instruction is a return statement
        /// </summary>
        public static bool IsReturnStatement(SmaliCfgInstruction instruction) =>
        (
            instruction.InstructionType == ESmaliInstruction.ReturnObject ||
            instruction.InstructionType == ESmaliInstruction.Return ||
            instruction.InstructionType == ESmaliInstruction.ReturnVoid ||
            instruction.InstructionType == ESmaliInstruction.ReturnWide
        );

        /// <summary>
        /// Check if the instruction is a catch statement
        /// </summary>
        public static bool IsCatchStatement(SmaliCfgInstruction instruction) =>
        (
            instruction.InstructionType == ESmaliInstruction.Catch ||
            instruction.InstructionType == ESmaliInstruction.CatchAll
        );

        /// <summary>
        /// Check if the instruction is a put statement
        /// </summary>
        public static bool IsPutStatement(SmaliCfgInstruction instruction) =>
            (
                // NOTE: We do not include aput-kind opcodes here, since they do not get arrays.
                instruction.InstructionType == ESmaliInstruction.InstancePut ||
                instruction.InstructionType == ESmaliInstruction.InstancePutWide ||
                instruction.InstructionType == ESmaliInstruction.InstancePutObject ||
                instruction.InstructionType == ESmaliInstruction.InstancePutBoolean ||
                instruction.InstructionType == ESmaliInstruction.InstancePutByte ||
                instruction.InstructionType == ESmaliInstruction.InstancePutChar ||
                instruction.InstructionType == ESmaliInstruction.InstancePutShort ||
                instruction.InstructionType == ESmaliInstruction.StaticPut ||
                instruction.InstructionType == ESmaliInstruction.StaticPutWide ||
                instruction.InstructionType == ESmaliInstruction.StaticPutObject ||
                instruction.InstructionType == ESmaliInstruction.StaticPutBoolean ||
                instruction.InstructionType == ESmaliInstruction.StaticPutByte ||
                instruction.InstructionType == ESmaliInstruction.StaticPutChar ||
                instruction.InstructionType == ESmaliInstruction.StaticPutShort
            );

        /// <summary>
        /// Check if the instruction is a set statement
        /// </summary>
        public static bool IsGetStatement(SmaliCfgInstruction instruction) =>
            (
                // NOTE: We do not include aget-kind opcodes here, since they do not get arrays.
                instruction.InstructionType == ESmaliInstruction.InstanceGet ||
                instruction.InstructionType == ESmaliInstruction.InstanceGetWide ||
                instruction.InstructionType == ESmaliInstruction.InstanceGetObject ||
                instruction.InstructionType == ESmaliInstruction.InstanceGetBoolean ||
                instruction.InstructionType == ESmaliInstruction.InstanceGetByte ||
                instruction.InstructionType == ESmaliInstruction.InstanceGetChar ||
                instruction.InstructionType == ESmaliInstruction.InstanceGetShort ||
                instruction.InstructionType == ESmaliInstruction.StaticGet ||
                instruction.InstructionType == ESmaliInstruction.StaticGetWide ||
                instruction.InstructionType == ESmaliInstruction.StaticGetObject ||
                instruction.InstructionType == ESmaliInstruction.StaticGetBoolean ||
                instruction.InstructionType == ESmaliInstruction.StaticGetByte ||
                instruction.InstructionType == ESmaliInstruction.StaticGetChar ||
                instruction.InstructionType == ESmaliInstruction.StaticGetShort
            );

        /// <summary>
        /// Check if the current instruction is a switch statement.
        /// </summary>
        public static bool IsSwitchStatement(SmaliCfgInstruction instruction) =>
                    instruction.InstructionType == ESmaliInstruction.PackedSwitch ||
                    instruction.InstructionType == ESmaliInstruction.SparseSwitch;

        /// <summary>
        /// Check if the current instruction is a switch data label.
        /// </summary>
        public static bool IsSwitchDataLabel(SmaliCfgInstruction instruction) =>
                    instruction.InstructionType == ESmaliInstruction.LabelPSwitchData ||
                    instruction.InstructionType == ESmaliInstruction.LabelSSwitchData;

        /// <summary>
        /// Check if the current instruction is a switch label.
        /// </summary>
        public static bool IsSwitchLabel(SmaliCfgInstruction instruction) =>
                    instruction.InstructionType == ESmaliInstruction.LabelPSwitch ||
                    instruction.InstructionType == ESmaliInstruction.LabelSSwitch;

        /// <summary>
        /// Check if the current instruction is the beginning of switch block.
        /// </summary>
        protected static bool IsSwitchBlockBeginLabel(SmaliCfgInstruction instruction) =>
            instruction.InstructionType == ESmaliInstruction.PackedSwitchBegin ||
            instruction.InstructionType == ESmaliInstruction.SparseSwitchBegin;

        /// <summary>
        /// Check if the current instruction is the end of a switch block.
        /// </summary>
        /// <param name="instruction"></param>
        /// <returns></returns>
        public static bool IsSwitchBlockEndLabel(SmaliCfgInstruction instruction) =>
            instruction.InstructionType == ESmaliInstruction.PackedSwitchEnd ||
            instruction.InstructionType == ESmaliInstruction.SparseSwitchEnd;

        /// <summary>
        /// Get instruction from a code line
        /// </summary>
        /// <param name="codeLine">Code line</param>
        /// <returns>Return structure</returns>
        protected SmaliCfgInstruction GetInstruction(string codeLine)
        {
            var v = new SmaliCfgInstruction {CodeLine = codeLine};
            if (ParseMoveInstruction(codeLine, ref v)) return v;
            if (ParseArrayLengthInstruction(codeLine, ref v)) return v;
            if (ParseArrayOpInstruction(codeLine, ref v)) return v;
            if (ParseBinaryOperationAddr2Instruction(codeLine, ref v)) return v;
            if (ParseBinaryOperationInstruction(codeLine, ref v)) return v;
            if (ParseBinaryOperationLiteralInstruction(codeLine, ref v)) return v;
            if (ParseCatchInstruction(codeLine, ref v)) return v;
            if (ParseCheckCastInstruction(codeLine, ref v)) return v;
            if (ParseCompareInstruction(codeLine, ref v)) return v;
            if (ParseConstInstruction(codeLine, ref v)) return v;
            if (ParseFillArrayDataInstruction(codeLine, ref v)) return v;
            if (ParseFilledNewArrayInstruction(codeLine, ref v)) return v;
            if (ParseGotoInstruction(codeLine, ref v)) return v;
            if (ParseInstanceOpInstruction(codeLine, ref v)) return v;
            if (ParseInstanceOfInstruction(codeLine, ref v)) return v;
            if (ParseIfTestInstruction(codeLine, ref v)) return v;
            if (ParseCompareInstruction(codeLine, ref v)) return v;
            if (ParseCatchInstruction(codeLine, ref v)) return v;
            if (ParseConstInstruction(codeLine, ref v)) return v;
            if (ParseLabel(codeLine, ref v)) return v;
            if (ParseSSwitchLabel(codeLine, ref v)) return v;
            if (ParseSwitchInstruction(codeLine, ref v)) return v;
            if (ParseSwitchBlockInstruction(codeLine, ref v)) return v;
            if (ParseUnaryOperationInstruction(codeLine, ref v)) return v;
            if (ParseMonitorInstruction(codeLine, ref v)) return v;
            if (ParseNewArrayInstruction(codeLine, ref v)) return v;
            if (ParseReturnInstruction(codeLine, ref v)) return v;
            if (ParseThrowInstruction(codeLine, ref v)) return v;
            if (ParseInvokeRangeInstruction(codeLine, ref v)) return v;
            if (ParseInvokeInstruction(codeLine, ref v)) return v;
            if (ParseNewInstanceInstruction(codeLine, ref v)) return v;
            if (ParseStaticOpInstruction(codeLine, ref v)) return v;

            return null;
        }

        #endregion


        #region Parsing out instructions

        /// <summary>
        /// Parses out 'move*' operations. Sets Src and Dest registers Ids. Also sets type (v or p).
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is a move instruction.</returns>
        protected bool ParseMoveInstruction(string codeLine , ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>move([-a-z0-9]+)?(/[a-z0-9]+)?)\s(?<dest>[pv]\d+)(,\s(?<src>[pv]\d+))?$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "move":
                        v.InstructionType = ESmaliInstruction.Move;
                        break;
                    case "move/from16":
                        v.InstructionType = ESmaliInstruction.MoveFrom16;
                        break;
                    case "move/16":
                        v.InstructionType = ESmaliInstruction.Move16;
                        break;
                    case "move-wide":
                        v.InstructionType = ESmaliInstruction.MoveWide;
                        break;
                    case "move-wide/from16":
                        v.InstructionType = ESmaliInstruction.MoveWideFrom16;
                        break;
                    case "move-wide/16":
                        v.InstructionType = ESmaliInstruction.MoveWide16;
                        break;
                    case "move-object":
                        v.InstructionType = ESmaliInstruction.MoveObject;
                        break;
                    case "move-object/from16":
                        v.InstructionType = ESmaliInstruction.MoveObjectFrom16;
                        break;
                    case "move-object/16":
                        v.InstructionType = ESmaliInstruction.MoveObject16;
                        break;
                    case "move-result":
                        v.InstructionType = ESmaliInstruction.MoveResult;
                        break;
                    case "move-result-wide":
                        v.InstructionType = ESmaliInstruction.MoveResultWide;
                        break;
                    case "move-result-object":
                        v.InstructionType = ESmaliInstruction.MoveResultObject;
                        break;
                    case "move-exception":
                        v.InstructionType = ESmaliInstruction.MoveException;
                        break;
                    default:
                        throw new Exception("Unknown move operation");

                }

                // Get the dest register
                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                if (match.Groups["src"].Success)
                {
                    v.Src = new DalvikRegister(match.Groups["src"].Value);
                }

            }
            return result;
        }

        /// <summary>
        /// Parses out 'return*' operations. Will set the Src register and type (p/v). 
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is a return instruction.</returns>
        protected bool ParseReturnInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>return([-a-z0-9]+)?)(\s(?<src>[pv]\d+))?$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "return-void":
                        v.InstructionType = ESmaliInstruction.ReturnVoid;
                        break;
                    case "return":
                        v.InstructionType = ESmaliInstruction.Return;
                        break;
                    case "return-wide":
                        v.InstructionType = ESmaliInstruction.ReturnWide;
                        break;
                    case "return-object":
                        v.InstructionType = ESmaliInstruction.ReturnObject;
                        break;
                    default:
                        throw new Exception("Unknown return operation");

                }

                if (match.Groups["src"].Success)
                {
                    v.Src = new DalvikRegister(match.Groups["src"].Value);
                }

            }
            return result;
        }

        /// <summary>
        /// Parses out 'const*' operations. Will set the Dest register and type (p/v). 
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is a const instruction.</returns>
        protected bool ParseConstInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>const([-a-z0-9]+)?(/[a-z0-9]+)?)\s(?<dest>[pv]\d+),\s(?<value>(.)+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "const/4":
                        v.InstructionType = ESmaliInstruction.Const4;
                        break;
                    case "const/16":
                        v.InstructionType = ESmaliInstruction.Const16;
                        break;
                    case "const":
                        v.InstructionType = ESmaliInstruction.Const;
                        break;
                    case "const/high16":
                        v.InstructionType = ESmaliInstruction.ConstHigh16;
                        break;
                    case "const-wide/16":
                        v.InstructionType = ESmaliInstruction.ConstWide16;
                        break;
                    case "const-wide/32":
                        v.InstructionType = ESmaliInstruction.ConstWide32;
                        break;
                    case "const-wide":
                        v.InstructionType = ESmaliInstruction.ConstWide;
                        break;
                    case "const-wide/high16":
                        v.InstructionType = ESmaliInstruction.ConstWideHigh16;
                        break;
                    case "const-string":
                        v.InstructionType = ESmaliInstruction.ConstString;
                        break;
                    case "const-string/jumbo":
                        v.InstructionType = ESmaliInstruction.ConstStringJumbo;
                        break;
                    case "const-class":
                        v.InstructionType = ESmaliInstruction.ConstClass;
                        break;
                    default:
                        throw new Exception("Unknown const operation");

                }

                // Get the dest register
                v.Dest = new DalvikRegister(match.Groups["dest"].Value);

                if (match.Groups["value"].Value.StartsWith("\""))
                {
                    // Get rid of " marks
                    v.ConstStrValue = match.Groups["value"].Value.Substring(1);
                    v.ConstStrValue = v.ConstStrValue.Substring(0, v.ConstStrValue.Length - 1);
                }
                else if (match.Groups["value"].Value.StartsWith("L") && v.InstructionType == ESmaliInstruction.ConstClass)
                {
                    v.ConstStrValue = match.Groups["value"].Value;

                }
                else if (match.Groups["value"].Value.StartsWith("-0x") || match.Groups["value"].Value.StartsWith("0x"))
                {
                    match = Regex.Match(match.Groups["value"].Value, @"^(-)?0x[-0-9a-f]+");
                    if (match.Value.StartsWith("-"))
                        v.ConstLongValue = -Convert.ToInt64(match.Value.Substring(1), 16);
                    else
                        v.ConstLongValue = Convert.ToInt64(match.Value, 16);
                }

            }
            return result;
        }

        /// <summary>
        /// Parses out 'monitor*' operations. Will set the Src register and type (p/v). 
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is a monitor instruction.</returns>
        protected bool ParseMonitorInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>monitor([-a-z0-9]+)(/[a-z0-9]+)?)\s(?<src>[pv]\d+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "monitor-enter":
                        v.InstructionType = ESmaliInstruction.MonitorEnter;
                        break;
                    case "monitor-exit":
                        v.InstructionType = ESmaliInstruction.MonitorExit;
                        break;
                    default:
                        throw new Exception("Unknown monitor operation");

                }

                // Get the src register, register is always present
                v.Src= new DalvikRegister(match.Groups["src"].Value);
            }
            return result;
        }

        /// <summary>
        /// Parses out 'check-cast' operations. Will set the Src register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is a check-cast instruction.</returns>
        protected bool ParseCheckCastInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>check-cast)\s(?<src>[pv]\d+),\s(?<typeName>L[^\s]+;)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "check-cast":
                        v.InstructionType = ESmaliInstruction.CheckCast;
                        break;
                    default:
                        throw new Exception("Unknown check-cast operation");

                }

                // Get the src register, register is always present
                v.Src = new DalvikRegister(match.Groups["src"].Value);
                v.TypeName = match.Groups["typeName"].Value;
            }
            return result;
        }

        /// <summary>
        /// Parses out 'instance-of' operations. Will set the Src\Dest register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is a instance-of instruction.</returns>
        protected bool ParseInstanceOfInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>instance-of)\s(?<dest>[pv]\d+),\s(?<src>[pv]\d+),\s(?<typeName>L[^\s]+;)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "instance-of":
                        v.InstructionType = ESmaliInstruction.InstanceOf;
                        break;
                    default:
                        throw new Exception("Unknown instance-of operation");

                }

                // Get the src register, register is always present
                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.Src = new DalvikRegister(match.Groups["src"].Value);
                v.TypeName = match.Groups["typeName"].Value;
            }
            return result;
        }

        /// <summary>
        /// Parses out 'array-length' operations. Will set the Src\Dest register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is array-lenght instruction.</returns>
        protected bool ParseArrayLengthInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>array-length)\s(?<dest>[pv]\d+),\s(?<src>[pv]\d+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "array-length":
                        v.InstructionType = ESmaliInstruction.ArrayLength;
                        break;

                    default:
                        throw new Exception("Unknown array-length operation");

                }
                // Get the src register, register is always present
                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.Src = new DalvikRegister(match.Groups["src"].Value);
            }
            return result;
        }

        /// <summary>
        /// Parses out 'array-length' operations. Will set the Src\Dest register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is array-lenght instruction.</returns>
        protected bool ParseNewInstanceInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>new-instance)\s(?<dest>[pv]\d+),\s(?<typeName>L[^\s]+;)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "new-instance":
                        v.InstructionType = ESmaliInstruction.NewInstance;
                        break;
                    default:
                        throw new Exception("Unknown new-instance operation");

                }

                // Get the src register, register is always present
                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.TypeName = match.Groups["typeName"].Value;
            }
            return result;
        }

        /// <summary>
        /// Parses out 'new-array' operations. Will set the Src\Dest register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is new-array instruction.</returns>
        protected bool ParseNewArrayInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>new-array)\s(?<dest>[pv]\d+),\s(?<src>[pv]\d+),\s(?<typeName>(\[)+[^\s]+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "new-array":
                        v.InstructionType = ESmaliInstruction.NewArray;
                        break;
                    default:
                        throw new Exception("Unknown new-array operation");

                }

                // Get the src register, register is always present
                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.Src = new DalvikRegister(match.Groups["src"].Value);
                var type = match.Groups["typeName"].Value;
                while (type.StartsWith("["))
                {
                    v.ArrayDimentions++;
                    type = type.Substring(1);
                }
                v.TypeName = type;
            }
            return result;
        }

        /// <summary>
        /// Parses out 'filled-new-array' operations. Will set the Src\Dest register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is filled-new-array instruction.</returns>
        protected bool ParseFilledNewArrayInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>filled-new-array(/range)?)\s{((?<registers>[pv]\d+)(,\s)?)+},\s(?<typeName>\[(.)+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                v.HasResultValue = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "filled-new-array":
                        v.InstructionType = ESmaliInstruction.FilledNewArray;
                        break;
                    case "filled-new-array/range":
                        v.InstructionType = ESmaliInstruction.FilledNewArrayRange;
                        throw new Exception("Have not seen it before. Issue #82");
                    default:
                        throw new Exception("Unknown filled-new-array operation");

                }

                // Get the size register
                // First register is always the size
                v.ArraySizeReg = new DalvikRegister(match.Groups["registers"].Captures[0].Value);
                v.ArrayValueRegs = new DalvikRegister[match.Groups["registers"].Captures.Count - 1]; // -1, since size reg is taken already
                for (int i = 1; i < match.Groups["registers"].Captures.Count; i++)
                {
                    v.ArrayValueRegs[i - 1] = new DalvikRegister(match.Groups["registers"].Captures[i].Value);
                }

                // Note, array can contains simple items like ints, bytes etc.
                var type = match.Groups["typeName"].Value;
                while (type.StartsWith("["))
                {
                    v.ArrayDimentions++;
                    type = type.Substring(1);
                }
                v.TypeName = type;
            }
            return result;
        }

        /// <summary>
        /// Parses out 'fill-array-data' operations. Will set the Src\Dest register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is fill-array-data instruction.</returns>
        protected bool ParseFillArrayDataInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>fill-array-data)\s(?<dest>[pv]\d+),\s(?<label>:[a-z0-9_]+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "fill-array-data":
                        v.InstructionType = ESmaliInstruction.FillArrayData;
                        break;
                    default:
                        throw new Exception("Unknown fill-array-data operation");

                }

                // Get the src register, register is always present
                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.Label = match.Groups["label"].Value;
            }
            return result;
        }

        /// <summary>
        /// Parses out 'throw' operations. Will set the Src\Dest register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is throw instruction.</returns>
        protected bool ParseThrowInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>throw)\s(?<src>[pv]\d+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "throw":
                        v.InstructionType = ESmaliInstruction.Throw;
                        break;
                    default:
                        throw new Exception("Unknown throw operation");
                }
                v.Dest = new DalvikRegister(match.Groups["src"].Value);
            }
            return result;
        }

        /// <summary>
        /// Parses out 'catch' operations. Will set the Src\Dest register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is throw instruction.</returns>
        protected bool ParseCatchInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>\.catch(all)?)(\s(?<exceptName>L[^\s]+;))?\s{(?<start>:[^\s]+)\s\.\.\s(?<end>:[^\s]+)}\s(?<label>:[^\s]+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case ".catch":
                        v.InstructionType = ESmaliInstruction.Catch;
                        break;
                    case ".catchall":
                        v.InstructionType = ESmaliInstruction.CatchAll;
                        break;
                    default:
                        throw new Exception("Unknown throw operation");

                }

                if (match.Groups["exceptName"].Success && v.InstructionType == ESmaliInstruction.Catch)
                    v.TypeName = match.Groups["exceptName"].Value;
                v.Label = match.Groups["label"].Value;
                v.TryStart = match.Groups["start"].Value;
                v.TryEnd = match.Groups["end"].Value;
            }
            return result;
        }

        /// <summary>
        /// Parses out 'goto' operations. Will set the Src\Dest register and type (p/v) and TypeName.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is goto instruction.</returns>
        protected bool ParseGotoInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>goto((/16)|(/32))?)\s(?<label>:goto_[a-z0-9]+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "goto":
                        v.InstructionType = ESmaliInstruction.Goto;
                        break;
                    case "goto/16":
                        v.InstructionType = ESmaliInstruction.Goto16;
                        break;
                    case "goto/32":
                        v.InstructionType = ESmaliInstruction.Goto32;
                        break;
                    default:
                        throw new Exception("Unknown goto operation");

                }
                v.Label = match.Groups["label"].Value;
            }
            return result;
        }

        /// <summary>
        /// Parses out '*-switch' operations. Will set the Src register and label.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is '*-switch' instruction.</returns>
        protected bool ParseSwitchInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>((packed)|(sparse))?-switch)\s(?<src>[pv]\d+),\s(?<label>:[_a-z0-9]+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "packed-switch":
                        v.InstructionType = ESmaliInstruction.PackedSwitch;
                        break;
                    case "sparse-switch":
                        v.InstructionType = ESmaliInstruction.SparseSwitch;
                        break;
                    default:
                        throw new Exception("Unknown goto operation");

                }

                v.Src = new DalvikRegister(match.Groups["src"].Value);
                v.Label = match.Groups["label"].Value;
            }
            return result;
        }

        /// <summary>
        /// Parses out '.*-switch' operations. Will set the value.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is '.*-switch' instruction.</returns>
        protected bool ParseSwitchBlockInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>\.(end\s)?((sparse)|(packed))-switch)(\s(?<value>0x[0-9a-fA-F]+))?$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case ".packed-switch":
                        v.InstructionType = ESmaliInstruction.PackedSwitchBegin;
                        break;
                    case ".sparse-switch":
                        v.InstructionType = ESmaliInstruction.SparseSwitchBegin;
                        break;
                    case ".end packed-switch":
                        v.InstructionType = ESmaliInstruction.PackedSwitchEnd;
                        break;
                    case ".end sparse-switch":
                        v.InstructionType = ESmaliInstruction.SparseSwitchEnd;
                        break;
                    default:
                        throw new Exception("Unknown goto operation");

                }

                if (v.InstructionType == ESmaliInstruction.PackedSwitchBegin)
                    v.ConstLongValue = Convert.ToInt64(match.Groups["value"].Value.Replace("0x",""), 16);
            }
            return result;
        }

        /// <summary>
        /// Parses out 'cmpkind' operations. Will set the Dest, Src and Src2 registers.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is 'cmpkind' instruction.</returns>
        protected bool ParseCompareInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"^(\s)*(?<opcode>cmp[lg]?-((float)|(double)|(long)))\s(?<dest>[pv]\d+),\s(?<valA>[pv]\d+),\s(?<valB>[pv]\d+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value)
                {
                    case "cmpl-float":
                        v.InstructionType = ESmaliInstruction.CompareLtFloat;
                        break;
                    case "cmpg-float":
                        v.InstructionType = ESmaliInstruction.CompareGtFloat;
                        break;
                    case "cmpl-double":
                        v.InstructionType = ESmaliInstruction.CompareLtDouble;
                        break;
                    case "cmpg-double":
                        v.InstructionType = ESmaliInstruction.CompareGtDouble;
                        break;
                    case "cmp-long":
                        v.InstructionType = ESmaliInstruction.CompareGtDouble;
                        break;
                    default:
                        throw new Exception("Unknown cmpkind operation");

                }

                // Destination
                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.Src = new DalvikRegister(match.Groups["valA"].Value);
                v.SrcB = new DalvikRegister(match.Groups["valB"].Value);
            }
            return result;
        }

        /// <summary>
        /// Parses out 'if-test' operations. Will set the Src and Src2 registers and label.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is 'if-test' instruction.</returns>
        protected bool ParseIfTestInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>if-((eq)|(ne)|(lt)|(ge)|(gt)|(le)))\s(?<valA>[pv]\d+),\s(?<valB>[pv]\d+),\s(?<label>[^\s]+)(?=$)");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "if-eq":
                        v.InstructionType = ESmaliInstruction.IfEqual;
                        break;
                    case "if-ne":
                        v.InstructionType = ESmaliInstruction.IfNotEqual;
                        break;
                    case "if-lt":
                        v.InstructionType = ESmaliInstruction.IfLessThan;
                        break;
                    case "if-ge":
                        v.InstructionType = ESmaliInstruction.IfGreaterOrEqual;
                        break;
                    case "if-gt":
                        v.InstructionType = ESmaliInstruction.IfGreaterThan;
                        break;
                    case "if-le":
                        v.InstructionType = ESmaliInstruction.IfLessOrEqual;
                        break;
                    default:
                        throw new Exception("Unknown if-test operation");

                }

                v.Src = new DalvikRegister(match.Groups["valA"].Value);
                v.SrcB = new DalvikRegister(match.Groups["valB"].Value);
                v.Label = match.Groups["label"].Value;
            }
            else
            {
                match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>if-((eq)|(ne)|(lt)|(ge)|(gt)|(le))z)\s(?<valA>[pv]\d+),\s(?<label>[^\s]+)(?=$)");
                if (match.Success)
                {
                    result = true;
                    switch (match.Groups["opcode"].Value.TrimEnd())
                    {
                        case "if-eqz":
                            v.InstructionType = ESmaliInstruction.IfEqualToZero;
                            break;
                        case "if-nez":
                            v.InstructionType = ESmaliInstruction.IfNotEqualToZero;
                            break;
                        case "if-ltz":
                            v.InstructionType = ESmaliInstruction.IfLessThanToZero;
                            break;
                        case "if-gez":
                            v.InstructionType = ESmaliInstruction.IfGreaterOrEqualToZero;
                            break;
                        case "if-gtz":
                            v.InstructionType = ESmaliInstruction.IfGreaterThanToZero;
                            break;
                        case "if-lez":
                            v.InstructionType = ESmaliInstruction.IfLessOrEqualToZero;
                            break;
                        default:
                            throw new Exception("Unknown if-testz operation");

                    }

                    v.Src = new DalvikRegister(match.Groups["valA"].Value);
                    v.Label = match.Groups["label"].Value;
                }

            }
            return result;
        }

        /// <summary>
        /// Parses out 'arrayop' operations. Will set the Dest Src and Src2 registers and label.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is 'arrayop' instruction.</returns>
        protected bool ParseArrayOpInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>a((get)|(put))((-wide)|(-object)|(-boolean)|(-byte)|(-char)|(-short))?)\s(?<destorsrc>[pv]\d+),\s(?<array>[pv]\d+),\s(?<index>[pv]\d+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "aget":
                        v.InstructionType = ESmaliInstruction.ArrayGet;
                        break;
                    case "aget-wide":
                        v.InstructionType = ESmaliInstruction.ArrayGetWide;
                        break;
                    case "aget-object":
                        v.InstructionType = ESmaliInstruction.ArrayGetObject;
                        break;
                    case "aget-boolean":
                        v.InstructionType = ESmaliInstruction.ArrayGetBoolean;
                        break;
                    case "aget-byte":
                        v.InstructionType = ESmaliInstruction.ArrayGetByte;
                        break;
                    case "aget-char":
                        v.InstructionType = ESmaliInstruction.ArrayGetChar;
                        break;
                    case "aget-short":
                        v.InstructionType = ESmaliInstruction.ArrayGetShort;
                        break;
                    case "aput":
                        v.InstructionType = ESmaliInstruction.ArrayPut;
                        break;
                    case "aput-wide":
                        v.InstructionType = ESmaliInstruction.ArrayPutWide;
                        break;
                    case "aput-object":
                        v.InstructionType = ESmaliInstruction.ArrayPutObject;
                        break;
                    case "aput-boolean":
                        v.InstructionType = ESmaliInstruction.ArrayPutBoolean;
                        break;
                    case "aput-byte":
                        v.InstructionType = ESmaliInstruction.ArrayPutByte;
                        break;
                    case "aput-char":
                        v.InstructionType = ESmaliInstruction.ArrayPutChar;
                        break;
                    case "aput-short":
                        v.InstructionType = ESmaliInstruction.ArrayPutShort;
                        break;
                    default:
                        throw new Exception("Unknown arrayop operation");
                }

                if (match.Groups["opcode"].Value.StartsWith("aget"))
                {
                    v.Dest = new DalvikRegister(match.Groups["destorsrc"].Value);
                    v.Src = new DalvikRegister(match.Groups["array"].Value);
                    v.Index = new DalvikRegister(match.Groups["index"].Value);
                }
                else
                {
                    v.Src = new DalvikRegister(match.Groups["destorsrc"].Value);
                    v.Dest = new DalvikRegister(match.Groups["array"].Value);
                    v.Index = new DalvikRegister(match.Groups["index"].Value);
                }

            }
           
            return result;
        }

        /// <summary>
        /// Parses out 'instanceop' operations. Will set the Dest and Src registers and field.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is 'instanceop' instruction.</returns>
        protected bool ParseInstanceOpInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>i((get)|(put))((-wide)|(-object)|(-boolean)|(-byte)|(-char)|(-short))?)\s(?<value>[pv]\d+),\s(?<object>[pv]\d+),\s(?<field>[^\s]+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "iget":
                        v.InstructionType = ESmaliInstruction.InstanceGet;
                        break;
                    case "iget-wide":
                        v.InstructionType = ESmaliInstruction.InstanceGetWide;
                        break;
                    case "iget-object":
                        v.InstructionType = ESmaliInstruction.InstanceGetObject;
                        break;
                    case "iget-boolean":
                        v.InstructionType = ESmaliInstruction.InstanceGetBoolean;
                        break;
                    case "iget-byte":
                        v.InstructionType = ESmaliInstruction.InstanceGetByte;
                        break;
                    case "iget-char":
                        v.InstructionType = ESmaliInstruction.InstanceGetChar;
                        break;
                    case "iget-short":
                        v.InstructionType = ESmaliInstruction.InstanceGetShort;
                        break;
                    case "iput":
                        v.InstructionType = ESmaliInstruction.InstancePut;
                        break;
                    case "iput-wide":
                        v.InstructionType = ESmaliInstruction.InstancePutWide;
                        break;
                    case "iput-object":
                        v.InstructionType = ESmaliInstruction.InstancePutObject;
                        break;
                    case "iput-boolean":
                        v.InstructionType = ESmaliInstruction.InstancePutBoolean;
                        break;
                    case "iput-byte":
                        v.InstructionType = ESmaliInstruction.InstancePutByte;
                        break;
                    case "iput-char":
                        v.InstructionType = ESmaliInstruction.InstancePutChar;
                        break;
                    case "iput-short":
                        v.InstructionType = ESmaliInstruction.InstancePutShort;
                        break;
                    default:
                        throw new Exception("Unknown instanceop operation");
                }

                if (match.Groups["opcode"].Value.StartsWith("iget"))
                {
                    v.Dest = new DalvikRegister(match.Groups["value"].Value);
                    v.Src = new DalvikRegister(match.Groups["object"].Value);
                }
                else
                {
                    v.Src = new DalvikRegister(match.Groups["value"].Value);
                    v.Dest = new DalvikRegister(match.Groups["object"].Value);
                }
                v.Field = match.Groups["field"].Value;
            }

            return result;
        }

        /// <summary>
        /// Parses out 'staticop' operations. Will set the Dest or Src registers and field.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is 'static' instruction.</returns>
        protected bool ParseStaticOpInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>s((get)|(put))((-wide)|(-object)|(-boolean)|(-byte)|(-char)|(-short))?)\s(?<value>[pv]\d+),\s(?<field>[^\s]+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "sget":
                        v.InstructionType = ESmaliInstruction.StaticGet;
                        break;
                    case "sget-wide":
                        v.InstructionType = ESmaliInstruction.StaticGetWide;
                        break;
                    case "sget-object":
                        v.InstructionType = ESmaliInstruction.StaticGetObject;
                        break;
                    case "sget-boolean":
                        v.InstructionType = ESmaliInstruction.StaticGetBoolean;
                        break;
                    case "sget-byte":
                        v.InstructionType = ESmaliInstruction.StaticGetByte;
                        break;
                    case "sget-char":
                        v.InstructionType = ESmaliInstruction.StaticGetChar;
                        break;
                    case "sget-short":
                        v.InstructionType = ESmaliInstruction.StaticGetShort;
                        break;
                    case "sput":
                        v.InstructionType = ESmaliInstruction.StaticPut;
                        break;
                    case "sput-wide":
                        v.InstructionType = ESmaliInstruction.StaticPutWide;
                        break;
                    case "sput-object":
                        v.InstructionType = ESmaliInstruction.StaticPutObject;
                        break;
                    case "sput-boolean":
                        v.InstructionType = ESmaliInstruction.StaticPutBoolean;
                        break;
                    case "sput-byte":
                        v.InstructionType = ESmaliInstruction.StaticPutByte;
                        break;
                    case "sput-char":
                        v.InstructionType = ESmaliInstruction.StaticPutChar;
                        break;
                    case "sput-short":
                        v.InstructionType = ESmaliInstruction.StaticPutShort;
                        break;
                    default:
                        throw new Exception("Unknown instanceop operation");
                }

                if (match.Groups["opcode"].Value.StartsWith("sget"))
                {
                    v.Dest = new DalvikRegister(match.Groups["value"].Value);
                }
                else
                {
                    v.Src = new DalvikRegister(match.Groups["value"].Value);
                }
                v.Field = match.Groups["field"].Value;
            }

            return result;
        }

        /// <summary>
        /// Parses out 'invoke' operations. Will set the Dest or Src registers and field.
        /// </summary>
        /// <param name="codeLine">Code line that we are parsing out</param>
        /// <param name="v">Vertex object that will be configured accordingly</param>
        /// <returns>True if the codeLine is 'static' instruction.</returns>
        protected bool ParseInvokeInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>invoke-((virtual)|(super)|(direct)|(static)|(interface)))\s{((?<args>[pv]\d+)(,\s)?)*},\s(?<function>(.)+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "invoke-virtual":
                        v.InstructionType = ESmaliInstruction.InvokeVirtual;
                        break;
                    case "invoke-super":
                        v.InstructionType = ESmaliInstruction.InvokeSuper;
                        break;
                    case "invoke-direct":
                        v.InstructionType = ESmaliInstruction.InvokeDirect;
                        break;
                    case "invoke-static":
                        v.InstructionType = ESmaliInstruction.InvokeStatic;
                        break;
                    case "invoke-interface":
                        v.InstructionType = ESmaliInstruction.InvokeInterface;
                        break;
                    default:
                        throw new Exception("Unknown invoke-kind operation");
                }

                var regs = new List<DalvikRegister>();
                for (int i = 0; i < match.Groups["args"].Captures.Count; i++)
                {
                    regs.Add(new DalvikRegister(match.Groups["args"].Captures[i].Value));
                }
                v.ArgsRegs = regs.ToArray();
                v.Function = match.Groups["function"].Value;
            }

            return result;
        }

        protected bool ParseInvokeRangeInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>invoke-((virtual)|(super)|(direct)|(static)|(interface))/range)\s{((?<arg1>[pv]\d+)\s\.\.\s(?<arg2>[pv]\d+))},\s(?<function>(.)+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "invoke-virtual/range":
                        v.InstructionType = ESmaliInstruction.InvokeVirtualRange;
                        break;
                    case "invoke-super/range":
                        v.InstructionType = ESmaliInstruction.InvokeSuperRange;
                        break;
                    case "invoke-direct/range":
                        v.InstructionType = ESmaliInstruction.InvokeDirectRange;
                        break;
                    case "invoke-static/range":
                        v.InstructionType = ESmaliInstruction.InvokeStaticRange;
                        break;
                    case "invoke-interface/range":
                        v.InstructionType = ESmaliInstruction.InvokeInterfaceRange;
                        break;
                    default:
                        throw new Exception("Unknown invoke-kind/range operation");
                }

                var regs = new List<DalvikRegister>();
                var arg1 = new DalvikRegister(match.Groups["arg1"].Value);
                var arg2 = new DalvikRegister(match.Groups["arg2"].Value);
                for (int i = arg1.N; i <= arg2.N; i++)
                {
                    regs.Add(new DalvikRegister(arg1.N + 1, arg1.IsParameter));
                }
                v.ArgsRegs = regs.ToArray();
                v.Function = match.Groups["function"].Value;
            }

            return result;
        }

        protected bool ParseUnaryOperationInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>("+
                @"(neg-int)|(not-int)|(neg-long)|(not-long)|(neg-float)"+
                @"|(neg-double)|(int-to-long)|(int-to-float)|(int-to-double)|(long-to-int)" +
                @"|(long-to-float)|(long-to-double)|(float-to-int)|(float-to-long)|(float-to-double)" +
                @"|(double-to-int)|(double-to-long)|(double-to-float)|(int-to-byte)|(int-to-char)" +
                @"|(int-to-short)" +
                @"))\s((?<dest>[pv]\d+),\s(?<src>[pv]\d+))$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "neg-int":
                        v.InstructionType = ESmaliInstruction.NegInt;
                        break;
                    case "neg-long":
                        v.InstructionType = ESmaliInstruction.NegLong;
                        break;
                    case "neg-float":
                        v.InstructionType = ESmaliInstruction.NegFloat;
                        break;
                    case "neg-double":
                        v.InstructionType = ESmaliInstruction.NegDouble;
                        break;
                    case "not-int":
                        v.InstructionType = ESmaliInstruction.NotInt;
                        break;
                    case "not-long":
                        v.InstructionType = ESmaliInstruction.NotLong;
                        break;
                    case "int-to-long":
                        v.InstructionType = ESmaliInstruction.IntToLong;
                        break;
                    case "int-to-float":
                        v.InstructionType = ESmaliInstruction.IntToFloat;
                        break;
                    case "int-to-double":
                        v.InstructionType = ESmaliInstruction.IntToDouble;
                        break;
                    case "long-to-int":
                        v.InstructionType = ESmaliInstruction.LongToInt;
                        break;
                    case "long-to-float":
                        v.InstructionType = ESmaliInstruction.LongToFloat;
                        break;
                    case "long-to-double":
                        v.InstructionType = ESmaliInstruction.LongToDouble;
                        break;
                    case "float-to-int":
                        v.InstructionType = ESmaliInstruction.FloatToInt;
                        break;
                    case "float-to-long":
                        v.InstructionType = ESmaliInstruction.FloatToLong;
                        break;
                    case "float-to-double":
                        v.InstructionType = ESmaliInstruction.FloatToDouble;
                        break;
                    case "double-to-int":
                        v.InstructionType = ESmaliInstruction.DoubleToInt;
                        break;
                    case "double-to-long":
                        v.InstructionType = ESmaliInstruction.DoubleToLong;
                        break;
                    case "double-to-float":
                        v.InstructionType = ESmaliInstruction.DoubleToFloat;
                        break;
                    case "int-to-byte":
                        v.InstructionType = ESmaliInstruction.IntToByte;
                        break;
                    case "int-to-char":
                        v.InstructionType = ESmaliInstruction.IntToChar;
                        break;
                    case "int-to-short":
                        v.InstructionType = ESmaliInstruction.IntToShort;
                        break;
                    default:
                        throw new Exception("Unknown unop operation");
                }

                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.Src = new DalvikRegister(match.Groups["src"].Value);
            }

            return result;
        }

        protected bool ParseBinaryOperationInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>(" +
                @"(add-int)|(sub-int)|(mul-int)|(div-int)|(rem-int)|(and-int)|(or-int)|(xor-int)" +
                @"|(shl-int)|(shr-int)|(ushr-int)|(add-long)|(sub-long)|(mul-long)|(div-long)|(rem-long)" +
                @"|(and-long)|(or-long)|(xor-long)|(shl-long)|(shr-long)|(ushr-long)|(add-float)|(sub-float)" +
                @"|(mul-float)|(div-float)|(rem-float)|(add-double)|(sub-double)|(mul-double)|(div-double)|(rem-double)" +
                @"))\s((?<dest>[pv]\d+),\s(?<src>[pv]\d+),\s(?<srcB>[pv]\d+))$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "add-int":
                        v.InstructionType = ESmaliInstruction.AddInt;
                        break;
                    case "sub-int":
                        v.InstructionType = ESmaliInstruction.SubInt;
                        break;
                    case "mul-int":
                        v.InstructionType = ESmaliInstruction.MulInt;
                        break;
                    case "div-int":
                        v.InstructionType = ESmaliInstruction.DivInt;
                        break;
                    case "rem-int":
                        v.InstructionType = ESmaliInstruction.RemInt;
                        break;
                    case "and-int":
                        v.InstructionType = ESmaliInstruction.AndInt;
                        break;
                    case "or-int":
                        v.InstructionType = ESmaliInstruction.OrInt;
                        break;
                    case "xor-int":
                        v.InstructionType = ESmaliInstruction.XorInt;
                        break;
                    case "shl-int":
                        v.InstructionType = ESmaliInstruction.ShlInt;
                        break;
                    case "shr-int":
                        v.InstructionType = ESmaliInstruction.ShrInt;
                        break;
                    case "ushr-int":
                        v.InstructionType = ESmaliInstruction.UshrInt;
                        break;
                    case "add-long":
                        v.InstructionType = ESmaliInstruction.AddLong;
                        break;
                    case "sub-long":
                        v.InstructionType = ESmaliInstruction.SubLong;
                        break;
                    case "mul-long":
                        v.InstructionType = ESmaliInstruction.MulLong;
                        break;
                    case "div-long":
                        v.InstructionType = ESmaliInstruction.DivLong;
                        break;
                    case "rem-long":
                        v.InstructionType = ESmaliInstruction.RemLong;
                        break;
                    case "and-long":
                        v.InstructionType = ESmaliInstruction.AndLong;
                        break;
                    case "or-long":
                        v.InstructionType = ESmaliInstruction.OrLong;
                        break;
                    case "xor-long":
                        v.InstructionType = ESmaliInstruction.XorLong;
                        break;
                    case "shl-long":
                        v.InstructionType = ESmaliInstruction.ShlLong;
                        break;
                    case "shr-long":
                        v.InstructionType = ESmaliInstruction.ShrLong;
                        break;
                    case "ushr-long":
                        v.InstructionType = ESmaliInstruction.UshrLong;
                        break;
                    case "add-float":
                        v.InstructionType = ESmaliInstruction.AddFloat;
                        break;
                    case "sub-float":
                        v.InstructionType = ESmaliInstruction.SubFloat;
                        break;
                    case "mul-float":
                        v.InstructionType = ESmaliInstruction.MulFloat;
                        break;
                    case "div-float":
                        v.InstructionType = ESmaliInstruction.DivFloat;
                        break;
                    case "rem-float":
                        v.InstructionType = ESmaliInstruction.RemFloat;
                        break;
                    case "add-double":
                        v.InstructionType = ESmaliInstruction.AddDouble;
                        break;
                    case "sub-double":
                        v.InstructionType = ESmaliInstruction.SubDouble;
                        break;
                    case "mul-double":
                        v.InstructionType = ESmaliInstruction.MulDouble;
                        break;
                    case "div-double":
                        v.InstructionType = ESmaliInstruction.DivDouble;
                        break;
                    case "rem-double":
                        v.InstructionType = ESmaliInstruction.RemDouble;
                        break;
                    default:
                        throw new Exception("Unknown binop operation");
                }

                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.Src = new DalvikRegister(match.Groups["src"].Value);
                v.SrcB = new DalvikRegister(match.Groups["srcB"].Value);
            }

            return result;
        }

        protected bool ParseBinaryOperationAddr2Instruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>(" +
                @"(add-int)|(sub-int)|(mul-int)|(div-int)|(rem-int)|(and-int)|(or-int)|(xor-int)" +
                @"|(shl-int)|(shr-int)|(ushr-int)|(add-long)|(sub-long)|(mul-long)|(div-long)|(rem-long)" +
                @"|(and-long)|(or-long)|(xor-long)|(shl-long)|(shr-long)|(ushr-long)|(add-float)|(sub-float)" +
                @"|(mul-float)|(div-float)|(rem-float)|(add-double)|(sub-double)|(mul-double)|(div-double)|(rem-double)" +
                @"))/addr2\s((?<dest>[pv]\d+),\s(?<src>[pv]\d+))$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "add-int":
                        v.InstructionType = ESmaliInstruction.AddIntAddr2;
                        break;
                    case "sub-int":
                        v.InstructionType = ESmaliInstruction.SubIntAddr2;
                        break;
                    case "mul-int":
                        v.InstructionType = ESmaliInstruction.MulIntAddr2;
                        break;
                    case "div-int":
                        v.InstructionType = ESmaliInstruction.DivIntAddr2;
                        break;
                    case "rem-int":
                        v.InstructionType = ESmaliInstruction.RemIntAddr2;
                        break;
                    case "and-int":
                        v.InstructionType = ESmaliInstruction.AndIntAddr2;
                        break;
                    case "or-int":
                        v.InstructionType = ESmaliInstruction.OrIntAddr2;
                        break;
                    case "xor-int":
                        v.InstructionType = ESmaliInstruction.XorIntAddr2;
                        break;
                    case "shl-int":
                        v.InstructionType = ESmaliInstruction.ShlIntAddr2;
                        break;
                    case "shr-int":
                        v.InstructionType = ESmaliInstruction.ShrIntAddr2;
                        break;
                    case "ushr-int":
                        v.InstructionType = ESmaliInstruction.UshrIntAddr2;
                        break;
                    case "add-long":
                        v.InstructionType = ESmaliInstruction.AddLongAddr2;
                        break;
                    case "sub-long":
                        v.InstructionType = ESmaliInstruction.SubLongAddr2;
                        break;
                    case "mul-long":
                        v.InstructionType = ESmaliInstruction.MulLongAddr2;
                        break;
                    case "div-long":
                        v.InstructionType = ESmaliInstruction.DivLongAddr2;
                        break;
                    case "rem-long":
                        v.InstructionType = ESmaliInstruction.RemLongAddr2;
                        break;
                    case "and-long":
                        v.InstructionType = ESmaliInstruction.AndLongAddr2;
                        break;
                    case "or-long":
                        v.InstructionType = ESmaliInstruction.OrLongAddr2;
                        break;
                    case "xor-long":
                        v.InstructionType = ESmaliInstruction.XorLongAddr2;
                        break;
                    case "shl-long":
                        v.InstructionType = ESmaliInstruction.ShlLongAddr2;
                        break;
                    case "shr-long":
                        v.InstructionType = ESmaliInstruction.ShrLongAddr2;
                        break;
                    case "ushr-long":
                        v.InstructionType = ESmaliInstruction.UshrLongAddr2;
                        break;
                    case "add-float":
                        v.InstructionType = ESmaliInstruction.AddFloatAddr2;
                        break;
                    case "sub-float":
                        v.InstructionType = ESmaliInstruction.SubFloatAddr2;
                        break;
                    case "mul-float":
                        v.InstructionType = ESmaliInstruction.MulFloatAddr2;
                        break;
                    case "div-float":
                        v.InstructionType = ESmaliInstruction.DivFloatAddr2;
                        break;
                    case "rem-float":
                        v.InstructionType = ESmaliInstruction.RemFloatAddr2;
                        break;
                    case "add-double":
                        v.InstructionType = ESmaliInstruction.AddDoubleAddr2;
                        break;
                    case "sub-double":
                        v.InstructionType = ESmaliInstruction.SubDoubleAddr2;
                        break;
                    case "mul-double":
                        v.InstructionType = ESmaliInstruction.MulDoubleAddr2;
                        break;
                    case "div-double":
                        v.InstructionType = ESmaliInstruction.DivDoubleAddr2;
                        break;
                    case "rem-double":
                        v.InstructionType = ESmaliInstruction.RemDoubleAddr2;
                        break;
                    default:
                        throw new Exception("Unknown binop operation");
                }

                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.Src = new DalvikRegister(match.Groups["src"].Value);
            }

            return result;
        }

        protected bool ParseBinaryOperationLiteralInstruction(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<opcode>(" +
                @"(add-int)|(rsub-int)|(mul-int)|(div-int)|(rem-int)|(and-int)|(or-int)|(xor-int)" +
                @")/lit((16)|(8)))\s((?<dest>[pv]\d+),\s(?<src>[pv]\d+), (?<value>(-)?0x[0-9a-fA-F]*))$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["opcode"].Value.TrimEnd())
                {
                    case "add-int/lit16":
                        v.InstructionType = ESmaliInstruction.AddIntLit16;
                        break;
                    case "rsub-int/lit16":
                        v.InstructionType = ESmaliInstruction.RSubIntLit16;
                        break;
                    case "mul-int/lit16":
                        v.InstructionType = ESmaliInstruction.MulIntLit16;
                        break;
                    case "div-int/lit16":
                        v.InstructionType = ESmaliInstruction.DivIntLit16;
                        break;
                    case "rem-int/lit16":
                        v.InstructionType = ESmaliInstruction.RemIntLit16;
                        break;
                    case "and-int/lit16":
                        v.InstructionType = ESmaliInstruction.AndIntLit16;
                        break;
                    case "or-int/lit16":
                        v.InstructionType = ESmaliInstruction.OrIntLit16;
                        break;
                    case "xor-int/lit16":
                        v.InstructionType = ESmaliInstruction.XorIntLit16;
                        break;
                    case "add-int/lit8":
                        v.InstructionType = ESmaliInstruction.AddIntLit8;
                        break;
                    case "rsub-int/lit8":
                        v.InstructionType = ESmaliInstruction.RSubIntLit8;
                        break;
                    case "mul-int/lit8":
                        v.InstructionType = ESmaliInstruction.MulIntLit8;
                        break;
                    case "div-int/lit8":
                        v.InstructionType = ESmaliInstruction.DivIntLit8;
                        break;
                    case "rem-int/lit8":
                        v.InstructionType = ESmaliInstruction.RemIntLit8;
                        break;
                    case "and-int/lit8":
                        v.InstructionType = ESmaliInstruction.AndIntLit8;
                        break;
                    case "or-int/lit8":
                        v.InstructionType = ESmaliInstruction.OrIntLit8;
                        break;
                    case "xor-int/lit8":
                        v.InstructionType = ESmaliInstruction.XorIntLit8;
                        break;
                    default:
                        throw new Exception("Unknown binop/lit operation");
                }

                v.Dest = new DalvikRegister(match.Groups["dest"].Value);
                v.Src = new DalvikRegister(match.Groups["src"].Value);
                if (match.Groups["value"].Value.StartsWith("-"))
                    v.ConstLongValue = -Convert.ToInt64(match.Groups["value"].Value.Substring(1), 16);
                else
                    v.ConstLongValue = Convert.ToInt64(match.Groups["value"].Value, 16);

            }

            return result;
        }

        /// <summary>
        /// Parses out label opcode. Sets Label property
        /// </summary>
        /// <param name="codeLine">Codeline to parse out</param>
        /// <param name="v">Instruction structure to setup</param>
        /// <returns>True if this opcode is a label, False otherwise</returns>
        protected bool ParseLabel(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<label>:([a-z_]*?))(?<labelId>_[0-9a-z]+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["label"].Value.TrimEnd())
                {
                    case ":goto":
                        v.InstructionType = ESmaliInstruction.LabelGoto;
                        break;
                    case ":try_start":
                        v.InstructionType = ESmaliInstruction.LabelTryStart;
                        break;
                    case ":cond":
                        v.InstructionType = ESmaliInstruction.LabelCond;
                        break;
                    case ":pswitch":
                        v.InstructionType = ESmaliInstruction.LabelPSwitch;
                        break;
                    case ":pswitch_data":
                        v.InstructionType = ESmaliInstruction.LabelPSwitchData;
                        break;
                    case ":try_end":
                        v.InstructionType = ESmaliInstruction.LabelTryEnd;
                        break;
                    case ":catch":
                        v.InstructionType = ESmaliInstruction.LabelCatch;
                        break;
                    case ":catchall":
                        v.InstructionType = ESmaliInstruction.LabelCatchAll;
                        break;
                    case ":sswitch":
                        v.InstructionType = ESmaliInstruction.LabelSSwitch;
                        break;
                    case ":sswitch_data":
                        v.InstructionType = ESmaliInstruction.LabelSSwitchData;
                        break;
                    case ":array":
                        v.InstructionType = ESmaliInstruction.LabelArray;
                        break;
                    default:
                        throw new Exception("Unknown label operation");
                }

                v.Label = match.Groups["label"].Value + match.Groups["labelId"].Value;

            }

            return result;
        }

        /// <summary>
        /// Parses out label opcode. Sets Label property
        /// </summary>
        /// <param name="codeLine">Codeline to parse out</param>
        /// <param name="v">Instruction structure to setup</param>
        /// <returns>True if this opcode is a label, False otherwise</returns>
        protected bool ParseSSwitchLabel(string codeLine, ref SmaliCfgInstruction v)
        {
            var match = Regex.Match(codeLine, @"(?<=^(\s)*)(?<value>0x[0-9a-fA-F]+)\s->\s(?<label>:sswitch)(?<labelId>_[0-9a-z]+)$");
            var result = false;
            if (match.Success)
            {
                result = true;
                switch (match.Groups["label"].Value.TrimEnd())
                {
                    case ":sswitch":
                        v.InstructionType = ESmaliInstruction.LabelSSwitch;
                        break;
                    default:
                        throw new Exception("Unknown label operation");
                }

                v.Label = match.Groups["label"].Value + match.Groups["labelId"].Value;
                v.ConstLongValue = Convert.ToInt32(match.Groups["value"].Value.Replace("0x", ""), 16);

            }

            return result;
        }

        #endregion

    }
}