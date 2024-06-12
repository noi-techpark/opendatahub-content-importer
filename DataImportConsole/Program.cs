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

            //await CreateAccoAmenityJob(scheduler, dataimport);
            //await CreateAccoTypeJob(scheduler, dataimport);
            //await CreateAccoCategoryJob(scheduler, dataimport);

            await CreateAccoChangedJob(scheduler, dataimport);



            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();

            // and last shut down the scheduler when you are ready to close your program
            await scheduler.Shutdown();
        }
     
        
        private static async Task CreateAccoAmenityJob(IScheduler scheduler, DataImport dataimport)
        {
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<ImportAccoAmenitiesJob>()
                .WithIdentity("job_accoamenities", "accommodation")
                .SetJobData(new JobDataMap() { { "dataimporter", dataimport } })
                .Build();

            // Trigger the job to run now, and then repeat every 30 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger_accoamenities", "accommodation")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(30)
                    .RepeatForever())
                .Build();

            // Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        private static async Task CreateAccoCategoryJob(IScheduler scheduler, DataImport dataimport)
        {
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<ImportAccoCategoriesJob>()
                .WithIdentity("job_accocategories", "accommodation")
                .SetJobData(new JobDataMap() { { "dataimporter", dataimport } })
                .Build();

            // Trigger the job to run now, and then repeat every 45 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger_accocategories", "accommodation")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(45)
                    .RepeatForever())
                .Build();

            // Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        private static async Task CreateAccoTypeJob(IScheduler scheduler, DataImport dataimport)
        {
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<ImportAccoTypesJob>()
                .WithIdentity("job_accotypes", "accommodation")
                .SetJobData(new JobDataMap() { { "dataimporter", dataimport } })
                .Build();

            // Trigger the job to run now, and then repeat every 30 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger_accotypes", "accommodation")
                .StartNow()
                .WithCronSchedule("0 0 0/3 1/1 * ? *")          //http://www.cronmaker.com/                       
                .Build();

            // Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
        }

        private static async Task CreateAccoChangedJob(IScheduler scheduler, DataImport dataimport)
        {
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<ImportAccoTypesJob>()
                .WithIdentity("job_accochanged", "accommodation")
                .SetJobData(new JobDataMap() { { "dataimporter", dataimport } })
                .SetJobData(new JobDataMap() { { "datefrom", DateTime.Now.AddDays(-1) } })
                .Build();

            // Trigger the job to run now, and then repeat every 30 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger_accochanged", "accommodation")
                .StartNow()
                 //.WithCronSchedule("0 0 23 1/1 * ? *")          //http://www.cronmaker.com/  Daily at 23:00                     
                 .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(60)
                    .RepeatForever())
                .Build();

            // Tell Quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);
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

    public class ImportAccoAmenitiesJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var importer = (DataImport)context.JobDetail.JobDataMap["dataimporter"];

            if (importer != null)
                await importer.ImportLTSAccoAmenities();

            await Console.Out.WriteLineAsync("Accommodation Amenities Import Job processed");
        }
    }

    public class ImportAccoCategoriesJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var importer = (DataImport)context.JobDetail.JobDataMap["dataimporter"];

            if (importer != null)
                await importer.ImportLTSAccoCategories();

            await Console.Out.WriteLineAsync("Accommodation Categories Import Job processed");
        }
    }

    public class ImportAccoTypesJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var importer = (DataImport)context.JobDetail.JobDataMap["dataimporter"];

            if (importer != null)
                await importer.ImportLTSAccoTypes();

            await Console.Out.WriteLineAsync("Accommodation Types Import Job processed");
        }
    }

    public class ImportAccoChangedJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var importer = (DataImport)context.JobDetail.JobDataMap["dataimporter"];
            var datefrom = (DateTime)context.JobDetail.JobDataMap["datefrom"];

            if (importer != null)
                await importer.ImportLTSAccommodationChanged(datefrom);

            await Console.Out.WriteLineAsync(String.Format("Accommodation Changed {0} Import Job processed", datefrom.ToShortDateString()));
        }
    }

    public class ImportAccoDetailJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var importer = (DataImport)context.JobDetail.JobDataMap["dataimporter"];
            var id = (string)context.JobDetail.JobDataMap["id"];

            if (importer != null)
                await importer.ImportLTSAccommodationSingle(id);

            await Console.Out.WriteLineAsync(String.Format("Accommodation Detail {0} Import Job processed", id));
        }
    }
}