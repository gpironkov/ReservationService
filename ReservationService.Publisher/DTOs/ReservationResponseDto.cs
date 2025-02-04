using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationService.Publisher.DTOs
{
    public class ReservationResponseDto
    {
        public DateTime Timestamp { get; set; }

        public string RawRequest { get; set; }
    }
}
