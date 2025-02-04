using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationService.Publisher.Models
{
    public class CreateDbContext
    {
        public static async Task InitializeAsync(string[] args)
        {
            var factory = new DesignTimeDbContextFactory();

            using var dbContext = factory.CreateDbContext(args);

            if (!await dbContext.Database.CanConnectAsync())
            {
                await dbContext.Database.MigrateAsync();
            }
        }
    }
}
