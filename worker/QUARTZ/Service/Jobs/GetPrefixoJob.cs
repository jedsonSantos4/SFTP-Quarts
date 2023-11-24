using Polly.CircuitBreaker;
using Quartz;
using SFTP.Service.Implementation;
using SFTP.Service.Interface;
using worker.QUARTZ.Model;

namespace worker.QUARTZ.Service.Jobs
{
    /// <summary>
    /// This class simulates generating invoice notification. This is a job which is managd by the zookeeper
    /// </summary>
    public class GetPrefixoJob : WorkerJob
    {
        public static string JobName = "GetPrefixoJob";
        public static string JobGroup = "GetPrefixoJobGroup";
        public static string TriggerName = "GetPrefixoTrigger";
        public static string TriggerGroup = "GetPrefixoTriggerGroup";

      
        private readonly ISftpService _SftpService;

        public GetPrefixoJob() : base(
            JobName, JobGroup, TriggerName, TriggerGroup,
            AppSettings.GetPrefixoTriggerTime, typeof(GetPrefixoJob)
            )
        {

        }
        public GetPrefixoJob(ISftpService sftpService) : base(
            JobName, JobGroup, TriggerName, TriggerGroup,
            AppSettings.GetPrefixoTriggerTime, typeof(GetPrefixoJob)
            )
        {
            _SftpService = sftpService;
        }

        public override IJob GetJob(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<GetPrefixoJob>();
        }
        public override Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine("Running Get Prefixo Job at " + DateTime.Now.ToString());
                var triggerTime = AppSettings.GetValue(AppSettings.GetPrefixoTriggerTime);
                
                var files = _SftpService.ListAllFiles();
                if (files.Count() > 0)
                {
                    foreach (var file  in files)
                    {
                        Console.WriteLine("Download a file from the SFTP directory:");
                        string pngFile =file.FullName;
                        File.Delete(pngFile);
                        _SftpService.DownloadFile(@"/pub/example/imap-console-client.png", pngFile);
                        if (File.Exists(pngFile))
                        {
                            Console.WriteLine($"\t file {pngFile} downloaded");
                        }
                    }
                    // download a file
                   


                    Console.WriteLine();
                }
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine("Received an error when executing Get Prefixon job. " + e.Message);
                return Task.FromException(e);
            }
        }
    }
}
