using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class more_cache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cached_infowijs_calendar",
                table: "users",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cached_infowijs_news",
                table: "users",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cached_school_informationscreen",
                table: "users",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cached_infowijs_calendar",
                table: "users");

            migrationBuilder.DropColumn(
                name: "cached_infowijs_news",
                table: "users");

            migrationBuilder.DropColumn(
                name: "cached_school_informationscreen",
                table: "users");
        }
    }
}
