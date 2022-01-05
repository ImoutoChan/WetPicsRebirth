using Microsoft.EntityFrameworkCore.Migrations;

namespace WetPicsRebirth.Migrations;

public partial class ChangeActressPk : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_PostedMedia_Actresses_ChatId_ImageSource",
            table: "PostedMedia");

        migrationBuilder.DropIndex(
            name: "IX_PostedMedia_ChatId_ImageSource",
            table: "PostedMedia");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Actresses",
            table: "Actresses");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Actresses",
            table: "Actresses",
            column: "Id");

        migrationBuilder.CreateIndex(
            name: "IX_Actresses_ChatId",
            table: "Actresses",
            column: "ChatId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_Actresses",
            table: "Actresses");

        migrationBuilder.DropIndex(
            name: "IX_Actresses_ChatId",
            table: "Actresses");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Actresses",
            table: "Actresses",
            columns: new[] { "ChatId", "ImageSource" });

        migrationBuilder.CreateIndex(
            name: "IX_PostedMedia_ChatId_ImageSource",
            table: "PostedMedia",
            columns: new[] { "ChatId", "ImageSource" });

        migrationBuilder.AddForeignKey(
            name: "FK_PostedMedia_Actresses_ChatId_ImageSource",
            table: "PostedMedia",
            columns: new[] { "ChatId", "ImageSource" },
            principalTable: "Actresses",
            principalColumns: new[] { "ChatId", "ImageSource" },
            onDelete: ReferentialAction.Cascade);
    }
}