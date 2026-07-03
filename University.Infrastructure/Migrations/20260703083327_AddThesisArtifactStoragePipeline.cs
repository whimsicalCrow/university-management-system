using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThesisArtifactStoragePipeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Data",
                table: "ThesisArtifacts",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AddColumn<int>(
                name: "ScanStatus",
                table: "ThesisArtifacts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "ThesisArtifacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedByUserId",
                table: "ThesisArtifacts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "e4d85104-57ed-40fe-8bbc-c687d2d4bf32");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "9fb112b7-dfd0-4ac0-9b30-cee27e963432");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScanStatus",
                table: "ThesisArtifacts");

            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "ThesisArtifacts");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "ThesisArtifacts");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Data",
                table: "ThesisArtifacts",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0],
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "d9338a47-42d6-41ca-b8d7-0e40a555b252");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "81a2a4ef-b0b0-40bb-b771-2e8c519a6c90");
        }
    }
}
