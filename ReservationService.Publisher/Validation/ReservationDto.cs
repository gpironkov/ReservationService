namespace ReservationService.Services.Validation
{
    internal class ReservationDto
    {
        internal string ClientName { get; set; }
        internal string ClientTelephone { get; set; }
        internal int NumberOfReservedTable { get; set; }
        internal string DateOfReservation { get; set; }
    }
}
