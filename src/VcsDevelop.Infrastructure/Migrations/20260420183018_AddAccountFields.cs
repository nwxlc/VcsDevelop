using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VcsDevelop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "avatar_url",
                table: "accounts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bio",
                table: "accounts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "accounts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "accounts",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "avatar_url",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "bio",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "accounts");
        }
    }
}
