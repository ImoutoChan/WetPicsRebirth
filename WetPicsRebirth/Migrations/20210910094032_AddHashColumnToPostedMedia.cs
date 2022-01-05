using Microsoft.EntityFrameworkCore.Migrations;

namespace WetPicsRebirth.Migrations;

public partial class AddHashColumnToPostedMedia : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PostHash",
            table: "PostedMedia",
            type: "text",
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PostHash",
            table: "PostedMedia");
    }
}