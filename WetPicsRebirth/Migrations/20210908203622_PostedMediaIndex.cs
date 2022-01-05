using Microsoft.EntityFrameworkCore.Migrations;

namespace WetPicsRebirth.Migrations;

public partial class PostedMediaIndex : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_PostedMedia_ChatId_ImageSource_PostId",
            table: "PostedMedia",
            columns: new[] { "ChatId", "ImageSource", "PostId" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_PostedMedia_ChatId_ImageSource_PostId",
            table: "PostedMedia");
    }
}