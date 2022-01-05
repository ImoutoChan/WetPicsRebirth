using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WetPicsRebirth.Migrations;

public partial class Init : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Scenes",
            columns: table => new
            {
                ChatId = table.Column<long>(type: "bigint", nullable: false),
                MinutesInterval = table.Column<int>(type: "integer", nullable: false),
                LastPostedTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                Enabled = table.Column<bool>(type: "boolean", nullable: false),
                AddedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Scenes", x => x.ChatId);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                FirstName = table.Column<string>(type: "text", nullable: false),
                LastName = table.Column<string>(type: "text", nullable: false),
                Username = table.Column<string>(type: "text", nullable: false),
                AddedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Actresses",
            columns: table => new
            {
                ChatId = table.Column<long>(type: "bigint", nullable: false),
                ImageSource = table.Column<int>(type: "integer", nullable: false),
                Options = table.Column<string>(type: "text", nullable: false),
                AddedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Actresses", x => new { x.ChatId, x.ImageSource });
                table.ForeignKey(
                    name: "FK_Actresses_Scenes_ChatId",
                    column: x => x.ChatId,
                    principalTable: "Scenes",
                    principalColumn: "ChatId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PostedMedia",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ChatId = table.Column<long>(type: "bigint", nullable: false),
                MessageId = table.Column<int>(type: "integer", nullable: false),
                FileId = table.Column<string>(type: "text", nullable: false),
                ImageSource = table.Column<int>(type: "integer", nullable: false),
                PostId = table.Column<int>(type: "integer", nullable: false),
                AddedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PostedMedia", x => x.Id);
                table.ForeignKey(
                    name: "FK_PostedMedia_Actresses_ChatId_ImageSource",
                    columns: x => new { x.ChatId, x.ImageSource },
                    principalTable: "Actresses",
                    principalColumns: new[] { "ChatId", "ImageSource" },
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Votes",
            columns: table => new
            {
                UserId = table.Column<int>(type: "integer", nullable: false),
                PostedMediaId = table.Column<Guid>(type: "uuid", nullable: false),
                ChatId = table.Column<long>(type: "bigint", nullable: false),
                MessageId = table.Column<int>(type: "integer", nullable: false),
                AddedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                ModifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Votes", x => new { x.UserId, x.PostedMediaId });
                table.ForeignKey(
                    name: "FK_Votes_PostedMedia_PostedMediaId",
                    column: x => x.PostedMediaId,
                    principalTable: "PostedMedia",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Votes_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_PostedMedia_ChatId_ImageSource",
            table: "PostedMedia",
            columns: new[] { "ChatId", "ImageSource" });

        migrationBuilder.CreateIndex(
            name: "IX_Votes_PostedMediaId",
            table: "Votes",
            column: "PostedMediaId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Votes");

        migrationBuilder.DropTable(
            name: "PostedMedia");

        migrationBuilder.DropTable(
            name: "Users");

        migrationBuilder.DropTable(
            name: "Actresses");

        migrationBuilder.DropTable(
            name: "Scenes");
    }
}
