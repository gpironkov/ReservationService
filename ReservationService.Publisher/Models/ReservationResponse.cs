using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationService.Publisher.Models
{
    public class ReservationResponse
    {
        public int Id { get; set; }
        public string RawResponse { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
