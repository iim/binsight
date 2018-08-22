using System.Collections.Generic;
using APKInsight.Logic.Analysis.Data;

namespace APKInsight.Logic.Analysis
{
    public abstract partial class AnalysisLogic
    {
        private HashSet<string> _visitedVertices;

        private void InitializeVisitedVertexCache()
        {
			_visitedVertices = new HashSet<string>();
        }

        /// <summary>
        /// Returns true if the vertext has been visited already
        /// </summary>
        /// <param name="slice">Slice that we are testing</param>
        /// <returns>True if the instruction has been visited already</returns>
        private bool HasCurrentInstructionBeenVisited(ProgramSliceState slice)
        {
            var instructionId = $"{slice.CurrentVertex.UniqueName}_{slice.CurrentInstruction.InstructionIndexInMethod}";
            if (instructionId == "Lcom/google/ads/util/c$c;->a([BIIZ)Z :goto_2_111")
            {
            }

            if (_visitedVertices.Contains(instructionId))
            {
                return true;
            }
            _visitedVertices.Add(instructionId);
            return false;
        }

    }
}
