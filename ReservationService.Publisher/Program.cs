using ReservationService.Services.Validation;
using ReservationService.Publisher.Validation;
using ReservationService.Publisher.Services;
using System.Text;


while (true)
{
    Console.Write("Enter reservation JSON: \n");
    StringBuilder jsonBuilder = new StringBuilder();
    string line;

    while ((line = Console.ReadLine()) != null && line.ToUpper() != "END")
    {
        jsonBuilder.AppendLine(line);
    }

    string input = jsonBuilder.ToString().Trim();

    if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
        break;

    if (!JsonValidator.IsValidJson<ReservationDto>(input))
    {
        Console.WriteLine("Invalid JSON format. Please enter a valid reservation JSON.");
        continue;
    }

    await Task.Run(() => Process.BookReservationAsync(input, args));
}
