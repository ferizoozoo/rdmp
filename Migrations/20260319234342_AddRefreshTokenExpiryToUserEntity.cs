using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace rdmp.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenExpiryToUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiry",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiry",
                table: "Users");
        }
    }
}
