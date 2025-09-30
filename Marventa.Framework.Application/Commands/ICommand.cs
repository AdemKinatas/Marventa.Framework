using Marventa.Framework.Core.Interfaces.Validation;

namespace Marventa.Framework.Application.Commands;

/// <summary>
/// Marker interface for CQRS commands that modify state and support validation
/// </summary>
/// <remarks>
/// Commands represent write operations in CQRS pattern. They should:
/// - Change system state
/// - Be validated before execution
/// - Be wrapped in transactions (via TransactionBehavior)
/// - Not return data (use queries for reads)
/// </remarks>
/// <example>
/// <code>
/// public class CreateProductCommand : ICommand&lt;ApiResponse&lt;ProductDto&gt;&gt;
/// {
///     public string Name { get; set; }
///     public decimal Price { get; set; }
/// }
/// </code>
/// </example>
public interface ICommand : IValidatable
{
}

/// <summary>
/// CQRS command interface with response type
/// </summary>
/// <typeparam name="TResponse">The type of response returned after command execution</typeparam>
/// <remarks>
/// Generic command interface that specifies the response type.
/// Used with MediatR for command handling with pipeline behaviors:
/// - ValidationBehavior: Validates command before execution
/// - LoggingBehavior: Logs command execution time
/// - TransactionBehavior: Wraps command in database transaction
/// </remarks>
/// <example>
/// <code>
/// public class UpdateProductCommand : ICommand&lt;ApiResponse&lt;ProductDto&gt;&gt;
/// {
///     public Guid Id { get; set; }
///     public string Name { get; set; }
///     public decimal Price { get; set; }
/// }
///
/// public class UpdateProductCommandHandler : IRequestHandler&lt;UpdateProductCommand, ApiResponse&lt;ProductDto&gt;&gt;
/// {
///     public async Task&lt;ApiResponse&lt;ProductDto&gt;&gt; Handle(UpdateProductCommand request, CancellationToken cancellationToken)
///     {
///         // Handler implementation
///     }
/// }
/// </code>
/// </example>
public interface ICommand<out TResponse> : ICommand
{
}