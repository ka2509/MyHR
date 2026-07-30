using Microsoft.AspNetCore.Mvc;
using MyHr.Service.Interface;

namespace MyHr.Api.Controllers
{
    [ApiController]
    [Route("api/Professions")]
    public class ProfessionController : ControllerBase
    {
        private readonly IProfessionDBService professionDBService;

        public ProfessionController(IProfessionDBService professionDBService)
        {
            this.professionDBService = professionDBService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllProfessions()
        {
            var result = await professionDBService.GetAllProfessionsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfessionById(string id)
        {
            var result = await professionDBService.GetProfessionByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy nghiệp vụ chuyên môn" });
            }
            return Ok(result);
        }
    }
}
