namespace University.UnitTests.Domain;

using University.Domain.Entities;
using University.Domain.Exceptions;

public class StudentTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsStudentInstance()
    {
        // Arrange
        const string userId = "user123";
        const string specialization = "AI";

        // Act
        var student = Student.Create(userId, specialization);

        // Assert
        Assert.NotNull(student);
        Assert.Equal(userId, student.UserId);
        Assert.Equal(specialization, student.Specialization);
        Assert.Null(student.SupervisorId);
        Assert.Equal(DateTime.UtcNow.Date, student.EnrollmentDate.Date);
    }

    [Fact]
    public void Create_WithNullUserId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => Student.Create(null!, "AI"));
        Assert.Contains("UserId cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithEmptyUserId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => Student.Create("", "AI"));
        Assert.Contains("UserId cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithWhitespaceUserId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => Student.Create("   ", "AI"));
        Assert.Contains("UserId cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithNullSpecialization_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => Student.Create("user123", null!));
        Assert.Contains("Specialization cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithEmptySpecialization_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => Student.Create("user123", ""));
        Assert.Contains("Specialization cannot be null or empty", ex.Message);
    }

    [Fact]
    public void AssignSupervisor_WithValidProfessorId_SucceedsAndSetsSupervisorId()
    {
        // Arrange
        var student = Student.Create("user123", "AI");
        const int professorId = 5;

        // Act
        student.AssignSupervisor(professorId);

        // Assert
        Assert.Equal(professorId, student.SupervisorId);
    }

    [Fact]
    public void AssignSupervisor_WhenAlreadyAssigned_ThrowsDomainException()
    {
        // Arrange
        var student = Student.Create("user123", "AI");
        student.AssignSupervisor(1);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => student.AssignSupervisor(2));
        Assert.Contains("already has a supervisor", ex.Message);
    }

    [Fact]
    public void AssignSupervisor_WithZeroProfessorId_ThrowsDomainException()
    {
        // Arrange
        var student = Student.Create("user123", "AI");

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => student.AssignSupervisor(0));
        Assert.Contains("must be greater than zero", ex.Message);
    }

    [Fact]
    public void AssignSupervisor_WithNegativeProfessorId_ThrowsDomainException()
    {
        // Arrange
        var student = Student.Create("user123", "AI");

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => student.AssignSupervisor(-1));
        Assert.Contains("must be greater than zero", ex.Message);
    }

    [Fact]
    public void RemoveSupervisor_WhenAssigned_ClearsSupervisorId()
    {
        // Arrange
        var student = Student.Create("user123", "AI");
        student.AssignSupervisor(1);

        // Act
        student.RemoveSupervisor();

        // Assert
        Assert.Null(student.SupervisorId);
        Assert.Null(student.Supervisor);
    }

    [Fact]
    public void SubmitUpdate_WithValidContent_CreatesThesisUpdateAndAddsToCollection()
    {
        // Arrange
        var student = Student.Create("user123", "AI");
        student.Id = 1;
        const string content = "Initial thesis framework";

        // Act
        var update = student.SubmitUpdate(content);

        // Assert
        Assert.NotNull(update);
        Assert.Equal(content, update.Content);
        Assert.Equal(student.Id, update.StudentId);
        Assert.Single(student.ThesisUpdates);
    }

    [Fact]
    public void SubmitUpdate_WithEmptyContent_ThrowsDomainException()
    {
        // Arrange
        var student = Student.Create("user123", "AI");
        student.Id = 1;

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => student.SubmitUpdate(""));
        Assert.Contains("cannot be null or empty", ex.Message);
    }

    [Fact]
    public void SubmitUpdate_WithContentExceedingLimit_ThrowsDomainException()
    {
        // Arrange
        var student = Student.Create("user123", "AI");
        student.Id = 1;
        var longContent = new string('x', 10001);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => student.SubmitUpdate(longContent));
        Assert.Contains("cannot exceed 10000 characters", ex.Message);
    }

    [Fact]
    public void SubmitUpdate_WithValidMaxLengthContent_Succeeds()
    {
        // Arrange
        var student = Student.Create("user123", "AI");
        student.Id = 1;
        var maxContent = new string('x', 10000);

        // Act
        var update = student.SubmitUpdate(maxContent);

        // Assert
        Assert.NotNull(update);
        Assert.Equal(10000, update.Content.Length);
    }

    [Fact]
    public void ThesisUpdates_IsInitializedAsEmptyCollection()
    {
        // Arrange & Act
        var student = Student.Create("user123", "AI");

        // Assert
        Assert.NotNull(student.ThesisUpdates);
        Assert.Empty(student.ThesisUpdates);
    }
}
