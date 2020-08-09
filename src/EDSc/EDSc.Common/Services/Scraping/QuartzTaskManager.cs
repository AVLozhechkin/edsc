namespace EDSc.Common.Services.Scraping
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Quartz;
    using Quartz.Impl;
    
    public class QuartzTaskManager<T> where T : IJob
    {
        private readonly IDictionary<string, object> dataForJob;
        private readonly string cronInterval;
        private IScheduler scheduler;
        public QuartzTaskManager(IDictionary<string, object> dataForJob, string cronInterval)
        {
            this.dataForJob = dataForJob;
            this.cronInterval = cronInterval;
        }
        public async Task Start()
        {
            try
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                this.scheduler = await schedFact.GetScheduler();

                await scheduler.Start();

                IJobDetail jobDetail = JobBuilder
                    .Create<ImageScrapingJob>()
                    .WithIdentity("imageDownloadJob", "group1")
                    .Build();

                foreach (var item in dataForJob)
                {
                    jobDetail.JobDataMap[item.Key] = item.Value;
                }

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("cronTrigger", "group1")
                    .WithCronSchedule(this.cronInterval)
                    .ForJob("imageDownloadJob", "group1")
                    .Build();

                await scheduler.ScheduleJob(jobDetail, trigger);

                if (await scheduler.GetCurrentlyExecutingJobs() == null)
                {
                    await this.ShutDownQuartzSchedulerAsync();
                }
            }
            catch
            {
                throw;
            }
        }
        private async Task ShutDownQuartzSchedulerAsync()
        {
            if (this.scheduler == null)
            {
                throw new NullReferenceException("The scheduler doesn't exist");
            }

            await scheduler.Shutdown();
        }
    }
}
