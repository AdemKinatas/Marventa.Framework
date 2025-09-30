using Marventa.Framework.Core.Interfaces.Validation;

namespace Marventa.Framework.Application.Queries;

/// <summary>
/// CQRS query interface that represents a read operation with validation support
/// </summary>
/// <typeparam name="TResponse">The type of data returned by the query</typeparam>
/// <remarks>
/// Queries represent read operations in CQRS pattern. They should:
/// - NOT modify system state (read-only)
/// - Return data
/// - Be cacheable when appropriate
/// - Execute outside of transactions (no TransactionBehavior)
/// - Still be validated before execution
/// </remarks>
/// <example>
/// <code>
/// public class GetProductByIdQuery : IQuery&lt;ApiResponse&lt;ProductDto&gt;&gt;
/// {
///     public Guid Id { get; set; }
/// }
///
/// public class GetProductByIdQueryHandler : IRequestHandler&lt;GetProductByIdQuery, ApiResponse&lt;ProductDto&gt;&gt;
/// {
///     private readonly IRepository&lt;Product&gt; _repository;
///
///     public async Task&lt;ApiResponse&lt;ProductDto&gt;&gt; Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
///     {
///         var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
///
///         if (product == null)
///             return ApiResponse&lt;ProductDto&gt;.FailureResult("Product not found");
///
///         return ApiResponse&lt;ProductDto&gt;.SuccessResult(_mapper.Map&lt;ProductDto&gt;(product));
///     }
/// }
/// </code>
/// </example>
public interface IQuery<out TResponse> : IValidatable
{
}