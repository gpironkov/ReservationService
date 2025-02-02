using Microsoft.Extensions.Configuration;
using Pulsar.Client.Api;
using Pulsar.Client.Common;
using ReservationService.SubscriberFailed.Data;
using System.Text;

namespace ReservationService.SubscriberFailed.Services
{
    public class Subscriber
    {
        private readonly MongoDbService _mongoDbService;
        private readonly string _pulsarServiceUrl;
        private const string TopicName = "pulsar_failed";

        public Subscriber(IConfiguration configuration, MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
            _pulsarServiceUrl = configuration["Pulsar:ServiceUrl"];
        }

        public async Task StartAsync()
        {
            var client = await new PulsarClientBuilder().ServiceUrl(_pulsarServiceUrl).BuildAsync();

            var consumer = await client.NewConsumer()
                .Topic(TopicName)
                .SubscriptionName("failed-subscription")
                .SubscriptionType(SubscriptionType.Exclusive)
                .SubscribeAsync();

            Console.WriteLine("Subscriber failed service started. Listening for failed reservations...");

            while (true)
            {
                try
                {
                    var message = await consumer.ReceiveAsync();
                    var rawRequest = Encoding.UTF8.GetString(message.Data);

                    Console.WriteLine($"Received failed reservation: {rawRequest}");

                    var failedReservation = new FailedReservation
                    {
                        RawRequest = rawRequest,
                        DT = DateTime.UtcNow,
                        ValidationResult = 0
                    };

                    await _mongoDbService.SaveFailedReservationAsync(failedReservation);

                    await consumer.AcknowledgeAsync(message.MessageId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            }
        }
    }
}
