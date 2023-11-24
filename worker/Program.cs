using worker.Resilience;
using worker;
using SFTP.Service.Implementation;
using SFTP.Service.Interface;
using SFTP.Service.Model;
using worker.QUARTZ.Infrastructure;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton(CircuitBreakerExtensions.CreatePolicy(
            Convert.ToInt32(context.Configuration["CircuitBreaker:NumberOfExceptionsBeforeBreaking"]),
            Convert.ToInt32(context.Configuration["CircuitBreaker:DurationOfBreakInSeconds"])));
        services.AddSingleton(new SftpConfig(
            context.Configuration["SftpConfig:Host"],
            Convert.ToInt32(context.Configuration["SftpConfig:Port"]),
            context.Configuration["SftpConfig:UserName"],
            context.Configuration["SftpConfig:password"]));

        services.AddSingleton<ISftpService, SftpService>();

        services.AddOptions();
        ServiceRegistration.AddServices(services);
        _ = SchedulerConfiguration.Configure(services);

        //services.AddHostedService<Worker>();
    })
    .Build();



host.Run();

