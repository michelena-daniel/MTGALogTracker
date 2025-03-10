using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitMtgaTracker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserNameWithCode = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    UserName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    UserCode = table.Column<string>(type: "VARCHAR(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRanks",
                columns: table => new
                {
                    RankId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConstructedSeasonOrdinal = table.Column<int>(type: "integer", nullable: false),
                    ConstructedClass = table.Column<string>(type: "text", nullable: false),
                    ConstructedLevel = table.Column<int>(type: "integer", nullable: false),
                    ConstructedStep = table.Column<int>(type: "integer", nullable: false),
                    ConstructedMatchesWon = table.Column<int>(type: "integer", nullable: false),
                    ConstructedMatchesLost = table.Column<int>(type: "integer", nullable: false),
                    ConstructedMatchesDrawn = table.Column<int>(type: "integer", nullable: false),
                    LimitedSeasonOrdinal = table.Column<int>(type: "integer", nullable: false),
                    LimitedClass = table.Column<string>(type: "text", nullable: false),
                    LimitedLevel = table.Column<int>(type: "integer", nullable: false),
                    LimitedStep = table.Column<int>(type: "integer", nullable: false),
                    LimitedMatchesWon = table.Column<int>(type: "integer", nullable: false),
                    LimitedMatchesLost = table.Column<int>(type: "integer", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LogId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRanks", x => x.RankId);
                    table.ForeignKey(
                        name: "FK_PlayerRanks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRanks_LogId",
                table: "PlayerRanks",
                column: "LogId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRanks_UserId",
                table: "PlayerRanks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserNameWithCode",
                table: "Users",
                column: "UserNameWithCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerRanks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
