using Microsoft.AspNetCore.Mvc;
using MyHr.Service.Interface;

namespace MyHr.Api.Controllers
{
    [ApiController]
    [Route("api/Organizations")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService organizationService;

        public OrganizationController(IOrganizationService organizationService)
        {
            this.organizationService = organizationService;
        }

        [HttpGet("main")]
        public async Task<IActionResult> GetMainOrganizations()
        {
            var result = await organizationService.GetMainOrganizations();
            return Ok(result);
        }

        [HttpGet("{parentId}/sub")]
        public async Task<IActionResult> GetSubOrganizations(string parentId)
        {
            var result = await organizationService.GetSubOrganizations(parentId);
            return Ok(result);
        }
    }
}
