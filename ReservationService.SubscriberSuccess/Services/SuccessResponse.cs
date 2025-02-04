using Pulsar.Client.Api;
using Pulsar.Client.Common;
using System.Text;
using System.Text.Json;

namespace ReservationService.SubscriberSuccess.Services
{
    public class SuccessResponse
    {
        private const string TopicName = "pulsar_success_response";

        public static async Task SendSuccessMessage(PulsarClient client, Message<byte[]> message)
        {
            var responseProducer = await client.NewProducer().Topic(TopicName).CreateAsync();

            var responseMessage = new
            {
                Timestamp = DateTime.UtcNow,
                RawResponse = Encoding.UTF8.GetString(message.Data)
            };

            await responseProducer.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseMessage)));

            Console.WriteLine("Sent confirmation message to pulsar_success_response.");
        }
    }
}
