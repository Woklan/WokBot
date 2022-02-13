using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net;

namespace WokBot.Classes
{
    public class ZipInsight
    {
        private const string zipFileType = ".zip";
        public List<string> ZipContents(string zipUrl, string videoOutput)
        {
            List<string> list = new List<string>();
            var uri = new System.Uri(zipUrl);

            var guid = System.Guid.NewGuid().ToString();

            var zipFileName = videoOutput + guid + zipFileType;

            using(var client = new WebClient())
            {
                client.DownloadFile(uri, zipFileName);
            }

            using (var zip = ZipFile.OpenRead(zipFileName))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    var updatedString = "-> " + entry.Name;
                    list.Add(updatedString);
                }
            }
            
            return list;
        }
    }
}
