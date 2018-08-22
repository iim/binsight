using System;
using System.Collections.Generic;
using System.Linq;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis.Data
{
    /// <summary>
    /// Holds info about single point on a slice
    /// </summary>
    public class ProgramSliceState
    {
        public CfgVertex CurrentVertex { get; set; }
        public int InstructionIndex { get; set; }
        public SmaliCfgInstruction CurrentInstruction { get; set; }
        public DalvikRegister TargerRegister { get; set; }
        public List<CfgVertex> VertexPath { get; set; } = new List<CfgVertex>();
        public List<SmaliCfgInstruction> Instructions { get; set; } = new List<SmaliCfgInstruction>();
        public bool IsTrackingNextMoveResultInstructionRegister { get; set; } = false;

        public void AddPredecessorVertex()
        {
            var p = CurrentVertex.Predecessor;
            InjectVertexInPathHeadForBackwardWalk(p);
        }

        public void AddSuccessorVertex()
        {
            var s = CurrentVertex.Successor;
            InjectVertexInPathHeadForForwardWalk(s);
        }

        public void InjectVertexInPathHeadForBackwardWalk(CfgVertex v)
        {
            try
            {
                VertexPath.Add(CurrentVertex);
                CurrentVertex = v;
                InstructionIndex = CurrentVertex.Instructions.Count - 1;
                CurrentInstruction = CurrentVertex.Instructions[InstructionIndex];
            }
            catch (Exception exp)
            {
            }
        }

        public void InjectVertexInPathHeadForForwardWalk(CfgVertex v)
        {
            try
            {
                VertexPath.Add(CurrentVertex);
                CurrentVertex = v;
                InstructionIndex = 0;
                CurrentInstruction = CurrentVertex.Instructions[0];
            }
            catch (Exception exp)
            {
            }
        }

        public ProgramSliceState CreateCopy()
        {
            return new ProgramSliceState
            {
                CurrentVertex = CurrentVertex,
                CurrentInstruction = CurrentInstruction,
                Instructions = new List<SmaliCfgInstruction>(Instructions.ToArray()),
                TargerRegister = new DalvikRegister(TargerRegister),
                VertexPath = new List<CfgVertex>(VertexPath.ToArray()),
                InstructionIndex = InstructionIndex
            };
        }

        /// <summary>
        /// Converts slice to presentable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Instructions.Count == 0) return "";

            CfgVertex currentVertex = Instructions.Last().ParentVertex;
            string result = "";
            for (int i = Instructions.Count - 1; i >= 0; i--)
            {
                if (!currentVertex.Equals(Instructions[i].ParentVertex))
                {
                    result += Environment.NewLine;
                    currentVertex = Instructions[i].ParentVertex;
                }
                result += Instructions[i].CodeLine + Environment.NewLine;
            }

            return result;
        }
    }
}
