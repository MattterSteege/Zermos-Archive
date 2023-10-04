using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class setting_system : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "default_page",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "theme",
                table: "users",
                newName: "settings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "settings",
                table: "users",
                newName: "theme");

            migrationBuilder.AddColumn<string>(
                name: "default_page",
                table: "users",
                type: "longtext",
                nullable: true);
        }
    }
}
