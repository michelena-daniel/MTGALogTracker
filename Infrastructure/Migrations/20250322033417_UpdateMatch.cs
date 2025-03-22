using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeckId",
                table: "Matches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeckName",
                table: "Matches",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventType",
                table: "Matches",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeckId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "DeckName",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Matches");
        }
    }
}
