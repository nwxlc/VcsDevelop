using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GitDevelop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "blobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    size = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hash = table.Column<byte[]>(type: "bytea", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_blobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    default_branch_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    metadata_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    metadata_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_documents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "trees",
                columns: table => new
                {
                    tree_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hash = table.Column<byte[]>(type: "bytea", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trees", x => x.tree_id);
                });

            migrationBuilder.CreateTable(
                name: "document_tags",
                columns: table => new
                {
                    tag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_tags", x => new { x.document_id, x.tag });
                    table.ForeignKey(
                        name: "FK_document_tags_documents_document_id",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "commits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    repository_id = table.Column<Guid>(type: "uuid", nullable: false),
                    root_tree_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hash = table.Column<byte[]>(type: "bytea", maxLength: 32, nullable: false),
                    message = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_commits", x => x.id);
                    table.ForeignKey(
                        name: "fk_commits_account",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_commits_repository",
                        column: x => x.repository_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_commits_root_tree",
                        column: x => x.root_tree_id,
                        principalTable: "trees",
                        principalColumn: "tree_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tree_entries",
                columns: table => new
                {
                    tree_id = table.Column<Guid>(type: "uuid", nullable: false),
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    object_id = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tree_entries", x => new { x.tree_id, x.id });
                    table.ForeignKey(
                        name: "FK_tree_entries_trees_tree_id",
                        column: x => x.tree_id,
                        principalTable: "trees",
                        principalColumn: "tree_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "branches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    head_commit_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_branches", x => x.id);
                    table.ForeignKey(
                        name: "fk_branches_document",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_branches_head_commit",
                        column: x => x.head_commit_id,
                        principalTable: "commits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "commit_parents",
                columns: table => new
                {
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    commit_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_commit_parents", x => new { x.commit_id, x.parent_id });
                    table.ForeignKey(
                        name: "FK_commit_parents_commits_commit_id",
                        column: x => x.commit_id,
                        principalTable: "commits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_accounts_email",
                table: "accounts",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_blobs_hash",
                table: "blobs",
                column: "hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_branches_head_commit_id",
                table: "branches",
                column: "head_commit_id");

            migrationBuilder.CreateIndex(
                name: "ux_branches_document_name",
                table: "branches",
                columns: new[] { "document_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_commit_parents_parent_id",
                table: "commit_parents",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_commits_account_id",
                table: "commits",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_commits_created_at",
                table: "commits",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_commits_repository_id",
                table: "commits",
                column: "repository_id");

            migrationBuilder.CreateIndex(
                name: "IX_commits_root_tree_id",
                table: "commits",
                column: "root_tree_id");

            migrationBuilder.CreateIndex(
                name: "ux_commits_hash",
                table: "commits",
                column: "hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tree_entries_tree_name",
                table: "tree_entries",
                columns: new[] { "tree_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_trees_hash",
                table: "trees",
                column: "hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "blobs");

            migrationBuilder.DropTable(
                name: "branches");

            migrationBuilder.DropTable(
                name: "commit_parents");

            migrationBuilder.DropTable(
                name: "document_tags");

            migrationBuilder.DropTable(
                name: "tree_entries");

            migrationBuilder.DropTable(
                name: "commits");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "trees");
        }
    }
}
