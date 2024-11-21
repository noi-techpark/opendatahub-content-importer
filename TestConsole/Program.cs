using LTSAPI;
using Microsoft.Extensions.Configuration;
using RabbitPusher;
using DataImportHelper;
using GenericHelper;

Console.WriteLine("Test!");
var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
//.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddUserSecrets<Program>();
//.AddEnvironmentVariables();
IConfiguration config = builder.Build();

Settings settings = new Settings(config);

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

LtsApi ltsapi = new LtsApi(settings.LtsCredentials);

LTSAvailabilitySeachBody body = new LTSAvailabilitySeachBody() {
    accommodationRids = new List<string>() { "B2D5F382CE2611D1BAAA00805A13E75D" },
    startDate = "2024-12-14",
    endDate = "2024-12-15",
    paging = new LTSPaging() { pageNumber = 1, pageSize = 10000 },
    cacheLifeTimeInSeconds = 300,    
    onlySuedtirolInfoActive = true,
    roomOptions = new List<LTSAvailabilitySearchRoomoption>() { new LTSAvailabilitySearchRoomoption() { id = 1, guests = 2, guestAges = new List<int>() { 18, 18 } } }
};


var ltsavailablilitysearch = await ltsapi.AccommodationAvailabilitySearchRequest(null, body);

var testlts = await ltsapi.AccommodationDetailRequest("2657B7CBCb85380B253D2fBE28AF100E", null);

int i = 0;