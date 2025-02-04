using System.Text.Json;

namespace ReservationService.Publisher.Validation
{
    public class JsonValidator
    {
        public static bool IsValidJson<T>(string json)
        {
            try
            {
                JsonSerializer.Deserialize<T>(json);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}
