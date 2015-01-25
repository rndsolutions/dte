
namespace RnD.Business
{
    public class TaskInfo
    {
        public AgentTask Task { get; set; }
        public string ControllerMethodName { get; set; }
        public string Data { get; set; }

        public static TaskInfo NoneTask = new TaskInfo(AgentTask.None);
        public static TaskInfo AbortTask = new TaskInfo(AgentTask.Abort);

        public TaskInfo(AgentTask task)
        {
            this.Task = task;
        }

        public TaskInfo()
        {

        }
    }
}
