using System;
using System.IO;
using System.Text;

namespace RnD.Controller
{
    //TODO: Implement - http://stackoverflow.com/questions/3935331/how-to-implement-a-configurationsection-with-a-configurationelementcollection
    public class Configuration
    {
        private static string _lastGeneratedRunFolder;

        public static string ControllerName { get; set; }
        public static int ControllerPort { get; set; }

        public static string WorkingDirectoryRootFolder { get; set; }
        public static string RunFolderFormatString { get; set; }
        public static string InputFolderName { get; set; }
        public static string OutputFolderName { get; set; }
        public static string DeploymentItemsFolderName { get; set; }

        public static string ZipFileName { get; set; }

        public static int BucketSize { get; set; }

        //***** TEMP ******
        public static string TestMaterialsSource { get; set; }
        public static string ExecutionArguments { get; set; }


        static Configuration()
        {
            ControllerName = "localhost";
            ControllerPort = 8080;
            WorkingDirectoryRootFolder = "Sandbox";
            RunFolderFormatString = "TestRun_{0:yyyy-MM-dd_hh-mm-ss-tt}";
            TestMaterialsSource = "C:\\TestMaterialsMock";
            ExecutionArguments = "";
            BucketSize = 5;
            InputFolderName = "input";
            OutputFolderName = "output";
            DeploymentItemsFolderName = "deployment-items";
            ZipFileName = "materials.package.zip";
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

        public static string CreateRunFolderPath()
        {
            Configuration._lastGeneratedRunFolder = string.Format(RunFolderFormatString, DateTime.Now);

            return GetFullPath(WorkingDirectoryRootFolder, Configuration._lastGeneratedRunFolder);
        }

        public static string GetFullPathToInputFolder()
        {
            return GetFullPath(WorkingDirectoryRootFolder, Configuration._lastGeneratedRunFolder, InputFolderName);
        }

        public static string GetFullPathToOutputFolder()
        {
            return GetFullPath(WorkingDirectoryRootFolder, Configuration._lastGeneratedRunFolder, OutputFolderName);
        }

        public static string GetFullPathToDeploymentItemsFolder()
        {
            return GetFullPath(WorkingDirectoryRootFolder, Configuration._lastGeneratedRunFolder, InputFolderName, DeploymentItemsFolderName);
        }
    }
}
