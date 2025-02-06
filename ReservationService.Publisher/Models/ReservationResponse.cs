namespace ReservationService.Publisher.Models
{
    public class ReservationResponse
    {
        public int Id { get; set; }
        public string RawResponse { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
