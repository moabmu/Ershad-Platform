using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzWrapper
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            //// Will not work with jobs depend on scoped services.
            //IJob job = _serviceProvider.GetService(bundle.JobDetail.JobType) as IJob;
            //return job ?? throw new InvalidCastException($"The type {bundle.JobDetail.JobType.ToString()} does not implement IJob interface. Cannot Create a job of the latter type.");

            return _serviceProvider.GetRequiredService<JobRunner>();
        }

        public void ReturnJob(IJob job){}
    }
}
