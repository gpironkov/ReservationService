using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReservationService.SubscriberFailed.Data
{
    public class FailedReservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("RawRequest")]
        public string RawRequest { get; set; }

        [BsonElement("DT")]
        public DateTime DT { get; set; }

        [BsonElement("ValidationResult")]
        public int ValidationResult { get; set; } = 0;
    }
}
