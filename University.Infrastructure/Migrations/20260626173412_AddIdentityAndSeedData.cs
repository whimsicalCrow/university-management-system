using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace University.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "380d0ae7-0f89-4088-b0fc-7f6207d4d43a", "Professor", "PROFESSOR" },
                    { "2", "316e7b3c-8f60-4053-a19b-e5978b762128", "Student", "STUDENT" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "prof-1", 0, "prof-1-concurrency", "prof1@univ.edu", true, false, null, "PROF1@UNIV.EDU", "PROF1@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "prof-1-stamp", false, "prof1@univ.edu" },
                    { "prof-2", 0, "prof-2-concurrency", "prof2@univ.edu", true, false, null, "PROF2@UNIV.EDU", "PROF2@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "prof-2-stamp", false, "prof2@univ.edu" },
                    { "prof-3", 0, "prof-3-concurrency", "prof3@univ.edu", true, false, null, "PROF3@UNIV.EDU", "PROF3@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "prof-3-stamp", false, "prof3@univ.edu" },
                    { "prof-4", 0, "prof-4-concurrency", "prof4@univ.edu", true, false, null, "PROF4@UNIV.EDU", "PROF4@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "prof-4-stamp", false, "prof4@univ.edu" },
                    { "prof-5", 0, "prof-5-concurrency", "prof5@univ.edu", true, false, null, "PROF5@UNIV.EDU", "PROF5@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "prof-5-stamp", false, "prof5@univ.edu" },
                    { "student-1", 0, "student-1-concurrency", "student1@univ.edu", true, false, null, "STUDENT1@UNIV.EDU", "STUDENT1@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-1-stamp", false, "student1@univ.edu" },
                    { "student-10", 0, "student-10-concurrency", "student10@univ.edu", true, false, null, "STUDENT10@UNIV.EDU", "STUDENT10@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-10-stamp", false, "student10@univ.edu" },
                    { "student-11", 0, "student-11-concurrency", "student11@univ.edu", true, false, null, "STUDENT11@UNIV.EDU", "STUDENT11@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-11-stamp", false, "student11@univ.edu" },
                    { "student-12", 0, "student-12-concurrency", "student12@univ.edu", true, false, null, "STUDENT12@UNIV.EDU", "STUDENT12@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-12-stamp", false, "student12@univ.edu" },
                    { "student-13", 0, "student-13-concurrency", "student13@univ.edu", true, false, null, "STUDENT13@UNIV.EDU", "STUDENT13@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-13-stamp", false, "student13@univ.edu" },
                    { "student-14", 0, "student-14-concurrency", "student14@univ.edu", true, false, null, "STUDENT14@UNIV.EDU", "STUDENT14@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-14-stamp", false, "student14@univ.edu" },
                    { "student-15", 0, "student-15-concurrency", "student15@univ.edu", true, false, null, "STUDENT15@UNIV.EDU", "STUDENT15@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-15-stamp", false, "student15@univ.edu" },
                    { "student-2", 0, "student-2-concurrency", "student2@univ.edu", true, false, null, "STUDENT2@UNIV.EDU", "STUDENT2@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-2-stamp", false, "student2@univ.edu" },
                    { "student-3", 0, "student-3-concurrency", "student3@univ.edu", true, false, null, "STUDENT3@UNIV.EDU", "STUDENT3@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-3-stamp", false, "student3@univ.edu" },
                    { "student-4", 0, "student-4-concurrency", "student4@univ.edu", true, false, null, "STUDENT4@UNIV.EDU", "STUDENT4@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-4-stamp", false, "student4@univ.edu" },
                    { "student-5", 0, "student-5-concurrency", "student5@univ.edu", true, false, null, "STUDENT5@UNIV.EDU", "STUDENT5@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-5-stamp", false, "student5@univ.edu" },
                    { "student-6", 0, "student-6-concurrency", "student6@univ.edu", true, false, null, "STUDENT6@UNIV.EDU", "STUDENT6@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-6-stamp", false, "student6@univ.edu" },
                    { "student-7", 0, "student-7-concurrency", "student7@univ.edu", true, false, null, "STUDENT7@UNIV.EDU", "STUDENT7@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-7-stamp", false, "student7@univ.edu" },
                    { "student-8", 0, "student-8-concurrency", "student8@univ.edu", true, false, null, "STUDENT8@UNIV.EDU", "STUDENT8@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-8-stamp", false, "student8@univ.edu" },
                    { "student-9", 0, "student-9-concurrency", "student9@univ.edu", true, false, null, "STUDENT9@UNIV.EDU", "STUDENT9@UNIV.EDU", "AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=", null, false, "student-9-stamp", false, "student9@univ.edu" }
                });

            migrationBuilder.InsertData(
                table: "Professors",
                columns: new[] { "Id", "Department", "Expertise", "UserId" },
                values: new object[,]
                {
                    { 1, "Computer Science", "Machine Learning", "prof-1" },
                    { 2, "Mathematics", "Algebra", "prof-2" },
                    { 3, "Physics", "Quantum", "prof-3" },
                    { 4, "Chemistry", "Organic", "prof-4" },
                    { 5, "Biology", "Genetics", "prof-5" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "prof-1" },
                    { "1", "prof-2" },
                    { "1", "prof-3" },
                    { "1", "prof-4" },
                    { "1", "prof-5" },
                    { "2", "student-1" },
                    { "2", "student-10" },
                    { "2", "student-11" },
                    { "2", "student-12" },
                    { "2", "student-13" },
                    { "2", "student-14" },
                    { "2", "student-15" },
                    { "2", "student-2" },
                    { "2", "student-3" },
                    { "2", "student-4" },
                    { "2", "student-5" },
                    { "2", "student-6" },
                    { "2", "student-7" },
                    { "2", "student-8" },
                    { "2", "student-9" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "EnrollmentDate", "Specialization", "SupervisorId", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "AI", 1, "student-1" },
                    { 2, new DateTime(2025, 8, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Theoretical", 2, "student-2" },
                    { 3, new DateTime(2025, 9, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Nuclear", 3, "student-3" },
                    { 4, new DateTime(2025, 10, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Synthetic", 4, "student-4" },
                    { 5, new DateTime(2025, 11, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Molecular", 5, "student-5" },
                    { 6, new DateTime(2025, 12, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "AI", 1, "student-6" },
                    { 7, new DateTime(2026, 1, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Theoretical", 2, "student-7" },
                    { 8, new DateTime(2026, 2, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Nuclear", 3, "student-8" },
                    { 9, new DateTime(2026, 3, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Synthetic", 4, "student-9" },
                    { 10, new DateTime(2026, 4, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Molecular", 5, "student-10" },
                    { 11, new DateTime(2026, 5, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "AI", 1, "student-11" },
                    { 12, new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Theoretical", 2, "student-12" },
                    { 13, new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Nuclear", 3, "student-13" },
                    { 14, new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Synthetic", 4, "student-14" },
                    { 15, new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), "Molecular", 5, "student-15" }
                });

            migrationBuilder.InsertData(
                table: "Assignments",
                columns: new[] { "Id", "AssignedDate", "ProfessorId", "StudentId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), 1, 1 },
                    { 2, new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), 2, 2 },
                    { 3, new DateTime(2026, 6, 26, 17, 34, 12, 322, DateTimeKind.Utc).AddTicks(6874), 3, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "prof-1" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "prof-2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "prof-3" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "prof-4" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "prof-5" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-1" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-10" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-11" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-12" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-13" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-14" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-15" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-3" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-4" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-5" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-6" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-7" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-8" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "2", "student-9" });

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Assignments",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "prof-1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "prof-2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "prof-3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "prof-4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "prof-5");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-10");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-11");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-12");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-13");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-14");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-15");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-3");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-5");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-6");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-7");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-8");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "student-9");

            migrationBuilder.DeleteData(
                table: "Professors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Professors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Professors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Professors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Professors",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
