// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataImportHelper;
using DataModel;
using GenericHelper;
using LTSAPI;
using LTSAPI.Parser;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestConsole
{
    public class TestRequests
    {
        public static async Task TestAccommodationSingle(Settings settings)
        {
            LtsApi ltsapi = new LtsApi(settings.LtsCredentials);

            DataImport dataimport = new DataImport(settings);

            var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };

            await dataimport.ImportLTSAccommodationSingle("1DF9C17D6F24E87E4EDAAB3408588A6C");
            await dataimport.ImportLTSAccommodationSingle("525B5D14566741D3B5910721027B5ED7");

            var ltsacco = await ltsapi.AccommodationDetailRequest("525B5D14566741D3B5910721027B5ED7", null);
        }

        public static async Task TestAvailabilitySearch(Settings settings)
        {
            Stopwatch watch = Stopwatch.StartNew();

            LtsApi ltsapi = new LtsApi(settings.LtsCredentials);

            //https://tourism.api.opendatahub.bz.it/v1/Accommodation?pagenumber=1&pagesize=10000&arrival=2025-02-03&departure=2025-02-10&roominfo=0-18%2C18&language=de&availabilitychecklanguage=de&availabilitycheck=true&removenullvalues=false&bookablefilter=true&fields=Id%2CMssResponseShort&idfilter=FF050E6BE1F245FAD5E61495E17522D2&bokfilter=lts

            LTSAvailabilitySearchRequestBody body = new LTSAvailabilitySearchRequestBody()
            {
                //accommodationRids = new List<string>() { "FF050E6BE1F245FAD5E61495E17522D2" },
                //marketingGroupRids = new List<string>() { "" },
                startDate = "2025-11-07",
                endDate = "2025-11-10",
                paging = new LTSAvailabilitySearchRequestPaging() { pageNumber = 1, pageSize = 10000 },
                cacheLifeTimeInSeconds = 300,
                onlySuedtirolInfoActive = true,
                roomOptions = new List<LTSAvailabilitySearchRequestRoomoption>() { new LTSAvailabilitySearchRequestRoomoption() { id = 1, guests = 2, guestAges = new List<int>() { 18, 18 } } }
            };


            var ltsavailablilitysearch = await ltsapi.AccommodationAvailabilitySearchRequest(null, body);

            var parsedavailabilitysearch = ltsavailablilitysearch[0].ToObject<LTSAvailabilitySearchResult>();

            var mssresult = AccommodationSearchResultParser.ParseLTSAccommodation(ltsavailablilitysearch.FirstOrDefault(), 1);

            //var testlts = await ltsapi.AccommodationDetailRequest("2657B7CBCb85380B253D2fBE28AF100E", null);

            Console.WriteLine(parsedavailabilitysearch.success);

            watch.Stop();

            Console.WriteLine("elapsed time: " + watch.ElapsedMilliseconds);

            Console.ReadLine();
        }

        public static async Task RetrieveAndParseEvent(Settings settings)
        {
            LtsApi ltsapi = new LtsApi(settings.LtsCredentials);
            var ltsevent = await ltsapi.EventDetailRequest("5F53C72C7A5045BEBAE4099E3E60C033", null);
            var parsedevent = EventParser.ParseLTSEventV1(ltsevent.FirstOrDefault().Value<JObject>(), false);
        }

        public static async Task RetrieveAndParseAccommodation(Settings settings)
        {
            LtsApi ltsapi = new LtsApi(settings.LtsCredentials);
            var ltsacco = await ltsapi.AccommodationDetailRequest("06F7A0918A0F11D2B477006097AD12DB", null);

            //Load all XDocuments
            var xmlfiles = LoadXmlFiles(Path.Combine(".\\xml\\"));
            var jsonfiles = await LoadJsonFiles("Features", Path.Combine(".\\json\\"));

            var parsedacco = AccommodationParser.ParseLTSAccommodation(ltsacco.FirstOrDefault().Value<JObject>(), false, xmlfiles, jsonfiles);
        }

        public static async Task RetrieveAndParseGastronomy(Settings settings)
        {
            LtsApi ltsapi = new LtsApi(settings.LtsCredentials);
            var ltsgastro = await ltsapi.GastronomyDetailRequest("724FDBF6CFC811D1BA6B444553540000", null);
            var parsedgastro = GastronomyParser.ParseLTSGastronomy(ltsgastro.FirstOrDefault().Value<JObject>(), false, null);

            //LtsApi ltsapi2 = new LtsApi(settings.LtsCredentials);
            //var ltsgastro2 = await ltsapi2.GastronomyDetailRequest("9896159359D3CA450BBF0676D879CD9D", null);
            //var parsedgastro2 = GastronomyParser.ParseLTSGastronomy(ltsgastro2.FirstOrDefault().Value<JObject>(), false, null);
        }

        public static async Task RetrieveAndParseActivity(Settings settings, List<string> idlist, LTSCredentials ltscreds)
        {
            foreach (var id in idlist)
            {
                LtsApi ltsapi = new LtsApi(ltscreds);
                var ltsactivity = await ltsapi.ActivityDetailRequest(id.ToUpper(), null);
                var parsedactivity = ActivityParser.ParseLTSActivity(ltsactivity.FirstOrDefault().Value<JObject>(), false);

                // Create settings with alphabetical property ordering
                var serializersettings = new JsonSerializerSettings
                {
                    ContractResolver = new AlphabeticalContractResolver(),
                    Formatting = Formatting.Indented // Optional: for pretty printing
                };

                Console.WriteLine(JsonConvert.SerializeObject(parsedactivity, serializersettings));
            }
        }

        public static async Task RetrieveAndParsePoi(Settings settings, List<string> idlist, LTSCredentials ltscreds)
        {
            foreach(var id in idlist)
            {
                LtsApi ltsapi = new LtsApi(ltscreds);
                var ltspoi = await ltsapi.PoiDetailRequest(id.ToUpper(), null);
                var parsedpoi = PointofInterestParser.ParseLTSPointofInterest(ltspoi.FirstOrDefault().Value<JObject>(), false);

                // Create settings with alphabetical property ordering
                var serializersettings = new JsonSerializerSettings
                {
                    ContractResolver = new AlphabeticalContractResolver(),
                    Formatting = Formatting.Indented // Optional: for pretty printing
                };

                Console.WriteLine(JsonConvert.SerializeObject(parsedpoi, serializersettings));
            }            
        }

        public static async Task RetrieveAndParseMeasuringpoint(Settings settings, List<string> idlist, LTSCredentials ltscreds)
        {
            foreach (var id in idlist)
            {
                LtsApi ltsapi = new LtsApi(ltscreds);
                var measuringpoint = await ltsapi.WeatherSnowDetailRequest(id.ToUpper(), null);
                var parsed = MeasuringpointParser.ParseLTSMeasuringpoint(measuringpoint.FirstOrDefault().Value<JObject>(), false);

                // Create settings with alphabetical property ordering
                var serializersettings = new JsonSerializerSettings
                {
                    ContractResolver = new AlphabeticalContractResolver(),
                    Formatting = Formatting.Indented // Optional: for pretty printing
                };

                Console.WriteLine(JsonConvert.SerializeObject(parsed, serializersettings));
            }
        }

        public static async Task RetrieveAndParseVenue(Settings settings, List<string> idlist, LTSCredentials ltscreds)
        {
            foreach (var id in idlist)
            {
                LtsApi ltsapi = new LtsApi(ltscreds);
                var venue = await ltsapi.VenueDetailRequest(id.ToUpper(), null);
                var parsed = VenueParser.ParseLTSVenue(venue.FirstOrDefault().Value<JObject>(), false);

                // Create settings with alphabetical property ordering
                var serializersettings = new JsonSerializerSettings
                {
                    ContractResolver = new AlphabeticalContractResolver(),
                    Formatting = Formatting.Indented // Optional: for pretty printing
                };

                Console.WriteLine(JsonConvert.SerializeObject(parsed, serializersettings));
            }
        }


        public static async Task RetrieveAndParseWebcam(Settings settings, List<string> idlist, LTSCredentials ltscreds)
        {
            foreach (var id in idlist)
            {
                LtsApi ltsapi = new LtsApi(ltscreds);
                var webcam = await ltsapi.WebcamDetailRequest(id.ToUpper(), null);
                var parsed = WebcamInfoParser.ParseLTSWebcam(webcam.FirstOrDefault().Value<JObject>(), false);

                // Create settings with alphabetical property ordering
                var serializersettings = new JsonSerializerSettings
                {
                    ContractResolver = new AlphabeticalContractResolver(),
                    Formatting = Formatting.Indented // Optional: for pretty printing
                };

                Console.WriteLine(JsonConvert.SerializeObject(parsed, serializersettings));
            }
        }


        #region Accommodation Helper

        //TODO add this to helper class

        public static IDictionary<string, XDocument> LoadXmlFiles(string directory)
        {
            //TODO move this files to Database

            IDictionary<string, XDocument> myxmlfiles = new Dictionary<string, XDocument>();
            myxmlfiles.Add("AccoCategories", XDocument.Load(directory + "AccoCategories.xml"));
            myxmlfiles.Add("AccoTypes", XDocument.Load(directory + "AccoTypes.xml"));
            myxmlfiles.Add("Alpine", XDocument.Load(directory + "Alpine.xml"));
            myxmlfiles.Add("Boards", XDocument.Load(directory + "Boards.xml"));
            myxmlfiles.Add("City", XDocument.Load(directory + "City.xml"));
            myxmlfiles.Add("Dolomites", XDocument.Load(directory + "Dolomites.xml"));
            myxmlfiles.Add("Features", XDocument.Load(directory + "Features.xml"));
            myxmlfiles.Add("Mediterranean", XDocument.Load(directory + "Mediterranean.xml"));
            myxmlfiles.Add("NearSkiArea", XDocument.Load(directory + "NearSkiArea.xml"));
            myxmlfiles.Add("RoomAmenities", XDocument.Load(directory + "RoomAmenities.xml"));
            myxmlfiles.Add("Vinum", XDocument.Load(directory + "Vinum.xml"));
            myxmlfiles.Add("Wine", XDocument.Load(directory + "Wine.xml"));

            return myxmlfiles;
        }

        public static IDictionary<string, XDocument> LoadXmlFiles(string directory, string filename)
        {
            //TODO move this files to Database

            IDictionary<string, XDocument> myxmlfiles = new Dictionary<string, XDocument>();
            myxmlfiles.Add(filename, XDocument.Load(directory + filename + ".xml"));

            return myxmlfiles;
        }

        public static async Task<IDictionary<string, JArray>> LoadJsonFiles(string directory, string filename)
        {
            IDictionary<string, JArray> myjsonfiles = new Dictionary<string, JArray>();
            myjsonfiles.Add(filename, await LoadFromJsonAndDeSerialize(filename, directory));

            return myjsonfiles;
        }

        public static async Task<JArray> LoadFromJsonAndDeSerialize(string filename, string path)
        {
            using (StreamReader r = new StreamReader(Path.Combine(path, filename + ".json")))
            {
                string json = await r.ReadToEndAsync();

                return JArray.Parse(json) ?? new JArray();
            }
        }

        #endregion
    }

    public class AlphabeticalContractResolver : DefaultContractResolver
    {
        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, memberSerialization)
                .OrderBy(p => p.PropertyName)
                .ToList();
        }
    }
}
