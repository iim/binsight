using System;
using System.Collections.Generic;
using System.Linq;
using APKInsight.Logic.Analysis.Data;
using APKInsight.Logic.ControlFlowGraph;
using APKInsight.Logic.ControlFlowGraph.Specific;

namespace APKInsight.Logic.Analysis
{
    /// <summary>
    /// Logic to evaluate where data comes and goes.
    /// </summary>
    public class AnalysisLogicDataFlowForCipher : AnalysisLogic
    {
        private static AnalysisReport _report;
        public ApkInfo ApkInfo { get; set; }

        public static void InitReport()
        {
            _report = new AnalysisReport();
            _report.AddLineWithoutCounter("N,inFileLoc,fileName,result,labels");
        }

        public static void SaveReport(string filename)
        {
            _report.SaveReport(filename);
        }

        public override bool Process()
        {
            var useCases = ApkInfo.UseCases.Where(uc => uc.IsDataEncryptionOrDecryption).ToList();
            var useCasesCipherInit = ApkInfo.UseCases.Where(uc => uc.IsCipherUseCaseRule2).ToArray();
            foreach (var useCase in useCases)
            {
                try
                {
                    ProcessUseCase(useCase, useCasesCipherInit);
                }
                catch(Exception exp)
                {
                    //TODO(ildarm): Report on failure
                }
            }
            return true;
        }

        private void ProcessUseCase(UseCase useCase, UseCase[] cipherInit)
        {
            // Init the control flow graph
            AnalysisState.InitCfg();
            if (ProcessFileForUseCase(useCase))
            {
                // First find the location where the object is initialized
                // Get the entry point details (instruction, method and vertex)
                SetupEntryPointForUseCase(useCase);

                var cipherInitSlice = SliceProgramBack(0);

                // Consider special case, one slice and dead-code
                if (cipherInitSlice.Count == 1 && cipherInitSlice[0].Instructions.Last() == null)
                {
                    // TODO: Add reporting for dead-code
                    return;
                }

                var initFunctionCall =
                    cipherInitSlice.SelectMany(sl => sl.Instructions)
                        .FirstOrDefault(
                            i => i.Function != null && i.Function.StartsWith("Ljavax/crypto/Cipher;->init(I"));

                if (initFunctionCall == null)
                {
                    // TODO: Add reporting
                    return;                    
                }
                // Find specific usecase to map them
                var specificInitUseCase =
                    cipherInit.FirstOrDefault(ci => 
                        ci.InMethodPos == initFunctionCall.InstructionIndexInMethod &&
                        string.CompareOrdinal(ci.SmaliClassName + "->" + ci.SmaliMethodName,initFunctionCall.ParentEntryPointVertex.Name) == 0);

                if (specificInitUseCase == null)
                {
                    // TODO: Add reporting
                    return;
                }
                // Find if it is Encryption or Decryption
                SetupEntryPointForUseCase(initFunctionCall);
                var modeParameter = SliceProgramBack(1);

                if (modeParameter == null || modeParameter.Count != 1)
                {
                    // TODO: Add reporting
                    return;
                }
                var modeSetInstruction = modeParameter[0].Instructions.LastOrDefault();
                if (modeSetInstruction?.ConstLongValue == null)
                {
                    // TODO: Add reporting
                    return;
                }
                var modeValue = modeSetInstruction.ConstLongValue.Value;
                SetupEntryPointForUseCase(useCase);

                var dataComingIn = SliceProgramBack(1);
                // Setup slicing machine
                var dataComingTo = SliceProgramForward(-1);


            }
        }

    }
}
