using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rdmp.Migrations
{
    /// <inheritdoc />
    public partial class CreateTrelloConnectionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrelloConnections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    MemberId = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    ConnectedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrelloConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrelloConnections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrelloConnections_UserId",
                table: "TrelloConnections",
                column: "UserId",
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT INTO "TrelloConnections" ("UserId", "Token", "MemberId", "Username", "ConnectedAt")
                SELECT "Id",
                       "TrelloToken",
                       COALESCE("TrelloMemberId", ''),
                       COALESCE("TrelloUsername", ''),
                       COALESCE("TrelloConnectedAt", CURRENT_TIMESTAMP)
                FROM "Users"
                WHERE "TrelloToken" IS NOT NULL AND TRIM("TrelloToken") <> '';
                """);

            migrationBuilder.DropColumn(
                name: "TrelloConnectedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TrelloMemberId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TrelloToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TrelloUsername",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrelloConnections");

            migrationBuilder.AddColumn<DateTime>(
                name: "TrelloConnectedAt",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrelloMemberId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrelloToken",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrelloUsername",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE "Users"
                SET "TrelloToken" = (
                        SELECT "Token"
                        FROM "TrelloConnections"
                        WHERE "TrelloConnections"."UserId" = "Users"."Id"
                    ),
                    "TrelloMemberId" = (
                        SELECT "MemberId"
                        FROM "TrelloConnections"
                        WHERE "TrelloConnections"."UserId" = "Users"."Id"
                    ),
                    "TrelloUsername" = (
                        SELECT "Username"
                        FROM "TrelloConnections"
                        WHERE "TrelloConnections"."UserId" = "Users"."Id"
                    ),
                    "TrelloConnectedAt" = (
                        SELECT "ConnectedAt"
                        FROM "TrelloConnections"
                        WHERE "TrelloConnections"."UserId" = "Users"."Id"
                    )
                WHERE EXISTS (
                    SELECT 1
                    FROM "TrelloConnections"
                    WHERE "TrelloConnections"."UserId" = "Users"."Id"
                );
                """);
        }
    }
}
