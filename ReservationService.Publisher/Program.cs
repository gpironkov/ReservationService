using DotPulsar.Extensions;
using Microsoft.Extensions.Configuration;
using System.Text;
using PulsarClient = DotPulsar.PulsarClient;
using ReservationService.Services.Validation;
using DotPulsar.Abstractions;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

string pulsarUrl = configuration["Pulsar:ServiceUrl"];
string topicSuccess = "pulsar_success";
string topicFailed = "pulsar_failed";

var client = PulsarClient.Builder().ServiceUrl(new Uri(pulsarUrl)).Build();
await CreateTopicIfMissing(client, topicSuccess);
await CreateTopicIfMissing(client, topicFailed);

var producerSuccess = client.NewProducer().Topic(topicSuccess).Create();

var producerFailed = client.NewProducer().Topic(topicFailed).Create();

var message = "{ \"ClientName\": \"Johny Depp\", \"ClientTelephone\": \"1234561111\", \"NumberOfReservedTable\": 1, \"DateOfReservation\": \"2025-02-14 11:20\" }";

var (isValid, errorMessage) = ReservationValidator.ValidateReservation(message);

if (isValid)
{
    await producerSuccess.Send(Encoding.UTF8.GetBytes(message));
    Console.WriteLine("Message published to pulsar_success.");
}
else
{
    await producerFailed.Send(Encoding.UTF8.GetBytes(message)); //$"{{ \"error\": \"{errorMessage}\" }}"
    Console.WriteLine($"Validation failed: {errorMessage} \nMessage published to pulsar_failed.");
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
