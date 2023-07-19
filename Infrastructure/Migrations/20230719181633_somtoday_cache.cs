using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class somtoday_cache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cached_somtoday_grades",
                table: "users",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cached_somtoday_homework",
                table: "users",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cached_somtoday_grades",
                table: "users");

            migrationBuilder.DropColumn(
                name: "cached_somtoday_homework",
                table: "users");
        }
    }
}
