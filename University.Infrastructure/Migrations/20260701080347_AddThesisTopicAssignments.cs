using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace University.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThesisTopicAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ThesisTopicAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    ProfessorId = table.Column<int>(type: "int", nullable: false),
                    AssignedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesisTopicAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThesisTopicAssignments_Professors_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThesisTopicAssignments_Students_StudentId",
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
                value: "cba1039a-88c5-4279-9421-73bdbd0cac69");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "ea599a34-de65-4f11-b7fb-3eefe8b0f3bb");

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 2,
                column: "AssignedDate",
                value: new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 3,
                column: "AssignedDate",
                value: new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                column: "EnrollmentDate",
                value: new DateTime(2025, 7, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                column: "EnrollmentDate",
                value: new DateTime(2025, 8, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3,
                column: "EnrollmentDate",
                value: new DateTime(2025, 9, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4,
                column: "EnrollmentDate",
                value: new DateTime(2025, 10, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5,
                column: "EnrollmentDate",
                value: new DateTime(2025, 11, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 6,
                column: "EnrollmentDate",
                value: new DateTime(2025, 12, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 7,
                column: "EnrollmentDate",
                value: new DateTime(2026, 1, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 8,
                column: "EnrollmentDate",
                value: new DateTime(2026, 2, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 9,
                column: "EnrollmentDate",
                value: new DateTime(2026, 3, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 10,
                column: "EnrollmentDate",
                value: new DateTime(2026, 4, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 11,
                column: "EnrollmentDate",
                value: new DateTime(2026, 5, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 12,
                column: "EnrollmentDate",
                value: new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 13,
                column: "EnrollmentDate",
                value: new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 14,
                column: "EnrollmentDate",
                value: new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 15,
                column: "EnrollmentDate",
                value: new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_ThesisTopicAssignments_ProfessorId",
                table: "ThesisTopicAssignments",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisTopicAssignments_StudentId",
                table: "ThesisTopicAssignments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisTopicAssignments_TopicId",
                table: "ThesisTopicAssignments",
                column: "TopicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThesisTopicAssignments");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "380d0ae7-0f89-4088-b0fc-7f6207d4d43a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "316e7b3c-8f60-4053-a19b-e5978b762128");

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssignedDate",
                value: new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 2,
                column: "AssignedDate",
                value: new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 3,
                column: "AssignedDate",
                value: new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                column: "EnrollmentDate",
                value: new DateTime(2025, 7, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2,
                column: "EnrollmentDate",
                value: new DateTime(2025, 8, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3,
                column: "EnrollmentDate",
                value: new DateTime(2025, 9, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4,
                column: "EnrollmentDate",
                value: new DateTime(2025, 10, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5,
                column: "EnrollmentDate",
                value: new DateTime(2025, 11, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 6,
                column: "EnrollmentDate",
                value: new DateTime(2025, 12, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 7,
                column: "EnrollmentDate",
                value: new DateTime(2026, 1, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 8,
                column: "EnrollmentDate",
                value: new DateTime(2026, 2, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 9,
                column: "EnrollmentDate",
                value: new DateTime(2026, 3, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 10,
                column: "EnrollmentDate",
                value: new DateTime(2026, 4, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 11,
                column: "EnrollmentDate",
                value: new DateTime(2026, 5, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 12,
                column: "EnrollmentDate",
                value: new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 13,
                column: "EnrollmentDate",
                value: new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 14,
                column: "EnrollmentDate",
                value: new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 15,
                column: "EnrollmentDate",
                value: new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874));
        }
    }
}
