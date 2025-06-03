using BuildingBlocks.Exceptions;
using Management.API.Users.Commands;
using Management.API.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Management.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = $"{BuildingBlocks.Strings.Roles.ADMIN},{BuildingBlocks.Strings.Roles.HEAD_OF_DEPARTMENT}")]
    public class UsersController(
        ISender sender) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Get Users")]
        [EndpointDescription("Get Users")]
        public async Task<IActionResult> GetUsersAsync(int pageIndex = 1, int pageSize = 100, string? keyword = null)
        {
            var query = new GetUsersQuery(pageIndex, pageSize, keyword, HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value);

            var result = await sender.Send(query);

            return Ok(result);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointSummary("Get User")]
        [EndpointDescription("Get User By Id")]
        public async Task<IActionResult> GetUserByIdAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new BadRequestException(ValidationStrings.REQUIRED_USER_ID);
            }

            var query = new GetUserByIdQuery(userId, HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value);

            var result = await sender.Send(query);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Create User")]
        [EndpointDescription("Create User")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException(ValidationStrings.INVALID_USER_DATA);
            }

            var command = new CreateUserCommand(request.UserName, request.Email, request.Password, request.PhoneNumber, request.Code, request.Name, request.Address, request.ProfilePictureUrl, request.RoleNames, request.DepartmentIds, int.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value));

            var result = await sender.Send(command);

            return Created($"/users/{result.User.Id}", result);
        }
        public record CreateUserRequest(string UserName, string Email, string Password, string PhoneNumber, string Code, string Name, string Address, string ProfilePictureUrl, List<string> RoleNames, List<int> DepartmentIds);

        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Update User")]
        [EndpointDescription("Update User")]
        public async Task<IActionResult> UpdateUserAsync(int userId, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new BadRequestException(ValidationStrings.INVALID_USER_DATA);
            }

            var command = new UpdateUserCommand(userId, request.UserName, request.Email, request.PhoneNumber, request.Code, request.Name, request.Address, request.ProfilePictureUrl, request.RoleNames, request.DepartmentIds, request.IsSuspended, int.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value));

            var result = await sender.Send(command);

            return Ok(result);
        }
        public record UpdateUserRequest(string UserName, string Email, string PhoneNumber, string Code, string Name, string Address, string ProfilePictureUrl, List<string> RoleNames, List<int> DepartmentIds, bool IsSuspended);

        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointSummary("Delete User")]
        [EndpointDescription("Delete User By Id")]
        public async Task<IActionResult> DeleteUserAsync(int userId)
        {
            var command = new DeleteUserCommand(userId, int.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value));

            var result = await sender.Send(command);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                throw new BadRequestException(result.Message);
            }
        }
    }
}