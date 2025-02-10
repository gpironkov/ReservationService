using Microsoft.EntityFrameworkCore;
using ReservationService.Publisher.DTOs;

namespace ReservationService.Publisher.Models
{
    public class ReservationDbContext : DbContext
    {
        public ReservationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<SuccessResponseDto> ReservationResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SuccessResponseDto>().HasKey(r => r.Id);
        }
    }
}
