using APKInsight.Logic.Analysis.Data;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis
{
    public abstract partial class AnalysisLogic
    {
        /// <summary>
        /// Get an entry point for a given use-case.
        /// </summary>
        /// <param name="useCase">Use case that defines specific call site in the CFG</param>
        /// <returns>Vertext of the use-case. null if not found.</returns>
        /// <remarks>null vertext, most probably, means we are trying to locate system library</remarks>
        public CfgVertex GetEntryPoint(UseCase useCase) =>
                    AnalysisState.ControlFlowGraph.GetEntryPointVertexByName($"{useCase.SmaliClassName}->{useCase.SmaliMethodName}", false);

        /// <summary>
        /// Sets current walking machine for slicing program back and forward.
        /// </summary>
        /// <param name="useCase">A use case that will be used for the entry point setup</param>
        public void SetupEntryPointForUseCase(UseCase useCase)
        {
            EntryPoint = GetEntryPoint(useCase);
            EntryPointInstruction = EntryPoint.AllInstructions[useCase.InMethodPos];
            EntryPointInstructionVertex = EntryPoint.InstructionInVertex[useCase.InMethodPos];
        }

        /// <summary>
        /// Sets current walking machine for slicing program back and forward.
        /// </summary>
        /// <param name="instruction">Specific instruction that will be used for the entry point setup</param>
        public void SetupEntryPointForUseCase(SmaliCfgInstruction instruction)
        {
            EntryPoint = instruction.ParentEntryPointVertex;
            EntryPointInstruction = instruction;
            EntryPointInstructionVertex = instruction.ParentVertex;
        }

    }
}
