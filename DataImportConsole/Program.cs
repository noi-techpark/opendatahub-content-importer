using System;
using System.Configuration;
using System.Threading.Tasks;
using DataImportHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration.UserSecrets;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace DataImportConsole
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>();
            //.AddEnvironmentVariables();
            IConfiguration config = builder.Build();

            var ltsidm = config.GetSection("LTSApiIDM");
            var rabbitconn = config.GetConnectionString("RabbitConnection");

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
                        { "connectionstring", rabbitconn }
                    }
                }
            };

            DataImport dataimport = new DataImport(settings);


            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();

            // and start it off
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<ImportAmenitiesJob>()
                .WithIdentity("job1", "group1")                                
                .SetJobData(new JobDataMap() { { "dataimporter", dataimport } })
                .Build();                        

            // Trigger the job to run now, and then repeat every 30 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(30)
                    .RepeatForever())
                .Build();

            // Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);

            //// some sleep to show what's happening
            //await Task.Delay(TimeSpan.FromSeconds(60));

   
            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();

            // and last shut down the scheduler when you are ready to close your program
            await scheduler.Shutdown();
        }

        // simple log provider to get something to the console
        private class ConsoleLogProvider : ILogProvider
        {
            public Logger GetLogger(string name)
            {
                return (level, func, exception, parameters) =>
                {
                    if (level >= LogLevel.Info && func != null)
                    {
                        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
                    }
                    return true;
                };
            }

            public IDisposable OpenNestedContext(string message)
            {
                throw new NotImplementedException();
            }

            public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
            {
                throw new NotImplementedException();
            }
        }
    }

    public class ImportAmenitiesJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var importer = (DataImport)context.JobDetail.JobDataMap["dataimporter"];

            if (importer != null)
                await importer.ImportLTSAmenities();

            await Console.Out.WriteLineAsync("Amenities Import Job processed");
        }
    }
}