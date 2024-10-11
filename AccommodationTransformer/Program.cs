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
using GenericHelper;

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
                    Settings settings = new Settings(hostContext.Configuration);

                    WorkerSettings workersettings = new WorkerSettings()
                    {
                        RabbitConnectionString = settings.RabbitConnection,
                        MongoDBConnectionString = settings.MongoDBConnection,
                        ReadQueue = settings.RabbitMQConfiguration.ReadQueue
                    };                    

                    var writetoapisettings = hostContext.Configuration.GetSection("ODHApiCore");
                    var apiwriter = new ODHApiConnector(
                        settings.ODHApiCoreConfiguration.AuthServer,
                        settings.ODHApiCoreConfiguration.ClientId,
                        settings.ODHApiCoreConfiguration.ClientSecret,
                        settings.ODHApiCoreConfiguration.ApiEndpoint);


                    DataImport dataimport = new DataImport(settings.LtsCredentials, settings.HgvCredentials, settings.RabbitConnection);                    
                    DataImport dataimportopen = new DataImport(settings.LtsCredentialsOpen, settings.HgvCredentials, settings.RabbitConnection);

                    IDictionary<string, DataImport> dataimportlist = new Dictionary<string, DataImport>()
                    {
                        { "idm", dataimport },
                        { "open", dataimportopen }
                    };

                    services.AddSingleton(workersettings);
                    services.AddSingleton(dataimportlist);
                    services.AddSingleton(apiwriter);

                    services.AddSingleton<IReadAccommodation, ReadAccommodation>();
                    
                    services.AddHostedService<Worker>();
                });
    }
}