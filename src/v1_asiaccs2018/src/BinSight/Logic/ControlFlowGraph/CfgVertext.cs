using System.Collections.Generic;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.ControlFlowGraph
{
    /// <summary>
    /// This represents a node in CFG
    /// </summary>
    public class CfgVertex
    {
        /// <summary>
        /// Contrains all instructions in that vertex
        /// </summary>
        public List<SmaliCfgInstruction> Instructions { get; set; } = new List<SmaliCfgInstruction>();

        public Dictionary<int, SmaliCfgInstruction> AllInstructions { get; set; } = new Dictionary<int, SmaliCfgInstruction>();
        public Dictionary<int, CfgVertex> InstructionInVertex { get; set; } = new Dictionary<int, CfgVertex>();

        /// <summary>
        /// Label for vertex. Empty for entry points, label name for labeled sub-nodes, and location number for location
        /// </summary>
        public string Label { get; set; } = "";

        /// <summary>
        /// The actual code lines that comprise this vertex
        /// </summary>
        public string InstructionsCode { get; set; } = "";
        
        /// <summary>
        /// Shows if that vertex is an entry point (e.g., into a function)
        /// </summary>
        public bool IsEntryPoint { get; set; } = false;

        /// <summary>
        /// Entry point vertex for the function to which this vertex belongs
        /// </summary>
        public CfgVertex EntryPointVertex { get; set; } = null;

        /// <summary>
        /// The name of the vertex. For functions, its a full class name, for labels its class name+label(s)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The vertex in a function that preceeds this vertex (i.e., follow through type of link)
        /// Null for entry point.
        /// </summary>
        public CfgVertex Predecessor { get; set; } = null;

        /// <summary>
        /// Collection of catch vertices where this vertex might jump
        /// </summary>
        public List<CfgVertex> CatchVertices { get; set; } = new List<CfgVertex>();

        /// <summary>
        /// The vertex in a function that succeeds this vertex (i.e., follow through type of link)
        /// Null for last vertex in the function or return based vertex
        /// </summary>
        public CfgVertex Successor { get; set; } = null;

        /// <summary>
        /// Incoming vertices objects
        /// </summary>
        public List<CfgVertex> EdgeIncomingVertex { get; set; } = new List<CfgVertex>();

        /// <summary>
        /// The index of an instruction in an incoming vertex
        /// </summary>
        public List<int> EdgeIncomingVertexInstruction { get; set; } = new List<int>();

        /// <summary>
        /// Outgoing vertices objects
        /// </summary>
        public List<CfgVertex> EdgeOutgoingVertex { get; set; } = new List<CfgVertex>();

        /// <summary>
        /// The index of an instruction in an outgoing vertex
        /// </summary>
        public List<int> EdgeOutgoingVertexInstruction { get; set; } = new List<int>();

        /// <summary>
        /// Incoming vertices objects
        /// </summary>
        public List<CfgVertex> EdgeReturnVertex { get; set; } = new List<CfgVertex>();

        public List<CfgVertex> ReturnVertices { get; set; } = new List<CfgVertex>();

        /// <summary>
        /// Adds an incoming edge to the current vertex from the specified edge and specified instruction
        /// </summary>
        /// <param name="vertex">Vertex we are adding edge to</param>
        /// <param name="instructionIndex">Instruction index</param>
        public void AddIncomingEdge(CfgVertex vertex, int instructionIndex)
        {
            EdgeIncomingVertex.Add(vertex);
            EdgeIncomingVertexInstruction.Add(instructionIndex);
        }

        /// <summary>
        /// Adds an outgoing edge to the current vertex from the specified edge and specified instruction
        /// </summary>
        /// <param name="vertex">Vertex we are adding edge to</param>
        /// <param name="instructionIndex">Instruction index</param>
        public void AddOutgoingEdge(CfgVertex vertex, int instructionIndex)
        {
            EdgeOutgoingVertex.Add(vertex);
            EdgeOutgoingVertexInstruction.Add(instructionIndex);
        }

        /// <summary>
        /// Display name of the vertex
        /// </summary>
        public string UniqueName => (IsEntryPoint ? "-> " : "") + (string.IsNullOrWhiteSpace(Label) ? Name : Name + " " + Label);

    }
}
