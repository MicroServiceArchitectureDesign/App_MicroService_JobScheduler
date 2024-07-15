namespace App_MicroService_JobScheduler.Utilities;

public static class ConstantVariables
{

    public static class ServiceDiscoveryConfig
    {
        public static Uri ServiceDiscoveryAddress = new("http://localhost:8500");
        public static Uri ServiceAddress = new("http://localhost:5000");
        public static Uri ServiceHealthCheckEndpoint = new("http://localhost:5000/hc");
        public static string ServiceName = "JobService";
        public static string ServiceId = $"{ServiceName}_{01}";
    }
    public static class CronIntervalType
    {
        public const string Second = "Second";
        public const string Minute = "Minute";
        public const string Hour = "Hour";
        public const string Day = "Day";
        public const string Month = "Month";
    }
}
