using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace WebApplication2.Services
{
    public class SheduleService
    {
        private string cron = @"* 0/1 * * * ? *";

        public async Task GetUpToDateExchangeRates()
        {
            try
            {
                cron = ConfigurationManager.AppSettings["cron"];
                RunRatesUpdater(cron).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed start updater." + ex.Message + ex.StackTrace);
            }
        }
        
        private static async Task RunRatesUpdater(string cron)
        {
            try
            {
                StdSchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = await factory.GetScheduler();

                IJobDetail job = JobBuilder.Create<AddCurrentRateJob>()
                    .WithIdentity("RatesJob")
                    .Build();


                Console.WriteLine("Starting scheduling...");

                var trigger = TriggerBuilder.Create()
                    .ForJob(job)
                    .WithCronSchedule(cron)
                    .WithIdentity("Trigger")
                    .StartNow()
                    .Build();


                await scheduler.ScheduleJob(job, trigger);
                await scheduler.Start();

                Console.WriteLine("Press any key to stop...");
                Console.ReadKey();

                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }
    }

    public class AddCurrentRateJob : IJob
    {
        ParseExchangeRatesService _parseExchangeRatesService;

        public async Task Execute(IJobExecutionContext context)
        {
            _parseExchangeRatesService.ParseCurrentExchangeRates();
            Console.WriteLine("Done!");
        }
    }
}
