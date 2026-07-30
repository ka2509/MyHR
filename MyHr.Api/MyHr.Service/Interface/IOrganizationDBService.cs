using MyHr.Data.Model;

namespace MyHr.Service.Interface
{
    public interface IOrganizationDBService
    {
        Task<String?> GetOrganizationNameByIdAsync(String orgId);
        Task<List<Organization>> GetAllOrganizationsAsync();
        Task<Organization?> GetOrganizationByIdAsync(String orgId);
    }
}
