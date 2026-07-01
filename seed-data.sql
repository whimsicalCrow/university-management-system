-- Seed Identity Roles
INSERT INTO [dbo].[AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
VALUES 
  ('1', '380d0ae7-0f89-4088-b0fc-7f6207d4d43a', 'Professor', 'PROFESSOR'),
  ('2', '316e7b3c-8f60-4053-a19b-e5978b762128', 'Student', 'STUDENT');

-- Seed AspNetUsers (5 Professors + 15 Students)
INSERT INTO [dbo].[AspNetUsers] ([Id], [AccessFailedCount], [ConcurrencyStamp], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
VALUES 
  ('prof-1', 0, 'prof-1-concurrency', 'prof1@univ.edu', 1, 0, NULL, 'PROF1@UNIV.EDU', 'PROF1@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'prof-1-stamp', 0, 'prof1@univ.edu'),
  ('prof-2', 0, 'prof-2-concurrency', 'prof2@univ.edu', 1, 0, NULL, 'PROF2@UNIV.EDU', 'PROF2@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'prof-2-stamp', 0, 'prof2@univ.edu'),
  ('prof-3', 0, 'prof-3-concurrency', 'prof3@univ.edu', 1, 0, NULL, 'PROF3@UNIV.EDU', 'PROF3@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'prof-3-stamp', 0, 'prof3@univ.edu'),
  ('prof-4', 0, 'prof-4-concurrency', 'prof4@univ.edu', 1, 0, NULL, 'PROF4@UNIV.EDU', 'PROF4@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'prof-4-stamp', 0, 'prof4@univ.edu'),
  ('prof-5', 0, 'prof-5-concurrency', 'prof5@univ.edu', 1, 0, NULL, 'PROF5@UNIV.EDU', 'PROF5@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'prof-5-stamp', 0, 'prof5@univ.edu'),
  ('student-1', 0, 'student-1-concurrency', 'student1@univ.edu', 1, 0, NULL, 'STUDENT1@UNIV.EDU', 'STUDENT1@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-1-stamp', 0, 'student1@univ.edu'),
  ('student-2', 0, 'student-2-concurrency', 'student2@univ.edu', 1, 0, NULL, 'STUDENT2@UNIV.EDU', 'STUDENT2@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-2-stamp', 0, 'student2@univ.edu'),
  ('student-3', 0, 'student-3-concurrency', 'student3@univ.edu', 1, 0, NULL, 'STUDENT3@UNIV.EDU', 'STUDENT3@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-3-stamp', 0, 'student3@univ.edu'),
  ('student-4', 0, 'student-4-concurrency', 'student4@univ.edu', 1, 0, NULL, 'STUDENT4@UNIV.EDU', 'STUDENT4@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-4-stamp', 0, 'student4@univ.edu'),
  ('student-5', 0, 'student-5-concurrency', 'student5@univ.edu', 1, 0, NULL, 'STUDENT5@UNIV.EDU', 'STUDENT5@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-5-stamp', 0, 'student5@univ.edu'),
  ('student-6', 0, 'student-6-concurrency', 'student6@univ.edu', 1, 0, NULL, 'STUDENT6@UNIV.EDU', 'STUDENT6@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-6-stamp', 0, 'student6@univ.edu'),
  ('student-7', 0, 'student-7-concurrency', 'student7@univ.edu', 1, 0, NULL, 'STUDENT7@UNIV.EDU', 'STUDENT7@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-7-stamp', 0, 'student7@univ.edu'),
  ('student-8', 0, 'student-8-concurrency', 'student8@univ.edu', 1, 0, NULL, 'STUDENT8@UNIV.EDU', 'STUDENT8@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-8-stamp', 0, 'student8@univ.edu'),
  ('student-9', 0, 'student-9-concurrency', 'student9@univ.edu', 1, 0, NULL, 'STUDENT9@UNIV.EDU', 'STUDENT9@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-9-stamp', 0, 'student9@univ.edu'),
  ('student-10', 0, 'student-10-concurrency', 'student10@univ.edu', 1, 0, NULL, 'STUDENT10@UNIV.EDU', 'STUDENT10@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-10-stamp', 0, 'student10@univ.edu'),
  ('student-11', 0, 'student-11-concurrency', 'student11@univ.edu', 1, 0, NULL, 'STUDENT11@UNIV.EDU', 'STUDENT11@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-11-stamp', 0, 'student11@univ.edu'),
  ('student-12', 0, 'student-12-concurrency', 'student12@univ.edu', 1, 0, NULL, 'STUDENT12@UNIV.EDU', 'STUDENT12@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-12-stamp', 0, 'student12@univ.edu'),
  ('student-13', 0, 'student-13-concurrency', 'student13@univ.edu', 1, 0, NULL, 'STUDENT13@UNIV.EDU', 'STUDENT13@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-13-stamp', 0, 'student13@univ.edu'),
  ('student-14', 0, 'student-14-concurrency', 'student14@univ.edu', 1, 0, NULL, 'STUDENT14@UNIV.EDU', 'STUDENT14@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-14-stamp', 0, 'student14@univ.edu'),
  ('student-15', 0, 'student-15-concurrency', 'student15@univ.edu', 1, 0, NULL, 'STUDENT15@UNIV.EDU', 'STUDENT15@UNIV.EDU', 'AQAAAAIAAYagAAAAECm7pNz7z7vqR0Z5vX2Z7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W5mZ7W=', NULL, 0, 'student-15-stamp', 0, 'student15@univ.edu');

-- Seed Professors
INSERT INTO [dbo].[Professors] ([Id], [Department], [Expertise], [UserId])
VALUES 
  (1, 'Computer Science', 'Machine Learning', 'prof-1'),
  (2, 'Mathematics', 'Algebra', 'prof-2'),
  (3, 'Physics', 'Quantum', 'prof-3'),
  (4, 'Chemistry', 'Organic', 'prof-4'),
  (5, 'Biology', 'Genetics', 'prof-5');

-- Seed AspNetUserRoles (5 Professors + 15 Students)
INSERT INTO [dbo].[AspNetUserRoles] ([RoleId], [UserId])
VALUES 
  ('1', 'prof-1'),
  ('1', 'prof-2'),
  ('1', 'prof-3'),
  ('1', 'prof-4'),
  ('1', 'prof-5'),
  ('2', 'student-1'),
  ('2', 'student-2'),
  ('2', 'student-3'),
  ('2', 'student-4'),
  ('2', 'student-5'),
  ('2', 'student-6'),
  ('2', 'student-7'),
  ('2', 'student-8'),
  ('2', 'student-9'),
  ('2', 'student-10'),
  ('2', 'student-11'),
  ('2', 'student-12'),
  ('2', 'student-13'),
  ('2', 'student-14'),
  ('2', 'student-15');

-- Seed Students
INSERT INTO [dbo].[Students] ([Id], [UserId], [Specialization], [EnrollmentDate], [SupervisorId])
VALUES 
  (1, 'student-1', 'Computer Science', '2026-06-26', 1),
  (2, 'student-2', 'Computer Science', '2026-06-26', 1),
  (3, 'student-3', 'Computer Science', '2026-06-26', 1),
  (4, 'student-4', 'Mathematics', '2026-06-26', 2),
  (5, 'student-5', 'Mathematics', '2026-06-26', 2),
  (6, 'student-6', 'Mathematics', '2026-06-26', 2),
  (7, 'student-7', 'Physics', '2026-06-26', 3),
  (8, 'student-8', 'Physics', '2026-06-26', 3),
  (9, 'student-9', 'Physics', '2026-06-26', 3),
  (10, 'student-10', 'Chemistry', '2026-06-26', 4),
  (11, 'student-11', 'Chemistry', '2026-06-26', 4),
  (12, 'student-12', 'Chemistry', '2026-06-26', 4),
  (13, 'student-13', 'Biology', '2026-06-26', 5),
  (14, 'student-14', 'Biology', '2026-06-26', 5),
  (15, 'student-15', 'Biology', '2026-06-26', 5);

-- Seed Assignments (3 sample)
INSERT INTO [dbo].[Assignments] ([StudentId], [ProfessorId], [Title], [DueDate], [CreatedDate])
VALUES 
  (1, 1, 'Thesis Proposal', '2026-07-31', '2026-06-26'),
  (2, 1, 'Literature Review', '2026-07-31', '2026-06-26'),
  (3, 1, 'Methodology', '2026-07-31', '2026-06-26');

-- Seed EFMigrationsHistory  
INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES 
  ('20260626161745_InitialCreate', '10.0.0'),
  ('20260626173412_AddIdentityAndSeedData', '10.0.0');
