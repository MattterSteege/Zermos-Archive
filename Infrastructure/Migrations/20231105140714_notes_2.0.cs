using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class notes_20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false),
                    NoteBookId = table.Column<string>(type: "longtext", nullable: true),
                    lastEdit = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Titel = table.Column<string>(type: "longtext", nullable: true),
                    Omschrijving = table.Column<string>(type: "longtext", nullable: true),
                    Paragraphs = table.Column<string>(type: "longtext", nullable: true),
                    Tags = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notes");
        }
    }
}
