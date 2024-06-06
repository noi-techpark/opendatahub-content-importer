// See https://aka.ms/new-console-template for more information
using Amazon.Runtime.Internal.Transform;
using LTSAPI;
using Microsoft.Extensions.Configuration;

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
    password = ltsidm.GetSection("password").Value }, 
    null);

var myobject = await ltsapi.AccommodationAmenitiesRequest();

Console.WriteLine(myobject["success"]);

Console.ReadLine();