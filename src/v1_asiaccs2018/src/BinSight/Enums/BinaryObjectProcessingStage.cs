namespace APKInsight.Enums
{
    enum BinaryObjectApkProcessingStage
    {
        // Generic state for all unprocessed binary objects
        Unprocessed = 0,
        ExtractingAndUploadingInternals = 1,
        InternalsExtracted = 2,
        SmaliFilesProcessingInProgress = 3,
        SmaliFilesProcessed = 4,
    }

    enum BinaryObjectSmaliProcessingStage
    {
        Unprocessed = 0,
        Processed = 1,
        Stage2SuperClassExtracted = 2,
        Stage3OuterClassExtracted = 3,
        Stage4ImplementedInterfacesExtracted = 4,
        Stage5MethodsInfoExtracted = 5,
        Stage6MethodsCrossRefExtracted = 6
    }
}
