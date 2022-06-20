using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using Microsoft.Build.Framework;
using Microsoft.Build.BuildEngine;

namespace sfxnet
{
    public class SFXCreator
    {
        public Engine buildEngine;
        private ILogger logger;

        public void createsfx(string zipfile, string sfxuiprojectzip, string sfxuiprojectname, string saveto, string mode)
        {
            string TempDir = Path.GetTempPath() + "sfxnetstuff";
            Directory.CreateDirectory(TempDir);
            ZipFile.ExtractToDirectory(sfxuiprojectzip, TempDir);
            File.Copy(zipfile, TempDir + "main.zip");
            this.buildEngine = new Engine();
            BuildPropertyGroup props = new BuildPropertyGroup();
            props.SetProperty("Configuration", "Debug");
            this.buildEngine.RegisterLogger(this.logger);
            Project proj = new Project(this.buildEngine);
            proj.Load(TempDir + sfxuiprojectname, ProjectLoadSettings.None);
            this.buildEngine.BuildProject(proj, "Build");
            CopyDirectory(TempDir + @"build\" + mode, saveto, true);
            Directory.Delete(TempDir);





        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
