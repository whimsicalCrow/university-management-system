using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThesisArtifactBinaryStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThesisArtifacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtifactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThesisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesisArtifacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThesisArtifacts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ThesisArtifacts_ArtifactId",
                table: "ThesisArtifacts",
                column: "ArtifactId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThesisArtifacts_StudentId",
                table: "ThesisArtifacts",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisArtifacts_ThesisId",
                table: "ThesisArtifacts",
                column: "ThesisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThesisArtifacts");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "e5a294fd-abcb-4a1c-962c-8a61ed9214d4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "bfc55cfe-7ccf-44d8-9250-d883c9da8d0c");
        }
    }
}
