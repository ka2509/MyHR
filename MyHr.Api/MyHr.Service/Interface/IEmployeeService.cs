using MyHr.Data.Dto;

namespace MyHr.Service.Interface
{
    public interface IEmployeeService
    {
        Task<EmployeeResponse?> Login(String identityCardNumber, String password);
        Task<List<EmployeeResponse>> GetEmployeesByOrganization(String organizationId);
        Task<EmployeeResponse?> GetEmployeeById(String employeeId);
        Task<Boolean> ImportEmployee(ImportEmployeeDto request);
        Task<Boolean> AddEmployee(AddEmployeeDto request);
        Task<Boolean> DeleteEmployee(String employeeId);
    }
}
