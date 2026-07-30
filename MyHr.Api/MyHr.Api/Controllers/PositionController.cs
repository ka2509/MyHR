using Microsoft.AspNetCore.Mvc;
using MyHr.Service.Interface;

namespace MyHr.Api.Controllers
{
    [ApiController]
    [Route("api/Positions")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionDBService positionDBService;

        public PositionController(IPositionDBService positionDBService)
        {
            this.positionDBService = positionDBService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllPositions()
        {
            var result = await positionDBService.GetAllPositionsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPositionById(string id)
        {
            var result = await positionDBService.GetPositionByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy chức vụ" });
            }
            return Ok(result);
        }
    }
}
