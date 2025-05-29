using Management.API.Departments.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
            var query = new GetDepartmentsQuery(pageIndex, pageSize, keyword);

            var result = await sender.Send(query);

            return Ok(result);
        }

        //[HttpGet("{departmentId}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[EndpointSummary("Get Department")]
        //[EndpointDescription("Get Department By Id")]
        //public async Task<IActionResult> GetDepartmentById(int departmentId)
        //{
        //    if (departmentId <= 0)
        //    {
        //        throw new BadRequestException(ValidationStrings.REQUIRED_DEPARTMENT_ID);
        //    }
        //    var query = new GetDepartmentByIdQuery(departmentId, HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Role).Value);
        //    var result = await sender.Send(query);
        //    return Ok(result);
        //}
    }
}
