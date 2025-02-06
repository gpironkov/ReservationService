using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReservationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationService.SubscriberSuccess.Services
{
    public class ConstraintApplier
    {
        public static async Task ApplyReservationConstraints(ReservationDbContext dbContext)
        {
            // Apply constraint to prevent two users from reserving same table for same date and hour
            try
            {
                await using var connection = new SqlConnection(dbContext.Database.GetConnectionString());
                await connection.OpenAsync();

                var sqlScript = await File.ReadAllTextAsync("Scripts/ReservationConstraint.sql");
                await using var command = new SqlCommand(sqlScript, connection);
                await command.ExecuteNonQueryAsync();

                Console.WriteLine("Constraints applied!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not apply constraints. Error: {ex.Message}");
            }
        }
    }
}
