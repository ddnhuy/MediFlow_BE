using Management.API.Roles.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Management.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RolesController(
        ISender sender)
        : ControllerBase
    {
        [HttpGet("names")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [EndpointSummary("Get Role Names")]
        [EndpointDescription("Get All Roles Name")]
        public async Task<IActionResult> GetRoleNames()
        {
            var query = new GetRoleNamesQuery();

            var result = await sender.Send(query);

            return Ok(result);
        }
    }
}
