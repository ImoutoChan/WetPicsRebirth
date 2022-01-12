using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WetPicsRebirth.Migrations
{
    public partial class AddIndexForPostedMedia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PostedMedia_ChatId_MessageId",
                table: "PostedMedia",
                columns: new[] { "ChatId", "MessageId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostedMedia_ChatId_MessageId",
                table: "PostedMedia");
        }
    }
}
