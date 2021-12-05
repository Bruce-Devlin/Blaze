using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Blaze.Functions
{
    class Update
    {
        /*
        private static async Task<StreamReader> GetUpdateFiles()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://devlin.gg");
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential("blaze@devlin", "z}2ye0b0X1u*");

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            return reader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        }
        */

        public static async Task Start()
        {
            List<string> urls = new List<string>();



            var tasks = new List<Task>();
            var updateWebClient = new WebClient();
            for (int i = 0; i < urls.Count; i++)
            {
                MessageBox.Show("downloading " + urls[i]);
                var uri = new Uri(urls[i]);
                tasks.Add(updateWebClient.DownloadFileTaskAsync(uri, Variables.HomeDir + @"\" + i.ToString()));
            }

            await Task.WhenAll(tasks);
            MessageBox.Show("Done.");
        }

    }
}
