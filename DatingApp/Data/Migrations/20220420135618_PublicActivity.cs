using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatingApp.Data.Migrations
{
    public partial class PublicActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublicActivity",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicActivity",
                table: "AspNetUsers");
        }
    }
}
