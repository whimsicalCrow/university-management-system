using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace University.Infrastructure.Persistence.Migrations;

public partial class AddThesisUpdateStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "LastModifiedOn",
            table: "ThesisUpdate",
            type: "datetime2",
            precision: 0,
            nullable: false,
            defaultValueSql: "SYSUTCDATETIME()");

        migrationBuilder.AddColumn<string>(
            name: "Status",
            table: "ThesisUpdate",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "Draft");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "LastModifiedOn",
            table: "ThesisUpdate");

        migrationBuilder.DropColumn(
            name: "Status",
            table: "ThesisUpdate");
    }
}