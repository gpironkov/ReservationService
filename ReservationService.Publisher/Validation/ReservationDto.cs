using System.Text.Json.Serialization;

namespace ReservationService.Services.Validation
{
    public class ReservationDto
    {
        [JsonPropertyName("ClientName")]
        public string ClientName { get; set; }

        [JsonPropertyName("ClientTelephone")]
        public string ClientTelephone { get; set; }

        [JsonPropertyName("NumberOfReservedTable")]
        public int NumberOfReservedTable { get; set; }

        [JsonPropertyName("DateOfReservation")]
        public string DateOfReservation { get; set; }
    }
}
