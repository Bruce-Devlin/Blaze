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

        public static Preload preload;

        public static async Task<bool> CheckInstallation() { if (Directory.Exists(Variables.HomeDir) || Directory.GetCurrentDirectory().EndsWith("Debug")) { return true; } else return false; }
        
        public static async Task<VersionNumber> GetLocalVersion()
        {
            List<int> nums = new List<int>();
            foreach (string num in System.Windows.Forms.Application.ProductVersion.Split('.')) nums.Add(int.Parse(num));
            return new VersionNumber() { Major = nums[0], Minor = nums[1], Build = nums[2], Patch = nums[3] };
        }

        public static async Task<bool> CheckForUpdates()
        {
            VersionNumber currVersion = await GetLocalVersion();

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

        public static async Task Replace()
        {
            if (System.IO.File.Exists(Variables.HomeDir + @"\Blaze.exe")) System.IO.File.Delete(Variables.HomeDir + @"\Blaze.exe");
            System.IO.File.Copy(System.Windows.Forms.Application.ExecutablePath, Variables.HomeDir + @"\Blaze.exe");

            Process secondProc = new Process();
            secondProc.StartInfo.FileName = Variables.HomeDir + @"\Blaze.exe";
            secondProc.Start();

            ProcessStartInfo cmd = new ProcessStartInfo();
            cmd.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " + System.Windows.Forms.Application.ExecutablePath;
            cmd.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.CreateNoWindow = true;
            cmd.FileName = "cmd.exe";
            Process.Start(cmd);

            Application.Current.Shutdown();
        }

        public static async Task StartInstall(Preload win)
        {
            preload = win;
            preload.StatusBox.Text = "Creating home directory...";
            if (!Directory.Exists(Variables.HomeDir)) Directory.CreateDirectory(Variables.HomeDir);
            await DownloadDLL();
        }

        static async Task DownloadDLL()
        {
            preload.StatusBox.Text = "Downloading DLL's...";
            var client = new WebClient();
            var uri = new Uri("https://devlin.gg/blaze/install/dll.zip");

            client.DownloadFileCompleted += (sender, e) => FinishInstall().Wait();
            client.DownloadFileAsync(uri, Variables.HomeDir + @"\dll.zip");
        }

        static async Task FinishInstall()
        {
            preload.StatusBox.Text = "Finishing up...";
            string dllZipFile = Variables.HomeDir + @"\dll.zip";

            using (var fileStream = new FileStream(dllZipFile, FileMode.Open))
            {
                ZipArchive zipFile = new ZipArchive(fileStream);
                foreach (ZipArchiveEntry file in zipFile.Entries)
                {
                    string completeFileName = Path.GetFullPath(Path.Combine(Variables.HomeDir, file.FullName));

                    if (!completeFileName.StartsWith(Variables.HomeDir, StringComparison.OrdinalIgnoreCase))
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

            preload.StatusBox.Text = "All done! Restarting Blaze...";
            System.IO.File.Delete(Variables.HomeDir + @"\dll.zip");

            if (System.IO.File.Exists(Variables.HomeDir + @"\Blaze.exe")) System.IO.File.Delete(Variables.HomeDir + @"\Blaze.exe");
            System.IO.File.Copy(System.Windows.Forms.Application.ExecutablePath, Variables.HomeDir + @"\Blaze.exe");

            if (!System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Blaze.lnk"))
            {
                WshShell shell = new WshShell();
                string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\Blaze.lnk";

                IWshShortcut desktopShortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                desktopShortcut.Description = "Launch game using Blaze!";
                desktopShortcut.TargetPath = Variables.HomeDir + @"\Blaze.exe";
                desktopShortcut.Save();
            }
            if (!System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\Blaze.lnk"))
            {
                WshShell shell = new WshShell();
                string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + @"\Blaze.lnk";

                IWshShortcut addToWindows = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
                addToWindows.Description = "Launch games using Blaze!";
                addToWindows.TargetPath = Variables.HomeDir + @"\Blaze.exe";
                addToWindows.Save();
            }

            Process secondProc = new Process();
            secondProc.StartInfo.FileName = Variables.HomeDir + @"\Blaze.exe";
            secondProc.StartInfo.Arguments = "-installed";
            secondProc.Start();

            ProcessStartInfo cmd = new ProcessStartInfo();
            cmd.Arguments = "/C choice /C Y /N /D Y /T 3 & Del " + System.Windows.Forms.Application.ExecutablePath;
            cmd.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.CreateNoWindow = true;
            cmd.FileName = "cmd.exe";
            Process.Start(cmd);

            Application.Current.Shutdown();
        }

        public static async Task StartUpdate()
        {
            WebClient Client = new WebClient();
            Client.DownloadFile("https://devlin.gg/blaze/install/dll.zip", Directory.GetCurrentDirectory() + @"\Blaze\dll.zip");
        }

        static async Task DownloadUpdate()
        {
            preload.StatusBox.Text = "Downloading DLL's...";
            var client = new WebClient();
            var uri = new Uri("https://devlin.gg/blaze/install/dll.zip");

            client.DownloadFileCompleted += (sender, e) => FinishInstall().Wait();
            client.DownloadFileAsync(uri, Variables.HomeDir + @"\dll.zip");
        }
    }
}
