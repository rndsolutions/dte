using RnD.Business;

namespace RnD.Controller
{

    // [SD] Ako get task ne se vika dokato se executva n   qkakva zadacha i controlera iska da abortne runa shte trqbva da se izchaka tekushtata operaciq da prikluchi.
    public class Dispatcher
    {
        public TaskInfo AssignTaskToAgent(AgentInfo agentInfo)
        {
            switch (agentInfo.Status)
            {
                case AgentStatus.Offline:
                    break;
                case AgentStatus.Enabled:
                    break;
                case AgentStatus.Disabled:
                    break;
                case AgentStatus.Idle:
                    return new TaskInfo
                    {
                        Task = AgentTask.DownloadMaterials,
                        ControllerMethodName = "DownloadMaterial",
                        Data = "materials.package.zip"
                    };
                case AgentStatus.DownloadingMaterials:
                    return new TaskInfo
                    {
                        Task = AgentTask.Continue,
                    };
                case AgentStatus.DownloadedMaterials:
                    return new TaskInfo
                    {
                        Task = AgentTask.ExecuteTests,
                        Data = "nunit.exe test.dll /test=t_one /test=t_two"
                    };
                case AgentStatus.ExecutingTasks:
                    break;
                case AgentStatus.UploadingResuls:
                    break;
                default:
                    break;
            }

            return new TaskInfo { Task = AgentTask.None };

        }
    }
}
