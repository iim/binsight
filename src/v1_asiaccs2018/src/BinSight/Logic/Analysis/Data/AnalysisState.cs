using System.Collections.Generic;
using APKInsight.Logic.ContentParsing.SmaliParser;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis.Data
{
    /// <summary>
    /// Analysis state object that is passed from one analysis to another in order to reuse partially constructed CFG (i.e., program slice)
    /// </summary>
    public class AnalysisState
    {
        /// <summary>
        /// The CFG constructed so far.
        /// </summary>
        public Cfg ControlFlowGraph { get; set; }

        /// <summary>
        /// Files that are already part of the CFG
        /// </summary>
        public HashSet<string> FilesProcessed { get; set; }

        /// <summary>
        /// Files that are yet to be processed
        /// </summary>
        public HashSet<string> FilesToProcess { get; set; }

        /// <summary>
        /// Contents of the files in the APK
        /// </summary>
        public Dictionary<string, string> FileContents { get; set; }

        ///// <summary>
        ///// Maps full filenames (i.e., that include mount point) to short location (i.e., within app)
        ///// </summary>
        //public Dictionary<string, string> FullToShortFilenamesMap { get; set; }
        //public Dictionary<string, string> ShortToFullFilenamesMap { get; set; }
        
        /// <summary>
        /// Initialize the CFG (does it once)
        /// </summary>
        public void InitCfg()
        {
            if (ControlFlowGraph == null)
            {
                ControlFlowGraph = new Cfg(new SmaliParser());
            }
        }

        ///// <summary>
        ///// Get full name for a file from a short name
        ///// </summary>
        //public string GetFullFilename(string shortFilename)
        //{
        //    return FullToShortFilenamesMap.ContainsKey(shortFilename) ? FullToShortFilenamesMap[shortFilename] : null;
        //}

    }

}
