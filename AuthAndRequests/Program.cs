using System;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace AuthAndRequests
{
    class Program
    {
        static void Main()
        {
            var clientId = "2437951f-0035-4b1d-a372-26b0e7d6d43a";
            var clientSecret = "WK-AEilujM[D5o7H/rvDC4/o4BB-*m1o";
            var authority = "https://login.microsoftonline.com/9e4b3374-4afb-4472-a026-7d890be74ad8";
            var resourceBaseUrl = "https://mamis9.crm4.dynamics.com";

            var token = TokenHandler.GetToken(clientId, clientSecret, authority, resourceBaseUrl);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new HttpRequestMessage(HttpMethod.Post, $"{resourceBaseUrl}/api/data/v9.1/contacts");
                request.Headers.Add("OData-MaxVersion", "4.0");
                request.Headers.Add("OData-Version", "4.0");
                //request.Headers.Add("Prefer", "odata.include-annotations=\"*\"");

                var o = new JObject();
                o.Add("firstname","Max");
                o.Add("lastname", "Max");

                request.Content = new StringContent(o.ToString(), Encoding.UTF8, "application/json");

                var result = client.SendAsync(request).Result;
                Console.WriteLine(result.Content.ReadAsStringAsync().Result);
            }
        }
    }
}
