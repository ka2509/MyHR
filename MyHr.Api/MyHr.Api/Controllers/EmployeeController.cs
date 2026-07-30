using Microsoft.AspNetCore.Mvc;
using MyHr.Data.Dto;
using MyHr.Service.Interface;

namespace MyHr.Api.Controllers
{
    [ApiController]
    [Route("api/Employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> logger;
        private readonly IEmployeeService employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            this.logger = logger;
            this.employeeService = employeeService;
        }

        [HttpGet("organization/{organizationId}")]
        public async Task<IActionResult> GetEmployeesByOrganization(string organizationId)
        {
            var result = await employeeService.GetEmployeesByOrganization(organizationId);
            return Ok(result);
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetEmployeeById(string employeeId)
        {
            var result = await employeeService.GetEmployeeById(employeeId);
            if (result == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên" });
            }
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await employeeService.Login(request.IdentityCardNumber, request.Password);
            if (result == null)
            {
                return Unauthorized(new { message = "Số CCCD hoặc mật khẩu không đúng" });
            }
            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> ImportEmployee([FromBody] ImportEmployeeDto request)
        {
            var result = await employeeService.ImportEmployee(request);
            return result ? Ok() : BadRequest();
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeDto request)
        {
            var result = await employeeService.AddEmployee(request);
            if (result)
            {
                return Ok(new { message = "Thêm nhân viên thành công" });
            }
            return BadRequest(new { message = "Thêm nhân viên thất bại. Vui lòng kiểm tra dữ liệu hoặc nhân viên đã tồn tại." });
        }

        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteEmployee(string employeeId)
        {
            var result = await employeeService.DeleteEmployee(employeeId);
            if (result)
            {
                return Ok(new { message = "Xóa nhân viên thành công" });
            }
            return BadRequest(new { message = "Xóa nhân viên thất bại. Nhân viên không tồn tại." });
        }
    }
}
