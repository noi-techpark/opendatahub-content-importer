using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using Helper;

namespace LTSAPI
{
    public class LtsApi
    {
        LTSCredentials credentials;
        string baseurl;
        IDictionary<string,string>? parameters;
        string endpoint;
        bool getallpages = false;

        public LtsApi(LTSCredentials _credentials) 
        {
            //TODO SET THIS URL IN CONFIG
            this.baseurl = "https://go.lts.it/api/v1";
            this.credentials = _credentials;                        
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

        private async Task<List<JObject>> RequestAndParseToJObject()
        {
            var response = await LTSRESTRequest();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jobjectlist = new List<JObject>() { };

                var contentstr = await response.Content.ReadAsStringAsync();

                if (String.IsNullOrEmpty(contentstr))
                {
                    jobjectlist.Add(new JObject(new { error = true, message = "LTS Api error", exception = "No data from api" }));
                    return jobjectlist;
                }

                var contentjson = (JObject)JsonConvert.DeserializeObject(contentstr);

                if (contentjson == null)
                {
                    jobjectlist.Add(new JObject(new { error = true, message = "LTS Api error ", exception = "Deserialization failed" }));
                    return jobjectlist;
                }

                jobjectlist.Add(contentjson);

                //Get all Pages and add the response
                if (getallpages)
                {
                    var pagesquantity = (string)contentjson["paging"]["pagesQuantity"];
                    int.TryParse(pagesquantity, out int pagesquantityint);

                    var currentpage = (string)contentjson["paging"]["pageNumber"];
                    int.TryParse(currentpage, out int currentpageint);

                    var resultrid = "";
                    if(contentjson.ContainsKey("resultSet"))
                        resultrid = (string)contentjson["resultSet"]["rid"];


                    if (currentpageint < pagesquantityint)
                    {
                        //Add page parameter
                        parameters.TryAddOrUpdate("page[number]", (currentpageint + 1).ToString());
                        //Add filter[resultSet][rid]
                        if(!String.IsNullOrEmpty(resultrid))
                            parameters.TryAddOrUpdate("filter[resultSet][rid]", resultrid);
                        //Request again
                        jobjectlist.AddRange(await RequestAndParseToJObject());
                    }                  
                }

                return jobjectlist;
            }
            else
            {
                return new List<JObject>() { new JObject(new { error = true, message = "LTS Api error ", exception = await response.Content.ReadAsStringAsync() }) };
            }
        }

        private async Task<List<JObject>> LTSRequestMethod(string _endpoint, IDictionary<string, string>? _parameters, bool _getallpages)
        {
            endpoint = _endpoint;
            parameters = _parameters == null ? new Dictionary<string, string>() : _parameters;
            getallpages = _getallpages;

            return await RequestAndParseToJObject();
        }


        #region Accommodation

        public async Task<List<JObject>> AccommodationAmenitiesRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {            
            return await LTSRequestMethod("amenities", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationCategoriesRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("accommodations/categories", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationTypesRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {            
            return await LTSRequestMethod("accommodations/types", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {            
            return await LTSRequestMethod("accommodations", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("accommodations/" + id, _parameters, false);            
        }

        public async Task<List<JObject>> AccommodationDeleteRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {            
            return await LTSRequestMethod("accommodations/deleted", _parameters, _getallpages);
        }

        #endregion

        #region Poi

        public async Task<List<JObject>> PoiListRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("pointofinterests", _parameters, _getallpages);
        }

        public async Task<List<JObject>> PoiDetailRequest(string id, Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("pointofinterests/" + id, _parameters, false);
        }

        public async Task<List<JObject>> PoiDeletedRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("pointofinterests/deleted", _parameters, false);
        }

        #endregion

        #region Activity

        public async Task<List<JObject>> ActivityListRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("activities", _parameters, _getallpages);
        }

        public async Task<List<JObject>> ActivityDetailRequest(string id, Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("activities/" + id, _parameters, false);
        }

        public async Task<List<JObject>> ActivityDeletedRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("activities/deleted", _parameters, false);
        }

        #endregion

        #region Gastronomy

        public async Task<List<JObject>> GastronomyListRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies", _parameters, _getallpages);
        }

        public async Task<List<JObject>> GastronomyDetailRequest(string id, Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/" + id, _parameters, false);
        }

        public async Task<List<JObject>> GastronomyDeletedRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/deleted", _parameters, false);
        }

        public async Task<List<JObject>> GastronomyCategoriesRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/categories", _parameters, false);
        }

        #endregion

        #region Beacon

        public async Task<List<JObject>> BeaconListRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("beacons", _parameters, _getallpages);
        }

        public async Task<List<JObject>> BeaconDetailRequest(string id, Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("beacons/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> BeaconDeletedRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("beacons/deleted", _parameters, false);
        }

        #endregion

        #region Event

        public async Task<List<JObject>> EventListRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventDetailRequest(string id, Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/" + id, _parameters, false);
        }

        public async Task<List<JObject>> EventDeletedRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/deleted", _parameters, false);
        }

        #endregion

        #region Venue
        public async Task<List<JObject>> VenueListRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("venues", _parameters, _getallpages);
        }

        public async Task<List<JObject>> VenueDetailRequest(string id, Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("venues/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> VenueDeletedRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("venues/deleted", _parameters, false);
        }

        public async Task<List<JObject>> VenueCategoriesRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("venues/categories", _parameters, false);
        }

        #endregion

        #region Webcam

        public async Task<List<JObject>> WebcamListRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("webcams", _parameters, _getallpages);
        }

        public async Task<List<JObject>> WebcamDetailRequest(string id, Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("webcams/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> WebcamDeletedRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("webcams/deleted", _parameters, false);
        }

        #endregion

        #region WeatherSnow

        public async Task<List<JObject>> WeatherSnowListRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("weathersnows", _parameters, _getallpages);
        }

        public async Task<List<JObject>> WeatherSnowDetailRequest(string id, Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("weathersnows/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> WeatherSnowDeletedRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("weathersnows/deleted", _parameters, false);
        }

        #endregion

        #region Tag

        public async Task<List<JObject>> TagListRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("tags", _parameters, _getallpages);
        }

        public async Task<List<JObject>> TagDetailRequest(string id, Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("tags/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> TagDeletedRequest(Dictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("tags/deleted", _parameters, false);
        }

        #endregion

        #region QueryString Helper

        public IDictionary<string, string> GetLTSQSDictionary(LTSQueryStrings qs)
        {
            IDictionary<string, string> qsdict = new Dictionary<string, string>();
            
            foreach (var prop in qs.GetType().GetProperties())
            {
                string propkey = "";
                var propkeysplitted = prop.Name.Split("_");
                int i = 0;
                foreach(var pk in propkeysplitted)
                {
                    if (i == 0)
                        propkey = propkey + pk;
                    else
                        propkey = propkey + "[" + pk + "]";

                    i++;
                }

                var propvalue = prop.GetValue(qs, null);

                if (propvalue != null)
                {
                    string? valuetoadd = null;

                    if(prop.PropertyType == typeof(DateTime?))
                    {
                        valuetoadd = String.Format("{0:yyyy-MM-ddThh:mm:ss.fff}", (DateTime)propvalue);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(propvalue.ToString()))
                            valuetoadd = propvalue.ToString();
                    }

                    if (!String.IsNullOrEmpty(valuetoadd))
                        qsdict.Add(propkey, valuetoadd);
                }
                    
            }

            return qsdict;
        }

        #endregion
    }

    public class LTSCredentials
    {
        public string ltsclientid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class LTSQueryStrings
    {
        //generic
        public string? fields { get; set; }
        public string? sort { get; set; }
        public string? language { get; set; }
        public int? page_number { get; set; }
        public int? page_size { get; set; }
        public string? filter_language { get; set; }
        public bool? filter_onlyActive { get; set; }
        public string? filter_rids { get; set; }
        public DateTime? filter_lastUpdate { get; set; }
        public string? filter_categoryRids { get; set; }
        public string? filter_districtRids { get; set; }
        public string? filter_searchTerm { get; set; }

        //accommodation
        public string? filter_tourismOrganizationRids { get; set; }
        public string? filter_marketingGroupRids { get; set; }
        public bool? filter_onlySuedtirolInfoActive { get; set; }        
        public int? filter_minAltitude { get; set; }
        public int? filter_maxAltitude { get; set; }
        public string? filter_representationMode { get; set; }
        public string? filter_mealPlanRids { get; set; }
        public string? filter_amenityRids { get; set; }
        public string? filter_roomGroup_amenityRids { get; set; }
        public string? filter_typeRids { get; set; }
        public bool? filter_hasRooms { get; set; }
        public bool? filter_hasApartments { get; set; }        
        public string? filter_holidayPackageRids { get; set; }
        public string? filter_addressGroupRids { get; set; }
        public bool? filter_onlyBookable { get; set; }
        public bool? filter_onlyActiveGaleries { get; set; }
        public string? filter_contactType { get; set; }
        public bool? filter_onlyHgvActive { get; set; }
        public bool? filter_allowedMarketingGroupRids { get; set; }
        public string? filter_allowedTourismOrganizationRids { get; set; }

        //event

        public string? filter_posId { get; set; }
        public string? filter_startDate { get; set; }
        public string? filter_endDate { get; set; }
        public int? filter_publisherSettingMinImportanceRate { get; set; }        
        public string? filter_zoneRids { get; set; }        
        public string? filter_organizerRids { get; set; }
        public bool? filter_onlyVisibleInEventFinder { get; set; }

        //poi

        public bool? filter_hasFreeEntry { get; set; }
        public bool? filter_isOpen { get; set; }        
        public bool? filter_favouriteFor { get; set; }        
        public string? filter_tagRids { get; set; }
        public string? filter_ownerRids { get; set; }
        public string? filter_areaRids { get; set; }        
        public string? filter_aroundPosition_ { get; set; }
        public string? filter_coordinates_example { get; set; }
        public string? filter_radiusInMeters { get; set; }

        //activity
        public bool? filter_isIlluminated { get; set; }
        public bool? filter_hasRental { get; set; }
        public bool? filter_isPrepared { get; set; }

        public int? filter_rating_minStamina { get; set; }
        public int? filter_rating_maxStamina { get; set; }
        public int? filter_rating_minExperience { get; set; }
        public int? filter_rating_maxExperience { get; set; }
        public int? filter_rating_minLandscape { get; set; }
        public int? filter_rating_maxLandscape { get; set; }
        public int? filter_rating_minDifficulty { get; set; }
        public int? filter_rating_maxDifficulty { get; set; }
        public int? filter_rating_minTechnique { get; set; }
        public int? filter_rating_maxTechnique { get; set; }
        public int? filter_rating_minViaFerrataTechnique { get; set; }
        public int? filter_rating_maxViaFerrataTechnique { get; set; }
        public int? filter_rating_minScaleUIAATechnique { get; set; }
        public int? filter_rating_maxScaleUIAATechnique { get; set; }

        //gastronomy
        public bool? filter_onlyTourismOrganizationMember { get; set; }
        public string? filter_facilityRids { get; set; }
        public DateTime? filter_openOnDate { get; set; }
        
        //webcams
        public bool? filter_excludeOutOfOrder { get; set; }

        //beacons
        public string? filter_pointOfInterestRids { get; set; }

        //tags
        public string? filter_codes { get; set; }
        public string? filter_rootTagRids { get; set; }
        public int? filter_level { get; set; }
        public string? filter_entity { get; set; }
    }
    
}
