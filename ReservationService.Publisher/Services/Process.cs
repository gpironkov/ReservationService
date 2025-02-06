using Microsoft.Extensions.Configuration;
using ReservationService.Publisher.Models;
using ReservationService.Services.Validation;
using DotPulsar.Extensions;
using System.Text;
using PulsarClient = DotPulsar.PulsarClient;
using DotPulsar.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using CommonUtilities;

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
            Console.WriteLine("Publisher service started. Connecting to Pulsar...");
            await PulsarHealthCheck.EnsurePulsarRunningAsync(pulsarUrl);

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

                await ReceiveSuccessMessage(configuration);
            }
            else
            {
                await producerFailed.Send(Encoding.UTF8.GetBytes(message));
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

        static async Task ReceiveSuccessMessage(IConfigurationRoot config)
        {
            var serviceProvider = new ServiceCollection()
                    .AddDbContext<ReservationDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")))
                    .AddSingleton<IConfiguration>(config)
                    .AddSingleton<ResponseReceiver>()
                    .BuildServiceProvider();

            var receiver = serviceProvider.GetRequiredService<ResponseReceiver>();
            await receiver.SaveReceivedSuccessResponse();
        }
    }
}
