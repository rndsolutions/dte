using RnD.Business;
using System.Collections.Generic;

namespace RnD.Controller
{
    public class AgentsRepository
    {
        private Dictionary<string, AgentInfo> _registeredAgents;

        public AgentsRepository()
        {
            _registeredAgents = new Dictionary<string, AgentInfo>();
        }

        public bool RegisterAgent(AgentInfo agent)
        {
            if (!_registeredAgents.ContainsKey(agent.Id))
            {
                _registeredAgents.Add(agent.Id, agent);
            }

            return true;
        }

        public bool UpdateAgentStatus(AgentInfo agent)
        {

            if (_registeredAgents.ContainsKey(agent.Id))
            {
                _registeredAgents[agent.Id].Status = agent.Status;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateAgentStatus(string agentId, AgentStatus status)
        {

            if (_registeredAgents.ContainsKey(agentId))
            {
                _registeredAgents[agentId].Status = status;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
