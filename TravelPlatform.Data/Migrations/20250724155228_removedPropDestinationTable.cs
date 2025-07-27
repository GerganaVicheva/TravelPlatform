using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelPlatform.Data.Migrations
{
    /// <inheritdoc />
    public partial class removedPropDestinationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Destinations");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Destinations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Destinations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Destinations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
