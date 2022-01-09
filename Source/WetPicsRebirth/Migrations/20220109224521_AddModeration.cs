using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace WetPicsRebirth.Migrations
{
    public partial class AddModeration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModeratedMedia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<int>(type: "integer", nullable: false),
                    Hash = table.Column<string>(type: "text", nullable: false),
                    MessageId = table.Column<int>(type: "integer", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    AddedDate = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    ModifiedDate = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeratedMedia", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModeratedMedia_MessageId",
                table: "ModeratedMedia",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ModeratedMedia_PostId_Hash",
                table: "ModeratedMedia",
                columns: new[] { "PostId", "Hash" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModeratedMedia");
        }
    }
}
