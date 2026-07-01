namespace University.UnitTests.Domain;

using University.Domain.Entities;
using University.Domain.Exceptions;

public class ProfessorTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsProfessorInstance()
    {
        // Arrange
        const string userId = "prof001";
        const string department = "Computer Science";
        const string expertise = "Machine Learning";

        // Act
        var professor = Professor.Create(userId, department, expertise);

        // Assert
        Assert.NotNull(professor);
        Assert.Equal(userId, professor.UserId);
        Assert.Equal(department, professor.Department);
        Assert.Equal(expertise, professor.Expertise);
    }

    [Fact]
    public void Create_WithNullUserId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Professor.Create(null!, "CS", "ML"));
        Assert.Contains("UserId cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithEmptyDepartment_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Professor.Create("prof001", "", "ML"));
        Assert.Contains("Department cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithNullExpertise_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Professor.Create("prof001", "CS", null!));
        Assert.Contains("Expertise cannot be null or empty", ex.Message);
    }

    [Fact]
    public void ProvideFeedback_WithValidParameters_CreatesFeedbackAndAddsToCollection()
    {
        // Arrange
        var professor = Professor.Create("prof001", "CS", "ML");
        professor.Id = 1;
        const int updateId = 5;
        const string comment = "Great work on the thesis framework";

        // Act
        var feedback = professor.ProvideFeedback(updateId, comment);

        // Assert
        Assert.NotNull(feedback);
        Assert.Equal(updateId, feedback.UpdateId);
        Assert.Equal(professor.Id, feedback.ProfessorId);
        Assert.Equal(comment, feedback.Comment);
        Assert.Single(professor.FeedbackProvided);
    }

    [Fact]
    public void ProvideFeedback_WithZeroUpdateId_ThrowsDomainException()
    {
        // Arrange
        var professor = Professor.Create("prof001", "CS", "ML");
        professor.Id = 1;

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            professor.ProvideFeedback(0, "comment"));
        Assert.Contains("UpdateId must be greater than zero", ex.Message);
    }

    [Fact]
    public void ProvideFeedback_WithNegativeUpdateId_ThrowsDomainException()
    {
        // Arrange
        var professor = Professor.Create("prof001", "CS", "ML");

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            professor.ProvideFeedback(-1, "comment"));
        Assert.Contains("UpdateId must be greater than zero", ex.Message);
    }

    [Fact]
    public void ProvideFeedback_WithEmptyComment_ThrowsDomainException()
    {
        // Arrange
        var professor = Professor.Create("prof001", "CS", "ML");
        professor.Id = 1;

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            professor.ProvideFeedback(1, ""));
        Assert.Contains("cannot be null or empty", ex.Message);
    }

    [Fact]
    public void ProvideFeedback_WithCommentExceedingLimit_ThrowsDomainException()
    {
        // Arrange
        var professor = Professor.Create("prof001", "CS", "ML");
        professor.Id = 1;
        var longComment = new string('x', 5001);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            professor.ProvideFeedback(1, longComment));
        Assert.Contains("cannot exceed 5000 characters", ex.Message);
    }

    [Fact]
    public void ProvideFeedback_WithMaxLengthComment_Succeeds()
    {
        // Arrange
        var professor = Professor.Create("prof001", "CS", "ML");
        professor.Id = 1;
        var maxComment = new string('x', 5000);

        // Act
        var feedback = professor.ProvideFeedback(1, maxComment);

        // Assert
        Assert.NotNull(feedback);
        Assert.Equal(5000, feedback.Comment.Length);
    }

    [Fact]
    public void AssignedStudents_IsInitializedAsEmptyCollection()
    {
        // Arrange & Act
        var professor = Professor.Create("prof001", "CS", "ML");

        // Assert
        Assert.NotNull(professor.AssignedStudents);
        Assert.Empty(professor.AssignedStudents);
    }

    [Fact]
    public void FeedbackProvided_IsInitializedAsEmptyCollection()
    {
        // Arrange & Act
        var professor = Professor.Create("prof001", "CS", "ML");

        // Assert
        Assert.NotNull(professor.FeedbackProvided);
        Assert.Empty(professor.FeedbackProvided);
    }
}
