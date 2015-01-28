using RestSharp;
using RnD.Business;
using System;
using System.Collections.Generic;
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
        private TestRunner _testRunner;
        private AgentInfo _agentInfo;

        private Object thisLock = new Object();

        public Shell()
        {
            _testRunner = new TestRunner();

            _restClient = new RestClient(Configuration.GetControllerAddress());

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
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
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


            request.AddObject(_agentInfo);
            var response = _restClient.Execute<HttpStatusCode>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Logg("Error sending Register request to controller. StatusCode: {0}{1}Content: {2}", response.StatusCode, Environment.NewLine, response.Content);
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
        private TaskInfo GetTask()
        {
            Logger.Logg("Start GetTask");

            var request = new RestRequest("api/main/GetTask", Method.GET);

            request.AddParameter("agentId", this._agentInfo.Id, ParameterType.QueryString);

            var response = _restClient.Execute<TaskInfo>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Logger.Logg("Error in GetTask. StatusCode: {0}{1}{2}", response.StatusCode, Environment.NewLine, response.Content);
                return TaskInfo.NoneTask;
            }

            Logger.Logg("Completed GetTask. Controller returned: {0}", response.Data.Task);

            ExecuteTask(response.Data);

            return response.Data;
        }

        /// <summary>
        /// Sends the results from test execution.
        /// </summary>
        private bool UploatTestResults()
        {
            var request = new RestRequest("api/main/UploadReport", Method.POST);

            request.AddParameter("agentId", _agentInfo.Id, ParameterType.QueryString);

            request.AddFile("TestResultFile.txt", this._testRunner.ActiveTestResultFilePath.Replace("\"", ""));

            var response = _restClient.Execute(request);
            UpdateStatus(AgentStatus.FileExtracted);

            return true;
        }

        public bool DownloadMaterials(string donwloadMethodName, string fileName)
        {
            Configuration.DownloadedZipPath = string.Empty;

            Logger.Logg("START DownloadMaterials");
            var request = new RestRequest("api/main/" + donwloadMethodName, Method.GET);

            var response = _restClient.Execute(request);

            //[SD] Check the status and log errors if any 
            var status = response.StatusCode;

            Logger.Logg("DownloadMaterials StatusCode: {0}", status);
            Logger.Logg("END DownloadMaterials");

            if (status == HttpStatusCode.OK)
            {
                Logger.Logg("START Saving File: {0}", fileName);

                var deploymentDirectory = Configuration.GetFullPathToInputFolder();

                Directory.CreateDirectory(deploymentDirectory);

                Configuration.DownloadedZipPath = deploymentDirectory + "\\" + fileName;
                File.WriteAllBytes(Configuration.DownloadedZipPath, response.RawBytes);
                //[SD] We need to add verification of the download like GO server 
                Logger.Logg("END Saving File: {0}", fileName);
            }

            return status == HttpStatusCode.OK;
        }

        private void ClearSandbox()
        {
            UpdateStatus(AgentStatus.ClearingSandBox);

            var sandbox = Configuration.GetFullPath(Configuration.WorkingDirectoryRootFolder);

            DeleteDirectory(sandbox);

            Directory.CreateDirectory(sandbox);
        }

        /// <summary>
        /// Do the actual job. Run tests, send update etc.
        /// </summary>
        private void ExecuteTask(TaskInfo task)
        {
            _getTaskTimer.Stop();

            switch (task.Task)
            {
                case AgentTask.DownloadMaterials:

                    ClearSandbox();

                    if (DownloadMaterials(task.ControllerMethodName, task.Data))
                    {
                        UpdateStatus(AgentStatus.DownloadedMaterials);

                        UpdateStatus(AgentStatus.ExtractingFile);
                        _testRunner.UnzipMaterials(Configuration.DownloadedZipPath);
                        UpdateStatus(AgentStatus.FileExtracted);
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
                    UpdateStatus(AgentStatus.ExecutingTasks);
                    _testRunner.RunTests(task.Data);
                    UploatTestResults();
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

            Logger.Logg("Agent Status: {0}", this._status);
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

        #region File System


        public static void CopyAll(string source, string target, SearchOption option = SearchOption.AllDirectories, List<string> dirsToSkip = null)
        {
            if (!source.EndsWith("\\"))
                source += "\\";
            if (!target.EndsWith("\\"))
                target += "\\";

            var dirsSource = new List<string>(Directory.GetDirectories(source, "*", option));
            var filesSource = new List<string>(Directory.GetFiles(source, "*", option));

            if (dirsToSkip != null)
            {
                foreach (var dir in dirsToSkip)
                {
                    dirsSource.RemoveAll(e => e.Contains(dir));
                    filesSource.RemoveAll(e => e.Contains(dir));
                }
            }

            //if (Directory.Exists(target))
            //{
            //    Directory.Delete(target, true);
            //}

            foreach (var dir in dirsSource)
                Directory.CreateDirectory(dir.Replace(source, target));

            foreach (var file in filesSource)
                File.Copy(file, file.Replace(source, target), true);
        }

        public static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                var dirs = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    File.Delete(file);
                }

                Directory.Delete(path, true);
            }
        }

        #endregion //File System
    }
}
