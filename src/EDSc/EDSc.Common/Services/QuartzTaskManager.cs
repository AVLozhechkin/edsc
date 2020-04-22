using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace EDSc.Common.Services
{
    public class QuartzTaskManager<T> where T : IJob
    {
        private readonly IDictionary<string, object> dataForJob;
        private readonly TimeSpan interval;
        private IScheduler scheduler;
        public QuartzTaskManager(IDictionary<string, object> dataForJob, TimeSpan interval)
        {
            this.dataForJob = dataForJob;
            this.interval = interval;
        }
        public async Task Start()
        {
            try
            {
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                this.scheduler = await schedFact.GetScheduler();

                await scheduler.Start();

                IJobDetail jobDetail = JobBuilder
                    .Create<ImgScrapingJob>()
                    .WithIdentity("imgDLJob", "group1")
                    .Build();

                foreach (var item in dataForJob)
                {
                    jobDetail.JobDataMap[item.Key] = item.Value;
                }

                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1")
                    .ForJob("imgDLJob", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithInterval(this.interval)
                        .RepeatForever())
                    .Build();

                await scheduler.ScheduleJob(jobDetail, trigger);

                if (scheduler.GetCurrentlyExecutingJobs().Result == null)
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
                throw new NullReferenceException("The sheduler doesn't exist");
            }

            await scheduler.Shutdown();
        }
    }
}
