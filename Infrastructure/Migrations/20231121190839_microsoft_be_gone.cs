using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class microsoft_be_gone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "notities",
                table: "users");

            migrationBuilder.DropColumn(
                name: "teams_access_token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "teams_refresh_token",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "notities",
                table: "users",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "teams_access_token",
                table: "users",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "teams_refresh_token",
                table: "users",
                type: "longtext",
                nullable: true);
        }
    }
}
