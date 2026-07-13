// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataImportHelper;
using GenericHelper;
using LTSAPI;
using LTSAPI.Parser;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using RabbitPusher;
using System.Diagnostics;
using System.Text.Json.Nodes;
using TestConsole;

Console.WriteLine("Test!");
var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
//.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddUserSecrets<Program>();
//.AddEnvironmentVariables();
IConfiguration config = builder.Build();

Settings settings = new Settings(config);


List<string> testcases = new List<string>() { "poi" };



#region Availability Searc

if (testcases.Contains("availabilitysearch"))
{
    await TestRequests.TestAvailabilitySearchSingle(settings);

    await TestRequests.TestAvailabilitySearch(settings);
}

#endregion

#region Accommodation

if (testcases.Contains("accommodation"))
{
    //LtsApi ltsapi = new LtsApi(settings.LtsCredentials);
    //var qs = new LTSQueryStrings() { fields = "rid", filter_marketingGroupRids = "9E72B78AC5B14A9DB6BED6C2592483BF" };
    //qs.filter_lastUpdate = DateTime.Now.AddMinutes(-15);

    ////Acco List Requests LastChanged TEST
    //var dict = ltsapi.GetLTSQSDictionary(qs);
    //var resultchanged = await ltsapi.AccommodationListRequest(dict, true);
    ////Acco Deleted Requests TEST
    //qs.filter_lastUpdate = DateTime.Now.AddDays(-15);
    //var resultdeleted = await ltsapi.AccommodationDeleteRequest(dict, true);


    //Acco Single Requests TEST
    var idlistaccos = new List<string>()
    {
        //Insert all ids here to test
        "2AAFF5576F167C4F3AED3498D8DABBF4"
    };

    int i = 0;
    foreach (var accoid in idlistaccos)
    {
        await TestRequests.RetrieveAndParseAccommodation(settings, new List<string>() {
        accoid
        }, settings.LtsCredentials);
        Thread.Sleep(500);
        i++;
        Console.WriteLine(i + " : REQUEST");
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
    await TestRequests.RetrieveAndParseEvent(settings, new List<string>() {
    "FA440216CBFD4DAD99389D584FC83B81"
},
    settings.LtsCredentials);

    await TestRequests.RetrieveAndParseEvent(settings, new List<string>() {
    "FA440216CBFD4DAD99389D584FC83B81"
},
    settings.LtsCredentials);
}

#endregion

#region Poi

if (testcases.Contains("poi"))
{
    await TestRequests.RetrieveAndParsePoi(settings, new List<string>() { "E6784AC192B04E2B83BC5684B51986F0"
    },
    settings.LtsCredentials);

    //await TestRequests.RetrieveAndParsePoi(settings, new List<string>() {
    //    "4DB4B03B746FB952B4525C691E04A125"
    //},
    //settings.LtsCredentialsOpen);

    //await TestRequests.RetrieveAndParsePoi(settings, new List<string>() {
    //    "4DB4B03B746FB952B4525C691E04A125"
    //},
    //settings.LtsCredentials);

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

if (testcases.Contains("measuringpointlist"))
{
    LtsApi ltsapi = new LtsApi(settings.LtsCredentials);

    ICollection<string> areas = [
            "65A480C2B81D441CBBC6BD05120A4DDE",
            "CA2E54E09FC64DB4AB59CA4CEB82E1C9",
            "E7D4023A77774CB69410BF265BE4E603",
            "01788B57E7D2488DB39E3500137F3C08",
            "D1D07C6316764A0E9401A7DC208C1242",
            "432A27B52669427CB341920E8D14828A",
            "BE8882107B724B8B98ACC591618BBCA4",
            "9408E5F8327642DF9B40A68A70B74815",
            "797B66B31C1B49EB871EEBDB0BE15A1F",
            "3AAF79FA886C43E7B9FD1A15F1E8F8FA",
            "ADC9CC8971AE4D1092F503C12AF90E51",
            "8EF63837BF6E4F6B8C968C5E58A2495E",
            "B19FAB3519888857FAE87472F0C0955F",
            "80091720F545F09E6E874538210A4534"
        ];

    var qs = new LTSQueryStrings() { page_size = 1, filter_onlyActive = true, filter_areaRids = string.Join(",", areas) };
    var dict = ltsapi.GetLTSQSDictionary(qs);

    var ltsdata = await ltsapi.WeatherSnowListRequest(dict, true);
    List<LTSWeatherSnowsList> weathersnowdata = new List<LTSWeatherSnowsList>();

    foreach (var ltsdatasingle in ltsdata)
    {
        weathersnowdata.Add(
            ltsdatasingle.ToObject<LTSWeatherSnowsList>()
        );
    }

    foreach (var data in weathersnowdata)
    {
        foreach(var data2 in data.data)
        {
            var measuringpointparsed = MeasuringpointParser.ParseLTSMeasuringpoint(data2, false);
        }        
    }

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

#region SnowReport

if(testcases.Contains("snowreport"))
{

    LtsApi ltsapi = new LtsApi(settings.LtsCredentials);

    //Construct the post body
    LTSActivitySearchRequestBody body = new LTSActivitySearchRequestBody();
    body.areaRids = [
            "65A480C2B81D441CBBC6BD05120A4DDE",
            "CA2E54E09FC64DB4AB59CA4CEB82E1C9",
            "E7D4023A77774CB69410BF265BE4E603",
            "01788B57E7D2488DB39E3500137F3C08",
            "D1D07C6316764A0E9401A7DC208C1242",
            "432A27B52669427CB341920E8D14828A",
            "BE8882107B724B8B98ACC591618BBCA4",
            "9408E5F8327642DF9B40A68A70B74815",
            "797B66B31C1B49EB871EEBDB0BE15A1F",
            "3AAF79FA886C43E7B9FD1A15F1E8F8FA",
            "ADC9CC8971AE4D1092F503C12AF90E51",
            "8EF63837BF6E4F6B8C968C5E58A2495E",
            "B19FAB3519888857FAE87472F0C0955F",
            "80091720F545F09E6E874538210A4534"
        ];

    body.onlyActive = true;
    body.paging = new LTSAvailabilitySearchRequestPaging() { pageNumber = 1, pageSize = 25 };

    LTSActivitySearchBodyFilterAndSummaryGroups filterandsummarygroups = new LTSActivitySearchBodyFilterAndSummaryGroups();

    filterandsummarygroups.id = 0;
    filterandsummarygroups.type = "tag";
    filterandsummarygroups.filters.Add(new LTSActivitySearchBodyFilters() { id = 0, isSelected = false, rids = [
                                "EB5D6F10C0CB4797A2A04818088CD6AB", // Slopes
                                "1D273A84DBCA4709B68D295C89A003E4", // Circuit
                                "7CA6D68BF134495F865FDD47B94320C0", // Snowpark
                                "6285F49DBBE04393BAD29E6EF219EB03" // Other slopes
                            ] });

    filterandsummarygroups.filters.Add(new LTSActivitySearchBodyFilters()
    {
        id = 1,
        isSelected = false,
        rids = [
                                "D544A6312F8A47CF80CC4DFF8833FE50", // Cross-country ski-track
                                "379E895958FD4693B04F5734A9CFAFAB", // Classic
                                "E2B3F9B5B2F747A1968ECD033BED5D2B", // Skating
                                "835EF4A6853F414DA9782607F01EEE48" // Classic and skating
                            ]
    });

    filterandsummarygroups.filters.Add(new LTSActivitySearchBodyFilters()
    {
        id = 2,
        isSelected = false,
        rids = [
                                "E23AA37B2AE3477F96D1C0782195AFDF", // Lifts
                                "9CBAC00246A8467E93DD66F3A1A9C594" // Other Lifts
                            ]
    });



    body.resultSet = new LTSActivitySearchBodyResultSet()
    {
        filterAndSummaryGroups = new List<LTSActivitySearchBodyFilterAndSummaryGroups>() { filterandsummarygroups }
    };

    var snowreportsearch = await ltsapi.ActivitySearchRequest(null, new List<string>() { "rid" }, body);

    var parsedsnowreportsearch = snowreportsearch[0].ToObject<LTSActivitySearchResult>();

}

#endregion


Console.ReadLine();