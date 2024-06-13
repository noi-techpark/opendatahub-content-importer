// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AccommodationTransformer;
using DataImportHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace AccommodationTransformer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)            
            .ConfigureServices((hostContext, services) =>
                {
                    WorkerSettings workersettings = new WorkerSettings()
                    {
                        RabbitConnectionString = hostContext.Configuration.GetConnectionString("RabbitConnection"),
                        MongoDBConnectionString = hostContext.Configuration.GetConnectionString("MongoDBConnection"),
                        ReadQueue = hostContext.Configuration.GetSection("RabbitMQConfiguration").GetValue<string>("ReadQueue", "")
                    };

                    var ltsidm = hostContext.Configuration.GetSection("LTSApiIDM");
                    Dictionary<string, Dictionary<string, string>> settings = new Dictionary<string, Dictionary<string, string>>()
                    {
                        { "lts" , new Dictionary<string, string>()
                            {
                                { "clientid", ltsidm.GetSection("xltsclientid").Value },
                                { "username", ltsidm.GetSection("username").Value },
                                { "password", ltsidm.GetSection("password").Value },
                            }
                        },
                        {
                            "rabbitmq", new Dictionary<string, string>()
                            {
                                { "connectionstring", hostContext.Configuration.GetConnectionString("RabbitConnection") }
                            }
                        }
                    };

                    DataImport dataimport = new DataImport(settings);

                    var ltsnoi = hostContext.Configuration.GetSection("LTSApiIDM");
                    Dictionary<string, Dictionary<string, string>> settingsopen = new Dictionary<string, Dictionary<string, string>>()
                    {
                        { "lts" , new Dictionary<string, string>()
                            {
                                { "clientid", ltsnoi.GetSection("xltsclientid").Value },
                                { "username", ltsnoi.GetSection("username").Value },
                                { "password", ltsnoi.GetSection("password").Value },
                            }
                        },
                        {
                            "rabbitmq", new Dictionary<string, string>()
                            {
                                { "connectionstring", hostContext.Configuration.GetConnectionString("RabbitConnection") }
                            }
                        }
                    };
                    DataImport dataimportopen = new DataImport(settingsopen, true);

                    IDictionary<string, DataImport> dataimportlist = new Dictionary<string, DataImport>()
                    {
                        { "idm", dataimport },
                        { "open", dataimportopen }
                    };

                    services.AddSingleton(workersettings);
                    services.AddSingleton(dataimportlist);                    

                    services.AddSingleton<IReadAccommodation, ReadAccommodation>();
                    
                    services.AddHostedService<Worker>();
                });
    }
}