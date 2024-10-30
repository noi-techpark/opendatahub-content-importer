using LTSAPI;
using HGVApi;
using GenericHelper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitPusher;
using System.Net;
using System.Net.Http.Json;
using System.Security.Principal;
using System.Text;
using System.Xml.Linq;
using System;
using Newtonsoft.Json.Linq;

namespace DataImportHelper
{
    public interface IDataImport
    {
        Task ImportLTSAccoAmenities();
        Task ImportLTSAccoCategories();
        Task ImportLTSAccoTypes();
        Task ImportLTSAccommodationChanged(DateTime datefrom);
        Task ImportLTSAccommodationDeleted(DateTime datefrom);
        Task ImportLTSAccommodationSingle(string rid, string? identifier = null);
        Task ImportHGVAccommodationSingle(string rid, string? identifier = null);
        Task ImportHGVAccommodationRoomList(string rid, string? identifier = null);
        Task ImportAccommodationFinished(JObject json, string? identifier = null);
    }

    public class DataImport : IDataImport
    {
        public LtsApi ltsapi { get; set; }
        public HgvApi hgvapi { get; set; }

        public string opendata { get; set; }

        RabbitMQSend rabbitsend { get; set; }

        public DataImport(ISettings settings)
        {
            ltsapi = new LtsApi(settings.LtsCredentials);
            hgvapi = new HgvApi(settings.HgvCredentials);
            if (settings.LtsCredentials.opendata)
                opendata = "_opendata";
            else
                opendata = "";

            rabbitsend = new RabbitMQSend(settings.RabbitConnection);
        }

        public DataImport(LTSCredentials ltscredentials, HGVCredentials hgvcredentials, string rabbitconnection)
        {
            ltsapi = new LtsApi(ltscredentials);
            hgvapi = new HgvApi(hgvcredentials);

            if (ltscredentials.opendata)
                opendata = "_opendata";
            else
                opendata = "";

            rabbitsend = new RabbitMQSend(rabbitconnection);
        }

        //This import methods are used by the Api and Console Application
        #region Import Methods




        /// <summary>
        /// Imports the LTS Accommodation Amenities and pushes it to RabbitMQ
        /// </summary>
        /// <returns></returns>
        public async Task ImportLTSAccoAmenities()
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationAmenityRequest(null, true);
            rabbitsend.Send("lts/accommodationamenities", ltsdata);                        
        }

        /// <summary>
        /// Imports the LTS Accommodation Categories and pushes it to RabbitMQ
        /// </summary>
        /// <returns></returns>
        public async Task ImportLTSAccoCategories()
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationCategoryRequest(null, true);
            rabbitsend.Send("lts/accommodationcategories", ltsdata);
        }

        /// <summary>
        ///  Imports the LTS Accommodation Types and pushes it to RabbitMQ
        /// </summary>
        /// <returns></returns>
        public async Task ImportLTSAccoTypes()
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationTypeRequest(null, true);
            rabbitsend.Send("lts/accommodationtypes", ltsdata);
        }

        public async Task ImportLTSAccommodationChanged(DateTime datefrom)
        {
            var qs = new LTSQueryStrings() { 
                page_size = 100, 
                filter_lastUpdate = datefrom,
                filter_marketingGroupRids = "9E72B78AC5B14A9DB6BED6C2592483BF",
                fields = "rid" //when fields rid is set lts api gives all pages without paging
            };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationListRequest(dict, true);
            rabbitsend.Send("lts/accommodationchanged", ltsdata);
        }

        public async Task ImportLTSAccommodationDeleted(DateTime datefrom)
        {
            var qs = new LTSQueryStrings() { page_size = 1, filter_lastUpdate = datefrom };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationDeleteRequest(dict, true);
            rabbitsend.Send("lts/accommodationdeleted", ltsdata);
        }

        public async Task ImportLTSAccommodationSingle(string rid, string? identifier = null)
        {
            var qs = new LTSQueryStrings() { page_size = 1 };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationDetailRequest(rid, null);
            rabbitsend.Send("lts/accommodationdetail" + opendata, ltsdata, identifier);
        }

        public async Task ImportHGVAccommodationSingle(string rid, string? identifier = null)
        {
            var qs = new LTSQueryStrings() { page_size = 1 };
            var dict = ltsapi.GetLTSQSDictionary(qs);

            var ltsdata = await ltsapi.AccommodationDetailRequest(rid, null);
            rabbitsend.Send("lts/accommodationdetail" + opendata, ltsdata, identifier);
        }

        public async Task ImportHGVAccommodationRoomList(string rid, string? identifier = null)
        {
            XElement roomdetail = new XElement("room_details", 69932);

            //To check, if room data has to be requested in each language?
            XDocument myrequest = hgvapi.BuildRoomlistPostData(roomdetail, rid, "lts", "", "sinfo", "2");
            var myresponses = await hgvapi.RequestRoomListAsync(myrequest);

            string roomresponsecontent = await myresponses.Content.ReadAsStringAsync();
            
            XElement fullresponse = XElement.Parse(roomresponsecontent);

            rabbitsend.Send("hgv/accoommodationroom", fullresponse, identifier);
        }

        public async Task ImportAccommodationFinished(JObject json, string? identifier = null)
        {
            rabbitsend.Send("base/accommodationimported", json, identifier);
        }

        #endregion

    }

    public interface IODHApiConnector
    {
        Task<T> GetFromODHApiCore<T>(T data, string id, string getendpoint);
        Task<HttpResponseMessage> PostToODHApiCore<T>(T data, string id, string putendpoint);
        Task<HttpResponseMessage> PutToODHApiCore<T>(T data, string id, string putendpoint);
        Task<HttpResponseMessage> DeleteFromODHApiCore(string id, string deleteendpoint);
    }

    public class ODHApiConnector : IODHApiConnector
    {
        protected string odhapicoreendpoint = "";
        protected string endpoint = "";
        protected string clientid = "";
        protected string clientsecret = "";

        public ODHApiConnector(string _endpoint, string _clientid, string _clientsecret, string _odhapicoreendpoint)
        {
            endpoint = _endpoint;
            clientid = _clientid;
            clientsecret = _clientsecret;
            odhapicoreendpoint = _odhapicoreendpoint;
        }

        public async Task<T> GetFromODHApiCore<T>(T data, string id, string getendpoint)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Referrer = new Uri("https://tourism.importer.v2");
                //Get Token from Singleton
                ODHTokenStore tokenstore = await ODHTokenStore.GetInstance(endpoint, clientid, clientsecret);
                var token = tokenstore.GetBearerHeader();
                //Add the Bearer Token to the Request Header
                client.DefaultRequestHeaders.Add("Authorization", token);

                var requesturl = odhapicoreendpoint + getendpoint + "/" + id;

                var result = await client.GetAsync(requesturl);

                return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
            }
        }

        public async Task<HttpResponseMessage> PushToODHApiCore<T>(T data, string id, string postendpoint)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Referrer = new Uri("https://tourism.importer.v2");
                //Get Token from Singleton
                ODHTokenStore tokenstore = await ODHTokenStore.GetInstance(endpoint, clientid, clientsecret);
                var token = tokenstore.GetBearerHeader();
                //Add the Bearer Token to the Request Header
                client.DefaultRequestHeaders.Add("Authorization", token);

                var requesturl = odhapicoreendpoint + postendpoint + "/" + id;

                return await client.PutAsync(requesturl, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
            }
        }

        public async Task<HttpResponseMessage> PostToODHApiCore<T>(T data, string id, string postendpoint)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Referrer = new Uri("https://tourism.importer.v2");
                //Get Token from Singleton
                ODHTokenStore tokenstore = await ODHTokenStore.GetInstance(endpoint, clientid, clientsecret);
                var token = tokenstore.GetBearerHeader();
                //Add the Bearer Token to the Request Header
                client.DefaultRequestHeaders.Add("Authorization", token);

                var requesturl = odhapicoreendpoint + postendpoint + "/" + id;

                return await client.PostAsync(requesturl, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
            }
        }

        public async Task<HttpResponseMessage> PutToODHApiCore<T>(T data, string id, string putendpoint)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Referrer = new Uri("https://tourism.importer.v2");
                //Get Token from Singleton
                ODHTokenStore tokenstore = await ODHTokenStore.GetInstance(endpoint, clientid, clientsecret);
                var token = tokenstore.GetBearerHeader();
                //Add the Bearer Token to the Request Header
                client.DefaultRequestHeaders.Add("Authorization", token);

                var requesturl = odhapicoreendpoint + putendpoint + "/" + id;

                return await client.PutAsync(requesturl, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
            }
        }
        
        public async Task<HttpResponseMessage> DeleteFromODHApiCore(string id, string deleteendpoint)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Referrer = new Uri("https://tourism.importer.v2");
                //Get Token from Singleton
                ODHTokenStore tokenstore = await ODHTokenStore.GetInstance(endpoint, clientid, clientsecret);
                var token = tokenstore.GetBearerHeader();
                //Add the Bearer Token to the Request Header
                client.DefaultRequestHeaders.Add("Authorization", token);

                var requesturl = odhapicoreendpoint + deleteendpoint + "/" + id;

               return await client.DeleteAsync(requesturl);                
            }
        }
    }

    public class ODHTokenStore
    {
        private static ODHTokenStore _instance;
        private static Token odhtoken;
        private DateTime expirationdate;

        static string serviceaccount_clientid = "";
        static string serviceaccount_clientsecret = "";
        static string serviceaccount_url = "";

        private ODHTokenStore(string endpoint, string clientid, string clientsecret)
        {
            serviceaccount_clientid = clientid;
            serviceaccount_clientsecret = clientsecret;
            serviceaccount_url = endpoint;
        }

        public static async Task<ODHTokenStore> GetInstance(string endpoint, string clientid, string clientsecret)
        {
            if (_instance == null || _instance.isTokenExpired())
            {
                _instance = new ODHTokenStore(endpoint, clientid, clientsecret);
                await _instance.GetToken();
            }

            return _instance;
        }

        private async Task GetToken()
        {
            HttpClient client = new HttpClient();

            string baseAddress = serviceaccount_url;

            string grant_type = "client_credentials";
            string client_id = serviceaccount_clientid;
            string client_secret = serviceaccount_clientsecret;

            var form = new Dictionary<string, string>
                {
                    {"grant_type", grant_type},
                    {"client_id", client_id},
                    {"client_secret", client_secret},
                };

            var now = DateTime.Now;

            HttpResponseMessage tokenResponse = await client.PostAsync(baseAddress, new FormUrlEncodedContent(form));
            var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
            odhtoken = JsonConvert.DeserializeObject<Token>(jsonContent);

            expirationdate = now.AddSeconds(odhtoken.ExpiresIn);
        }

        private bool isTokenExpired()
        {
            if (odhtoken == null)
                return true;
            else
            {
                if (expirationdate > DateTime.Now)
                    return false;
                else
                    return true;

            }
        }

        public string GetBearerHeader()
        {
            return "Bearer " + odhtoken.AccessToken;
        }
    }

    internal class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }

    public class DataImportLog
    {
        //TODO Add Log classes
    }
}
