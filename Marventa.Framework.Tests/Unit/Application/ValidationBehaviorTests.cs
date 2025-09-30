using FluentValidation;
using FluentValidation.Results;
using Marventa.Framework.Application.Behaviors;
using Marventa.Framework.Application.Commands;
using Marventa.Framework.Application.DTOs;
using MediatR;
using Moq;
using Xunit;

namespace Marventa.Framework.Tests.Unit.Application;

public class ValidationBehaviorTests
{
    public class TestCommand : ICommand<ApiResponse<string>>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCallNext()
    {
        // Arrange
        var validators = new List<IValidator<TestCommand>>
        {
            new TestCommandValidator()
        };

        var behavior = new ValidationBehavior<TestCommand, ApiResponse<string>>(validators);

        var command = new TestCommand { Name = "Valid Name" };
        var expectedResponse = ApiResponse<string>.SuccessResult("Success");

        RequestHandlerDelegate<ApiResponse<string>> next = () => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Success", result.Data);
    }

    [Fact]
    public async Task Handle_InvalidCommand_ShouldReturnValidationErrors()
    {
        // Arrange
        var validators = new List<IValidator<TestCommand>>
        {
            new TestCommandValidator()
        };

        var behavior = new ValidationBehavior<TestCommand, ApiResponse<string>>(validators);

        var command = new TestCommand { Name = "" }; // Invalid

        RequestHandlerDelegate<ApiResponse<string>> next = () =>
            Task.FromResult(ApiResponse<string>.SuccessResult("Should not reach here"));

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains("Name is required", result.Errors);
    }

    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = new List<IValidator<TestCommand>>();
        var behavior = new ValidationBehavior<TestCommand, ApiResponse<string>>(validators);

        var command = new TestCommand { Name = "" };
        var expectedResponse = ApiResponse<string>.SuccessResult("Success");

        RequestHandlerDelegate<ApiResponse<string>> next = () => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
    }
}