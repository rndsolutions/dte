using RestSharp;
using RnD.Business;
using System;
using System.IO;
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

        /// <summary>
        /// Starts the agent.
        /// </summary>
        public void Start()
        {
            Logger.Logg("Starting agent: {0}", this._agentInfo.Name);
            //[SD] 1. Register the Agent
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

            Logger.Logg("Successfully started agent: {0}", this._agentInfo.Name);
        }

        /// <summary>
        /// Stops the agent.
        /// </summary>
        public void Stop()
        {
            _sendStatusTimer.Stop();
            _getTaskTimer.Stop();

            UpdateStatus(AgentStatus.Disabled);
        }

        /// <summary>
        /// Registers the agent to a controller.
        /// </summary>
        /// <returns>True if controller sends OK status</returns>
        public bool Register()
        {
            Logger.Logg("Sending Register request to controller: {0}", Configuration.ControllerName);
            var request = new RestRequest("api/main/register", Method.POST);

            //  request.AddParameter("agentInfo", _agentInfo);

            request.AddObject(_agentInfo);
            var response = _restClient.Execute<HttpStatusCode>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Logg("Error sending Register request to controller. StatusCode: {0}", response.StatusCode);
                return false;
            }

            Logger.Logg("Agent was registered successfully", Configuration.ControllerName);
            return true;
        }

        /// <summary>
        /// Sends status the current status of the agent to the controller. 
        /// </summary>
        /// <returns>Returns the HttpStatusCode from the controller</returns>
        private bool SendStatus()
        {
            Logger.Logg("Start sending status to controller.");
            var request = new RestRequest("api/main/RecieveAgentStatus", Method.POST);
            request.AddParameter("agentId", "123", ParameterType.QueryString);
            request.AddParameter("status", this._status, ParameterType.QueryString);

            var response = _restClient.Execute<HttpStatusCode>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Logg("Error sending status to controller. StatusCode: {0}{1}{2}", response.StatusCode, Environment.NewLine, response.Content);
                return false;
            }

            Logger.Logg("Status to controller was successfully send.");
            return true;
        }

        /// <summary>
        /// Returns information about what tasks to execute.
        /// </summary>
        private AgentTask GetTask()
        {
            Logger.Logg("Start GetTask");

            var request = new RestRequest("api/main/GetTask", Method.GET);

            request.AddParameter("agentId", this._agentInfo.Id, ParameterType.QueryString);

            var response = _restClient.Execute<AgentTask>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Logg("Error in GetTask. StatusCode: {0}{1}{2}", response.StatusCode, Environment.NewLine, response.Content);
                return AgentTask.None;
            }

            Logger.Logg("Completed GetTask. Controller returned: {0}", response.Data);

            ExecuteTask(response.Data);

            return response.Data;
        }

        /// <summary>
        /// Sends the results from test execution.
        /// </summary>
        private void SendReport()
        {

        }

        public bool DownloadMaterials()
        {
            Logger.Logg("START DownloadMaterials");
            var request = new RestRequest("api/main/DownloadMaterial", Method.GET);

            var response = _restClient.Execute(request);

            //[SD] Check the status and log errors if any 
            var status = response.StatusCode;

            Logger.Logg("DownloadMaterials StatusCode: {0}", status);
            Logger.Logg("END DownloadMaterials");

            if (status == HttpStatusCode.OK)
            {
                Logger.Logg("START Saving File: {0}", "1.txt");
                File.WriteAllBytes(Configuration.WorkingDirectory + "1.txt", response.RawBytes);
                //[SD] We need to add verification of the download like GO server 
                Logger.Logg("END Saving File: {0}", "1.txt");
            }

            return status == HttpStatusCode.OK;
        }

        /// <summary>
        /// Do the actual job. Run tests, send update etc.
        /// </summary>
        private void ExecuteTask(AgentTask task)
        {
            _getTaskTimer.Stop();

            switch (task)
            {
                case AgentTask.DownloadMaterials:
                    if (DownloadMaterials())
                    {
                        UpdateStatus(AgentStatus.DownloadedMaterials);
                    }
                    else
                    {
                        //[SD] Set status to idle so the controller can decide if the agent needs to retry the download. The run may be over and retries may not be needed. 
                        UpdateStatus(AgentStatus.Idle);
                    }
                    break;
                case AgentTask.Abort:
                    break;
                case AgentTask.Continue:
                    break;
                case AgentTask.ExecuteTests:
                    break;
                default:
                    break;
            }

            _getTaskTimer.Start();

        }

        /// <summary>
        /// Updates the status of the agent.
        /// </summary>
        /// <param name="status">The new status.</param>
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
