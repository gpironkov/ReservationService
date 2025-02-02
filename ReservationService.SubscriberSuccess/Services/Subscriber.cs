using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pulsar.Client.Api;
using Pulsar.Client.Common;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ReservationService.Subscriber.Services
{
    public class Subscriber
    {
        private readonly string? _connectionString;
        private readonly string? _pulsarServiceUrl;
        private const string TopicName = "pulsar_success";

        public Subscriber(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
            _pulsarServiceUrl = configuration["Pulsar:ServiceUrl"] ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Success subscriber service started. Connecting to Pulsar...");

            var client = await new PulsarClientBuilder().ServiceUrl(_pulsarServiceUrl).BuildAsync();

            var consumer = await client.NewConsumer()
                .Topic(TopicName)
                .SubscriptionName("reservation-subscription")
                .SubscriptionType(SubscriptionType.Exclusive)
                .SubscribeAsync();

            Console.WriteLine("Success subscriber is waiting for messages...");

            while (true)
            {
                var message = await consumer.ReceiveAsync();
                try
                {
                    var rawRequest = Encoding.UTF8.GetString(message.Data);
                    Console.WriteLine($"Received message: {rawRequest}");

                    // ValidationResult = 9 (Success)
                    await InsertReservationAsync(rawRequest, 9);

                    await consumer.AcknowledgeAsync(message.MessageId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    await consumer.NegativeAcknowledge(message.MessageId);
                }
            }
        }

        private async Task InsertReservationAsync(string rawRequest, int validationResult)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("InsertReservation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@RawRequest", rawRequest);
                    command.Parameters.AddWithValue("@DT", DateTime.UtcNow);
                    command.Parameters.AddWithValue("@ValidationResult", validationResult);

                    await command.ExecuteNonQueryAsync();
                }
            }

            Console.WriteLine("Message stored successfully in SQL.");
        }
    }
}
