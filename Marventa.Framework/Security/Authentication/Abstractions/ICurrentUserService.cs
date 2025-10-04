namespace Marventa.Framework.Security.Authentication.Abstractions;

/// <summary>
/// Service for accessing current authenticated user information from the HTTP context.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's unique identifier.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current user's email address.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user's username.
    /// </summary>
    string? UserName { get; }

    /// <summary>
    /// Gets the current user's tenant identifier for multi-tenant scenarios.
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Checks if the current user has the specified role.
    /// </summary>
    /// <param name="role">The role name to check.</param>
    /// <returns>True if the user has the role, otherwise false.</returns>
    bool IsInRole(string role);

    /// <summary>
    /// Checks if the current user has the specified permission.
    /// </summary>
    /// <param name="permission">The permission to check.</param>
    /// <returns>True if the user has the permission, otherwise false.</returns>
    bool HasPermission(string permission);

    /// <summary>
    /// Gets all roles assigned to the current user.
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Gets all permissions assigned to the current user.
    /// </summary>
    IEnumerable<string> Permissions { get; }
}
