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

DataImport dataimport = new DataImport(settings);


//var qs = new LTSQueryStrings() { page_size = 1, filter_language = "de" };

await dataimport.ImportLTSAccommodationSingle("2657B7CBCb85380B253D2fBE28AF100E");

await dataimport.ImportLTSAccommodationSingle("525B5D14566741D3B5910721027B5ED7");

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

