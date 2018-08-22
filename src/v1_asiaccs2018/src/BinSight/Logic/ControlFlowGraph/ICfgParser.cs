namespace APKInsight.Logic.ControlFlowGraph
{
    /// <summary>
    /// A generic interface a parser must expose to be able to construct a CFG from code
    /// </summary>
    public interface ICfgParser
    {
        void ExtractAllVertices(Cfg cfg, string content);
    }
}
