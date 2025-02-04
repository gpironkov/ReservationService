//using DotPulsar;
//using DotPulsar.Abstractions;
//using DotPulsar.Extensions;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using Pulsar.Client.Common;
//using ReservationService.Publisher.DTOs;
//using ReservationService.Publisher.Models;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using SubscriptionType = DotPulsar.SubscriptionType;

//namespace ReservationService.Publisher.Services
//{
//    public class ResponseReceiver
//    {
//        private const string TopicName = "pulsar_success_response";
//        private const string SubscriptionName = "response-subscription";

//        public static async Task ReceiveSuccessResponse(IPulsarClient client)
//        {
//            var responseConsumer = client.NewConsumer()
//                .Topic(TopicName)
//                .SubscriptionName(SubscriptionName)
//                .SubscriptionType(SubscriptionType.Exclusive)
//                .Create();

//            Console.WriteLine("Subscribed to pulsar_success_response");

//            while (true)
//            {
//                var responseMessage = await responseConsumer.ReceiveAsync();
//                try
//                {
//                    var responseJson = Encoding.UTF8.GetString(responseMessage.Data);
//                    //var response = JsonSerializer.Deserialize<ReservationResponseDto>(responseJson);

//                    if (responseJson != null)
//                    {
//                        var responseEntity = new ReservationResponse
//                        {
//                            RawResponse = responseJson,
//                            Timestamp = DateTime.UtcNow
//                        };

//                        _dbContext.Add(responseEntity);
//                        await _dbContext.SaveChangesAsync();
//                        Console.WriteLine("Saved response to ReservationResponse table.");
//                    }

//                    await responseConsumer.Acknowledge(responseMessage);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error processing response: {ex.Message}");
//                }
//            }
//        }

//    }
//}
