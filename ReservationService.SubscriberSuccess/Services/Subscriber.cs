using CommonUtilities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pulsar.Client.Api;
using Pulsar.Client.Common;
using ReservationService.SubscriberSuccess.Services;
using System.Data;
using System.Text;

namespace ReservationService.Subscriber.Services
{
    public class Subscriber
    {
        private readonly string? _connectionString;
        private readonly string? _pulsarServiceUrl;
        private const string TopicName = "pulsar_success";
        private const string SubscriptionName = "reservation-subscription";

        public Subscriber(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
            _pulsarServiceUrl = configuration["Pulsar:ServiceUrl"] ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Success subscriber service started. Connecting to Pulsar...");

            await PulsarHealthCheck.EnsurePulsarRunningAsync(_pulsarServiceUrl);

            var client = await new PulsarClientBuilder().ServiceUrl(_pulsarServiceUrl).BuildAsync();

            var consumer = await client.NewConsumer()
                .Topic(TopicName)
                .SubscriptionName(SubscriptionName)
                .SubscriptionType(SubscriptionType.Exclusive)
                .SubscribeAsync();

            Console.WriteLine("Success subscriber is waiting for messages...");

            while (true)
            {
                var message = await consumer.ReceiveAsync();
                try
                {
                    var rawRequest = Encoding.UTF8.GetString(message.Data);
                    Console.WriteLine($"Received message: \n{rawRequest}");

                    // ValidationResult = 9 (Success)
                    await InsertReservationAsync(rawRequest, 9, client, message);

                    await consumer.AcknowledgeAsync(message.MessageId);                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    await consumer.NegativeAcknowledge(message.MessageId);
                }
            }
        }

        private async Task InsertReservationAsync(string rawRequest, int validationResult, PulsarClient client, Message<byte[]> message)
        {
            try
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

                Console.WriteLine("\nMessage stored successfully in SQL.\n");
                await ResponseMessage.SendSuccessMessage(client, message);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // Unique Constraint Violation
            {
                Console.WriteLine("\nThis table is already reserved for the selected date and time. Please choose another time or table.\n");
                await ResponseMessage.SendFailedMessage(client, message, "\nThis table is already reserved for the selected date and time.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
    }
}
