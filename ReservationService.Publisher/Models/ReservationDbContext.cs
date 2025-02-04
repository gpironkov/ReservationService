using Microsoft.EntityFrameworkCore;

namespace ReservationService.Publisher.Models
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<ReservationResponse> ReservationResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReservationResponse>().HasKey(r => r.Id);
        }
    }
}
