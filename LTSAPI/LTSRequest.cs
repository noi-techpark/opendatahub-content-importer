using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

namespace LTSAPI
{
    public class GetDataFromLTSApi
    {
        private static async Task<HttpResponseMessage> LTSRESTRequest(string serviceurl, LTSCredentials credentials)
        {
            try
            {
                CredentialCache wrCache = new CredentialCache();
                wrCache.Add(new Uri(serviceurl), "Basic", new NetworkCredential(credentials.username, credentials.password));

                using (var handler = new HttpClientHandler { Credentials = wrCache, PreAuthenticate = true })
                {
                    using (var client = new HttpClient(handler))
                    {
                        client.Timeout = TimeSpan.FromSeconds(10);
                        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password)));

                        client.DefaultRequestHeaders.Add("X-LTS-ClientID", credentials.ltsclientid);


                        var myresponse = await client.GetAsync(serviceurl);

                        return myresponse;
                    }
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }
        }

        //private static async Task<HttpResponseMessage> AccommodationListRequest(string serviceurl, string lang)
        //{
            

        //}

    }

    public class LTSCredentials
    {
        public string ltsclientid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
