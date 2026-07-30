using MyHr.Data.Model;

namespace MyHr.Service.Interface
{
    public interface IEmployeeSalaryDBService
    {
        Task<Boolean> AddEmployeeSalary(EmployeeSalary employeeSalary);
        Task<EmployeeSalary?> GetCurrentSalaryByEmployeeId(String employeeId);
        Task<List<EmployeeSalary>> GetSalaryHistoryByEmployeeId(String employeeId);
        Task<Boolean> DeleteEmployeeSalaries(String employeeId);
    }
}
