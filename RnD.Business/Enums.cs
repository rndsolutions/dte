
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

    public enum AgentTask { DownloadMaterials, Abort, Continue, ExecuteTests, None }
}
