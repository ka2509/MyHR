using MyHr.Data.Model;

namespace MyHr.Service.Interface
{
    public interface ISalaryScaleDBService
    {
        Task<SalaryScale?> GetSalaryScaleByIdAsync(String salaryScaleId);
        Task<List<SalaryScale>> GetAllSalaryScalesAsync();
    }
}
