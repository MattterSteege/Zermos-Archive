using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    email = table.Column<string>(type: "varchar(255)", nullable: false),
                    name = table.Column<string>(type: "longtext", nullable: true),
                    school_id = table.Column<string>(type: "longtext", nullable: true),
                    zermelo_access_token = table.Column<string>(type: "longtext", nullable: true),
                    zermelo_access_token_expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    somtoday_access_token = table.Column<string>(type: "longtext", nullable: true),
                    somtoday_refresh_token = table.Column<string>(type: "longtext", nullable: true),
                    somtoday_student_id = table.Column<string>(type: "longtext", nullable: true),
                    infowijs_access_token = table.Column<string>(type: "longtext", nullable: true),
                    VerificationToken = table.Column<string>(type: "longtext", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.email);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
