using System;
using System.IO;

namespace RnD.Controller
{
    //TODO: Implement - http://stackoverflow.com/questions/3935331/how-to-implement-a-configurationsection-with-a-configurationelementcollection
    public class Configuration
    {
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


        public Configuration()
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

        public static string GetFullPath(string subDirectory)
        {
            return Directory.GetCurrentDirectory() + subDirectory;
        }

        public static string GetRunFolderPath()
        {
            return Directory.GetCurrentDirectory() + WorkingDirectoryRootFolder + "\\" + string.Format(RunFolderFormatString, DateTime.Now);
        }

        public static string GetFullPathToInputFolder()
        {
            return GetFullPath(InputFolderName);
        }

        public static string GetFullPathToOutputFolder()
        {
            return GetFullPath(OutputFolderName);
        }

        public static string GetFullPathToDeploymentItemsFolder()
        {
            return GetFullPath(OutputFolderName + "\\" + DeploymentItemsFolderName);
        }
    }
}
