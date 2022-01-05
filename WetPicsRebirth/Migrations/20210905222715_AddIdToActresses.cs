using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WetPicsRebirth.Migrations;

public partial class AddIdToActresses : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "Id",
            table: "Actresses",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Id",
            table: "Actresses");
    }
}