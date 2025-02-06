using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReservationService.Publisher.DTOs
{
    public class ReservationResponseDto
    {
        [JsonPropertyName("RawRequest")]
        public string RawRequest { get; set; }

        [JsonPropertyName("Timestamp")]
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
