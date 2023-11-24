using worker.QUARTZ.Service.Jobs;

namespace worker.QUARTZ.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<ZookeeperJob>();
            services.AddSingleton<GetPrefixoJob>();
            services.AddSingleton<GetMigrationCel>();
        }
    }
}
