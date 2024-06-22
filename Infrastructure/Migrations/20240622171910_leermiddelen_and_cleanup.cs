using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class leermiddelen_and_cleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cached_infowijs_calendar",
                table: "users");

            migrationBuilder.DropColumn(
                name: "cached_school_informationscreen",
                table: "users");
            
            migrationBuilder.DropColumn(
                name: "cached_zermelo_schedule",
                table: "users");
            
            migrationBuilder.DropColumn(
                name: "cached_somtoday_absence",
                table: "users");
            
            migrationBuilder.AddColumn<string>(
                name: "custom_leermiddelen",
                table: "users",
                type: "longtext",
                nullable: true);
            
            migrationBuilder.AddColumn<string>(
                name: "cached_somtoday_leermiddelen",
                table: "users",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "custom_leermiddelen",
                table: "users");

            migrationBuilder.DropColumn(
                name: "cached_somtoday_leermiddelen",
                table: "users");
            
            migrationBuilder.AddColumn<string>(
                name: "cached_infowijs_calendar",
                table: "users",
                type: "longtext",
                nullable: true);
            
            migrationBuilder.AddColumn<string>(
                name: "cached_school_informationscreen",
                table: "users",
                type: "longtext",
                nullable: true);
            
            migrationBuilder.AddColumn<string>(
                name: "cached_zermelo_schedule",
                table: "users",
                type: "longtext",
                nullable: true);
            
            migrationBuilder.AddColumn<string>(
                name: "cached_somtoday_absence",
                table: "users",
                type: "longtext",
                nullable: true);
        }
    }
}