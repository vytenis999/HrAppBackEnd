using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MouseTagProject.Migrations
{
    public partial class addedclientnormalized : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Normalized",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Normalized",
                table: "Clients");
        }
    }
}
