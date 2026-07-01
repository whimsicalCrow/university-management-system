namespace University.UnitTests.Domain;

using University.Domain.Entities;
using University.Domain.Enums;
using University.Domain.Exceptions;

public class ThesisUpdateTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsThesisUpdateInstance()
    {
        // Arrange
        const int studentId = 1;
        const string content = "Chapter 1 draft completed";

        // Act
        var update = ThesisUpdate.Create(studentId, content);

        // Assert
        Assert.NotNull(update);
        Assert.Equal(studentId, update.StudentId);
        Assert.Equal(content, update.Content);
        Assert.Equal(UpdateStatus.Draft, update.Status);
        Assert.Equal(DateTime.UtcNow.Date, update.SubmittedAt.Date);
    }

    [Fact]
    public void Create_WithZeroStudentId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            ThesisUpdate.Create(0, "content"));
        Assert.Contains("StudentId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithNegativeStudentId_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            ThesisUpdate.Create(-1, "content"));
        Assert.Contains("StudentId must be greater than zero", ex.Message);
    }

    [Fact]
    public void Create_WithNullContent_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            ThesisUpdate.Create(1, null!));
        Assert.Contains("cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithEmptyContent_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            ThesisUpdate.Create(1, ""));
        Assert.Contains("cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithWhitespaceContent_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            ThesisUpdate.Create(1, "   "));
        Assert.Contains("cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Create_WithContentExceedingLimit_ThrowsDomainException()
    {
        // Arrange
        var longContent = new string('x', 10001);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => 
            ThesisUpdate.Create(1, longContent));
        Assert.Contains("cannot exceed 10000 characters", ex.Message);
    }

    [Fact]
    public void Create_WithMaxLengthContent_Succeeds()
    {
        // Arrange
        var maxContent = new string('x', 10000);

        // Act
        var update = ThesisUpdate.Create(1, maxContent);

        // Assert
        Assert.NotNull(update);
        Assert.Equal(10000, update.Content.Length);
    }

    [Fact]
    public void Submit_WhenInDraftStatus_ChangesStatusToSubmitted()
    {
        // Arrange
        var update = ThesisUpdate.Create(1, "content");
        Assert.Equal(UpdateStatus.Draft, update.Status);

        // Act
        update.Submit();

        // Assert
        Assert.Equal(UpdateStatus.Submitted, update.Status);
    }

    [Fact]
    public void Submit_WhenAlreadySubmitted_ThrowsDomainException()
    {
        // Arrange
        var update = ThesisUpdate.Create(1, "content");
        update.Submit();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => update.Submit());
        Assert.Contains("must be in Draft status", ex.Message);
    }

    [Fact]
    public void Submit_WhenReviewed_ThrowsDomainException()
    {
        // Arrange
        var update = ThesisUpdate.Create(1, "content");
        update.Submit();
        update.MarkAsReviewed();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => update.Submit());
        Assert.Contains("must be in Draft status", ex.Message);
    }

    [Fact]
    public void MarkAsReviewed_WhenInSubmittedStatus_ChangesStatusToReviewed()
    {
        // Arrange
        var update = ThesisUpdate.Create(1, "content");
        update.Submit();
        Assert.Equal(UpdateStatus.Submitted, update.Status);

        // Act
        update.MarkAsReviewed();

        // Assert
        Assert.Equal(UpdateStatus.Reviewed, update.Status);
    }

    [Fact]
    public void MarkAsReviewed_WhenInDraftStatus_ThrowsDomainException()
    {
        // Arrange
        var update = ThesisUpdate.Create(1, "content");

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => update.MarkAsReviewed());
        Assert.Contains("must be in Submitted status", ex.Message);
    }

    [Fact]
    public void MarkAsReviewed_WhenAlreadyReviewed_ThrowsDomainException()
    {
        // Arrange
        var update = ThesisUpdate.Create(1, "content");
        update.Submit();
        update.MarkAsReviewed();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => update.MarkAsReviewed());
        Assert.Contains("must be in Submitted status", ex.Message);
    }

    [Fact]
    public void Feedback_IsInitializedAsEmptyCollection()
    {
        // Arrange & Act
        var update = ThesisUpdate.Create(1, "content");

        // Assert
        Assert.NotNull(update.Feedback);
        Assert.Empty(update.Feedback);
    }

    [Fact]
    public void StatusTransition_Draft_To_Submitted_To_Reviewed_Succeeds()
    {
        // Arrange
        var update = ThesisUpdate.Create(1, "content");
        Assert.Equal(UpdateStatus.Draft, update.Status);

        // Act & Assert - First transition
        update.Submit();
        Assert.Equal(UpdateStatus.Submitted, update.Status);

        // Act & Assert - Second transition
        update.MarkAsReviewed();
        Assert.Equal(UpdateStatus.Reviewed, update.Status);
    }
}
