using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VcsDevelop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSHA1ToSHA256 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tree_id",
                table: "trees",
                type: "char(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(40)");

            migrationBuilder.AlterColumn<string>(
                name: "tree_id",
                table: "tree_entries",
                type: "char(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(40)");

            migrationBuilder.AlterColumn<string>(
                name: "root_tree_id",
                table: "commits",
                type: "char(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(40)");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "commits",
                type: "char(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(40)");

            migrationBuilder.AlterColumn<string>(
                name: "commit_id",
                table: "commit_parents",
                type: "char(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(40)");

            migrationBuilder.AlterColumn<string>(
                name: "head_commit_id",
                table: "branches",
                type: "char(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(40)");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "blobs",
                type: "char(64)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(40)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "tree_id",
                table: "trees",
                type: "char(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(64)");

            migrationBuilder.AlterColumn<string>(
                name: "tree_id",
                table: "tree_entries",
                type: "char(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(64)");

            migrationBuilder.AlterColumn<string>(
                name: "root_tree_id",
                table: "commits",
                type: "char(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(64)");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "commits",
                type: "char(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(64)");

            migrationBuilder.AlterColumn<string>(
                name: "commit_id",
                table: "commit_parents",
                type: "char(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(64)");

            migrationBuilder.AlterColumn<string>(
                name: "head_commit_id",
                table: "branches",
                type: "char(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(64)");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "blobs",
                type: "char(40)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(64)");
        }
    }
}
