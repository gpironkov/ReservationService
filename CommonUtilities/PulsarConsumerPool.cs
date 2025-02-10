using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Pulsar.Client.Api;
using Pulsar.Client.Common;

namespace CommonUtilities
{
    public class PulsarConsumerPool : IAsyncDisposable
    {
        private bool _disposed = false;
        private readonly PulsarClient _client;
        private readonly ConcurrentDictionary<string, IConsumer<byte[]>> _consumers = new();
        private readonly SemaphoreSlim _lock = new(1, 1);

        public PulsarConsumerPool(string pulsarServiceUrl)
        {
            _client = Task.Run(async () => await new PulsarClientBuilder()
                .ServiceUrl(pulsarServiceUrl)
                .BuildAsync()).GetAwaiter().GetResult();
        }

        public async Task<IConsumer<byte[]>> GetOrCreateConsumerAsync(string topic, string subscription)
        {
            string key = $"{topic}-{subscription}";

            if (_consumers.TryGetValue(key, out var existingConsumer))
            {
                //Console.WriteLine($"Existing consumer found for key: {key}");
                return existingConsumer;
            }

            await _lock.WaitAsync();
            try
            {
                if (!_consumers.TryGetValue(key, out existingConsumer))
                {
                    var newConsumer = await _client.NewConsumer()
                        .Topic(topic)
                        .SubscriptionName(subscription)
                        .SubscriptionType(SubscriptionType.Shared)
                        .SubscribeAsync();

                    _consumers[key] = newConsumer;
                    //Console.WriteLine($"New consumer created for key: {key}");

                    return newConsumer;
                }

                //Console.WriteLine($"Existing consumer found for key after lock: {key}");
                return existingConsumer;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            foreach (var consumer in _consumers.Values)
            {
                await consumer.DisposeAsync();
            }
            _consumers.Clear();
            await _client.CloseAsync();
        }
    }
}
