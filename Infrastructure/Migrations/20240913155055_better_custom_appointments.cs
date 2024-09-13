using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class better_custom_appointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_custom_appointments_users_UserEmail",
                table: "custom_appointments");

            migrationBuilder.DropIndex(
                name: "IX_custom_appointments_UserEmail",
                table: "custom_appointments");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "custom_appointments");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "custom_appointments",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "custom_appointments");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "custom_appointments",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_custom_appointments_UserEmail",
                table: "custom_appointments",
                column: "UserEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_custom_appointments_users_UserEmail",
                table: "custom_appointments",
                column: "UserEmail",
                principalTable: "users",
                principalColumn: "email");
        }
    }
}
