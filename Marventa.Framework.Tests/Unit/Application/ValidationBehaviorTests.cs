using FluentValidation;
using FluentValidation.Results;
using Marventa.Framework.Application.Behaviors;
using Marventa.Framework.Application.Commands;
using Marventa.Framework.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
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

        var mockLogger = new Mock<ILogger<ValidationBehavior<TestCommand, ApiResponse<string>>>>();
        var behavior = new ValidationBehavior<TestCommand, ApiResponse<string>>(validators, mockLogger.Object);

        var command = new TestCommand { Name = "Valid Name" };
        var expectedResponse = ApiResponse<string>.SuccessResult("Success");

        RequestHandlerDelegate<ApiResponse<string>> next = (ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
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

        var mockLogger = new Mock<ILogger<ValidationBehavior<TestCommand, ApiResponse<string>>>>();
        var behavior = new ValidationBehavior<TestCommand, ApiResponse<string>>(validators, mockLogger.Object);

        var command = new TestCommand { Name = "" }; // Invalid

        RequestHandlerDelegate<ApiResponse<string>> next = (ct) =>
            Task.FromResult(ApiResponse<string>.SuccessResult("Should not reach here"));

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.True(result.Errors.ContainsKey("Name"));
    }

    [Fact]
    public async Task Handle_NoValidators_ShouldCallNext()
    {
        // Arrange
        var validators = new List<IValidator<TestCommand>>();
        var mockLogger = new Mock<ILogger<ValidationBehavior<TestCommand, ApiResponse<string>>>>();
        var behavior = new ValidationBehavior<TestCommand, ApiResponse<string>>(validators, mockLogger.Object);

        var command = new TestCommand { Name = "" };
        var expectedResponse = ApiResponse<string>.SuccessResult("Success");

        RequestHandlerDelegate<ApiResponse<string>> next = (ct) => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
    }
}