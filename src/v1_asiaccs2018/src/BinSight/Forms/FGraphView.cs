using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using APKInsight.Controls.DisplayObjects;
using APKInsight.Globals;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Logic.ControlFlowGraph;
using Microsoft.Glee.Drawing;
using Color = Microsoft.Glee.Drawing.Color;

namespace APKInsight.Forms
{
    public partial class FGraphView : Form
    {
        private readonly JavaTypeDisplayInfo _javaTypeDisplayInfo;

        public FGraphView(JavaTypeDisplayInfo javaTypeDisplayInfo)
        {
            _javaTypeDisplayInfo = javaTypeDisplayInfo;
            InitializeComponent();
        }

        private void FGraphView_Load(object sender, EventArgs e)
        {
            Microsoft.Glee.GraphViewerGdi.GViewer viewer = new Microsoft.Glee.GraphViewerGdi.GViewer();
            Microsoft.Glee.Drawing.Graph graph = new Microsoft.Glee.Drawing.Graph("graph");

            var cfg = new Cfg(new SmaliParser());
            try
            {
                cfg.ProcessSourceFileContent(_javaTypeDisplayInfo.SourceCode);
                var className = PathResolver.GetJavaTypeSmaliName(_javaTypeDisplayInfo.JavaType.SmaliFullNameId.Value).Value + "->";


                // Add all vertices
                foreach (var cfgVertex in cfg.Vertices)
                {
                    var node = graph.AddNode(cfgVertex.UniqueName);
                    foreach (var incomingVertex in cfgVertex.EdgeIncomingVertex)
                    {
                        graph.AddEdge(incomingVertex.UniqueName, cfgVertex.UniqueName);
                    }
                    if (cfgVertex.Predecessor != null)
                    {
                        graph.AddEdge(cfgVertex.Predecessor.UniqueName, cfgVertex.UniqueName);
                    }
                    foreach (var returnVertex in cfgVertex.EdgeReturnVertex)
                    {
                        var edge = graph.AddEdge(cfgVertex.UniqueName, returnVertex.UniqueName);
                        edge.Attr.Color = Color.DarkGreen;
                    }
                    node.Attr.Shape = Shape.Box;
                    node.Attr.Label = "[" + node.Id + "]\n\n" + cfgVertex.InstructionsCode.TrimEnd('\n');
                }

                // Add all fields vertices
                foreach (var cfgVertex in cfg.Fields)
                {
                    var node = graph.AddNode(cfgVertex.UniqueName);
                    foreach (var vertex in cfgVertex.EdgeIncomingVertex)
                    {
                        var edge = graph.AddEdge(vertex.UniqueName, cfgVertex.UniqueName);
                        edge.Attr.Color = Color.Blue;
                    }
                    foreach (var vertex in cfgVertex.EdgeOutgoingVertex)
                    {
                        var edge = graph.AddEdge(cfgVertex.UniqueName, vertex.UniqueName);
                        edge.Attr.Color = Color.Red;
                    }
                    node.Attr.Shape = Shape.Octagon;
                    node.Attr.Fontcolor = Color.Brown;
                }

            }
            catch (Exception exception)
            {

                throw;
            }

            viewer.Graph = graph;
            
            SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            Controls.Add(viewer);
            ResumeLayout();


        }
    }
}
