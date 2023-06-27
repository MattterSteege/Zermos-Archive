using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class addedexpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "infowijs_session_token",
                table: "users");

            migrationBuilder.DropColumn(
                name: "school_naam_code",
                table: "users");

            migrationBuilder.DropColumn(
                name: "somtoday_student_profile_picture",
                table: "users");

            migrationBuilder.AddColumn<DateTime>(
                name: "zermelo_access_token_expires_at",
                table: "users",
                type: "datetime",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "zermelo_access_token_expires_at",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "infowijs_session_token",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "school_naam_code",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "somtoday_student_profile_picture",
                table: "users",
                type: "text",
                nullable: true);
        }
    }
}
