using Microsoft.Extensions.Configuration;
using Pulsar.Client.Api;
using Pulsar.Client.Common;
using ReservationService.Publisher.DTOs;
using ReservationService.Publisher.Models;
using System.Text;
using System.Text.Json;

namespace ReservationService.Publisher.Services
{
    public class ResponseReceiver
    {
        private const string TopicName = "pulsar_success_response";
        private const string SubscriptionName = "response-subscription";
        private readonly string _pulsarServiceUrl;
        private readonly ReservationDbContext _dbContext;

        public ResponseReceiver(IConfiguration configuration, ReservationDbContext dbContext)
        {
            _pulsarServiceUrl = configuration["Pulsar:ServiceUrl"] ?? throw new ArgumentNullException(nameof(configuration));
            _dbContext = dbContext;
        }

        public async Task SaveReceivedSuccessResponse()
        {
            var client = await new PulsarClientBuilder().ServiceUrl(_pulsarServiceUrl).BuildAsync();

            var responseConsumer = await client.NewConsumer()
                .Topic(TopicName)
                .SubscriptionName(SubscriptionName)
                .SubscriptionType(SubscriptionType.Exclusive)
                .SubscribeAsync();

            Console.WriteLine("Subscribed to pulsar_success_response");

            while (true)
            {
                var responseMessage = await responseConsumer.ReceiveAsync();
                try
                {
                    var responseJson = Encoding.UTF8.GetString(responseMessage.Data);
                    var response = JsonSerializer.Deserialize<ReservationResponseDto>(responseJson);

                    if (responseJson != null)
                    {
                        //var formattedJson = JsonSerializer.Deserialize<JsonElement>(response); // Deserialize to JSON element

                        var responseEntity = new ReservationResponse
                        {
                            RawResponse = response.RawRequest, //formattedJson.GetRawText(), // Store as raw JSON
                            Timestamp = response.Timestamp
                        };

                        _dbContext.Add(responseEntity);
                        await _dbContext.SaveChangesAsync();
                        Console.WriteLine("Saved response to ReservationResponse table.");
                    }

                    await responseConsumer.AcknowledgeAsync(responseMessage.MessageId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing response: {ex.Message}");
                }
            }
        }

    }
}
