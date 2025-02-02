using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace ReservationService.SubscriberFailed.Data
{
    public class MongoDbService
    {
        private readonly IMongoCollection<FailedReservation> _failedReservations;

        public MongoDbService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
            _failedReservations = database.GetCollection<FailedReservation>(configuration["MongoDB:CollectionName"]);
        }

        public async Task SaveFailedReservationAsync(FailedReservation reservation)
        {
            await _failedReservations.InsertOneAsync(reservation);
        }
    }
}
