﻿using RestSharp;
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
            var request = new RestRequest("api/main/register", Method.POST);

            request.AddParameter("agentInfo", _agentInfo);

            var response = _restClient.Execute<HttpStatusCode>(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Sends status the current status of the agent to the controller. 
        /// </summary>
        /// <returns>Returns the HttpStatusCode from the controller</returns>
        private bool SendStatus()
        {
            var request = new RestRequest("api/main/RecieveAgentStatus", Method.POST);
            request.AddParameter("agentId", "123");
            request.AddParameter("status", this._status);

            var response = _restClient.Execute<HttpStatusCode>(request);

            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Returns information about what tasks to execute.
        /// </summary>
        private AgentTask GetTask()
        {
            var request = new RestRequest("api/main/GetTask", Method.POST);

            request.AddParameter("status", this._status, ParameterType.GetOrPost);

            var response = _restClient.Execute<AgentTask>(request);

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
            //  var response = _restClient.DownloadData(request);

            var response = _restClient.Execute(request);

            //[SD] Check the status and log errors if any 
            var status = response.StatusCode;

            Logger.Logg("DownloadMaterials StatusCode: {0}", status);
            Logger.Logg("END DownloadMaterials");

            if (status == HttpStatusCode.OK)
            {
                Logger.Logg("START Saving File: {0}", "1.txt");
                File.WriteAllBytes(Configuration.WorkingDirectory + "1.txt", response.RawBytes);
                Logger.Logg("END Saving File: {0}", "1.txt");
            }

            return status == HttpStatusCode.OK;
        }

        /// <summary>
        /// Do the actual job. Run tests, send update etc.
        /// </summary>
        private void ExecuteTask(AgentTask task)
        {
            switch (task)
            {
                case AgentTask.DownloadMaterials:
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
