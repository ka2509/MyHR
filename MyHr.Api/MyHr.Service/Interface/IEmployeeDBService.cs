using MyHr.Data.Model;

namespace MyHr.Service.Interface
{
    public interface IEmployeeDBService
    {
        Task<Employee?> GetEmployeeByIdentityCardNumber(String identityCardNumber);
        Task<Employee?> GetEmployeeById(String employeeId);
        Task<List<Employee>> GetAllEmployees();
        Task<Boolean> AddEmployees(List<Employee> employees);
        Task<Boolean> ImportEmployee(Employee employee);
        Task<Boolean> DeleteEmployee(String employeeId);
    }
}
