using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WetPicsRebirth.Migrations;

public partial class ChangeVoteFk : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Votes_PostedMedia_PostedMediaId",
            table: "Votes");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Votes",
            table: "Votes");

        migrationBuilder.DropIndex(
            name: "IX_Votes_PostedMediaId",
            table: "Votes");

        migrationBuilder.DropColumn(
            name: "PostedMediaId",
            table: "Votes");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Votes",
            table: "Votes",
            columns: new[] { "ChatId", "MessageId", "UserId" });

        migrationBuilder.AddUniqueConstraint(
            name: "AK_PostedMedia_ChatId_MessageId",
            table: "PostedMedia",
            columns: new[] { "ChatId", "MessageId" });

        migrationBuilder.CreateIndex(
            name: "IX_Votes_UserId",
            table: "Votes",
            column: "UserId");

        migrationBuilder.AddForeignKey(
            name: "FK_Votes_PostedMedia_ChatId_MessageId",
            table: "Votes",
            columns: new[] { "ChatId", "MessageId" },
            principalTable: "PostedMedia",
            principalColumns: new[] { "ChatId", "MessageId" },
            onDelete: ReferentialAction.Cascade);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Votes_PostedMedia_ChatId_MessageId",
            table: "Votes");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Votes",
            table: "Votes");

        migrationBuilder.DropIndex(
            name: "IX_Votes_UserId",
            table: "Votes");

        migrationBuilder.DropUniqueConstraint(
            name: "AK_PostedMedia_ChatId_MessageId",
            table: "PostedMedia");

        migrationBuilder.AddColumn<Guid>(
            name: "PostedMediaId",
            table: "Votes",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddPrimaryKey(
            name: "PK_Votes",
            table: "Votes",
            columns: new[] { "UserId", "PostedMediaId" });

        migrationBuilder.CreateIndex(
            name: "IX_Votes_PostedMediaId",
            table: "Votes",
            column: "PostedMediaId");

        migrationBuilder.AddForeignKey(
            name: "FK_Votes_PostedMedia_PostedMediaId",
            table: "Votes",
            column: "PostedMediaId",
            principalTable: "PostedMedia",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}