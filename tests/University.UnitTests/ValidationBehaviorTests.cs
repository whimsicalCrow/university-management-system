using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using University.Application.Common.Behaviors;

namespace University.UnitTests;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WithNoValidators_ContinuesPipeline()
    {
        var behavior = new ValidationBehavior<TestRequest, string>(Array.Empty<IValidator<TestRequest>>());

        var result = await behavior.Handle(
            new TestRequest("value"),
            () => Task.FromResult("ok"),
            CancellationToken.None);

        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_WithValidationFailures_ThrowsValidationException()
    {
        var validator = new InlineValidator<TestRequest>();
        validator.RuleFor(request => request.Value).NotEmpty();

        var behavior = new ValidationBehavior<TestRequest, string>(new[] { validator });

        await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(
            new TestRequest(string.Empty),
            () => Task.FromResult("ok"),
            CancellationToken.None));
    }

    private sealed record TestRequest(string Value) : IRequest<string>;
}