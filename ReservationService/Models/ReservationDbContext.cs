using Microsoft.EntityFrameworkCore;

namespace ReservationService.Models
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>().HasKey(r => r.Id);
        }
    }
}
