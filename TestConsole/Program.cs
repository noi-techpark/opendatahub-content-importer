// See https://aka.ms/new-console-template for more information
using Amazon.Runtime.Internal.Transform;
using LTSAPI;
using Microsoft.Extensions.Configuration;
using RabbitPusher;

Console.WriteLine("Test!");
var builder = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddUserSecrets<Program>();
//.AddEnvironmentVariables();
IConfiguration config = builder.Build();

var ltsidm = config.GetSection("LTSApiIDM");

//LtsApi ltsapi = new LtsApi(new LTSCredentials() { ltsclientid = "", username = "", password = "" }, new Dictionary<string, string>() { "", "" });
LtsApi ltsapi = new LtsApi(new LTSCredentials() { 
    ltsclientid = ltsidm.GetSection("xltsclientid").Value, 
    username = ltsidm.GetSection("username").Value, 
    password = ltsidm.GetSection("password").Value }
    );

//RabbitMQSend rabbitsend = new RabbitMQSend(config.GetConnectionString("RabbitConnection"));

var ltsamenities = await ltsapi.AccommodationAmenitiesRequest(null, true);
//rabbitsend.Send("lts/accommodationamenities", ltsamenities);

var ltscategories = await ltsapi.AccommodationCategoriesRequest(null, true);
//rabbitsend.Send("lts/accommodationcategories", ltscategories);

var ltstypes = await ltsapi.AccommodationTypesRequest(null, true);
//rabbitsend.Send("lts/accommodationtypes", ltstypes);

Console.ReadLine();