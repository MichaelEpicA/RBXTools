using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RBXTools
{
    class Updater
    {
        public static bool CheckForUpdates()
        {
            FileVersionInfo currentVersion = FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName);
            Console.WriteLine("Checking for updates...");
            string tag = GetLatestVersion().Remove(0, 1);
            bool updateAvailable = CompareVersions(currentVersion.FileVersion,tag);
            return updateAvailable;
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
            string description = GetDescriptionOfRelease();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Update Changelog/Important info: " + description);
            Console.ResetColor();
            Console.WriteLine("Still want to update? (Y/N)");
            string input = Console.ReadLine();
            if(input.ToLower().Contains('y'))
            {
                //Yes
                Uri githubUri = new Uri("https://github.com/MichaelEpicA/RBXTools");
                Uri downloadbaseUri = new Uri(githubUri + "/releases/download/");
                string tag = GetLatestVersion();
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
            } else if(input.ToLower().Contains('n'))
            {
                //No
                Console.Clear();
                Console.WriteLine("Update cancelled.");
                Program.Welcome();
            } else
            {
                //Invalid
                Console.Clear();
                Console.WriteLine("That was an invalid input, lets try that again.");
                UpdateAndRestart();
            }
           
        }

        private static string GetLatestVersion()
        {
            try
            {
                Uri githubUri = new Uri("https://github.com/MichaelEpicA/RBXTools");
                Uri latestReleaseUri = new Uri(githubUri + "/releases/latest");
                HttpWebRequest request = WebRequest.CreateHttp(latestReleaseUri);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string tag = response.ResponseUri.OriginalString.Split('/')[7];
                return tag;
            } catch(Exception)
            {
                Console.WriteLine("An error occured while trying to get the latest version. Most likely, you are offline. Offline mode engaged, no online updates are supported here.");
                return "v0.0.0";
            }
            
        }

        private static string GetDescriptionOfRelease()
        {
            string tag = GetLatestVersion();
            Uri apiUrl = new Uri("https://api.github.com/" + "repos/MichaelEpicA/RBXTools/releases/tags/" + tag);
            HttpWebRequest client = WebRequest.CreateHttp(apiUrl);
            client.Headers.Add(HttpRequestHeader.Accept, "application/vnd.github+json");
            client.Headers.Add(HttpRequestHeader.UserAgent, "RBXTools");
            Stream response = client.GetResponse().GetResponseStream();
            using(StreamReader reader = new StreamReader(response))
            {
                string responsestring = reader.ReadToEnd();
                return JObject.Parse(responsestring)["body"].ToString();
            }
        }
    }
}
