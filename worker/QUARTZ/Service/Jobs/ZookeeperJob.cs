﻿using Quartz;
using worker.QUARTZ.Model;

namespace worker.QUARTZ.Service.Jobs
{

    /// <summary>
    /// Este é o método principal do zookeeper. Ele é executado periodicamente. Recebe os horários dos trabalhadores.
    /// Ele percorre cada agendamento do trabalhador e recupera os jobs anexados ao agendamento. Em seguida, ele coleta
    /// o trigger de cada job e atualiza o tempo de trigger.
    /// </summary>
    public class ZookeeperJob : IJob
    {
        public static string JobName = "ZookeeperJob";
        public static string JobGroup = "ZookeeperJobGroup";
        public static string TriggerName = "ZookeeperTrigger";
        public static string TriggerGroup = "ZookeeperTriggerGroup";

        int RandomId;

        IScheduler _workerScheduler = null;
        List<WorkerJob> _workers = new List<WorkerJob>();

        public ZookeeperJob() : base()
        {
            RandomId = new Random().Next(1, 1000);
        }

        public void AddWorkers(IScheduler workerScheduler, List<WorkerJob> workers)
        {
            _workerScheduler = workerScheduler;
            _workers = workers;
        }
        ITrigger getNewTrigger(string name, string group, string schedule)
        {
            return TriggerBuilder.Create()
                .WithIdentity(name, group)
                .WithCronSchedule(schedule)
                .Build();
        }
        async Task rescheduleJob()
        {
            //Console.WriteLine("Rescheduling worker jobs");
            if (_workerScheduler != null)
            {
                foreach (var worker in _workers)
                {
                    var job = await _workerScheduler.GetJobDetail(worker.GetJobKey);
                    //Console.WriteLine($"--Rescheduling worker {job.Key}");
                    var trigger = await _workerScheduler.GetTrigger(worker.GetTriggerKey);
                    if (trigger != null)
                    {
                        var cronTrigger = (ICronTrigger)trigger;
                        var newExpression = AppSettings.GetValue(worker.GetAppSettingsTriggerKey);

                        if (cronTrigger.CronExpressionString != newExpression)
                        {
                            Console.WriteLine($"--Rescheduling job for trigger {trigger.Key.Name}");
                            var newTrigger = getNewTrigger(trigger.Key.Name, trigger.Key.Group, newExpression);
                            await _workerScheduler.RescheduleJob(trigger.Key, newTrigger);
                        }
                    }
                }

            }
            else Console.WriteLine("Could not reschedule worker as the scheduler is null");
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                //Console.WriteLine($"Running Zookeeper Job {RandomId} at " + DateTime.Now.ToString());
                var triggerTime = AppSettings.GetValue(AppSettings.ZookeeperTriggerTime);
                //Console.WriteLine($"Current Zookeeper trigger time: {triggerTime}");


                var task = Task.Run(async () => await rescheduleJob());
                task.Wait();

                if (_workerScheduler != null && !_workerScheduler.IsStarted)
                {
                    Console.WriteLine("Starting worker scheduler as it has not been started yet");
                    _workerScheduler.Start();
                }

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine("Received an error when executing Zookeeper job. " + e.Message);
                return Task.FromException(e);
            }
        }
    }
}
