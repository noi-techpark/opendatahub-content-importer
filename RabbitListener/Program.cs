// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitListener;

namespace RabbitListener
{
    public class Program
    {
        public static void Main(string[] args)
        {           
             CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            //.ConfigureAppConfiguration((hostContext, builder) =>
            //{
            //    // Other providers
            //    if (hostContext.HostingEnvironment.IsDevelopment())
            //    {
            //        builder.AddUserSecrets<Program>();
            //    }
            //})
            .ConfigureServices((hostContext, services) =>
                {                                          
                    WorkerSettings workersettings = new WorkerSettings()
                    {
                        RabbitConnectionString = hostContext.Configuration.GetConnectionString("RabbitConnection"),
                        MongoDBConnectionString = hostContext.Configuration.GetConnectionString("MongoDBConnection"),
                        ReadQueue = hostContext.Configuration.GetSection("RabbitMQConfiguration").GetValue<string>("ReadQueue", "")
                    };
                    
                    services.AddSingleton(workersettings);

                    services.AddSingleton<IReadMessage, ReadMessage>();                    
                    services.AddHostedService<Worker>();
                });
    }
}