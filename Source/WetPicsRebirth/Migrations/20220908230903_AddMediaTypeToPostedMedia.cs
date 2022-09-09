using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WetPicsRebirth.Migrations
{
    public partial class AddMediaTypeToPostedMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileType",
                table: "PostedMedia",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "PostedMedia");
        }
    }
}
