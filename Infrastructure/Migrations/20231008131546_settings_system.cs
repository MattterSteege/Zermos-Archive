using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class settings_system : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "settings",
                table: "users",
                type: "longtext",
                nullable: true);
            
            migrationBuilder.DropColumn(
                name: "default_page", 
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "settings",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "default_page",
                table: "users",
                type: "longtext",
                nullable: true);
        }
    }
}
