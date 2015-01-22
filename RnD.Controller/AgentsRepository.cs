using RnD.Business;
using System.Collections.Generic;
using System.Linq;

namespace RnD.Controller
{
    public class AgentsRepository
    {
        private List<AgentInfo> _registeredAgents;

        public AgentsRepository()
        {
            _registeredAgents = new List<AgentInfo>();
        }

        public void RegisterAgent(AgentInfo agent)
        {
            if (_registeredAgents.FirstOrDefault(x => x.Id == agent.Id) == null)
                _registeredAgents.Add(agent);
        }

        public void UpdateAgentStatus(AgentInfo agent)
        {
            var agt = _registeredAgents.FirstOrDefault(x => x.Id == agent.Id);

            if (agt == null)
                _registeredAgents.Add(agent);
            else
                agt.Status = agent.Status;

        }
    }
}
