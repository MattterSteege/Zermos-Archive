using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class share_system : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notes");

            migrationBuilder.CreateTable(
                name: "shares",
                columns: table => new
                {
                    key = table.Column<string>(type: "varchar(255)", nullable: false),
                    email = table.Column<string>(type: "longtext", nullable: true),
                    value = table.Column<string>(type: "longtext", nullable: true),
                    page = table.Column<string>(type: "longtext", nullable: true),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    max_uses = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shares", x => x.key);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shares");

            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    NoteBookId = table.Column<string>(type: "longtext", nullable: true),
                    Omschrijving = table.Column<string>(type: "longtext", nullable: true),
                    Paragraphs = table.Column<string>(type: "longtext", nullable: true),
                    Tags = table.Column<string>(type: "longtext", nullable: true),
                    Titel = table.Column<string>(type: "longtext", nullable: true),
                    lastEdit = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }
    }
}
