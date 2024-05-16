using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MouseTagProject.Migrations
{
    public partial class toclientaddedwillBeContacted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "WillBeContacted",
                table: "Clients",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WillBeContacted",
                table: "Clients");
        }
    }
}
