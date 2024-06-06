using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LTSAPI
{
    public class LtsApi
    {
        LTSCredentials credentials;
        string baseurl;
        Dictionary<string,string>? parameters;
        string endpoint;

        public LtsApi(LTSCredentials _credentials, Dictionary<string, string>? _parameters) 
        {
            this.baseurl = "https://go.lts.it/api/v1";
            this.credentials = _credentials;
            this.parameters = _parameters;
        }

        private async Task<HttpResponseMessage> LTSRESTRequest()
        {
            try
            {
                var querystring = parameters != null ? "?" + string.Join("&", parameters.Select(x => String.Join("=", x.Key, x.Value))) : "";
                var serviceurl = baseurl + "/" + endpoint + querystring;
             
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials.username + ":" + credentials.password)));
                    client.DefaultRequestHeaders.Add("X-LTS-ClientID", credentials.ltsclientid);

                    var myresponse = await client.GetAsync(serviceurl);

                    return myresponse;
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }
        }

        private async Task<JObject> ParseResponseToJObject(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var contentstr = await response.Content.ReadAsStringAsync();

                return (JObject)JsonConvert.DeserializeObject(contentstr);
            }
            else
            {
                return new JObject(new { error = true, message = "LTS Api error ", exception = await response.Content.ReadAsStringAsync() });
            }
        }

        public async Task<JObject> AccommodationAmenitiesRequest()
        {
            endpoint = "amenities";            
            return await ParseResponseToJObject(await LTSRESTRequest());            
        }

        public async Task<JObject> AccommodationCategoriesRequest()
        {
            endpoint = "accommodations/categories";
            return await ParseResponseToJObject(await LTSRESTRequest());
        }

        public async Task<JObject> AccommodationTypesRequest()
        {
            endpoint = "accommodations/types";
            return await ParseResponseToJObject(await LTSRESTRequest());
        }

        public async Task<JObject> AccommodationListRequest()
        {
            endpoint = "accommodations";
            return await ParseResponseToJObject(await LTSRESTRequest());
        }

        public async Task<JObject> AccommodationSingleRequest(string id)
        {
            endpoint = "accommodations/" + id;
            return await ParseResponseToJObject(await LTSRESTRequest());
        }

        public async Task<JObject> AccommodationDeleteRequest()
        {
            endpoint = "accommodations/deleted";
            return await ParseResponseToJObject(await LTSRESTRequest());
        }
    }

    public class LTSCredentials
    {
        public string ltsclientid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
