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
using System.Collections;
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

        public static async Task RetrieveAndParseEvent(Settings settings, List<string> idlist, LTSCredentials ltscreds)
        {
            foreach (var id in idlist)
            {
                LtsApi ltsapi = new LtsApi(ltscreds);
                var ltsevent = await ltsapi.EventDetailRequest("5F53C72C7A5045BEBAE4099E3E60C033", null);
                var parsedevent = EventParser.ParseLTSEventV1(ltsevent.FirstOrDefault().Value<JObject>(), false);
                // Create settings with alphabetical property ordering
                var serializersettings = new JsonSerializerSettings
                {
                    ContractResolver = new AlphabeticalContractResolver(),
                    Formatting = Formatting.Indented // Optional: for pretty printing
                };

                Console.WriteLine(JsonConvert.SerializeObject(parsedevent, serializersettings));
            }
        }

        public static async Task RetrieveAndParseAccommodation(Settings settings, List<string> idlist, LTSCredentials ltscreds)
        {
            foreach (var id in idlist)
            {
                LtsApi ltsapi = new LtsApi(ltscreds);
                var ltsacco = await ltsapi.AccommodationDetailRequest(id, null);

                // Get the bin/debug directory and navigate up to project root
                var projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName;                

                //Load all XDocuments
                var xmlfiles = LoadXmlFiles(Path.Combine(projectRoot, "xml\\"));
                var jsonfiles = await LoadJsonFiles(Path.Combine(projectRoot, "json\\"), "Features");

                var parsedacco = AccommodationParser.ParseLTSAccommodation(ltsacco.FirstOrDefault().Value<JObject>(), false, xmlfiles, jsonfiles);

                // Create settings with alphabetical property ordering
                var serializersettings = new JsonSerializerSettings
                {
                    ContractResolver = new AlphabeticalContractResolver(),
                    Formatting = Formatting.Indented // Optional: for pretty printing
                };

                Console.WriteLine(JsonConvert.SerializeObject(parsedacco, serializersettings));

                var parsedaccorooms = AccommodationParser.ParseLTSAccommodationRoom(ltsacco.FirstOrDefault().Value<JObject>(), false, xmlfiles, jsonfiles);

                // Create settings with alphabetical property ordering
                var serializersettings = new JsonSerializerSettings
                {
                    ContractResolver = new AlphabeticalContractResolver(),
                    Formatting = Formatting.Indented // Optional: for pretty printing
                };

                Console.WriteLine(JsonConvert.SerializeObject(parsedacco, serializersettings));
            }
        }

        public static async Task RetrieveAndParseGastronomy(Settings settings, List<string> idlist, LTSCredentials ltscreds)
        {
            foreach (var id in idlist)
            {
                LtsApi ltsapi = new LtsApi(ltscreds);
                var ltsgastro = await ltsapi.GastronomyDetailRequest(id.ToUpper(), null);
                var parsedgastro = GastronomyParser.ParseLTSGastronomy(ltsgastro.FirstOrDefault().Value<JObject>(), false, null);

                // Create settings with alphabetical property ordering
                var serializersettings = new JsonSerializerSettings
                {
                    ContractResolver = new AlphabeticalContractResolver(),
                    Formatting = Formatting.Indented // Optional: for pretty printing
                };

                Console.WriteLine(JsonConvert.SerializeObject(parsedgastro, serializersettings));
            }
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
