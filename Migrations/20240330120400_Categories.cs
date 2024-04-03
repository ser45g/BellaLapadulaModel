using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultipleUserLoginForm.Migrations
{
    public partial class Categories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SecurityCategory",
                table: "Subjects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecurityCategory",
                table: "Objects",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityCategory",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SecurityCategory",
                table: "Objects");
        }
    }
}
