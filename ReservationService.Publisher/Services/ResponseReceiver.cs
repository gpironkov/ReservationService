using CommonUtilities;
using Microsoft.Extensions.Configuration;
using Pulsar.Client.Api;
using ReservationService.Publisher.DTOs;
using ReservationService.Publisher.Models;
using System.Text;
using System.Text.Json;

namespace ReservationService.Publisher.Services
{
    public class ResponseReceiver
    {
        private const string TopicSuccess = "pulsar_success_response";
        private const string TopicFailed = "pulsar_failed_response";
        private const string SubscriptionSuccess = "success-subscription";
        private const string SubscriptionFailed = "failure-subscription";

        private readonly PulsarConsumerPool _consumerPool;
        private readonly ReservationDbContext _dbContext;

        public ResponseReceiver(IConfiguration configuration, ReservationDbContext dbContext, PulsarConsumerPool consumerPool)
        {
            _dbContext = dbContext;
            _consumerPool = consumerPool;
        }

        public async Task WaitForReservationResponseAsync()
        {
            var successConsumer = await _consumerPool.GetOrCreateConsumerAsync(TopicSuccess, SubscriptionSuccess);

            var failureConsumer = await _consumerPool.GetOrCreateConsumerAsync(TopicFailed, SubscriptionFailed);

            Console.WriteLine("Waiting for reservation response...");

            var successTask = WaitForMessageAsync(successConsumer);
            var failureTask = WaitForMessageAsync(failureConsumer);

            var completedTask = await Task.WhenAny(successTask, failureTask);

            if (completedTask == successTask)
            {
                var receivedMessage = await successTask;
                await ReceiveSuccessResponse(receivedMessage);
            }
            else if (completedTask == failureTask)
            {
                var receivedMessage = await failureTask;
                await ReceiveFailureResponse(receivedMessage);
            }
        }

        private async Task ReceiveSuccessResponse(string receivedMessage)
        {
            try
            {
                var response = JsonSerializer.Deserialize<ReservationResponseDto>(receivedMessage);

                var responseEntity = new SuccessResponseDto
                {
                    RawResponse = response.RawResponse,
                    Timestamp = response.Timestamp
                };

                _dbContext.Add(responseEntity);
                await _dbContext.SaveChangesAsync();
                Console.WriteLine("Saved response to ReservationResponse table.");

                var formattedRawResponse = JsonSerializer.Deserialize<JsonElement>(response.RawResponse);
                Console.WriteLine($"Success: { JsonSerializer.Serialize(formattedRawResponse, new JsonSerializerOptions { WriteIndented = true })}\n");

                //await consumer.AcknowledgeAsync(receivedMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing response: {ex.Message}");
            }
        }

        private async Task ReceiveFailureResponse(string receivedMessage)
        {
            try
            {
                var failureResponse = JsonSerializer.Deserialize<FailureResponseDto>(receivedMessage);
                var formattedRawResponse = JsonSerializer.Deserialize<JsonElement>(failureResponse.RawResponse);

                Console.WriteLine("Failure:");
                Console.WriteLine($"RawResponse: {JsonSerializer.Serialize(formattedRawResponse, new JsonSerializerOptions { WriteIndented = true })}\n");
                Console.WriteLine($"ErrorMessage: {failureResponse.ErrorMessage}");

                //await consumer.AcknowledgeAsync(message.MessageId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing failure response: {ex.Message}");
            }
        }


        private async Task<string> WaitForMessageAsync(IConsumer<byte[]> consumer)
        {
            var message = await consumer.ReceiveAsync();
            await consumer.AcknowledgeAsync(message.MessageId);

            return Encoding.UTF8.GetString(message.Data);
        }

    }
}
