using System.Diagnostics;

namespace ApiBroker
{
    public class LocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ProviderSelector _providerSelector;

        public LocationService(HttpClient httpClient, ProviderSelector selector)
        {
            _httpClient = httpClient;
            _providerSelector = selector;
        }

        public async Task<string> GetLocationFromIPAsync(string ip)
        {
            var provider = _providerSelector.GetBestProvider();
            if (provider == null)
                throw new Exception("No providers available right now");

            provider.LogRequest();
            var url = $"{provider.BaseUrl}/{ip}";

            try
            {
                var stopwatch = Stopwatch.StartNew();
                var response = await _httpClient.GetAsync(url);
                stopwatch.Stop();
                provider.LogResponseTime(stopwatch.ElapsedMilliseconds);

                if (!response.IsSuccessStatusCode)
                {
                    provider.LogError();
                    throw new Exception("Provider failed");
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                provider.LogError();
                throw;
            }
        }
    }
}
