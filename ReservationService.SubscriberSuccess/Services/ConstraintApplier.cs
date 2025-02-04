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
        public static void ApplyReservationConstraints(ReservationDbContext dbContext)
        {
            // Apply constraint to prevent two users from reserving same table for same date and hour
            try
            {
                using (var connection = new SqlConnection(dbContext.Database.GetConnectionString()))
                {
                    connection.Open();
                    var command = new SqlCommand(File.ReadAllText("Scripts/ReservationConstraint.sql"), connection);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Constraints applied!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not apply constraints. Error: {ex.Message}");
            }
        }
    }
}
