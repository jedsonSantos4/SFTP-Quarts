namespace worker.QUARTZ.Model
{
    public static class AppSettings
    {
        public static string ZookeeperTriggerTime = "QuartzSchedule:ZookeeperTriggerTime";
        public static string GetPrefixoTriggerTime = "QuartzSchedule:GetPrefixoTriggerTime";
        public static string GetMigrationCelTriggerTime = "QuartzSchedule:GetMigrationCelTriggerTime";

        public static string GetValue(string key)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            return configuration.GetSection(key).Value;
        }
    }
}