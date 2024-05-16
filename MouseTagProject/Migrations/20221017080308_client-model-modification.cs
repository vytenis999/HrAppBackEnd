using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MouseTagProject.Migrations
{
    public partial class clientmodelmodification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Project",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Project",
                table: "Clients");
        }
    }
}
