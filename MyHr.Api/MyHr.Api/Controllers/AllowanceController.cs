using Microsoft.AspNetCore.Mvc;
using MyHr.Service.Interface;

namespace MyHr.Api.Controllers
{
    [ApiController]
    [Route("api/Allowances")]
    public class AllowanceController : ControllerBase
    {
        private readonly IAllowanceDBService allowanceDBService;

        public AllowanceController(IAllowanceDBService allowanceDBService)
        {
            this.allowanceDBService = allowanceDBService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllAllowances()
        {
            var result = await allowanceDBService.GetAllAllowancesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllowanceById(string id)
        {
            var result = await allowanceDBService.GetAllowanceByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy phụ cấp" });
            }
            return Ok(result);
        }
    }
}
