using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    uuid = table.Column<string>(type: "varchar(767)", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    school_id = table.Column<string>(type: "text", nullable: true),
                    school_naam_code = table.Column<string>(type: "text", nullable: true),
                    zermelo_access_token = table.Column<string>(type: "text", nullable: true),
                    somtoday_access_token = table.Column<string>(type: "text", nullable: true),
                    somtoday_refresh_token = table.Column<string>(type: "text", nullable: true),
                    somtoday_student_id = table.Column<string>(type: "text", nullable: true),
                    infowijs_access_token = table.Column<string>(type: "text", nullable: true),
                    infowijs_session_token = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.uuid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
