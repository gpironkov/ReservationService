using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReservationService.Services.Validation
{
    public class ReservationValidator
    {
        public static (bool IsValid, string ErrorMessage) ValidateReservation(string jsonData)
        {
            try
            {
                var reservation = JsonSerializer.Deserialize<ReservationDto>(jsonData);
                if (reservation == null)
                    return (false, "Invalid data format. Ensure JSON structure is correct.");

                if (string.IsNullOrWhiteSpace(reservation.ClientName))
                    return (false, "Client name cannot be empty.");

                if (string.IsNullOrWhiteSpace(reservation.ClientTelephone) ||
                    !Regex.IsMatch(reservation.ClientTelephone, @"^\d+$"))
                    return (false, "Client telephone must contain only numbers.");

                if (reservation.NumberOfReservedTable < 1 || reservation.NumberOfReservedTable > 20)
                    return (false, "Number of reserved tables must be between 1 and 20.");

                if (string.IsNullOrWhiteSpace(reservation.DateOfReservation))
                    return (false, "Date of reservation cannot be empty.");

                if (!DateTime.TryParseExact(reservation.DateOfReservation, "yyyy-MM-dd HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                {
                    return (false, "Date format is incorrect. Use 'YYYY-MM-DD HH:mm' (e.g., 2025-02-14 19:20).");
                }

                if (parsedDate < DateTime.Now)
                    return (false, "The reservation date must be today or a future date.");

                return (true, "Validation passed.");
            }
            catch (JsonException)
            {
                return (false, "Invalid JSON format. Please check the input structure.");
            }
        }
    }
}
