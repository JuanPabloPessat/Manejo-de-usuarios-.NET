using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagmentAPI.Models;
using UserManagmentAPI.Services;

namespace UserManagmentAPI.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService userService, ILogger<UsersController> logger) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IReadOnlyList<UserResponse>> GetAll(
        [FromQuery, Range(1, 1_000_000)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(userService.GetAll(page, pageSize, cancellationToken));
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to retrieve users.");
            return Problem(
                title: "Unable to retrieve users.",
                detail: "The user search could not be completed.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public ActionResult<UserResponse> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var user = userService.GetById(id);
            return user is null ? NotFound() : Ok(user);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to retrieve user {UserId}.", id);
            return Problem(
                title: "Unable to retrieve user.",
                detail: "The user search could not be completed.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
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
