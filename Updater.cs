using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RBXTools
{
    class Updater
    {
        public static void CheckForUpdates()
        {
            Uri githubUri = new Uri("https://github.com/MichaelEpicA/RBXTools");
            Uri latestReleaseUri = new Uri(githubUri + "/releases/latest");
            WebClient client = new WebClient();
            HttpWebRequest request = WebRequest.CreateHttp(latestReleaseUri);
            Console.WriteLine("Checking for updates...");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string tag = response.ResponseUri.OriginalString.Split('/')[7];
            CompareVersions();
        }

        public static void CompareVersions()
        {

        }
    }
}
