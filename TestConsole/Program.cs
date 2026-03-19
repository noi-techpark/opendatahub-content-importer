// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
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


List<string> testcases = new List<string>() { "accommodation" };



#region Availability Searc

if (testcases.Contains("availabilitysearch"))
{
    //await TestRequests.TestAvailabilitySearch(settings);


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
}

#endregion

#region Accommodation

if (testcases.Contains("accommodation"))
{
    var idlistaccos = new List<string>()
    {
    "E339DC2D60181EF76C669C680FE884AA",
    "6E173C0B03CD40F989A7910AD44CDE5A",
    "824D862F29E711D599D20050DA7125DB",
    "1365AB5F5D10DCA9A002F639CFE12EDF",
    "1189396BC0CD11D2AE71004095429799",
    "0D1F980EA581484ACCA31464B2949EDE",
    "5BBD387AEBA66BBEE1727E2C5FC38F1B",
    "824D87BB29E711D599D20050DA7125DB",
    "8C8D77B78EB14EF784A9E84AB4F70EA4",
    "824D891A29E711D599D20050DA7125DB",
    "3C02457E429C11D4B99700E07D035C68",
    "2E7736574142C7CF5BC6F1868F9ABB2C",
    "C0DF91D276D911D18F2900A02427D15E",
    "1C9AB8D4D6E87DA10D00062FB029969A",
    "FBB1B14867B211D4AB0F0050DA6D1102",
    "2F2F3E85DD3B11D2A24C00C026A25F22",
    "24A619A258719AD90DF112B51D935CA8",
    "11893B54C0CD11D2AE71004095429799",
    "6B4DA7C9726A11D2AE6600104BB288F4",
    "4E259C701E7740D4A8B840CBFF4CF775",
    "C75182C9544F11D2968200A0244EAF51",
    "B5E2E2A83CBF39A2C7E90AC896F2963D",
    "A2E20A16C4B211D19C5D006097AF193B",
    "6355DE27CB1F24F7F50B9C23C150FC14",
    "BED907015B23CE42917B5DA9038CE9D4",
    "8E257B5EFA9E1F925F5478732B250650",
    "AD7B68641337FC6ECD0488F09A0617E9",
    "728483CDB869F52A229C94E4F65EB44E",
    "388CED290E29CBC98A8CF951AC9DE205",
    "8748462293E311D1924700805A150B0B",
    "C0DF8D7E76D911D18F2900A02427D15E",
    "0512E2FAB7C94C6D8241A25BE93731C4",
    "0C5726B7DDE475601AACB85DF5A266F5",
    "118938F0C0CD11D2AE71004095429799",
    "617DDC7FCD175E354431FDC2ABA6D39B",
    "F7A27EFF59A3E084229C605D8BAF86CB",
    "A7D574F3766311D3A84700104BE4ECF9",
    "3F86097358AA7B9B40B9EAF111ED10A5",
    "A9E90E0C779111D1957700805A150B0B",
    "31976E88934511D2879000608C0B00B7",
    "23BEC72A7A36264B15E572683C7C86E5",
    "B8613FE2341C99162BAA370870FFCFBA",
    "8BE3606304245FA61E5DA57822B92630"
    };

    foreach (var accoid in idlistaccos)
    {
        await TestRequests.RetrieveAndParseAccommodation(settings, new List<string>() {
        accoid
        }, settings.LtsCredentials);
    }
    

    
    //await TestRequests.RetrieveAndParseAccommodation(settings, new List<string>() {
    //    "9BB0774FE7EAADE46D2DC44188E7A94C"        
    //}, settings.LtsCredentialsOpen);


    //TEST accommodation Parsing

    //RabbitMQSend rabbitsend = new RabbitMQSend(config.GetConnectionString("RabbitConnection"));

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

    //var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };

    //LtsApi ltsapi = new LtsApi(settings.LtsCredentials);
    //var qs = new LTSQueryStrings()
    //{
    //    page_size = 1,
    //    fields = "cinCode,amenities,suedtirolGuestPass,roomGroups",
    //};
    //var dict = ltsapi.GetLTSQSDictionary(qs);
    //var ltsacco = await ltsapi.AccommodationDetailRequest("06F7A0918A0F11D2B477006097AD12DB", dict);


}

#endregion

#region Event

if (testcases.Contains("event"))
{
    //await TestRequests.RetrieveAndParseEvent(settings);

    //var ltsevent = await ltsapi.EventDetailRequest("FA440216CBFD4DAD99389D584FC83B81", null);
    //var parsedevent = EventParser.ParseLTSEventV1(ltsevent.FirstOrDefault().Value<JObject>(), false);


    //var ltsevent = await ltsapi.EventDetailRequest("FA440216CBFD4DAD99389D584FC83B81", null);
    //var parsedevent = EventParser.ParseLTSEventV1(ltsevent.FirstOrDefault().Value<JObject>(), false);

}

#endregion

#region Poi

if (testcases.Contains("poi"))
{
    await TestRequests.RetrieveAndParsePoi(settings, new List<string>() { "3931c131da2923919105e403361a4cd0",
        "3361695701cccc5de1effcba2487245c",
        "a152e6bb003c43ae896cc0d146e3ff61",
        "3b52338426f97883a80b9de864af24f9",
        "deb6e4f3bdcc3855eb2bf1f5a98a7f8e",
        "5a22f7103afd3511e3ee39c200aec6d3",
        "c32182c4c64a23181cb6b127047ecfda",
        "94d61c67ecf788f21b6793e4262e04d7",
        "663a538db4088691ea0abbf04239db4d"
    },
    settings.LtsCredentialsOpen);

    await TestRequests.RetrieveAndParsePoi(settings, new List<string>() {
        "4DB4B03B746FB952B4525C691E04A125"
    },
    settings.LtsCredentialsOpen);

    await TestRequests.RetrieveAndParsePoi(settings, new List<string>() {
        "4DB4B03B746FB952B4525C691E04A125"
    },
    settings.LtsCredentials);

    //var ltspoi = await ltsapi.PoiDetailRequest("3741EF2230FC909CA46A925D3BBA3B45", null);
    //var parsedpoi = PointofInterestParser.ParseLTSPointofInterest(ltspoi.FirstOrDefault().Value<JObject>(), false);


}

#endregion

#region Activity

if (testcases.Contains("activity"))
{
    await TestRequests.RetrieveAndParseActivity(settings, new List<string>() {
        "0F5505E54E1B304216ED620EEEAE07FD",
        "B6A3E2F228D4FCCF33F39649E489D231"
    },
    settings.LtsCredentials);

    await TestRequests.RetrieveAndParseActivity(settings, new List<string>() {
        "78E3FD5425AED454DEFE139567DED23C"
    },
    settings.LtsCredentialsOpen);

    //var ltsactivity = await ltsapi.PoiDetailRequest("B9F7D5CE855542C03F95B1CCE8169A12", null);
    //var parsedactivity = PointofInterestParser.ParseLTSPointofInterest(ltspoi.FirstOrDefault().Value<JObject>(), false);

}

#endregion

#region Venue

if (testcases.Contains("venue"))
{
    await TestRequests.RetrieveAndParseVenue(settings, new List<string>() {
    "1A2A4C7533FE47BC90F33325AA707292"
},
    settings.LtsCredentials);

    await TestRequests.RetrieveAndParseVenue(settings, new List<string>() {
    "1A2A4C7533FE47BC90F33325AA707292"
},
    settings.LtsCredentialsOpen);

    await TestRequests.RetrieveAndParseVenue(settings, new List<string>() {
    "0079E30758054687AB9F972888BB5BA5"
},
    settings.LtsCredentialsOpen);

    await TestRequests.RetrieveAndParseVenue(settings, new List<string>() {
    "0079E30758054687AB9F972888BB5BA5"
},
    settings.LtsCredentialsOpen);
}

#endregion

#region Gastronomy

if (testcases.Contains("gastronomy"))
{
    await TestRequests.RetrieveAndParseGastronomy(settings, new List<string>() {
    "86DB9DE6547A11D3BBA90000E870A1E4"
},
settings.LtsCredentialsOpen);

    //await TestRequests.RetrieveAndParseGastronomy(settings);
}

#endregion

#region Measuringpoint

if (testcases.Contains("measuringpoint"))
{
    await TestRequests.RetrieveAndParseMeasuringpoint(settings, new List<string>() {
    "04AF7C73242FE23115FB6A120F5079D2"
},
settings.LtsCredentials);
    await TestRequests.RetrieveAndParseMeasuringpoint(settings, new List<string>() {
    "04AF7C73242FE23115FB6A120F5079D2"
},
    settings.LtsCredentialsOpen);

}

#endregion

#region Webcam

if (testcases.Contains("webcam"))
{

    await TestRequests.RetrieveAndParseWebcam(settings, new List<string>() {
    "02FC162B43AAB00A2C54FBC8A4F6EF03"
    },
    settings.LtsCredentialsOpen);

}

#endregion

Console.ReadLine();