using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rdmp.Migrations
{
    /// <inheritdoc />
    public partial class AddTrelloFieldsToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
