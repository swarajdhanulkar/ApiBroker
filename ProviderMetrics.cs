namespace ApiBroker
{
    public class ProviderMetrics
    {
        private readonly object _lock = new();
        private readonly Queue<DateTime> _requestTimestamps = new();
        private readonly Queue<(DateTime, long)> _responseTimes = new();
        private readonly Queue<DateTime> _errorTimestamps = new();

        public string ProviderName { get; set; }
        public string BaseUrl { get; set; }

        public void LogRequest()
        {
            lock (_lock)
            {
                _requestTimestamps.Enqueue(DateTime.UtcNow);
            }
        }

        public void LogResponseTime(long ms)
        {
            lock (_lock)
            {
                _responseTimes.Enqueue((DateTime.UtcNow, ms));
            }
        }

        public void LogError()
        {
            lock (_lock)
            {
                _errorTimestamps.Enqueue(DateTime.UtcNow);
            }
        }

        public int GetRequestCountLastMinute()
        {
            lock (_lock)
            {
                Cleanup(_requestTimestamps, TimeSpan.FromMinutes(1));
                return _requestTimestamps.Count;
            }
        }

        public double GetAverageResponseTimeLast5Min()
        {
            lock (_lock)
            {
                Cleanup(_responseTimes, TimeSpan.FromMinutes(5));
                return _responseTimes.Count == 0 ? double.MaxValue : _responseTimes.Average(r => r.Item2);
            }
        }

        public int GetErrorCountLast5Min()
        {
            lock (_lock)
            {
                Cleanup(_errorTimestamps, TimeSpan.FromMinutes(5));
                return _errorTimestamps.Count;
            }
        }

        private void Cleanup<T>(Queue<T> queue, TimeSpan timeWindow) where T : struct
        {
            var now = DateTime.UtcNow;
            while (queue.Count > 0 && (now - GetTime(queue.Peek())) > timeWindow)
                queue.Dequeue();
        }

        private DateTime GetTime<T>(T item)
        {
            if (item is DateTime dt)
                return dt;

            if (item is ValueTuple<DateTime, long> tuple)
                return tuple.Item1;

            throw new InvalidOperationException("Unsupported type in queue");
        }
    }
}
