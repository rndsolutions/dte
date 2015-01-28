using Ionic.Zip;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RnD.Agent
{
    public class TestRunner
    {
        private string _nunitConsoleExePath;
        private string _activeZipFile;
        private string _workingDirectory;
        private static int _resultFileCounter = 0;
        private string _activeTestResultFilePath;

        public string ActiveTestResultFilePath
        {
            get { return _activeTestResultFilePath; }
            }

        public TestRunner()
        {
            _workingDirectory = Configuration.GetFullPathToDeploymentItemsFolder();

            //This will crash the application if NuNit is not installed 
            LocateNunitConsoleExe();
        }

        public void UnzipMaterials(string zipFilePath)
        {
            string unpackDirectory = Configuration.GetFullPathToDeploymentItemsFolder();

            Directory.CreateDirectory(unpackDirectory);

            using (ZipFile zip = ZipFile.Read(zipFilePath))
            {
                foreach (ZipEntry e in zip)
                {
                    e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }

        }

        public void ZipMaterials()
        {
            var sourceFolder = Configuration.GetFullPathToResultsFolder();
            _activeZipFile = Configuration.GetFullPathToOutputFolder() + "\\" + Configuration.ZipFileName;

            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(sourceFolder);
                zip.Save(_activeZipFile);
            }
        }

        public void RunTests(string arguemetns)
        {
            //[SD] Edit arguments to add output xml file

            Directory.CreateDirectory(Configuration.GetFullPathToOutputFolder());

            _activeTestResultFilePath = "\"" + Configuration.GetFullPathToOutputFolder() + "\\result" + _resultFileCounter++ + ".xml\"";
            arguemetns = arguemetns + " /result=" + _activeTestResultFilePath;
            var output = InvokeExe(this._nunitConsoleExePath, arguemetns, _workingDirectory);
        }


        //[SD] TODO - decide what version to take
        public void LocateNunitConsoleExe()
        {
            string programFilesFolder = Environment.Is64BitOperatingSystem ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            var nunitFolders = Directory.GetDirectories(programFilesFolder, "NUnit*", SearchOption.TopDirectoryOnly);

            this._nunitConsoleExePath = Directory.GetFiles(nunitFolders[0], "*nunit-console.exe", SearchOption.AllDirectories).FirstOrDefault();
        }

        private string InvokeExe(string fullPathToExe, string arguments, string workingDirectory = "")
        {
            var nunitProcess = new Process();
            var output = string.Empty;

            try
            {
                nunitProcess.StartInfo.FileName = fullPathToExe;
                nunitProcess.StartInfo.CreateNoWindow = true;
                nunitProcess.StartInfo.UseShellExecute = false;
                nunitProcess.StartInfo.Arguments = arguments;
                nunitProcess.StartInfo.RedirectStandardOutput = true;

                if (!string.IsNullOrEmpty(workingDirectory))
                    nunitProcess.StartInfo.WorkingDirectory = workingDirectory;

                nunitProcess.Start();

                output = nunitProcess.StandardOutput.ReadToEnd();

                nunitProcess.WaitForExit();

                return output;
            }
            catch (Exception ex)
            {
                return string.Format("Exception in InvokeExe method.{0}{1}", Environment.NewLine, ex.Message);
            }
        }
    }
}
