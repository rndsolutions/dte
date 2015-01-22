
namespace RnD.Business
{
    public class AgentInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public AgentStatus Status { get; set; }
        public EnvironmentInfo Environment { get; set; }

        public AgentInfo()
        {

        }
    }
}
