using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using GenericHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Http.Json;

namespace LTSAPI
{
    public class LtsApi
    {
        LTSCredentials credentials;
        string baseurl;
        IDictionary<string, string>? parameters;
        string endpoint;
        bool getallpages = false;
        StringContent body;

        public LtsApi(LTSCredentials _credentials)
        {
            this.baseurl = _credentials.serviceurl;
            this.credentials = _credentials;
        }

        public LtsApi(string _serviceurl, string _username, string _password, string _xltsclientid, bool _opendata = false)
        {
            this.baseurl = _serviceurl;
            this.credentials = new LTSCredentials(_serviceurl, _username, _password, _xltsclientid, _opendata);
        }

        private async Task<HttpResponseMessage> LTSGETRequest()
        {
            try
            {
                var querystring = parameters != null ? "?" + string.Join("&", parameters.Select(x => String.Join("=", x.Key, x.Value))) : "";
                var serviceurl = baseurl + "/" + endpoint + querystring;

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
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

        private async Task<HttpResponseMessage> LTSPOSTRequest()
        {
            try
            {
                var querystring = parameters != null ? "?" + string.Join("&", parameters.Select(x => String.Join("=", x.Key, x.Value))) : "";
                var serviceurl = baseurl + "/" + endpoint + querystring;
 
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials.username + ":" + credentials.password)));
                    client.DefaultRequestHeaders.Add("X-LTS-ClientID", credentials.ltsclientid);

                    var myresponse = await client.PostAsync(serviceurl, body);

                    return myresponse;
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(ex.Message) };
            }
        }

        private async Task<HttpResponseMessage> LTSRESTRequest(string method = "GET")
        {
            if (method == HttpMethods.Post)
            {
                return await LTSPOSTRequest();
            }
            else
                return await LTSGETRequest();
        }


        private async Task<List<JObject>> RequestAndParseToJObject(string method = "GET")
        {
            var response = await LTSRESTRequest(method);

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
                    if (contentjson.ContainsKey("resultSet"))
                        resultrid = (string)contentjson["resultSet"]["rid"];


                    if (currentpageint < pagesquantityint)
                    {
                        //Add page parameter
                        parameters.TryAddOrUpdate("page[number]", (currentpageint + 1).ToString());
                        //Add filter[resultSet][rid]
                        if (!String.IsNullOrEmpty(resultrid))
                            parameters.TryAddOrUpdate("filter[resultSet][rid]", resultrid);
                        //Request again
                        jobjectlist.AddRange(await RequestAndParseToJObject());
                    }
                }

                return jobjectlist;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var error = await response.Content.ReadAsStringAsync();

                //LTS Api Object not Found
                return new List<JObject>() { JObject.FromObject(new { success = false, error = true, message = "LTS Api Object not Found", exception = error, status = 404 }) };
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                var error = await response.Content.ReadAsStringAsync();

                //LTS Api Internal Error
                return new List<JObject>() { JObject.FromObject(new { success = false, error = true, message = "LTS Api Internal Error", exception = error, status = 500 }) };
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var error = await response.Content.ReadAsStringAsync();

                //LTS Api Object not accessible
                return new List<JObject>() { JObject.FromObject(new { success = false, error = true, message = "LTS Api Object not accessible", exception = error, status = 403 }) };
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return new List<JObject>() { JObject.FromObject(new { success = false, error = true, message = "Generic LTS Api error ", exception = error, status = 400 }) };
            }
        }

        private async Task<List<JObject>> LTSRequestMethod(string _endpoint, IDictionary<string, string>? _parameters, bool _getallpages)
        {
            endpoint = _endpoint;
            parameters = _parameters == null ? new Dictionary<string, string>() : _parameters;
            getallpages = _getallpages;

            return await RequestAndParseToJObject();
        }

        private async Task<List<JObject>> LTSRequestMethod<T>(string _endpoint, IDictionary<string, string>? _parameters, bool _getallpages, T _body)
        {
            endpoint = _endpoint;
            parameters = _parameters == null ? new Dictionary<string, string>() : _parameters;
            getallpages = _getallpages;
            var postbody = new LTSPostBody<T>() { parameters = _body };
            //body = JsonContent.Create(postbody);
            body = new StringContent(JsonConvert.SerializeObject(postbody), Encoding.UTF8, "application/json");

            return await RequestAndParseToJObject("POST");
        }

        #region AccommodationAvailability

        public async Task<List<JObject>> AccommodationAvailabilitySearchRequest(IDictionary<string, string>? _parameters, LTSAvailabilitySearchRequestBody _body)
        {
            return await LTSRequestMethod<LTSAvailabilitySearchRequestBody>("accommodations/availabilities/search", _parameters, false, _body);
        }


        #endregion

        #region Accommodation

        public async Task<List<JObject>> AccommodationAmenityRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("amenities", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationAmenityDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("accommodations/amenities" + id, _parameters, false);
        }

        public async Task<List<JObject>> AccommodationCategoryRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("accommodations/categories", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationCategorysDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("accommodations/categories" + id, _parameters, false);
        }

        public async Task<List<JObject>> AccommodationTypeRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("accommodations/types", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationTypeDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("accommodations/types/" + id, _parameters, false);
        }

        public async Task<List<JObject>> AccommodationMealplanRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("accommodations/mealplans", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationMealplanDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("accommodations/mealplans/" + id, _parameters, false);
        }

        public async Task<List<JObject>> AccommodationRateplanRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("accommodations/rateplans", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationRateplanDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("accommodations/rateplans/" + id, _parameters, false);
        }

        public async Task<List<JObject>> AccommodationAvailabilityRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("accommodations/availabilities", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("accommodations", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("accommodations/" + id, _parameters, false);
        }


        public async Task<List<JObject>> AccommodationRoomGroupListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("accommodations/roomgroups", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AccommodationRoomGroupDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("accommodations/roomgroups/" + id, _parameters, false);
        }

        public async Task<List<JObject>> AccommodationDeleteRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("accommodations/deleted", _parameters, _getallpages);
        }

        #endregion

        #region Poi

        public async Task<List<JObject>> PoiListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("pointofinterests", _parameters, _getallpages);
        }

        public async Task<List<JObject>> PoiDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("pointofinterests/" + id, _parameters, false);
        }

        public async Task<List<JObject>> PoiDeletedRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("pointofinterests/deleted", _parameters, _getallpages);
        }

        #endregion

        #region Activity

        public async Task<List<JObject>> ActivityListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("activities", _parameters, _getallpages);
        }

        public async Task<List<JObject>> ActivityDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("activities/" + id, _parameters, false);
        }

        public async Task<List<JObject>> ActivityDeletedRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("activities/deleted", _parameters, _getallpages);
        }

        #endregion

        #region Gastronomy

        public async Task<List<JObject>> GastronomyListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies", _parameters, _getallpages);
        }

        public async Task<List<JObject>> GastronomyDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("gastronomies/" + id, _parameters, false);
        }

        public async Task<List<JObject>> GastronomyDeletedRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/deleted", _parameters, _getallpages);
        }

        public async Task<List<JObject>> GastronomyCategoryRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/categories", _parameters, _getallpages);
        }

        public async Task<List<JObject>> GastronomyCategoryDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("gastronomies/categories/" + id, _parameters, false);
        }

        public async Task<List<JObject>> GastronomyFacilityRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/facilities", _parameters, _getallpages);
        }

        public async Task<List<JObject>> GastronomyFacilityDetailRequest(string id, IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/facilities/" + id, _parameters, _getallpages);
        }

        public async Task<List<JObject>> GastronomyDishRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/dishes", _parameters, _getallpages);
        }

        public async Task<List<JObject>> GastronomyDishDetailRequest(string id, IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/dishes/" + id, _parameters, _getallpages);
        }

        public async Task<List<JObject>> GastronomyCeremonyRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/ceremonies", _parameters, _getallpages);
        }

        public async Task<List<JObject>> GastronomyCeremonyDetailRequest(string id, IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("gastronomies/ceremonies/" + id, _parameters, _getallpages);
        }

        #endregion

        #region Beacon

        public async Task<List<JObject>> BeaconListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("beacons", _parameters, _getallpages);
        }

        public async Task<List<JObject>> BeaconDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("beacons/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> BeaconDeletedRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("beacons/deleted", _parameters, _getallpages);
        }

        #endregion

        #region Event

        public async Task<List<JObject>> EventListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("events/" + id, _parameters, false);
        }

        public async Task<List<JObject>> EventDeletedRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/deleted", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventCategoryRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/categories", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventCategoryDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("events/categories/" + id, _parameters, false);
        }

        public async Task<List<JObject>> EventTagRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/tags", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventTagDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("events/tags/" + id, _parameters, false);
        }

        public async Task<List<JObject>> EventClassificationRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/classifications", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventClassificationDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("events/classifications/" + id, _parameters, false);
        }

        public async Task<List<JObject>> EventDateRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/dates", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventOrganizerRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/organizers", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventOrganizerDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("events/organizers/" + id, _parameters, false);
        }

        public async Task<List<JObject>> EventPickupListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/pickuplist", _parameters, _getallpages);
        }
        public async Task<List<JObject>> EventPickupListDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("events/pickuplist/" + id, _parameters, false);
        }


        public async Task<List<JObject>> EventAllotmentMapRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/allotmentmaps", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventAllotmentMapDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("events/allotmentmaps/" + id, _parameters, false);
        }

        public async Task<List<JObject>> EventVariantCategoryRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("events/variantcategories", _parameters, _getallpages);
        }

        public async Task<List<JObject>> EventVariantCategoryDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("events/variantcategories/" + id, _parameters, false);
        }

        #endregion

        #region Venue
        public async Task<List<JObject>> VenueListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("venues", _parameters, _getallpages);
        }

        public async Task<List<JObject>> VenueDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("venues/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> VenueDeletedRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("venues/deleted", _parameters, _getallpages);
        }

        public async Task<List<JObject>> VenueCategoryRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("venues/categories", _parameters, _getallpages);
        }

        public async Task<List<JObject>> VenueCategoryDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("venues/categories/" + id, _parameters, false);
        }

        public async Task<List<JObject>> VenueHallFeatureRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("venues/halls/features", _parameters, _getallpages);
        }

        public async Task<List<JObject>> VenueHallFeatureDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("venues/halls/features/" + id, _parameters, false);
        }

        #endregion

        #region Webcam

        public async Task<List<JObject>> WebcamListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("webcams", _parameters, _getallpages);
        }

        public async Task<List<JObject>> WebcamDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("webcams/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> WebcamDeletedRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("webcams/deleted", _parameters, _getallpages);
        }

        #endregion

        #region WeatherSnow

        public async Task<List<JObject>> WeatherSnowListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("weathersnows", _parameters, _getallpages);
        }

        public async Task<List<JObject>> WeatherSnowDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("weathersnows/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> WeatherSnowDeletedRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("weathersnows/deleted", _parameters, _getallpages);
        }

        #endregion

        #region Tag

        public async Task<List<JObject>> TagListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("tags", _parameters, _getallpages);
        }

        public async Task<List<JObject>> TagDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("tags/" + id, _parameters, false);
        }

        public async Task<List<JObject>> TagPropertyListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("tags/properties", _parameters, _getallpages);
        }

        public async Task<List<JObject>> TagPropertyDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("tags/properties/" + id, _parameters, false);
        }

        //not listed in lts api
        public async Task<List<JObject>> TagDeletedRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("tags/deleted", _parameters, _getallpages);
        }

        #endregion

        #region SuedtirolGuestPass

        public async Task<List<JObject>> SuedtirolGuestPassCardTypesRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("suedtirolguestpass/cardtypes", _parameters, _getallpages);
        }

        public async Task<List<JObject>> SuedtirolGuestPassCardTypesDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("suedtirolguestpass/cardtypes/" + id, _parameters, false);
        }

        public async Task<List<JObject>> SuedtirolGuestPassBenefitsRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("suedtirolguestpass/benefits", _parameters, _getallpages);
        }

        public async Task<List<JObject>> SuedtirolGuestPassBenefitsDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("suedtirolguestpass/benefits/" + id, _parameters, false);
        }

        #endregion

        #region Common

        public async Task<List<JObject>> MunicipalitiesListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("municipalities", _parameters, _getallpages);
        }

        public async Task<List<JObject>> MunicipalitiesDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("municipalities/" + id, _parameters, false);
        }

        public async Task<List<JObject>> DistrictsListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("districts", _parameters, _getallpages);
        }

        public async Task<List<JObject>> DistrictsDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("districts/" + id, _parameters, false);
        }

        public async Task<List<JObject>> CountriesListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("countries", _parameters, _getallpages);
        }

        public async Task<List<JObject>> CountriesDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("countries/" + id, _parameters, false);
        }

        public async Task<List<JObject>> AreasListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("areas", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AreasDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("areas/" + id, _parameters, false);
        }

        public async Task<List<JObject>> VideoGenreListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("videogenres", _parameters, _getallpages);
        }

        public async Task<List<JObject>> VideoGenreDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("videogenres/" + id, _parameters, false);
        }

        public async Task<List<JObject>> TaxRateListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("taxrate", _parameters, _getallpages);
        }

        public async Task<List<JObject>> TaxRateDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("taxrate/" + id, _parameters, false);
        }

        public async Task<List<JObject>> AmenityListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("amenities", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AmenityDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("amenities/" + id, _parameters, false);
        }

        public async Task<List<JObject>> TourismOrganizationListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("tourismorganizations", _parameters, _getallpages);
        }

        public async Task<List<JObject>> TourismOrganizationDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("tourismorganizations/" + id, _parameters, false);
        }

        public async Task<List<JObject>> AddressGroupListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("addressgroups", _parameters, _getallpages);
        }

        public async Task<List<JObject>> AddressGroupDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("addressgroups/" + id, _parameters, false);
        }

        public async Task<List<JObject>> PositionCategoryListRequest(IDictionary<string, string>? _parameters, bool _getallpages)
        {
            return await LTSRequestMethod("positions/categories", _parameters, _getallpages);
        }

        public async Task<List<JObject>> PositionCategoryDetailRequest(string id, IDictionary<string, string>? _parameters)
        {
            return await LTSRequestMethod("positions/categories/" + id, _parameters, false);
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
                foreach (var pk in propkeysplitted)
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

                    if (prop.PropertyType == typeof(DateTime?))
                    {
                        valuetoadd = String.Format("{0:yyyy-MM-ddThh:mm:ss.fff}", (DateTime)propvalue);
                    }
                    else if (prop.PropertyType == typeof(bool?))
                    {
                        valuetoadd = (bool)propvalue ? "1" : "0";
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

    public class LTSPostBody<T>
    {
        public T parameters { get; set; }
    }

    public class LTSAvailabilitySearchRequestBody
    {
        public LTSAvailabilitySearchRequestBody()
        {
            marketingGroupRids = new List<string>();
            accommodationRids = new List<string>();
            roomOptions = new List<LTSAvailabilitySearchRequestRoomoption>();
        }

        public ICollection<string> marketingGroupRids { get; set; }
        public ICollection<string> accommodationRids { get; set; }
        public bool onlySuedtirolInfoActive { get; set; }
        public int cacheLifeTimeInSeconds { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public LTSAvailabilitySearchRequestPaging paging { get; set; }
        public ICollection<LTSAvailabilitySearchRequestRoomoption> roomOptions { get; set; }
    }   

    public class LTSAvailabilitySearchRequestPaging
    {
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }

    public class LTSAvailabilitySearchRequestRoomoption
    {
        public int id { get; set; }
        public int guests { get; set; }
        public ICollection<int> guestAges { get; set; }
    }

}
