namespace ReservationService.Publisher.DTOs
{
    public class SuccessResponseDto
    {
        public int Id { get; set; }
        public string RawResponse { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
