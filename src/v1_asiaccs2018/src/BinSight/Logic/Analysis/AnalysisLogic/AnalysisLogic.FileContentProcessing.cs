using System;
using APKInsight.Logic.Analysis.Data;

namespace APKInsight.Logic.Analysis
{
    public abstract partial class AnalysisLogic
    {
        /// <summary>
        /// Processes specific use-case file for CFG.
        /// </summary>
        /// <param name="useCase">The usecase to process</param>
        /// <returns>True if file has been processed, false otherwise</returns>
        public bool ProcessFileForUseCase(UseCase useCase)
        {
            if (!AnalysisState.FileContents.ContainsKey(useCase.Filename)) return false;

            ProcessFile(AnalysisState.FileContents[useCase.Filename], useCase.Filename);

            return true;
        }

        /// <summary>
        /// Processes specific file with its content
        /// </summary>
        /// <param name="content">Content</param>
        /// <param name="filename">Filename</param>
        private void ProcessFile(string content, string filename)
        {
            if (AnalysisState.FilesToProcess.Contains(filename))
            {
                AnalysisState.ControlFlowGraph.ProcessSourceFileContent(content);
                AnalysisState.FilesToProcess.Remove(filename);
                AnalysisState.FilesProcessed.Add(filename);
            }
        }

        /// <summary>
        /// Process all files that have ref to the function name
        /// </summary>
        /// <param name="functionName">The instruction that does invokation</param>
        public void ProcessFilesForInvokation(string functionName)
        {
            var className =
                functionName.Substring(0, functionName.IndexOf("->", StringComparison.Ordinal)).TrimStart('[');
            // Return if this is a wellknown Java API
            if (!className.StartsWith("L")
                || className.StartsWith("Ljava/")
                || className.StartsWith("Ljavax/")
                || className.StartsWith("Lorg/bouncycastle")
                || className.StartsWith("Lorg/spongycastle")
                )
                return;
            foreach (var fileContent in AnalysisState.FileContents)
            {
                if (!AnalysisState.FilesProcessed.Contains(fileContent.Key))
                {
                    if (fileContent.Value.Contains(className))
                    {
                        ProcessFile(fileContent.Value, fileContent.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Process all files that have field references
        /// </summary>
        /// <param name="fieldName">Name of the field to be searched for</param>
        public void ProcessFilesForGettersSetters(string fieldName)
        {
            foreach (var fileContent in AnalysisState.FileContents)
            {
                if (!AnalysisState.FilesProcessed.Contains(fileContent.Key))
                {
                    if (fileContent.Value.Contains(fieldName))
                    {
                        ProcessFile(fileContent.Value, fileContent.Key);
                    }
                }
            }
        }

    }
}
