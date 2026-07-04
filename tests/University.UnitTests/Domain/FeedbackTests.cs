namespace University.UnitTests.Domain;

using University.Domain.Entities;
using University.Domain.Exceptions;

public class FeedbackTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsFeedbackInstance()
    {
        // Arrange
        const int updateId = 1;
        const int professorId = 5;
        const string comment = "Excellent thesis proposal";

        // Act
        var feedback = Feedback.Create(updateId, professorId, comment);

        // Assert
        Assert.NotNull(feedback);
        Assert.Equal(updateId, feedback.UpdateId);
        Assert.Equal(professorId, feedback.ProfessorId);
        Assert.Equal(comment, feedback.Comment);
        Assert.Equal(DateTime.UtcNow.Date, feedback.SubmittedAt.Date);
    }

    [Fact]
    public void Create_WithZeroUpdateId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(0, 5, "comment"));
        Assert.Contains("UpdateId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithNegativeUpdateId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(-1, 5, "comment"));
        Assert.Contains("UpdateId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithZeroProfessorId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(1, 0, "comment"));
        Assert.Contains("ProfessorId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithNegativeProfessorId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(1, -5, "comment"));
        Assert.Contains("ProfessorId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithNullComment_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(1, 5, null!));
        Assert.Contains("cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithEmptyComment_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(1, 5, ""));
        Assert.Contains("cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithWhitespaceComment_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(1, 5, "   "));
        Assert.Contains("cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithCommentExceedingLimit_ThrowsDomainException()
    {
        // Arrange
        var longComment = new string('x', 5001);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(1, 5, longComment));
        Assert.Contains("cannot exceed 5000 characters", ex.Message);
    }

    [Fact]
    public void Create_WithMaxLengthComment_Succeeds()
    {
        // Arrange
        var maxComment = new string('x', 5000);

        // Act
        var feedback = Feedback.Create(1, 5, maxComment);

        // Assert
        Assert.NotNull(feedback);
        Assert.Equal(5000, feedback.Comment.Length);
    }

    [Fact]
    public void NavigationProperties_AreInitializedAsNull()
    {
        // Arrange & Act
        var feedback = Feedback.Create(1, 5, "comment");

        // Assert
        Assert.Null(feedback.Update);
        Assert.Null(feedback.Professor);
    }

    [Fact]
    public void Create_WithBothInvalidIds_ThrowsDomainExceptionForUpdateIdFirst()
    {
        // Act & Assert (UpdateId validation happens first)
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(0, 0, "comment"));
        Assert.Contains("UpdateId", ex.Message);
    }

    [Fact]
    public void Create_ValidatesUpdateIdBeforeProfessorId()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(0, 5, "comment"));
        Assert.Contains("UpdateId", ex.Message);
    }

    [Fact]
    public void Create_ValidatesProfessorIdBeforeComment()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            Feedback.Create(1, 0, "comment"));
        Assert.Contains("ProfessorId", ex.Message);
    }
}
