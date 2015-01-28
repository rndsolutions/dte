
using System.IO;
using System.Text;
namespace RnD.Agent
{
    public class Configuration
    {
        public static string ControllerName { get; set; }
        public static int ControllerPortNumber { get; set; }
        public static string ControllerMainControllerName { get; set; }
        public static string ControllerConnectMethodName { get; set; }
        public static int PollingInterval { get; set; }

        public static string DownloadedZipPath { get; set; }

        public static string ZipFileName { get; set; }


        public static string WorkingDirectoryRootFolder { get; set; }
        public static string RunFolderFormatString { get; set; }
        public static string InputFolderName { get; set; }
        public static string OutputFolderName { get; set; }
        public static string DeploymentItemsFolderName { get; set; }
        public static string ResultsFolderName { get; set; }


        static Configuration()
        {
            ControllerName = "localhost"; // "igbgsofwpf17";// "localhost";
            ControllerPortNumber = 8080;
            ControllerMainControllerName = "Main";
            ControllerConnectMethodName = "Connect";
            PollingInterval = 3000;

            WorkingDirectoryRootFolder = "Sandbox";
            RunFolderFormatString = "TestRun_{0:yyyy-MM-dd_hh-mm-ss-tt}";
            InputFolderName = "input";
            OutputFolderName = "output";
            ResultsFolderName = "results";
            DeploymentItemsFolderName = "deployment-items";
            ZipFileName = "results.zip";

        }

        public static string GetControllerAddress()
        {
            return string.Format("http://{0}:{1}", ControllerName, ControllerPortNumber);
        }

        public static string GetFullPath(params string[] subDirectories)
        {
            if (subDirectories.Length == 0)
            {
                return Directory.GetCurrentDirectory();
            }
            else
            {
                var builder = new StringBuilder();
                builder.Append(Directory.GetCurrentDirectory());

                foreach (var dir in subDirectories)
                {
                    builder.Append("\\");
                    builder.Append(dir);
                }

                return builder.ToString();
            }

        }


        public static string GetFullPathToInputFolder()
        {
            return GetFullPath(WorkingDirectoryRootFolder, InputFolderName);
        }

        public static string GetFullPathToDeploymentItemsFolder()
        {
            return GetFullPath(WorkingDirectoryRootFolder, InputFolderName, DeploymentItemsFolderName);
        }

        public static string GetFullPathToOutputFolder()
        {
            return GetFullPath(WorkingDirectoryRootFolder, OutputFolderName);
        }

        public static string GetFullPathToResultsFolder()
        {
            return GetFullPath(WorkingDirectoryRootFolder, OutputFolderName, ResultsFolderName);
        }

    }
}
