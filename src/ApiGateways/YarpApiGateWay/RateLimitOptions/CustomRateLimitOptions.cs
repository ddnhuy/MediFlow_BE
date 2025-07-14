namespace YarpApiGateWay.RateLimitOptions
{
    public class CustomRateLimitOptions
    {
        public const string MyRateLimit = "RateLimiting";

        public int PermitLimit { get; set; } = 100;
        public int Window { get; set; } = 60;
        public int SegmentsPerWindow { get; set; } = 4;
        public int QueueLimit { get; set; } = 0;
    }
}
