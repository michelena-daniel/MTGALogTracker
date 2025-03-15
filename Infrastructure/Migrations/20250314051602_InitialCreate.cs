using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    MtgaInternalUserId = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    UserNameWithCode = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    UserName = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    UserCode = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    LastLogin = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.MtgaInternalUserId);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchId = table.Column<string>(type: "text", nullable: false),
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    TransactionId = table.Column<string>(type: "text", nullable: false),
                    TimeStamp = table.Column<string>(type: "text", nullable: false),
                    MatchCompletedReason = table.Column<string>(type: "text", nullable: false),
                    IsDraw = table.Column<bool>(type: "boolean", nullable: false),
                    WinningTeamId = table.Column<int>(type: "integer", nullable: false),
                    PlayerOneName = table.Column<string>(type: "text", nullable: false),
                    PlayerTwoName = table.Column<string>(type: "text", nullable: false),
                    PlayerOneMtgaId = table.Column<string>(type: "text", nullable: false),
                    PlayerTwoMtgaId = table.Column<string>(type: "text", nullable: false),
                    HomeUser = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchId);
                    table.ForeignKey(
                        name: "FK_Matches_Users_HomeUser",
                        column: x => x.HomeUser,
                        principalTable: "Users",
                        principalColumn: "MtgaInternalUserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PlayerRanks",
                columns: table => new
                {
                    LogId = table.Column<string>(type: "text", nullable: false),
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
                    MtgArenaUserId = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerRanks", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_PlayerRanks_Users_MtgArenaUserId",
                        column: x => x.MtgArenaUserId,
                        principalTable: "Users",
                        principalColumn: "MtgaInternalUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matches_HomeUser",
                table: "Matches",
                column: "HomeUser");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_MatchId",
                table: "Matches",
                column: "MatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRanks_LogId",
                table: "PlayerRanks",
                column: "LogId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRanks_MtgArenaUserId",
                table: "PlayerRanks",
                column: "MtgArenaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerRanks_TimeStamp",
                table: "PlayerRanks",
                column: "TimeStamp");

            migrationBuilder.CreateIndex(
                name: "IX_Users_MtgaInternalUserId",
                table: "Users",
                column: "MtgaInternalUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "PlayerRanks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
