using RnD.Business;

namespace RnD.Controller
{
    public class Dispatcher
    {
        public AgentTask AssignJobToAgent(AgentInfo agentInfo)
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
                    break;
                case AgentStatus.ExecutingTasks:
                    break;
                case AgentStatus.DownloadingMaterials:
                    break;
                case AgentStatus.UploadingResuls:
                    break;
                default:
                    break;
            }
            return AgentTask.Continue;

        }
    }
}
