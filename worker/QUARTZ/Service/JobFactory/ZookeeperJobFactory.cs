﻿using Quartz;
using Quartz.Spi;
using worker.QUARTZ.Model;
using worker.QUARTZ.Service.Jobs;

namespace worker.QUARTZ.Service.JobFactory
{

    public class ZookeeperJobFactory : IJobFactory
    {
        public static readonly List<WorkerJob> Workers = new List<WorkerJob>()
        {
            new GetPrefixoJob(),
            new GetMigrationCel()
        };
        readonly IServiceProvider _serviceProvider;
        readonly IScheduler _workerScheduler;

        public ZookeeperJobFactory(IServiceProvider serviceProvider, IScheduler workerScheduler)
        {
            Console.WriteLine("Creating new Zookeeper Job Factory");
            _serviceProvider = serviceProvider;
            _workerScheduler = workerScheduler;

        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            //Console.WriteLine($"Creating new job for {bundle.JobDetail.Key.Name}");
            IJob job = null;
            if (bundle.JobDetail.Key.Name == ZookeeperJob.JobName)
            {
                var zookeeperJob = _serviceProvider.GetService<ZookeeperJob>();
                zookeeperJob.AddWorkers(_workerScheduler, Workers);
                job = zookeeperJob;
            }
            else
            {
                foreach (var worker in Workers)
                {
                    if (bundle.JobDetail.Key.Name == worker.GetJobName)
                    {
                        job = worker.GetJob(_serviceProvider);
                    }
                }
            }
            return job;
        }

        public void ReturnJob(IJob job)
        {
            //Console.WriteLine("Returning job for zookeeper");
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}

