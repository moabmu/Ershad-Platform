using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzWrapper.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzWrapper.Extensions
{
    public static class ExtensionsForIServiceCollection
    {
        public static IServiceCollection AddQuartzWrapper(this IServiceCollection services)
        {
            // Add Quartz Job Factory (Will get the job from IoC instead of creating a 'new job()')
            services.AddSingleton<IJobFactory, JobFactory>();

            // Add Scheduler
            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();
            services.AddSingleton(scheduler);

            // Add Quartz Hosted Service
            services.AddSingleton<QuartzHostedService>();
            services.AddHostedService<HostedServiceStarter<QuartzHostedService>>();

            // Add Quartz Job Runner (to activate all jobs including the the scoped ones)
            services.AddSingleton<JobRunner>();

            return services;
        }
    }
}
