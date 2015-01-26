
namespace RnD.Business
{
    public enum AgentStatus
    {
        Offline,
        Enabled,
        Disabled,
        Idle,
        ExecutingTasks,
        DownloadingMaterials,
        DownloadedMaterials,
        UploadingResuls
    }

    public enum ControllerStatus
    {
        Idle,
        ClearingSandBox,
        CopyingMaterials,
        AnalyzeTestAssemblies,
        ZippingFiles,
        LocatingIdleAgents,
        RunInProgress,
        MeergingTestResults
    }

    public enum TestStatus
    {
        Executed,
        NotExecuted,
        Pendind
    }
    public enum AgentTask { DownloadMaterials, Abort, Continue, ExecuteTests, None }
}
