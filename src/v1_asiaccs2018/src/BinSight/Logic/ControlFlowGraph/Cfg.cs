using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsnowFramework.InputOutput;

namespace APKInsight.Logic.ControlFlowGraph
{
    /// <summary>
    /// This is the main Control Flow Graph of a binary.
    /// </summary>
    public class Cfg: IComparable
    {
        private readonly ICfgParser _parser;

        private readonly Dictionary<string, List<CfgVertex>> _vertices = new Dictionary<string, List<CfgVertex>>();

        public List<CfgVertex> Vertices { get; set; } = new List<CfgVertex>();

        private readonly Dictionary<string, List<CfgVertex>> _fields = new Dictionary<string, List<CfgVertex>>();

        public List<CfgVertex> Fields { get; set; } = new List<CfgVertex>();

        public Cfg(ICfgParser parser)
        {
            _parser = parser;
        }

        public void ProcessSourceFileName(string filename)
        {
            var content = Encoding.UTF8.GetString(Utilities.ReadAllBytes(filename));
            ProcessSourceFileContent(content);
        }

        public void ProcessSourceFileContent(string fileContent)
        {
            //TODO(ildarm): Issue #80 - Convert Smali parser to use line by line
            _parser.ExtractAllVertices(this, fileContent);
        }

        public CfgVertex GetVertexByName(string vertexName, string vertexLabel, bool createNew = true)
        {
            return GetVertexByNameAndLabel(vertexName, vertexLabel, createNew, _vertices, Vertices);
        }

        public CfgVertex GetEntryPointVertexByName(string vertexName, bool createNew = true)
        {
            return GetVertexByName(vertexName, createNew, _vertices, Vertices);
        }

        public CfgVertex GetFieldVertexByName(string vertexName, bool createNew = true)
        {
            return GetVertexByName(vertexName, createNew, _fields, Fields, false);
        }

        private CfgVertex GetVertexByNameAndLabel(
                    string vertexName, 
                    string vertexLabel, 
                    bool createNew, 
                    Dictionary<string, List<CfgVertex>> verticesDic,
                    List<CfgVertex> vertices)
        {
            if (!verticesDic.ContainsKey(vertexName))
            {
                verticesDic.Add(vertexName, new List<CfgVertex>());
            }
            var cfgVertices = verticesDic[vertexName];
            var vertex = cfgVertices.FirstOrDefault(v => v.Label == vertexLabel);
            if (vertex == null && !createNew)
            {
                return null;
            }
            if (vertex == null)
            {
                vertex = new CfgVertex() {Name = vertexName, Label = vertexLabel};
                vertices.Add(vertex);
                cfgVertices.Add(vertex);
            }

            return vertex;
        }

        private CfgVertex GetVertexByName(
                    string vertexName, 
                    bool createNew,
                    Dictionary<string, List<CfgVertex>> verticesDic,
                    List<CfgVertex> vertices,
                    bool isEntryPoint = true)
        {
            if (!verticesDic.ContainsKey(vertexName))
            {
                verticesDic.Add(vertexName, new List<CfgVertex>());
            }
            var cfgVertices = verticesDic[vertexName];
            CfgVertex vertex = null;
            if (isEntryPoint)
            {
                vertex = cfgVertices.FirstOrDefault(v => v.IsEntryPoint);
            }
            else
            {
                vertex = cfgVertices.FirstOrDefault();
            }
            if (vertex == null && !createNew)
            {
                return null;
            }
            if (vertex == null)
            {
                vertex = new CfgVertex() {Name = vertexName, Label = "", IsEntryPoint = isEntryPoint};
                vertices.Add(vertex);
                cfgVertices.Add(vertex);
            }

            return vertex;
        }

        public int CompareTo(object obj)
        {
            Cfg cfgB = obj as Cfg;
            if (cfgB == null) return -1;
            if (!IsSubgraph(this, cfgB))
            {
                return -1;
            }
            if (!IsSubgraph(cfgB, this))
            {
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// Checks if a is a sub-graph of b
        /// </summary>
        /// <param name="a">Graph a</param>
        /// <param name="b">Graph b</param>
        /// <returns>Returns true if a is a subgraph of b, where each node of a belongs to b</returns>
        private bool IsSubgraph(Cfg a, Cfg b)
        {
            foreach (var functionVertices in a._vertices)
            {
                foreach (var cfgVertex in functionVertices.Value)
                {
                    var verticeB = b.GetVertexByName(cfgVertex.Name, cfgVertex.Label);
                    if (verticeB == null) return false;
                    if (verticeB != cfgVertex) return false;
                }
            }
            return true;
        }
    }
}
