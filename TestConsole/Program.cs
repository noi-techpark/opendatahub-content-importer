﻿// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using LTSAPI;
using Microsoft.Extensions.Configuration;
using RabbitPusher;
using DataImportHelper;
using GenericHelper;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using LTSAPI.Parser;
using System.Text.Json.Nodes;
using TestConsole;
using Microsoft.AspNetCore.Http.Timeouts;

Console.WriteLine("Test!");
var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
//.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddUserSecrets<Program>();
//.AddEnvironmentVariables();
IConfiguration config = builder.Build();

Settings settings = new Settings(config);

//var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };

LtsApi ltsapi = new LtsApi(settings.LtsCredentials);
//var qs = new LTSQueryStrings()
//{
//    page_size = 1,
//    fields = "cinCode,amenities,suedtirolGuestPass,roomGroups",
//};
//var dict = ltsapi.GetLTSQSDictionary(qs);
//var ltsacco = await ltsapi.AccommodationDetailRequest("06F7A0918A0F11D2B477006097AD12DB", dict);

//var ltsevent = await ltsapi.EventDetailRequest("FA440216CBFD4DAD99389D584FC83B81", null);
//var parsedevent = EventParser.ParseLTSEventV1(ltsevent.FirstOrDefault().Value<JObject>(), false);

//await TestRequests.TestAvailabilitySearch(settings);


//var ltsevent = await ltsapi.EventDetailRequest("FA440216CBFD4DAD99389D584FC83B81", null);
//var parsedevent = EventParser.ParseLTSEventV1(ltsevent.FirstOrDefault().Value<JObject>(), false);

//var ltspoi = await ltsapi.PoiDetailRequest("3741EF2230FC909CA46A925D3BBA3B45", null);
//var parsedpoi = PointofInterestParser.ParseLTSPointofInterest(ltspoi.FirstOrDefault().Value<JObject>(), false);


//var ltsactivity = await ltsapi.PoiDetailRequest("B9F7D5CE855542C03F95B1CCE8169A12", null);
//var parsedactivity = PointofInterestParser.ParseLTSPointofInterest(ltspoi.FirstOrDefault().Value<JObject>(), false);

//DataImport dataimport = new DataImport(settings);


//var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };

//await dataimport.ImportLTSAccommodationSingle("1DF9C17D6F24E87E4EDAAB3408588A6C");

//await dataimport.ImportLTSAccommodationSingle("525B5D14566741D3B5910721027B5ED7");

//RabbitMQSend rabbitsend = new RabbitMQSend(config.GetConnectionString("RabbitConnection"));

//var ltsamenities = await ltsapi.AccommodationAmenitiesRequest(null, true);
//rabbitsend.Send("lts/accommodationamenities", ltsamenities);

//var ltscategories = await ltsapi.AccommodationCategoriesRequest(null, true);
//rabbitsend.Send("lts/accommodationcategories", ltscategories);

//var ltstypes = await ltsapi.AccommodationTypesRequest(null, true);
//rabbitsend.Send("lts/accommodationtypes", ltstypes);

//var ltsacco = await ltsapi.AccommodationDetailRequest("525B5D14566741D3B5910721027B5ED7", null);
//rabbitsend.Send("lts/accommodationdetail", ltsacco);

//var ltsacco2 = await ltsapi.AccommodationDetailRequest("2657B7CBCb85380B253D2fBE28AF100E", null);
//rabbitsend.Send("lts/accommodationdetail", ltsacco2);

//var qs1 = new LTSQueryStrings()
//{
//    page_size = 100,
//    filter_lastUpdate = DateTime.Now.AddHours(-1),
//    filter_marketingGroupRids = "9E72B78AC5B14A9DB6BED6C2592483BF",
//    fields = "rid"
//};
//var dict1 = ltsapi.GetLTSQSDictionary(qs1);
//var ltsaccochanged = await ltsapi.AccommodationListRequest(dict1, true);
//rabbitsend.Send("lts/accommodationchanged", ltsaccochanged);


//TEST accommodation Parsing

//var ltsacco2 = await ltsapi.AccommodationDetailRequest("2657B7CBCb85380B253D2fBE28AF100E", null);
//rabbitsend.Send("lts/accommodationdetail", ltsacco2);

//Console.WriteLine("key to start");

//Console.ReadLine();

//Stopwatch watch = Stopwatch.StartNew();

//LtsApi ltsapi = new LtsApi(settings.LtsCredentials);

//https://tourism.api.opendatahub.bz.it/v1/Accommodation?pagenumber=1&pagesize=10000&arrival=2025-02-03&departure=2025-02-10&roominfo=0-18%2C18&language=de&availabilitychecklanguage=de&availabilitycheck=true&removenullvalues=false&bookablefilter=true&fields=Id%2CMssResponseShort&idfilter=FF050E6BE1F245FAD5E61495E17522D2&bokfilter=lts

//LTSAvailabilitySearchRequestBody body = new LTSAvailabilitySearchRequestBody() {
//    accommodationRids = new List<string>() { "FF050E6BE1F245FAD5E61495E17522D2" },
//    //marketingGroupRids = new List<string>() { "" },
//    startDate = "2025-02-03",
//    endDate = "2025-02-10",
//    paging = new LTSAvailabilitySearchRequestPaging() { pageNumber = 1, pageSize = 10000 },
//    cacheLifeTimeInSeconds = 300,    
//    onlySuedtirolInfoActive = true,
//    roomOptions = new List<LTSAvailabilitySearchRequestRoomoption>() { new LTSAvailabilitySearchRequestRoomoption() { id = 1, guests = 2, guestAges = new List<int>() { 18, 18 } } }
//};


//var ltsavailablilitysearch = await ltsapi.AccommodationAvailabilitySearchRequest(null, body);

//var parsedavailabilitysearch = ltsavailablilitysearch[0].ToObject<LTSAvailabilitySearchResult>();

////var testlts = await ltsapi.AccommodationDetailRequest("2657B7CBCb85380B253D2fBE28AF100E", null);

//Console.WriteLine(parsedavailabilitysearch.success);

//watch.Stop();

//Console.WriteLine("elapsed time: " + watch.ElapsedMilliseconds);

//Console.ReadLine();

await TestRequests.RetrieveAndParseEvent(settings);

await TestRequests.RetrieveAndParseGastronomy(settings);

Console.ReadLine();