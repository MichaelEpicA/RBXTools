using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace RBXTools
{
    class Updater
    {
        public static bool CheckForUpdates()
        {
            FileVersionInfo currentVersion = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
            Uri githubUri = new Uri("https://github.com/MichaelEpicA/RBXTools");
            Uri latestReleaseUri = new Uri(githubUri + "/releases/latest");
            HttpWebRequest request = WebRequest.CreateHttp(latestReleaseUri);
            Console.WriteLine("Checking for updates...");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string tag = response.ResponseUri.OriginalString.Split('/')[7].Remove(0,1);
            bool updateAvailable = CompareVersions(currentVersion.FileVersion,tag);
            return true;
        }

        public static bool CompareVersions(string currentVersion, string downloadedVersion)
        {
            string[] splitCurrentVersion = currentVersion.Split('.');
            string[] splitDownloadedVersion = downloadedVersion.Split('.');
            for (int i = 0; i < splitCurrentVersion.Length; i++)
            {
                if(Convert.ToInt32(splitCurrentVersion[i]) < Convert.ToInt32(splitDownloadedVersion[i]))
                {
                    //Update detected
                    return true;
                }
            }
            return false;
        }

        public static void UpdateAndRestart()
        {
            Uri githubUri = new Uri("https://github.com/MichaelEpicA/RBXTools");
            Uri latestReleaseUri = new Uri(githubUri + "/releases/latest");
            Uri downloadbaseUri = new Uri(githubUri + "/releases/download/");
            HttpWebRequest request = WebRequest.CreateHttp(latestReleaseUri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string tag = response.ResponseUri.OriginalString.Split('/')[7];
            Uri downloadUri = new Uri(downloadbaseUri + tag + "/RBXTools.exe");
            WebClient client = new WebClient();
            Console.WriteLine("Downloading update...");
            client.DownloadFile(downloadUri, "RBXTools_new.exe");
            Console.WriteLine("Update downloaded.");
            Console.WriteLine("Restarting and deleting this old version...");
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "RBXTools_new.exe",
                Arguments = "-updatereboot",
                UseShellExecute = true,
                CreateNoWindow = false
            };
            Process.Start(info);
            Environment.Exit(0);
        }
    }
}
