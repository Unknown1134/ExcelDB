using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelDatabase.Migrations
{
    public partial class ClientClassAdded4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isNumberValid",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isNumberValid",
                table: "Clients");
        }
    }
}
