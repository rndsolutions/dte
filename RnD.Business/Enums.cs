
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
        UploadingResuls
    }

    public enum AgentTask { DownloadMaterials, Abort, Continue, ExecuteTests }
}
