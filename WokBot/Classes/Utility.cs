using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace WokBot.Classes
{
    public class Utility
    {
        HttpWebRequest request;

        public T ApiCall<T>(string url)
        {
            string html = string.Empty;

            request = (HttpWebRequest)WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(html);
        }
    }
}
