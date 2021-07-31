using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.IO.Compression;
using IWshRuntimeLibrary;
using System.Diagnostics;

namespace Blaze.Functions
{
    public class VersionNumber
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Patch { get; set; }
    }

    class Install
    {
        public static string HomeDir = Directory.GetCurrentDirectory() + @"\Blaze";

        public static Preload preload;

        public static async Task<bool> CheckInstallation() { if (Directory.GetCurrentDirectory().EndsWith("Blaze") || Directory.GetCurrentDirectory().EndsWith("Debug")) { return true; } else return false; }
        
        public static async Task<bool> CheckUpdates(VersionNumber currVersion)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://devlin.gg/blaze/version.txt");
            StreamReader reader = new StreamReader(stream);

            VersionNumber newVersion = new VersionNumber();
            Version CurrVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string[] versionNumbers = reader.ReadToEnd().Split('.');

            newVersion.Major = int.Parse(versionNumbers[0]);
            newVersion.Minor = int.Parse(versionNumbers[1]);
            newVersion.Build = int.Parse(versionNumbers[2]);
            newVersion.Patch = int.Parse(versionNumbers[3]);

            if (currVersion.Major < newVersion.Major) return true;
            if (currVersion.Minor < newVersion.Minor) return true;
            if (currVersion.Build < newVersion.Build) return true;
            if (currVersion.Patch < newVersion.Patch) return true;
            else return false;
        }

        public static async Task StartInstall()
        {
            preload.StatusBox.Text = "Creating home directory...";
            if (!Directory.Exists(HomeDir))Directory.CreateDirectory(HomeDir);
            await DownloadDLL();
        }

        static async Task DownloadDLL()
        {
            preload.StatusBox.Text = "Downloading DLL's...";
            var client = new WebClient();
            var uri = new Uri("https://devlin.gg/blaze/install/dll.zip");

            client.DownloadFileCompleted += (sender, e) => FinishInstall().Wait();
            client.DownloadFileAsync(uri, HomeDir + @"\dll.zip");
        }

        static async Task FinishInstall()
        {
            preload.StatusBox.Text = "Finishing up...";
            string dllZipFile = HomeDir + @"\dll.zip";

            using (var fileStream = new FileStream(dllZipFile, FileMode.Open))
            {
                ZipArchive zipFile = new ZipArchive(fileStream);
                foreach (ZipArchiveEntry file in zipFile.Entries)
                {
                    string completeFileName = Path.GetFullPath(Path.Combine(HomeDir, file.FullName));

                    if (!completeFileName.StartsWith(HomeDir, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new IOException("Trying to extract file outside of destination directory. See this link for more info: https://snyk.io/research/zip-slip-vulnerability");
                    }

                    if (file.Name == "")
                    {// Assuming Empty for Directory
                        Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                        continue;
                    }
                    file.ExtractToFile(completeFileName, true);
                }
                zipFile.Dispose();
            } 
            
            System.IO.File.Copy(System.Windows.Forms.Application.ExecutablePath, HomeDir + @"\Blaze.exe");

            WshShell shell = new WshShell();
            string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\Blaze.lnk";

            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Launch game using Blaze!";
            shortcut.TargetPath = HomeDir + @"\Blaze.exe";
            shortcut.Save();

            ProcessStartInfo Info = new ProcessStartInfo();
            Info.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " + System.Windows.Forms.Application.ExecutablePath;
            Info.WindowStyle = ProcessWindowStyle.Hidden;
            Info.CreateNoWindow = true;
            Info.FileName = "cmd.exe";
            Process.Start(Info);

            Process app = new Process();
            app.StartInfo.FileName = HomeDir + @"\Blaze.exe";

            preload.Visibility = Visibility.Hidden;
            app.Start();
            Environment.Exit(0);

            preload.StatusBox.Text = "All done!";
        }

        public static async Task StartUpdate()
        {
            WebClient Client = new WebClient();
            Client.DownloadFile("https://devlin.gg/blaze/install/dll.zip", Directory.GetCurrentDirectory() + @"\Blaze\dll.zip");
        }
    }
}
