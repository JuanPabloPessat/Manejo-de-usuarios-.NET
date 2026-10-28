using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using UserManagmentAPI.Models;

namespace UserManagmentAPI.Services;

public sealed class InMemoryUserService : IUserService
{
    private readonly ConcurrentDictionary<Guid, UserRecord> users = new();

    public InMemoryUserService()
    {
        AddSeedUser("it.admin", "Administrador IT", "it.admin@example.com", "IT", "IT", "ChangeMe123!");
        AddSeedUser("rrhh.admin", "Administrador RRHH", "rrhh.admin@example.com", "RRHH", "RRHH", "ChangeMe123!");
    }

    public IReadOnlyList<UserResponse> GetAll() =>
        users.Values.Select(ToResponse).OrderBy(user => user.Username).ToList();

    public UserResponse? GetById(Guid id) =>
        users.TryGetValue(id, out var user) ? ToResponse(user) : null;

    public UserResponse Create(CreateUserRequest request)
    {
        if (users.Values.Any(user => user.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("A user with that username already exists.");

        var user = new UserRecord(
            Guid.NewGuid(), request.Username, request.FullName, request.Email,
            request.Department, request.Role, true, HashPassword(request.Password));
        users[user.Id] = user;
        return ToResponse(user);
    }

    public UserResponse? Update(Guid id, UpdateUserRequest request)
    {
        if (!users.TryGetValue(id, out var existing))
            return null;

        var updated = existing with
        {
            FullName = request.FullName,
            Email = request.Email,
            Department = request.Department,
            Role = request.Role,
            IsActive = request.IsActive
        };
        users[id] = updated;
        return ToResponse(updated);
    }

    public bool Delete(Guid id) => users.TryRemove(id, out _);

    public bool ValidateCredentials(string username, string password, out UserResponse? user)
    {
        var matchedUser = users.Values.FirstOrDefault(candidate =>
            candidate.IsActive &&
            candidate.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            VerifyPassword(password, candidate.PasswordHash));
        user = matchedUser is null ? null : ToResponse(matchedUser);
        return user is not null;
    }

    private void AddSeedUser(string username, string fullName, string email, string department, string role, string password)
    {
        var id = Guid.NewGuid();
        users[id] = new UserRecord(id, username, fullName, email, department, role, true, HashPassword(password));
    }

    private static UserResponse ToResponse(UserRecord user) =>
        new(user.Id, user.Username, user.FullName, user.Email, user.Department, user.Role, user.IsActive);

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string storedValue)
    {
        var parts = storedValue.Split(':');
        if (parts.Length != 2)
            return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expectedHash = Convert.FromBase64String(parts[1]);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, expectedHash.Length);
        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }

    private sealed record UserRecord(
        Guid Id,
        string Username,
        string FullName,
        string Email,
        string Department,
        string Role,
        bool IsActive,
        string PasswordHash);
}
