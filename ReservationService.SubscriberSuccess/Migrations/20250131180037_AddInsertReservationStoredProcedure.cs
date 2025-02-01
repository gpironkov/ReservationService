using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservationService.Migrations
{
    /// <inheritdoc />
    public partial class AddInsertReservationStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE InsertReservation
                    @RawRequest NVARCHAR(MAX),
                    @DT DATETIME2,
                    @ValidationResult INT
                AS
                BEGIN
                    INSERT INTO Reservations (RawRequest, DT, ValidationResult)
                    VALUES (@RawRequest, @DT, @ValidationResult);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
