using System.ComponentModel.DataAnnotations;

namespace UserManagmentAPI.Models;

/// <summary>Payload used to authenticate a user.</summary>
public sealed record LoginRequest
{
    [Required, StringLength(100, MinimumLength = 1), RegularExpression(@".*\S.*")]
    public required string Username { get; init; }

    [Required, StringLength(100, MinimumLength = 1), RegularExpression(@".*\S.*")]
    public required string Password { get; init; }
}

/// <summary>JWT returned after successful authentication.</summary>
public sealed record LoginResponse(string Token, DateTimeOffset ExpiresAt);

/// <summary>Payload used to create a user.</summary>
public sealed record CreateUserRequest
{
    [Required, StringLength(50, MinimumLength = 1), RegularExpression(@".*\S.*")]
    public required string Username { get; init; }

    [Required, StringLength(150, MinimumLength = 1), RegularExpression(@".*\S.*")]
    public required string FullName { get; init; }

    [Required, EmailAddress, StringLength(254)]
    public required string Email { get; init; }

    [Required, StringLength(100, MinimumLength = 1), RegularExpression(@".*\S.*")]
    public required string Department { get; init; }

    [Required, StringLength(50, MinimumLength = 1), RegularExpression(@".*\S.*")]
    public required string Role { get; init; }

    [Required, StringLength(100, MinimumLength = 8), RegularExpression(@".*\S.*")]
    public required string Password { get; init; }
}

/// <summary>Payload used to update an existing user.</summary>
public sealed record UpdateUserRequest
{
    [Required, StringLength(150, MinimumLength = 1), RegularExpression(@".*\S.*")]
    public required string FullName { get; init; }

    [Required, EmailAddress, StringLength(254)]
    public required string Email { get; init; }

    [Required, StringLength(100, MinimumLength = 1), RegularExpression(@".*\S.*")]
    public required string Department { get; init; }

    [Required, StringLength(50, MinimumLength = 1), RegularExpression(@".*\S.*")]
    public required string Role { get; init; }
    public required bool IsActive { get; init; }
}

/// <summary>Public representation of a user.</summary>
public sealed record UserResponse(
    Guid Id,
    string Username,
    string FullName,
    string Email,
    string Department,
    string Role,
    bool IsActive);
