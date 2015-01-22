using RestSharp;
using RnD.Business;
using System;
using System.Net;
using System.Timers;

namespace RnD.Agent
{
    public class Shell
    {
        private RestClient _restClient;
        private Timer _sendStatusTimer;
        private Timer _getTaskTimer;
        private AgentStatus _status;
        private AgentInfo _agentInfo;

        private Object thisLock = new Object();

        public Shell()
        {
            _restClient = new RestClient("http://localhost:8080");

            _sendStatusTimer = new Timer(Configuration.PollingInterval);
            _getTaskTimer = new Timer(Configuration.PollingInterval);

            _sendStatusTimer.Elapsed += (s, a) => SendStatus();
            _getTaskTimer.Elapsed += (s, a) => GetTask();

            _agentInfo = new AgentInfo { Id = "123", Name = "TestAgentOne" };
        }


        private void Start()
        {
            //[SD] 1. Registere the Agent
            while (Register() != true)
            {
                //[SD] Block until the agent is registered
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(30));
            }

            //[SD] 2. Set the agent status to Idle
            UpdateStatus(AgentStatus.Idle);


            //[SD] 3. Start the two timers            
            _sendStatusTimer.Start();
            _getTaskTimer.Start();
        }

        private void Stop()
        {
            _sendStatusTimer.Stop();
            _getTaskTimer.Stop();

            UpdateStatus(AgentStatus.Disabled);
        }

        public bool Register()
        {
            var request = new RestRequest("api/main/register", Method.POST);

            request.AddParameter("agentInfo", _agentInfo, ParameterType.GetOrPost);

            var response = _restClient.Execute<HttpStatusCode>(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        private bool SendStatus()
        {
            var request = new RestRequest("api/main/RecieveAgentStatus", Method.POST);
            request.AddParameter("agentId","123");
            request.AddParameter("status", this._status);

            var response = _restClient.Execute<HttpStatusCode>(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Returns infromation about what tasks to execute
        /// </summary>
        private AgentTask GetTask()
        {
            var request = new RestRequest("api/main/GetTask", Method.POST);

            request.AddParameter("status", this._status, ParameterType.GetOrPost);

            var response = _restClient.Execute<AgentTask>(request);

            return response.Data;
        }

        private void SendReport()
        {

        }

        /// <summary>
        /// Do the actual job. Run tests, send update etc.
        /// </summary>
        private void Execute()
        {

        }

     

        private void UpdateStatus(AgentStatus status)
        {
            lock (thisLock)
            {
                this._status = status;
            }
        }

        /// <summary>
        /// [SD] For DEV testing only. This should be deleted from production code.
        /// </summary>
        /// <returns></returns>
        private HttpStatusCode Ping()
        {
            var agentInfo = new AgentInfo { Id = "123", Name = "TestAgentOne" };
            var request = new RestRequest("api/Main/Ping", Method.GET);
            var response = _restClient.Execute(request);
            return response.StatusCode;
        }
    }
}
