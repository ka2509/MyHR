using MyHr.Data.Model;

namespace MyHr.Service.Interface
{
    public interface IAllowanceDBService
    {
        Task<Allowance?> GetAllowanceByIdAsync(string allowanceId);
        Task<List<Allowance>> GetAllAllowancesAsync();
    }
}
