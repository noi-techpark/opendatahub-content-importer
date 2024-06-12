// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using AccommodationTransformer;
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

                    services.AddSingleton(workersettings);

                    services.AddSingleton<IReadAccommodationChanged, ReadAccommodationChanged>();
                    services.AddSingleton<IReadAccommodationDetail, ReadAccommodationDetail>();

                    services.AddHostedService<Worker>();
                });
    }
}