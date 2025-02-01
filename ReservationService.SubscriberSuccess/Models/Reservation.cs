namespace ReservationService.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string RawRequest { get; set; } = string.Empty;
        public DateTime DT { get; set; }
        public int ValidationResult { get; set; } // 0: Fail, 9: OK
    }
}
