using UserManagmentAPI.Models;

namespace UserManagmentAPI.Services;

public interface IUserService
{
    IReadOnlyList<UserResponse> GetAll();
    UserResponse? GetById(Guid id);
    UserResponse Create(CreateUserRequest request);
    UserResponse? Update(Guid id, UpdateUserRequest request);
    bool Delete(Guid id);
    bool ValidateCredentials(string username, string password, out UserResponse? user);
}
