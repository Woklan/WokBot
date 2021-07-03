using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WokBot.Classes
{
    public class Utility
    {
        HttpWebRequest request;
        static readonly HttpClient client = new HttpClient();

        public Utility()
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<T> ApiCall<T>(string url)
        {
            // Get Request to Website
            var data = await client.GetAsync(url);
            
            // Parses content into string
            string parse = await data.Content.ReadAsStringAsync();

            // Returns object created from JSON
            return JsonConvert.DeserializeObject<T>(parse);          
        }
    }
}
