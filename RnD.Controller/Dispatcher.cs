using Ionic.Zip;
using RnD.Business;
using RnD.Controller.Test;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace RnD.Controller
{
    /// <summary>
    /// This class is responsible for tracking the work of all agents once the controller receives a task.
    /// </summary>
    public class Dispatcher
    {
        private ControllerStatus _status;
        private TestRunner _testRunner;
        private AgentsRepository _agentsRepository;


        private string _activeZipFile;

        public string ActiveZipFile
        {
            get { return _activeZipFile; }
        }

        public Dispatcher(AgentsRepository repository)
        {
            _status = ControllerStatus.Idle;
            _testRunner = new TestRunner();

            _agentsRepository = repository;
        }

        /// <summary>
        /// ASsignes appropriate task to an agent by evaluating his current status. 
        /// </summary>
        /// <param name="agentInfo">Information for the agent</param>
        /// <returns></returns>
        public TaskInfo AssignTaskToAgent(AgentInfo agentInfo)
        {

            if (_status != ControllerStatus.RunInProgress)
            {
                return new TaskInfo { Task = AgentTask.None };
            }

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
                case AgentStatus.FileExtracted:
                    if (!this._testRunner.AreAllTestsExecuted())
                    {
                        return new TaskInfo
                            {
                                Task = AgentTask.ExecuteTests,
                                Data = this._testRunner.GetTestsForExecutionAsCommand(2)
                            };
                    }
                    break;
                case AgentStatus.ExecutingTasks:
                    break;
                case AgentStatus.UploadingResuls:
                    break;
                default:
                    break;
            }

            return new TaskInfo { Task = AgentTask.None };

        }

        public void ReadRequest(string materialsSourceFolder, string arguments)
        {
            if (this._status != ControllerStatus.Idle)
            {
                return;
            }

            ClearSandBox();
            CopyMaterials(materialsSourceFolder, arguments);
            AnalyzeAssemblies(arguments);
            ZipFiles();

            if (!LocateIdleAgents())
            {
                return;
            }

            StartTestRun();
            MeergeTestResults();

        }

        private void MeergeTestResults()
        {
            UpdateStatus(ControllerStatus.MeergingTestResults);
            //    throw new System.NotImplementedException();
        }

        private void StartTestRun()
        {
            UpdateStatus(ControllerStatus.RunInProgress);
            //[SD] USe for and create timeout. Control the max execution time
            while (_testRunner.NotExecutedTests.Count > 0)
            {
                Thread.Sleep(500);
            }
        }

        private bool LocateIdleAgents()
        {
            UpdateStatus(ControllerStatus.LocatingIdleAgents);

            var idleAgentsCount = 0;

            for (int i = 0; i < 10; i++)
            {
                idleAgentsCount = this._agentsRepository.GetIdleAgents().Count;

                if (idleAgentsCount == 0)
                {
                    //[SD] We should use timer to avoid blocking the controller
                    Thread.Sleep(2500);
                }
                else
                {
                    Logger.Logg("Located {0} Idle agents.", idleAgentsCount);
                    return true;
                }
            }

            Logger.Logg("ABORT - Unable to locate Idle agents.");

            return false;
        }

        private void ZipFiles()
        {
            UpdateStatus(ControllerStatus.ZippingFiles);

            var sourceFolder = Configuration.GetFullPathToDeploymentItemsFolder();
            _activeZipFile = Configuration.GetFullPathToInputFolder() + "\\" + Configuration.ZipFileName;

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(sourceFolder);
                zip.Save(_activeZipFile);
            }
        }

        private void AnalyzeAssemblies(string arguments)
        {
            UpdateStatus(ControllerStatus.AnalyzeTestAssemblies);

            _testRunner.ReadTests(Configuration.GetFullPathToDeploymentItemsFolder(), arguments);
        }

        private void CopyMaterials(string materialsSourceFolder, string arguments)
        {
            UpdateStatus(ControllerStatus.CopyingMaterials);

            Configuration.CreateRunFolderPath();

            var deploymentItemsFolder = Configuration.GetFullPathToDeploymentItemsFolder();

            Directory.CreateDirectory(deploymentItemsFolder);

            CopyAll(materialsSourceFolder, deploymentItemsFolder);

            File.WriteAllText(deploymentItemsFolder + "\\arguments.txt", arguments);
        }

        private void ClearSandBox()
        {
            UpdateStatus(ControllerStatus.ClearingSandBox);

            var sandbox = Configuration.GetFullPath(Configuration.WorkingDirectoryRootFolder);

            DeleteDirectory(sandbox);

            Directory.CreateDirectory(sandbox);
        }

        private void UpdateStatus(ControllerStatus status)
        {
            this._status = status;
            Logger.Logg("Controller Status: {0}", this._status);
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
