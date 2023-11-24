using Quartz;
using worker.QUARTZ.Model;

namespace worker.QUARTZ.Service.Jobs
{
    class GetMigrationCel : WorkerJob
    {
        public static string JobName = "GetMigrationCelJob";
        public static string JobGroup = "GetMigrationCelJobGroup";
        public static string TriggerName = "GetMigrationCelTrigger";
        public static string TriggerGroup = "GetMigrationCelTriggerGroup";

        public GetMigrationCel() : base(
            JobName, JobGroup, TriggerName, TriggerGroup,
            AppSettings.GetMigrationCelTriggerTime, typeof(GetMigrationCel)
            )
        {

        }

        public override IJob GetJob(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<GetMigrationCel>();
        }
        public override Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine("Running BackOrder Notification Job at " + DateTime.Now.ToString());
                var triggerTime = AppSettings.GetValue(AppSettings.GetMigrationCelTriggerTime);
                //Console.WriteLine($"Current Invoice Notification trigger time: {triggerTime}");
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine("Received an error when executing BackOrder Notification job. " + e.Message);
                return Task.FromException(e);
            }
        }
    }
}
