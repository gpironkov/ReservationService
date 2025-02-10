using Pulsar.Client.Api;
using Pulsar.Client.Common;
using System.Text;
using System.Text.Json;

namespace ReservationService.SubscriberSuccess.Services
{
    public class ResponseMessage
    {
        private const string TopicSuccess = "pulsar_success_response";
        private const string TopicFailed = "pulsar_failed_response";

        public static async Task SendSuccessMessage(PulsarClient client, Message<byte[]> message)
        {
            var responseProducer = await client.NewProducer().Topic(TopicSuccess).CreateAsync();

            var responseMessage = new
            {
                RawResponse = Encoding.UTF8.GetString(message.Data)
            };

            await responseProducer.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseMessage)));

            Console.WriteLine($"Sent confirmation message to {TopicSuccess}.");
        }

        public static async Task SendFailedMessage(PulsarClient client, Message<byte[]> message, string errorMessage)
        {
            var responseProducer = await client.NewProducer().Topic(TopicFailed).CreateAsync();

            var responseMessage = new
            {
                RawResponse = Encoding.UTF8.GetString(message.Data),
                ErrorMessage = errorMessage
            };

            await responseProducer.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(responseMessage)));

            Console.WriteLine($"Sent confirmation message to {TopicFailed}.");
        }
    }
}
