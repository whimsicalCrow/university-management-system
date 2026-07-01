namespace University.UnitTests.Domain;

using University.Domain.Entities;
using University.Domain.Exceptions;

public class AssignmentTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsAssignmentInstance()
    {
        // Arrange
        const int studentId = 1;
        const int professorId = 5;

        // Act
        var assignment = Assignment.Create(studentId, professorId);

        // Assert
        Assert.NotNull(assignment);
        Assert.Equal(studentId, assignment.StudentId);
        Assert.Equal(professorId, assignment.ProfessorId);
        Assert.Equal(DateTime.UtcNow.Date, assignment.AssignedDate.Date);
    }

    [Fact]
    public void Create_WithZeroStudentId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Assignment.Create(0, 5));
        Assert.Contains("StudentId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithNegativeStudentId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Assignment.Create(-1, 5));
        Assert.Contains("StudentId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithZeroProfessorId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Assignment.Create(1, 0));
        Assert.Contains("ProfessorId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithNegativeProfessorId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Assignment.Create(1, -5));
        Assert.Contains("ProfessorId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithBothInvalidIds_ThrowsDomainExceptionForStudentIdFirst()
    {
        // Act & Assert (StudentId validation happens first)
        var ex = Assert.Throws<DomainException>(() => 
            Assignment.Create(0, 0));
        Assert.Contains("StudentId", ex.Message);
    }

    [Fact]
    public void NavigationProperties_AreInitializedAsNull()
    {
        // Arrange & Act
        var assignment = Assignment.Create(1, 5);

        // Assert
        Assert.Null(assignment.Student);
        Assert.Null(assignment.Professor);
    }
}
