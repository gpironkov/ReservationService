using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReservationService.Publisher.DTOs
{
    public class FailureResponseDto
    {
        [JsonPropertyName("ErrorMessage")]
        public string ErrorMessage { get; set; }

        [JsonPropertyName("RawResponse")]
        public string RawResponse { get; set; }
    }
}
