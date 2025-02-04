using Microsoft.Extensions.Configuration;
using ReservationService.Publisher.Models;
using ReservationService.Services.Validation;
using DotPulsar.Extensions;
using System.Text;
using PulsarClient = DotPulsar.PulsarClient;
using DotPulsar.Abstractions;

namespace ReservationService.Publisher.Services
{
    public class Process
    {
        public static async Task BookReservationAsync(string message, string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var pulsarUrl = configuration["Pulsar:ServiceUrl"];
            const string topicSuccess = "pulsar_success";
            const string topicFailed = "pulsar_failed";
            var client = PulsarClient.Builder().ServiceUrl(new Uri(pulsarUrl)).Build();

            await CreateTopicIfMissing(client, topicSuccess);
            await CreateTopicIfMissing(client, topicFailed);

            var producerSuccess = client.NewProducer().Topic(topicSuccess).Create();
            var producerFailed = client.NewProducer().Topic(topicFailed).Create();
            var (isValid, errorMessage) = ReservationValidator.ValidateReservation(message);

            if (isValid)
            {
                await producerSuccess.Send(Encoding.UTF8.GetBytes(message));
                Console.WriteLine("\nMessage published to pulsar_success.\n");
                await CreateDbContext.InitializeAsync(args);
                // TODO: Task 3 - implement receive message from Success Subscriber and save it in new table 
                //await ResponseReceiver.ReceiveSuccessResponse(client);
            }
            else
            {
                await producerFailed.Send(Encoding.UTF8.GetBytes(message)); //$"{{ \"error\": \"{errorMessage}\" }}"
                Console.WriteLine($"Validation failed: {errorMessage} \nMessage published to pulsar_failed.\n");
            }
        }
        
        static async Task CreateTopicIfMissing(IPulsarClient client, string topicName)
        {
            try
            {
                var producer = client.NewProducer().Topic(topicName).Create();
                await producer.DisposeAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating topic {topicName}: {ex.Message}");
            }
        }
    }
}
