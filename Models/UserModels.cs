namespace UserManagmentAPI.Models;

/// <summary>Payload used to authenticate a user.</summary>
public sealed record LoginRequest
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}

/// <summary>JWT returned after successful authentication.</summary>
public sealed record LoginResponse(string Token, DateTimeOffset ExpiresAt);

/// <summary>Payload used to create a user.</summary>
public sealed record CreateUserRequest
{
    public required string Username { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string Department { get; init; }
    public required string Role { get; init; }
    public required string Password { get; init; }
}

/// <summary>Payload used to update an existing user.</summary>
public sealed record UpdateUserRequest
{
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string Department { get; init; }
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
