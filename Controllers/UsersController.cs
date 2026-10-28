using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagmentAPI.Models;
using UserManagmentAPI.Services;

namespace UserManagmentAPI.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IReadOnlyList<UserResponse>> GetAll(CancellationToken cancellationToken) => Ok(userService.GetAll());

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public ActionResult<UserResponse> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = userService.GetById(id);
        return user is null ? NotFound() : Ok(user);
    }

    [Authorize(Roles = "IT,RRHH")]
    [HttpPost]
    public ActionResult<UserResponse> Create(CreateUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = userService.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new ProblemDetails { Title = "User already exists", Detail = exception.Message });
        }
    }

    [Authorize(Roles = "IT,RRHH")]
    [HttpPut("{id:guid}")]
    public ActionResult<UserResponse> Update(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = userService.Update(id, request);
        return user is null ? NotFound() : Ok(user);
    }

    [Authorize(Roles = "IT,RRHH")]
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id, CancellationToken cancellationToken) =>
        userService.Delete(id) ? NoContent() : NotFound();
}
