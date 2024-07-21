using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuartzWrapper
{
    public class QuartzHostedService : IHostedService
    {
        private readonly IJobFactory _jobFactory;
        private readonly IScheduler _scheduler;
        private CancellationToken _cancellationToken;
        public QuartzHostedService(IScheduler scheduler, IJobFactory jobFactory)
        {
            _scheduler = scheduler;
            _jobFactory = jobFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler.JobFactory = _jobFactory;
            _cancellationToken = cancellationToken;
            return _scheduler.Start(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _scheduler.Shutdown(cancellationToken);
        }

        /// <summary>
        /// Schedule a recurrent job.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cronExpression"></param>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public Task ScheduleJob<T>(string cronExpression, string jobKey = null) where T : class, IJob
        {
            return _scheduler.ScheduleJob(
                jobDetail: this.CreateJobDetail<T>(jobKey),
                trigger: this.CreateBasicTriggerBuilder<T>(jobKey).WithCronSchedule(cronExpression).Build(),
                cancellationToken: _cancellationToken);
        }

        /// <summary>
        /// Schedule an infrequent job.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startDateTime"></param>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public async Task<TriggerKey> ScheduleJob<T>(DateTimeOffset startDateTime, string jobKey = null) where T : class, IJob
        {
            var trigger = this.CreateBasicTriggerBuilder<T>(jobKey).StartAt(startDateTime).Build();
            await this.ScheduleJob<T>(jobDetail: this.CreateJobDetail<T>(jobKey), trigger: trigger);
            return trigger.Key;
        }

        /// <summary>
        /// Schedule an immediate job.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobKey"></param>
        /// <returns></returns>
        public async Task<TriggerKey> ScheduleJob<T>(string jobKey = null) where T : class, IJob
        {
            var trigger = this.CreateBasicTriggerBuilder<T>(jobKey).StartNow().Build();
            await this.ScheduleJob<T>(jobDetail: this.CreateJobDetail<T>(jobKey), trigger: trigger);
            return trigger.Key;
        }

        public Task ScheduleJob<T>(IJobDetail jobDetail, ITrigger trigger) where T: class, IJob
        {
            return _scheduler.ScheduleJob(jobDetail, trigger, _cancellationToken);
        }

        public Task<bool> UnscheduleJob(string name, string group)
        {
            return this.UnscheduleJob(new TriggerKey(name, group));
        }

        public Task<bool> UnscheduleJob(TriggerKey triggerKey)
        {
            return _scheduler.UnscheduleJob(triggerKey, _cancellationToken);
        }

        public async Task<int> GetCountOfTriggers()
        {
            int result = 0;
            foreach(string groupName in await _scheduler.GetJobGroupNames(_cancellationToken))
            {
                foreach(JobKey jobKey in await _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName), _cancellationToken))
                {
                    result += (await _scheduler.GetTriggersOfJob(jobKey)).Count;
                }
            }
            return result;
        }

        #region Helpers
        private IJobDetail CreateJobDetail<T>(string jobKey = null) where T : class, IJob
        {
            if (string.IsNullOrEmpty(jobKey))
            {
                jobKey = Guid.NewGuid().ToString();
            }
            return JobBuilder.CreateForAsync<T>()
                .WithIdentity($"{typeof(T).Name}_{jobKey}")
    .WithDescription(typeof(T).FullName)
    .Build();
        }

        private TriggerBuilder CreateBasicTriggerBuilder<T>(string triggerId = null) where T: class, IJob
        {
            if (string.IsNullOrEmpty(triggerId))
            {
                triggerId = $"{typeof(T).Name}_TRIGGER_{Guid.NewGuid().ToString()}";
            }

            return TriggerBuilder
                .Create()
                .WithIdentity(triggerId)
                .WithDescription(triggerId);
        }
        #endregion
    }
}
