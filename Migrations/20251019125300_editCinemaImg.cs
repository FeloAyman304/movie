using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie_hospital_1.Migrations
{
    /// <inheritdoc />
    public partial class editCinemaImg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Cinemas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Cinemas");
        }
    }
}
