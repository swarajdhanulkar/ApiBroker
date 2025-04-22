namespace ApiBroker
{
    public class ProviderSelector
    {
        private readonly List<ProviderMetrics> _providers;
        private readonly int _maxRequestsPerMinute;

        public ProviderSelector(List<ProviderMetrics> providers, int maxPerMin)
        {
            _providers = providers;
            _maxRequestsPerMinute = maxPerMin;
        }

        public ProviderMetrics? GetBestProvider()
        {

            Console.WriteLine("------ Provider Metrics ------");

            foreach (var provider in _providers)
            {
                int requestCount = provider.GetRequestCountLastMinute();
                int errorCount = provider.GetErrorCountLast5Min();
                double avgResponseTime = provider.GetAverageResponseTimeLast5Min();

                Console.WriteLine($"Provider: {provider.ProviderName}");
                Console.WriteLine($"  Requests in last minute: {requestCount}");
                Console.WriteLine($"  Errors in last 5 mins:   {errorCount}");
                Console.WriteLine($"  Avg Response Time (ms):  {avgResponseTime:F2}");
                Console.WriteLine();
            }

            Console.WriteLine("------ Selecting Best Provider ------"); 
            
            return _providers
                .Where(p => p.GetRequestCountLastMinute() < _maxRequestsPerMinute)
                .OrderBy(p => p.GetErrorCountLast5Min())
                .ThenBy(p => p.GetAverageResponseTimeLast5Min())
                .FirstOrDefault();
        }
    }

}
