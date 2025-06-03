using BuildingBlocks.Exceptions;
using Management.API.Departments.Commands;
using Management.API.Departments.Queries;
using Management.API.DepartmentTypes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Management.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = $"{BuildingBlocks.Strings.Roles.ADMIN},{BuildingBlocks.Strings.Roles.HEAD_OF_DEPARTMENT}")]
    public class DepartmentsController(
        ISender sender)
        : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Get Departments")]
        [EndpointDescription("Get Departments")]
        public async Task<IActionResult> GetDepartments(int pageIndex = 1, int pageSize = 100, string? keyword = null)
        {
            PaginationHelper.VerifyPaginationRequest(pageIndex, pageSize);

            var query = new GetDepartmentsQuery(pageIndex, pageSize, keyword);

            var result = await sender.Send(query);

            return Ok(result);
        }

        [HttpGet("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointSummary("Get Department")]
        [EndpointDescription("Get Department By Id")]
        public async Task<IActionResult> GetDepartmentById(int departmentId)
        {
            if (departmentId <= 0)
            {
                throw new BadRequestException(ValidationStrings.REQUIRED_DEPARTMENT_ID);
            }

            var query = new GetDepartmentByIdQuery(departmentId);

            var result = await sender.Send(query);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Create Department")]
        [EndpointDescription("Create New Department")]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
        {
            var command = new CreateDepartmentCommand(
                request.Code,
                request.Name,
                request.NameInEnglish,
                request.DepartmentTypeId,
                int.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value));

            var result = await sender.Send(command);

            return Created($"/departments/{result.Department.Id}", result);
        }
        public record CreateDepartmentRequest(string Code, string Name, string NameInEnglish, int DepartmentTypeId);

        [HttpPut("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointSummary("Update Department")]
        [EndpointDescription("Update Department")]
        public async Task<IActionResult> UpdateDepartment(int departmentId, [FromBody] UpdateDepartmentRequest request)
        {
            var command = new UpdateDepartmentCommand(
                departmentId,
                request.Code,
                request.Name,
                request.NameInEnglish,
                request.DepartmentTypeId,
                request.IsSuspended,
                request.IsCancelled,
                int.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value));

            var result = await sender.Send(command);

            return Ok(result);
        }
        public record UpdateDepartmentRequest(string Code, string Name, string NameInEnglish, int DepartmentTypeId, bool IsSuspended, bool IsCancelled);

        [HttpDelete("{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [EndpointSummary("Delete Department")]
        [EndpointDescription("Delete Department By Id")]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            if (departmentId <= 0)
            {
                throw new BadRequestException(ValidationStrings.REQUIRED_DEPARTMENT_ID);
            }

            var command = new DeleteDepartmentCommand(departmentId, int.Parse(HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value));

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

        [HttpGet("types")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Get Department Types")]
        [EndpointDescription("Get All Department Types")]
        public async Task<IActionResult> GetDepartmentTypes()
        {
            var query = new GetDepartmentTypesQuery();

            var result = await sender.Send(query);

            return Ok(result);
        }

        [HttpGet("{departmentId}/employees")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Get Employees")]
        [EndpointDescription("Get Employees By Department Id")]
        public async Task<IActionResult> GetEmployeesByDepartmentId([FromRoute] int departmentId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 100)
        {
            PaginationHelper.VerifyPaginationRequest(pageIndex, pageSize);

            var query = new GetEmployeesByDepartmentIdQuery(departmentId, pageIndex, pageSize);

            var result = await sender.Send(query);

            return Ok(result);
        }
    }
}